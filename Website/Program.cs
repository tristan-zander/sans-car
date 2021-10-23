using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SansCar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.Sources.Clear();

                    var env = context.HostingEnvironment;

                    if (args != null)
                        builder.AddCommandLine(args);
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    builder.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((webHostContext, options) =>
                        {
                            
                            // Set developer endpoints
                            var env = webHostContext.HostingEnvironment;
                            if (env.IsDevelopment())
                            {
                                options.ListenLocalhost(5000);
                                options.ListenLocalhost(5001, listenOptions =>
                                {
                                    listenOptions.UseHttps();
                                });
                            }
                            
                            // Listen on a Unix socket, if set
                            var socketPath = webHostContext.Configuration["ListenUnixSocket"];
                            if (!string.IsNullOrEmpty(socketPath))
                            {
                                options.ListenUnixSocket(socketPath);
                            }
                        }
                    );
                    webBuilder.UseStartup<Startup>();
                });
    }
}