
using api.crud_cliente.src.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.crud_cliente.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ClienteResposta>>> GetTodosClientes()
        {
            var teste = new ClienteResposta();
            return Ok(teste);
        }
    }
}