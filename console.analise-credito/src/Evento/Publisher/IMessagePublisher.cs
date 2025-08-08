using System;

namespace console.analise_credito.src.Evento.Publisher;

public interface IMessagePublisher
{
    Task PublicarAsync<T>(string queueName, T message);
}
