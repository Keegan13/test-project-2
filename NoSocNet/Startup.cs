﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using NoSocNet.AutoMapper;
using NoSocNet.Infrastructure.AutoMapper;
using NoSocNet.Infrastructure.Services;
using NoSocNet.Core.Interfaces;
using NoSocNet.Infrastructure.Services.Notificator;
using NoSocNet.Infrastructure.Extensions;
using NoSocNet.Core.Services;
using NoSocNet.Domain.Models;
using NoSocNet.Infrastructure.Data;

namespace NoSocNet
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAutoMapper(typeof(UIProfile), typeof(InfrastructureProfile));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("LocalConnection")));

            services.AddHttpContextAccessor();

            services.AddScoped<DbContext, ApplicationDbContext>(factory => factory.GetRequiredService<ApplicationDbContext>());
            services.AddScoped(factory => factory.GetRequiredService<ApplicationDbContext>() as IUnitOfWork);
            services.AddScoped<IChatRoomRepository, EFCoreChatRoomRepository>();
            services.AddScoped<IUserRepository, EFCoreUserRepository>();
            services.AddScoped<IMessageRepository, EFCoreMessageRepository>();
            services.AddSingleton<NotificationService>();

            services.AddTransient<NotificationObserver>();
            services.AddSingleton(factory => factory.GetRequiredService<NotificationService>() as INotificator);

            services.AddHttpContextIdentityService();

            services.AddScoped<ChatService>();
            services.AddScoped(factory => factory.GetRequiredService<ChatService>() as IChatService);


            services.AddDefaultIdentity<UserEntity>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Chat}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}/{id?}",
                    defaults: new { Action = "Index" });
            });
        }
    }
}
