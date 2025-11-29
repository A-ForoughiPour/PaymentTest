using Microsoft.AspNetCore.Mvc;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.IranKish;

namespace PaymentTest.Controllers
{
    public class Payment : Controller
    {
        private readonly IOnlinePayment _OnlinePayment;

        public Payment(IOnlinePayment onlinePayment)
        {
            this._OnlinePayment = onlinePayment;
        }
        [HttpPost]
        [HttpPost("IranKishPayment")]
        public async Task<IActionResult> Pay(long Amount, long waletid, long userid)
        {
            var TRackingNumber = Guid.NewGuid();
           
            string callbackUrl = Url.Action(
                action: "IranKishVerify",
                controller: "Payment",
                values: null,
                protocol: Request.Scheme
            );
            var Result = await _OnlinePayment.RequestAsync(invoice =>
            {
                invoice.UseIranKish()
                .SetAmount(1)
                .SetCallbackUrl(callbackUrl)
                .SetTrackingNumber(Convert.ToInt64(TRackingNumber));
            });
            if (Result.IsSucceed)
            {
                return Ok(Result.GatewayTransporter.TransportToGateway());
            }
            return Conflict();
        }

        [HttpPost("IranKishVerify")]
        public async Task<IActionResult> Verify()
        {
            var invoice = await _OnlinePayment.FetchAsync();
            if (invoice == null)
            {
                return Conflict("invalid Verify");
            }
            if (invoice.Status != PaymentFetchResultStatus.ReadyForVerifying)
            {
                Refund(invoice.TrackingNumber);
            }
            var verifyed = await _OnlinePayment.VerifyAsync(invoice);
            if (verifyed.Status == PaymentVerifyResultStatus.Succeed)
            {
           
                return Ok();
            }
            return BadRequest();
        }
        [NonAction]
        public async Task Refund(long TrackingNumber)
        {
            var efund = _OnlinePayment.RefundCompletelyAsync(TrackingNumber);
        }

        
    }
}
