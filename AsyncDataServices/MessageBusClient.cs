using System.Text;
using System.Text.Json;
using PlatformService.Dto;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connection;
        private IModel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            int Port;
            bool success = int.TryParse(_configuration["RabbitMQPort"], out Port);

            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = success ? Port : 5672,                
            }; 

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> RabbitMQ: connected to MessageBus");
                
            }
            catch (Exception ex)
            {                
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
            }     
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection!.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection open, sending a message");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus disposed");
            if (_channel!.IsOpen)
            {
                _channel.Close();
                _connection!.Close();
            }
        }
    }
}