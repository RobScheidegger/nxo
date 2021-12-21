using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NXO.Server.Dependencies;
using NXO.Shared;
using NXO.Shared.Modules;
using System;
using NXO.Server.Modules.TicTacToe;

namespace NXO.Server
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

            services.AddSignalR();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages();

            //Shared Services
            services.AddSingleton<ILobbyCoordinator, LobbyCoordinator>();
            services.AddSingleton<IGuidProvider, GuidProvider>();
            

            //TODO: Replace with reflection
            services.AddSingleton<INXOModule, TicTacToeModule>();

            var provider = services.BuildServiceProvider();
            var modules = provider.GetServices<INXOModule>();
            foreach (var module in modules)
            {
                module.RegisterServices(services);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };

            app.UseWebSockets();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<TicTacToeHub>("tictactoe/ws");
            });
        }
    }
}
