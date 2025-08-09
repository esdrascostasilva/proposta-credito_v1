using System.Text;
using console.analise_credito.src.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace console.analise_credito.src.Evento.Consumer;

public class RabbitMQEventoConsumer
{
    private readonly string _hostName = "localhost";
    private readonly string _queueName = "clientes.criados";
    private readonly AvaliadorHandler _avaliadorHandler;

    // implementando retry e DLQ
    private readonly string _dlqQueueName = "clientes.criados.dql";
    private const int MaxRetries = 3;

    public RabbitMQEventoConsumer(AvaliadorHandler avaliadorHandler)
    {
        _avaliadorHandler = avaliadorHandler;
    }

    public void Start()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // criando a fila DLQ
        channel.QueueDeclare(
            queue: _dlqQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // criando a fila principal que aponta pra DLQ
        var mainQueueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" }, // "" significa default exchange
            { "x-dead-letter-routing-key", _dlqQueueName }
        };

        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArgs
        );

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensagem = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[x] Mensagem recebida: {mensagem}");

            try
            {
                var cliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(mensagem);

                if (cliente != null)
                {
                    await _avaliadorHandler.ProcessarCliente(cliente);
                }

                channel.BasicAck(ea.DeliveryTag, false);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erro ao processar: {ex.Message}");

                int retryCount = 0;
                if (ea.BasicProperties.Headers != null &&
                    ea.BasicProperties.Headers.ContainsKey("x-retry-count"))
                {
                    retryCount = Convert.ToInt32(ea.BasicProperties.Headers["x-retry-count"]);
                }

                if (retryCount < MaxRetries)
                {
                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;
                    props.Headers = props.Headers ?? new Dictionary<string, object>();
                    props.Headers["x-retry-count"] = retryCount + 1;

                    channel.BasicPublish("", _queueName, props, body);
                    channel.BasicAck(ea.DeliveryTag, false);
                    Console.WriteLine($"[↩] Retentativa {retryCount + 1}/{MaxRetries}");
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                    Console.WriteLine($"[☠] Mensagem enviada para DLQ após {MaxRetries} tentativas.");
                }
            }
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        Console.WriteLine("Aguardando mensagens. Pressione [enter] para sair");
        Console.ReadLine();
    }

}
