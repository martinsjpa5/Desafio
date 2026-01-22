using Application.Interfaces;
using Domain.Interfaces.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace Relatorio
{
    public class RelatorioJob : BackgroundService
    {
        private readonly IRelatorioQueue _relatorioQueue;
        private readonly IServiceScopeFactory _scopeFactory;

        public RelatorioJob(IRelatorioQueue relatorioQueue, IServiceScopeFactory scopeFactory)
        {
            _relatorioQueue = relatorioQueue;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _relatorioQueue.StartConsumers(1, async (msg) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var relatorioService = scope.ServiceProvider.GetRequiredService<IRelatorioService>();

                await relatorioService.Gerar(msg);
            }, stoppingToken);

            return Task.CompletedTask;
        }
    }
}
