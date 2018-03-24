using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SmartDataHub.Models;


namespace SmartDataHub
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
            services.AddMvc();
            services.AddSignalR();


            var optionsBuilder = new DbContextOptionsBuilder<SmartDataHubStorageContext>().UseSqlServer(Configuration.GetConnectionString("SmartDataHubContext"));

            DataAccess.Initialize(optionsBuilder.Options);

            services.AddDbContext<SmartDataHubStorageContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SmartDataHubContext")));                
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();



            app.UseSignalR(routes =>
            {
                routes.MapHub<SmartDataSignalRhub>("SmartDataSignalRhub");
            });


            app.UseSignalR(routes =>
            {
                routes.MapHub<ReportHub>("ReportHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Machines}/{action=Index}/{id?}");
            });

            SignalRgatewayClient.Initialize();
        }
    }
}
