using System;

namespace api.crud_cliente.src.Models;

public class Cliente
{
    public required Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Sobrenome { get; set; }
    public required string CPF { get; set; }
    public required string Email { get; set; }
    public required string Celular { get; set; }
    public required decimal RendaMensal { get; set; }
}
