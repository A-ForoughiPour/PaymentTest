using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Parbad;
using Parbad.Builder;
using Parbad.Gateway.IranKish;
using System.Security.Principal;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddXmlFile("C:/Users/Administrator/Documents/Shvdev/PaymentTest/DataXmlFile.xml", optional: false, reloadOnChange: true);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddParbad()
    .ConfigureGateways(g =>
    {
        g.AddIranKish()
         .WithAccounts(accounts =>
         {
             accounts.AddInMemory(account =>
             {
                 account.TerminalId = builder.Configuration["DocumentElement:IPGData:TerminalId"];
                 account.AcceptorId = builder.Configuration["DocumentElement:IPGData:AcceptorId"];
                 account.PublicKey = builder.Configuration["DocumentElement:IPGData:RSAPublicKey"];
                 account.PassPhrase = builder.Configuration["DocumentElement:IPGData:PublicKey"];
             });
         });
    }).ConfigureHttpContext(c => c.UseDefaultAspNetCore()).ConfigureStorage(s => s.UseMemoryCache());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
