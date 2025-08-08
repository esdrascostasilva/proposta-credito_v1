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

    public RabbitMQCartaoConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopedFactory = scopeFactory;
    }

    public void Start()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
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

        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }

    private string GerarNumeroCartao()
    {
        var random = new Random();
        return string.Join("", Enumerable.Range(0, 4).Select(_ => random.Next(1000, 9999).ToString()));
    }
}
