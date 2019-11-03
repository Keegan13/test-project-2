using Microsoft.Extensions.DependencyInjection;
using NoSocNet.Core.Interfaces;
using NoSocNet.Infrastructure.Services.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {

        public static void AddHttpContextIdentityService(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, HttpContextIdentityService>();
        }
    }
}
