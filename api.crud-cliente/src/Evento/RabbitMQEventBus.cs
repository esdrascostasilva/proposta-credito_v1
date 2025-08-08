using System.Text;
using System.Text.Json;
using api.crud_cliente.src.Evento.Interface;
using RabbitMQ.Client;

namespace api.crud_cliente.src.Evento;

public class RabbitMQEvent : IEventBus
{
    private readonly IConfiguration _configuration;

    public RabbitMQEvent(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public void Publish<T>(T message, string queue)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:Host"],
            UserName = _configuration["RabbitMQ:User"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish("", queue, properties, body);
    }
}
