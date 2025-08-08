
using console.analise_credito.src.Evento.Consumer;

var consumer = new RabbitMQEventoConsumer();
consumer.Start();

Console.WriteLine("Hello, World!");
