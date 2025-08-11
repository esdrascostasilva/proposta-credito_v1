using System.Text;
using System.Text.Json;
using console.analise_credito.src.Evento.Publisher;
using RabbitMQ.Client;

public class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqMessagePublisher(string hostname = "localhost")
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublicarAsync<T>(string queueName, T message)
    {
        DeclaraFilaComDLQ(_channel, queueName);

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish("", queueName, properties, body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
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