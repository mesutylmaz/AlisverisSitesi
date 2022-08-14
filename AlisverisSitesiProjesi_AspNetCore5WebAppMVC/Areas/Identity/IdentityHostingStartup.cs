using System;
using AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Areas.Identity.IdentityHostingStartup))]
namespace AlisverisSitesiProjesi_AspNetCore5WebAppMVC.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}