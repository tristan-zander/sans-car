using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace SansCar
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            // TODO: Runtime compilation only in dev mode.
            services.AddRazorPages().AddRazorRuntimeCompilation();
            
            services.AddDbContext<SansDbContext>(options =>
            {
                options.UseNpgsql();
            });
            
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDefaultFiles();
            // app.UseSpaStaticFiles();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                // var reactApp = endpoints.CreateApplicationBuilder();
                /* reactApp.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
                */
                
                // endpoints.MapGet("/app/{**extra}", reactApp.Build());

                endpoints.MapRazorPages();
               
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}");

                endpoints.MapFallback(context =>
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                });
            });
        }
    }
}