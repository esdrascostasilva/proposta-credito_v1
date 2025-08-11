using System.Text;
using System.Text.Json;
using console.cartao_credito.src.Data;
using console.cartao_credito.src.Models;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace console.cartao_credito.src.Evento.Consumer;

public class RabbitMQCartaoConsumer
{
    private readonly IServiceScopeFactory _scopedFactory;
    private readonly string _queueName = "clientes.elegiveis";
    private const int MaxRetries = 3;

    public RabbitMQCartaoConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopedFactory = scopeFactory;
    }

    public void Start()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        try
        {
            channel.QueueDeclarePassive(_queueName); // verificando a fila antes de criar para evitar o erro RabbitMQ.Client.Exceptions.OperationInterruptedException: The AMQP
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            DeclaraFilaComDLQ(channel, _queueName);
        }

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                var dadosEvento = JsonSerializer.Deserialize<ClienteAprovadoEvento>(json);

                using var scope = _scopedFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                var novoCartao = new ClienteCartaoCredito
                {
                    NomeTitular = dadosEvento.Nome,
                    Numero = GerarNumeroCartao(),
                    Validade = DateTime.UtcNow.AddYears(3).ToString("MM/yy"),
                    Cvv = new Random().Next(100, 999).ToString(),
                    Limite = dadosEvento.ValorCreditoAprovado
                };

                context.ClientesCartaoCredito.Add(novoCartao);
                await context.SaveChangesAsync();

                Console.WriteLine($"Cartao de credito gerado para {dadosEvento.Nome} com limite de {dadosEvento.ValorCreditoAprovado}");
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar: {ex.Message}");

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

                    Console.WriteLine($"Retentativa {retryCount + 1}/{MaxRetries}");
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                    Console.WriteLine("Mensagem enviada para DLQ");
                }
            }
        };

        // Registro de consumo fica FORA do evento
        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        Console.WriteLine("Aguardando mensagens. Pressione [enter] para sair");
        Console.ReadLine();
    }

    private string GerarNumeroCartao()
    {
        var random = new Random();
        return string.Join("", Enumerable.Range(0, 4).Select(_ => random.Next(1000, 9999).ToString()));
    }

    public static void DeclaraFilaComDLQ(IModel channel, string queueName)
    {
        string dlqName = $"{queueName}.dlq";

        channel.QueueDeclare(
            queue: dlqName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", dlqName }
        };

        channel.QueueDeclare( // erro nessa linha
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args
        );
    }
}