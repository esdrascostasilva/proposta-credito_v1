using System;
using api.crud_cliente.src.Data;
using api.crud_cliente.src.Evento.Interface;
using api.crud_cliente.src.Models;
using api.crud_cliente.src.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace api.crud_cliente.src.Service;

public class ClienteService : IClienteService
{
    private readonly ClienteContext _context;
    private readonly IEventBus _eventBus;

    public ClienteService(ClienteContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task<List<ClienteResposta>> GetTodosClientesAsync()
    {
        return await _context.Clientes
            .Select(c => new ClienteResposta
            {
                Id = c.Id,
                NomeCompleto = $"{c.Nome} {c.Sobrenome}",
                Email = c.Email,
                Celular = c.Celular
            }).ToListAsync();
    }

    public async Task<ClienteResposta> GetClientePorIdAsync(Guid id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return null;

        return new ClienteResposta
        {
            Id = cliente.Id,
            NomeCompleto = $"{cliente.Nome} {cliente.Sobrenome}",
            Email = cliente.Email,
            Celular = cliente.Celular
        };
    }

    public async Task<ClienteResposta> CriarClienteAsync(ClienteRequisicao requisicao)
    {
        var novoCliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = requisicao.Nome,
            Sobrenome = requisicao.Sobrenome,
            CPF = requisicao.CPF,
            Email = requisicao.Email,
            Celular = requisicao.Celular,
            RendaMensal = requisicao.RendaMensal
        };

        _context.Clientes.Add(novoCliente);
        await _context.SaveChangesAsync();

        _eventBus.Publish(new ClienteCriadoEvento
        {
            Id = novoCliente.Id,
            Nome = novoCliente.Nome,
            CPF = novoCliente.CPF,
            RendaMensal = novoCliente.RendaMensal
        }, "clientes.criados");


        return new ClienteResposta
        {
            Id = novoCliente.Id,
            NomeCompleto = $"{novoCliente.Nome} {novoCliente.Sobrenome}",
            Email = novoCliente.Email,
            Celular = novoCliente.Celular
        };
    }

    public async Task<bool> AtualizarClienteAsync(Guid id, ClienteRequisicao requisicao)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return false;

        cliente.Nome = requisicao.Nome;
        cliente.Sobrenome = requisicao.Sobrenome;
        cliente.CPF = requisicao.CPF;
        cliente.Email = requisicao.Email;
        cliente.Celular = requisicao.Celular;
        cliente.RendaMensal = requisicao.RendaMensal;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoverClienteAsync(Guid id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return true;
    }
}
