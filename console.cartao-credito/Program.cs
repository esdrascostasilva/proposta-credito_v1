using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using console.cartao_credito.src.Data;
using console.cartao_credito.src.Evento.Consumer;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<RabbitMQCartaoConsumer>();
    });

var host = builder.Build();

var consumer = host.Services.GetRequiredService<RabbitMQCartaoConsumer>();
consumer.Start();

await host.RunAsync();
