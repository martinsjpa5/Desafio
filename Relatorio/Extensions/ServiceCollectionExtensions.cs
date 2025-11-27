

using Application.Interfaces;
using Application.Services;
using Domain.Interfaces.Queues;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infraestrutura.Caching.Repositories;
using Infraestrutura.Dapper;
using Infraestrutura.EF.Context;
using Infraestrutura.EF.Interfaces;
using Infraestrutura.EF.Repositories;
using Infraestrutura.Queues;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Relatorio.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyJob(this IServiceCollection services, HostApplicationBuilder builder)
        {
            services.AddSingleton<IRelatorioQueue, RelatorioQueue>();
            services.AddScoped<ICommonEfRepository, CommonEfRepository>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddSingleton<TimeProvider>(TimeProvider.System);

            builder.Services.Configure<RabbitMqAppSettings>(builder.Configuration.GetSection("RabbitMq"));
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            services.AddSingleton<ICommonRepositoryDapper>(provider => new CommonRepositoryDapper(connectionString));
            services.AddSingleton<IRelatorioDapperRepository, RelatorioDapperRepository>();
            services.AddSingleton<ICommonCachingRepository, CommonCachingRepository>();


            var redisConnectionSection = builder.Configuration.GetSection("RedisConnection");
            services.Configure<RedisConnectionSettings>(redisConnectionSection);

            var redisConnection = redisConnectionSection.Get<RedisConnectionSettings>();

            builder.Services.AddStackExchangeRedisCache(o =>
            {
                o.InstanceName = redisConnection.InstanceName;
                o.Configuration = redisConnection.Configuration;
            });



            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Use AddIdentity completo
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders(); // isso garante DataProtectorTokenProvider

            // Autenticação obrigatória para SignInManager
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme);

            return services;
        }

    }
}
