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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((webHostContext, options) =>
                        {
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