using Microsoft.Extensions.DependencyInjection;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Interfaces.Repositories;
using NoSocNet.Infrastructure.Services;
using NoSocNet.Infrastructure.Services.Repositories;
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

        public static void AddEFCoreRepositories(this IServiceCollection services)
        {
            services.AddScoped<IChatRoomRepository, EFCoreChatRoomRepository>();
            services.AddScoped<IUserRepository, EFCoreUserRepository>();
            services.AddScoped<IMessageRepository, EFCoreMessageRepository>();


            services.AddScoped<ISurveyRepository, EFCoreSurveyRepository>();
            services.AddScoped<IQuestionRepository, EFCoreQuestionRepository>();
            services.AddScoped<IOptionRepository, EFCoreOptionRepository>();
            services.AddScoped<ISurveyInstanceRepository, EFCoreSurveyInstanceRepository>();
            services.AddScoped<ISurveyUserResultRepository, EFCoreSurveyUserResultRepository>();
            services.AddScoped<IQuestionResultRepository, EFCoreQuestionResultRepository>();
        }

        //    public static void AddEFCoreContext(this IServiceCollection services,)
        //    {
        //        services.AddDbContext<ApplicationDbContext>(options =>
        //options.UseSqlServer(
        //    Configuration.GetConnectionString("DefaultConnection")));


        //        services.AddScoped<DbContext, ApplicationDbContext>(factory => factory.GetRequiredService<ApplicationDbContext>());
        //        services.AddScoped(factory => factory.GetRequiredService<ApplicationDbContext>() as IUnitOfWork);


        //    }
    }
}
