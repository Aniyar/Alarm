using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.JSInterop;

using ALARm.Core;
using ALARm.DataAccess;
using MatBlazor;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace AlarmPP.Web
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazorContextMenu();
            services.AddScoped<Services.AppData>();
            services.AddScoped<Services.OnlineModeData>();
            
            services.AddScoped<IRdStructureRepository, RdStructureRepository>();
            services.AddScoped<IAdmStructureRepository, AdmStructureRepository>();
            services.AddScoped<IMainTrackStructureRepository, MainTrackStructureRepository>();
            services.AddScoped<IAdditionalParametersRepository, AdditionalParametersRepository>();
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomCenter;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 100;
                config.VisibleStateDuration = 5000;
                
                
            });


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
            app.UseRouting();
            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });


           
            
            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions() { WebPreferences = new() { DevTools = false }, Fullscreen = true, MinWidth = 1920, MinHeight = 1080, AutoHideMenuBar = true})); ;
        }
    }
}
