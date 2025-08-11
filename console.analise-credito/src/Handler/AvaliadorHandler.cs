using console.analise_credito.src.Data;
using console.analise_credito.src.Evento.Publisher;
using console.analise_credito.src.Models;
using console.analise_credito.src.Service;

public class AvaliadorHandler
{
    private readonly AvaliadorCreditoService _avaliador;
    private readonly IMessagePublisher _publisher;
    private readonly DataContext _dbContext;

    public AvaliadorHandler(AvaliadorCreditoService avaliador, IMessagePublisher publisher, DataContext dbContext)
    {
        _avaliador = avaliador;
        _publisher = publisher;
        _dbContext = dbContext;
    }

    public async Task ProcessarCliente(Cliente cliente)
    {
        var avaliacao = _avaliador.Avaliar(cliente);

        if (!_dbContext.AvaliacoesClientes.Any(a => a.Id == avaliacao.Id))
        {
            _dbContext.AvaliacoesClientes.Add(avaliacao);
            await _dbContext.SaveChangesAsync();
        }

        if (avaliacao.estaElegivel)
        {
            var clienteElegivel = new ClienteElegivel
            {
                Id = avaliacao.Id,
                Nome = cliente.Nome,
                CPF = cliente.CPF,
                Email = cliente.Email,
                RendaMensal = avaliacao.RendaMensal,
                ValorCreditoAprovado = avaliacao.ValorCreditoOferecido
            };

            Console.WriteLine($"[Publish] Publicando mensagem para clientes.elegiveis");
            await _publisher.PublicarAsync("clientes.elegiveis", clienteElegivel);
            Console.WriteLine($"[Publish] Mensagem publicada");

        }
    }
}
