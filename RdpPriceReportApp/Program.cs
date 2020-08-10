using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace RdpPriceReportApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                });
           /*
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            webBuilder.UseStartup<Startup>().UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                        })
                        .UseStartup<Startup>();
                });
           */
    }
}
