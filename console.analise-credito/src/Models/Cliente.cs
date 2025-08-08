using System;

namespace console.analise_credito.src.Models;

public class Cliente
{
    public required Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string CPF { get; set; }
    public required decimal RendaMensal { get; set; }
}

public class AvaliacaoCliente
{
    public Guid Id { get; set; }
    public decimal RendaMensal { get; set; }
    public decimal ValorCreditoOferecido { get; set; }
    public bool estaElegivel { get; set; }
    public DateTime AvalidoEm { get; set; } = DateTime.UtcNow;
}

public class ClienteElegivel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public decimal RendaMensal { get; set; }
    public decimal ValorCreditoAprovado { get; set; }
}