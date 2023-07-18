using Contracts;
using Domain.IdentityModel;
using Domain.Reposirory;
using Domain.Repository;
using LoggerManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectWebNotes.DbContextLayer;
using ProjectWebNotes.FileManager;
using ProjectWebNotes.Security.Requirements;
using ProjectWebNotes.Services.MailServices;
using Services;
using Services.Abstractions;
using System;

namespace ProjectWebNotes.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            //var connectionString = config["mysqlconnection:connectionString"];
            //services.AddDbContext<RepositoryContext>(o => o.UseMySql(connectionString,
            //    MySqlServerVersion.LatlestSupportedServerVersion));
           
            services.AddDbContext<AppDbcontext>(
                    options => options.UseSqlServer(
                    config.GetConnectionString("LocalHost"),
                    providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(
                    config.GetConnectionString("LocalHost"),
                    providerOptions => providerOptions.EnableRetryOnFailure())); ;


            services.AddIdentity<AppUser, IdentityRole>()
                          .AddEntityFrameworkStores<AppDbcontext>()
                          .AddDefaultTokenProviders();

          

        }
        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();

            services.AddHttpClient<IHttpClientServiceImplementation, HttpClientStreamService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IFileServices , FileServices>();

            services.AddSingleton<ISendMailServices, SendMailServices>();


        }
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager.LoggerManager>();
        }

        public static void ConfigureAuthorizationHandlerService(this IServiceCollection services)
        {

            services.AddAuthorization(options => {

                options.AddPolicy("Admin", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Admin");

                });

                options.AddPolicy("ProEmployee", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Admin", "ProEmployee");

                });

                options.AddPolicy("Employee", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Admin", "Employee", "ProEmployee");

                });

                //options.AddPolicy("EmployeeSharedpostCategory", builder =>
                //{
                //    builder.RequireAuthenticatedUser();
                //    builder.RequireRole("Admin", "Employee", "ProEmployee");

                //    builder.RequireClaim("sharedpost", "sharedpost");
                //});

                //options.AddPolicy("EmployeeSharedAdvancedAddChildPost", builder =>
                //{
                //    builder.RequireAuthenticatedUser();
                //    builder.RequireRole("Admin", "ProEmployee");

                //    builder.RequireClaim("advancedaddchildpost", "advancedaddchildpost");
                //});

            });

            services.AddTransient<IAuthorizationHandler, NewUpdatePostHandler>();
            services.AddTransient<IAuthorizationHandler, CanOptionPostUserHandler>();
            services.AddTransient<IAuthorizationHandler, CanOptionCategoryUserHandler>();
        }


    }
}
