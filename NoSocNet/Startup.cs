﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoSocNet.Infrastructure.Services;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.DAL.Context;
using AutoMapper;
using NoSocNet.BLL.Models;
using NoSocNet.Models;
using NoSocNet.Infrastructure.Services.Hub;

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

            services.AddAutoMapper(new[] {
                typeof(ApplicationDbContext).Assembly,
                typeof(Message<,>).Assembly,
                typeof(MessageViewModel).Assembly
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpContextAccessor();

            services.AddScoped<DbContext, ApplicationDbContext>(factory => factory.GetRequiredService<ApplicationDbContext>());
            services.AddSingleton<HubMessageSender>();
            services.AddTransient<MessageObserver>();
            services.AddSingleton<IMessageSender<User, string>>(factory => factory.GetRequiredService<HubMessageSender>() as IMessageSender<User, string>);

            services.AddScoped<IIdentityService<User>, IdentityService>();
            services.AddScoped<IApplicationUserStore<User>, ApplicationUserStore>();
            services.AddScoped<IChatService<User, string>, ChatService>();

            services.AddDefaultIdentity<User>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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