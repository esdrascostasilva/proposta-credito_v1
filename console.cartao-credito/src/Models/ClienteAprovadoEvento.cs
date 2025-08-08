using System;

namespace console.cartao_credito.src.Models;

public class ClienteAprovadoEvento
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public decimal RendaMensal { get; set; }
    public decimal ValorCreditoAprovado { get; set; }
}

public class ClienteCartaoCredito
{
    public Guid Id { get; set; }
    public required string NomeTitular { get; set; }
    public required string Numero { get; set; }
    public required string Validade { get; set; }
    public required string Cvv { get; set; }
    public required decimal Limite { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
