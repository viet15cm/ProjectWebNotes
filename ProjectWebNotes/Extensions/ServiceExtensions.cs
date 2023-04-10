using Contracts;
using Domain.Reposirory;
using Domain.Repository;
using Logger;
using Microsoft.EntityFrameworkCore;
using ProjectWebNotes.DbContextLayer;
using Services;
using Services.Abstractions;

namespace ProjectWebNotes.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            //var connectionString = config["mysqlconnection:connectionString"];
            //services.AddDbContext<RepositoryContext>(o => o.UseMySql(connectionString,
            //    MySqlServerVersion.LatlestSupportedServerVersion));
            services.AddDbContext<AppDbcontext>(options =>
            options.UseSqlServer(config.GetConnectionString("LocalHost")));;
            services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(config.GetConnectionString("LocalHost"))); ;


        }
        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IHttpClientServiceImplementation, HttpClientStreamService>();
        }
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
    }
}
