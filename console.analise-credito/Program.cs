using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using console.analise_credito.src.Data;
using console.analise_credito.src.Evento.Consumer;
using Microsoft.Extensions.Configuration;
using console.analise_credito.src.Service;
using console.analise_credito.src.Evento.Publisher;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<AvaliadorCreditoService>();
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
        services.AddScoped<AvaliadorHandler>();
        services.AddSingleton<RabbitMQEventoConsumer>();
    })
    .Build();

var consumer = host.Services.GetRequiredService<RabbitMQEventoConsumer>();
consumer.Start();

Console.WriteLine("Aplicacao em execucao. Pressione qualquer tecla para sair...");
Console.ReadKey();
