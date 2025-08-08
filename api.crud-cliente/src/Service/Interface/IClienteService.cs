using System;
using api.crud_cliente.src.Models;

namespace api.crud_cliente.src.Service.Interface;

public interface IClienteService
{
    Task<List<ClienteResposta>> GetTodosClientesAsync();
    Task<ClienteResposta> GetClientePorIdAsync(Guid id);
    Task<ClienteResposta> CriarClienteAsync(ClienteRequisicao requisicao);
    Task<bool> AtualizarClienteAsync(Guid id, ClienteRequisicao requisicao);
    Task<bool> RemoverClienteAsync(Guid id);
}