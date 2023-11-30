using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using StackExchangeRedis.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StackExchangeRedis.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Read Config

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();

            string httpUrl = configuration.GetValue<string>("Api:Kestrel:Http:Url");
            int httpPort = configuration.GetValue<int>("Api:Kestrel:Http:Port");

            string httpsUrl = configuration.GetValue<string>("Api:Kestrel:Https:Url");
            int httpsPort = configuration.GetValue<int>("Api:Kestrel:Https:Port");


            string certificateName = configuration.GetValue<string>("Api:Kestrel:Https:Certificate:Name");
            string certificateThumbPrint = configuration.GetValue<string>("Api:Kestrel:Https:Certificate:ThumbPrint");

            #endregion

            try
            {
                logger.Debug("Runnig Redis API");

                var host = new WebHostBuilder()
                    .UseConfiguration(configuration)
                    .UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.AllowSynchronousIO = true;
                        if (!string.IsNullOrEmpty(httpUrl) && httpPort != 0)
                        {
                            options.Listen(IPAddress.Parse(httpUrl), httpPort);
                        }

                        #region SSL

                        if (!string.IsNullOrEmpty(certificateThumbPrint))
                        {
                            var certificate = new x509().FindCertificateByThumbprint(certificateName, certificateThumbPrint);

                            if (certificate != null)
                            {
                                options.Listen(IPAddress.Parse(httpsUrl), httpsPort, listenOptions =>
                                {
                                    listenOptions.UseHttps(certificate);
                                });
                            }
                        }

                        #endregion
                    })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddNLog(hostingContext.Configuration.GetSection("Logging"));
                    })
                    .UseShutdownTimeout(TimeSpan.FromSeconds(10))
                    .Build();

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }


            //CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
