using Domain.Messages;
using Domain.Models;

namespace Domain.Interfaces.Queues
{
    public interface IRelatorioQueue : IRabbitMqQueue<FiltroRelatorio>
    {
    }
}
