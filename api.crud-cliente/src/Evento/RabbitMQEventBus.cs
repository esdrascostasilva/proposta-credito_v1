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

        DeclaraFilaComDLQ(channel, queue);
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish("", queue, properties, body);
    }

    public static void DeclaraFilaComDLQ(IModel channel, string queueName)
    {
        // Nome da fila dlq
        string dlqName = $"{queueName}.dlq";

        // Declara a DLQ
        channel.QueueDeclare(
            queue: dlqName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Declara a fila principal com DLQ configurada
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", dlqName }
        };

        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args
        );
    }

}
