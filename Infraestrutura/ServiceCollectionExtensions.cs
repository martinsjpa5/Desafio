using Domain.Interfaces.Queues;
using Domain.Interfaces.Repositories;
using Infraestrutura.EF.Context;
using Infraestrutura.EF.Interfaces;
using Infraestrutura.EF.Repositories;
using Infraestrutura.Models;
using Infraestrutura.Queues;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestrutura
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<IdentityMensagensEmPortugues>();

            services.AddScoped<ICommonEfRepository, CommonEfRepository>();
            services.AddScoped<IProdutoVendaRepository, ProdutoVendaRepository>();
            services.AddScoped<IRelatorioQueue, RelatorioQueue>();

            return services;
        }

    }
}
