using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(SistemaInventario.Areas.Identity.IdentityHostingStartup))]
namespace SistemaInventario.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}