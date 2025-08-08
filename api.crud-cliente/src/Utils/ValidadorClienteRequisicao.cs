using System;
using api.crud_cliente.src.Models;
using FluentValidation;

namespace api.crud_cliente.src.Utils;

public class ValidadorClienteRequisicao : AbstractValidator<ClienteRequisicao>
{
    public ValidadorClienteRequisicao()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("O campo email eh obrigatorio")
            .EmailAddress().WithMessage("O email informado esta invalido");

        RuleFor(c => c.Celular)
            .Matches(@"^\d{10,11}$").WithMessage("O celular esta invalido");
    }
}
