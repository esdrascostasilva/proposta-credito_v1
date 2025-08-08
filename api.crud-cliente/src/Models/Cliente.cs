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

public class ClienteResposta
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Celular { get; set; }
}

public class ClienteRequisicao
{
    public required Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Sobrenome { get; set; }
    public required string CPF { get; set; }
    public required string Email { get; set; }
    public required string Celular { get; set; }
    public required decimal RendaMensal { get; set; }
}

public class ClienteCriadoEvento
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CPF { get; set; }
    public decimal RendaMensal { get; set; }
}