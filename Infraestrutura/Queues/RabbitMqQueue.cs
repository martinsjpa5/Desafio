using System.Collections.Concurrent;
using System.Text;
using Domain.Interfaces.Queues;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Infraestrutura.Queues
{
    public abstract class RabbitMqQueue<TMessage> : IRabbitMqQueue<TMessage>
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConcurrentBag<Task> _consumers = new ConcurrentBag<Task>();
        private readonly ILogger<RabbitMqQueue<TMessage>> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public RabbitMqQueue(RabbitMqSettings settings, ILogger<RabbitMqQueue<TMessage>> logger)
        {
            _settings = settings;
            _logger = logger;

            var factory = new ConnectionFactory() { HostName = _settings.Hostname, Port = _settings.Port, Password = _settings.Password, UserName = _settings.Username };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _settings.Exchange, type: _settings.ExchangeType);

            _channel.QueueDeclare(queue: _settings.QueueName, durable: _settings.Durable, exclusive: _settings.Exclusive, autoDelete: _settings.AutoDelete, arguments: null);

            foreach (var routingKey in _settings.RoutingKeys)
            {
                _channel.QueueBind(queue: _settings.QueueName, exchange: _settings.Exchange, routingKey: routingKey);
            }

            _logger.LogInformation("RabbitMqQueue initialized with settings: {@Settings}", _settings);
        }

        public async Task PublishMessageAsync(TMessage message)
        {
            await Task.Run(() =>
            {
                var messageBody = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(messageBody);
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                foreach (var routingKey in _settings.RoutingKeys)
                {
                    _channel.BasicPublish(exchange: _settings.Exchange, routingKey: routingKey, basicProperties: properties, body: body);
                }
                _logger.LogInformation("Message published: {Message}", messageBody);
            });
        }

        public void StartConsumers(int consumerCount, Action<TMessage> processMessage, CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            for (int i = 0; i < consumerCount; i++)
            {
                AddConsumer(processMessage, _cancellationTokenSource.Token);
            }
            _logger.LogInformation("{ConsumerCount} consumers started.", consumerCount);
        }

        private void AddConsumer(Action<TMessage> processMessage, CancellationToken stoppingToken)
        {
            var consumerTask = Task.Run(() => StartConsuming(processMessage, stoppingToken), stoppingToken);
            _consumers.Add(consumerTask);
        }

        private void StartConsuming(Action<TMessage> processMessage, CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageBody = Encoding.UTF8.GetString(body);
                try
                {
                    var message = JsonConvert.DeserializeObject<TMessage>(messageBody);
                    processMessage?.Invoke(message);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogInformation("Message processed and acknowledged: {Message}", messageBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message: {Message}", messageBody);
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Consumer started for queue: {QueueName}", _settings.QueueName);

            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Consumer for queue {QueueName} is stopping.", _settings.QueueName);
            });
        }

        public void StopAllConsumers()
        {
            _cancellationTokenSource.Cancel();
            _channel.Close();
            _connection.Close();
            _logger.LogInformation("All consumers stopped and connection closed.");
        }
    }

}