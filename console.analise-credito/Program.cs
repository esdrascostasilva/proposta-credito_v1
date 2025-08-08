using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using console.analise_credito.src.Data;
using console.analise_credito.src.Evento.Consumer;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<RabbitMQEventoConsumer>();
    })
    .Build();

var consumer = host.Services.GetRequiredService<RabbitMQEventoConsumer>();
consumer.Start();

Console.WriteLine("Aplicacao em execucao. Pressione qualquer tecla para sair...");
Console.ReadKey();
