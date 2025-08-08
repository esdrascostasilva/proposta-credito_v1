using System;

namespace api.crud_cliente.src.Evento.Interface;

public interface IEventBus
{
    void Publish<T>(T message, string queue);
}
