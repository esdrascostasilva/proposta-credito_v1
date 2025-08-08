using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace console.analise_credito.src.Evento.Consumer;

public class RabbitMQEventoConsumer
{
    private readonly string _hostName = "localhost";
    private readonly string _queueName = "clientes.criados";

    public void Start()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(   queue: _queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensagem = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[x] Mensagem recebida: {mensagem}");

            AnaliseCredito(mensagem);
        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        Console.WriteLine("Aguardando mensagens. Pressione [enter] para sair.");
        Console.ReadLine();
    }

    private void AnaliseCredito(string mensagem)
    {
        Console.WriteLine($"Processando mensagem: {mensagem}");
    }

}
