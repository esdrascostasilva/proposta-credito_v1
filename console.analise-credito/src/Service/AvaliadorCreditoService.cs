using System;
using console.analise_credito.src.Models;

namespace console.analise_credito.src.Service;

public class AvaliadorCreditoService
{
    public AvaliacaoCliente Avaliar(Cliente cliente)
    {
        decimal valorCredito = 0;
        bool elegivel = false;

        if (cliente.RendaMensal > 1500 && cliente.RendaMensal <= 3000)
        {
            valorCredito = cliente.RendaMensal * 0.33m;
            elegivel = true;
        }

        else if (cliente.RendaMensal > 3000 && cliente.RendaMensal <= 5000)
        {
            valorCredito = cliente.RendaMensal * 0.66m;
            elegivel = true;
        }

        if (cliente.RendaMensal < 5000)
        {
            valorCredito = cliente.RendaMensal * 1.2m;
            elegivel = true;
        }

        return new AvaliacaoCliente
        {
            Id = cliente.Id,
            RendaMensal = cliente.RendaMensal,
            ValorCreditoOferecido = valorCredito,
            estaElegivel = elegivel
        };
    }

}
