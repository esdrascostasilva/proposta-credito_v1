
using api.crud_cliente.src.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace api.crud_cliente.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> GetAllClientes()
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Esdras",
                Sobrenome = "Costa",
                CPF = "37428208858",
                Email = "esdras.cts@outlook.com",
                Celular = "15988253887",
                RendaMensal = 2000.0m
            };
            
            return Ok(cliente);
        }
    }
}
