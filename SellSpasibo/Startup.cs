using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellSpasibo.BLL.Options;
using SellSpasibo.DAL;
using SellSpasibo.Extensions;

namespace SellSpasibo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SellSpasiboDbContext>(options =>
            {
                //TODO: реализовать работу с бд посредством PostgreSQL
                options.UseInMemoryDatabase("db");
            });
            
            services.Configure<SberOptions>(Configuration.GetSection(SberOptions.Sber));
            services.Configure<TinkoffOptions>(Configuration.GetSection(TinkoffOptions.Tinkoff));
                
            services.AddBusinessLogicLayerServicesExtensions();
            services.AddControllersWithViews();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //TODO: добавить RateLimit
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
