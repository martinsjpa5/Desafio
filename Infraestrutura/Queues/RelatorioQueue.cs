
using Domain.Interfaces.Queues;
using Domain.Messages;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infraestrutura.Queues
{
    public class RelatorioQueue : RabbitMqQueue<FiltroRelatorio>, IRelatorioQueue
    {
        public RelatorioQueue(IOptions<RabbitMqAppSettings> settings, ILogger<RabbitMqQueue<FiltroRelatorio>> logger) : base(GetQueueSettings(settings.Value), logger)
        {

        }

        public static RabbitMqSettings GetQueueSettings(RabbitMqAppSettings settings)
        {

            return new RabbitMqSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                Username = settings.Username,
                Password = settings.Password,
                Exchange = "EXG.RELATORIO",
                ExchangeType = ExchangeType.Direct,
                QueueName = "QUEUE.RELATORIO",
                RoutingKeys = ["produce_start"],
            };
        }
    }
}
