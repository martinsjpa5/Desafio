using Application.Interfaces;
using Application.Services;
using Domain.Interfaces.Queues;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infraestrutura.Caching.Repositories;
using Infraestrutura.Dapper;
using Infraestrutura.Queues;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, WebApplicationBuilder builder)
        {

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<ICompraService, CompraService>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddScoped<ICarrinhoService, CarrinhoService>();
            services.AddSingleton<IRelatorioQueue, RelatorioQueue>();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

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

            return services;
        }
    }
}
