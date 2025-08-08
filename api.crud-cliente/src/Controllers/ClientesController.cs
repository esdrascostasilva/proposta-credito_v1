
using api.crud_cliente.src.Models;
using api.crud_cliente.src.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace api.crud_cliente.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClienteResposta>>> ObterTodosClientes()
        {
            var clientes = await _clienteService.GetTodosClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResposta>> ObterClientePorId(Guid id)
        {
            var cliente = await _clienteService.GetClientePorIdAsync(id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteResposta>> CriarCliente([FromBody] ClienteRequisicao clienteRequisicao)
        {
            var novoCliente = await _clienteService.CriarClienteAsync(clienteRequisicao);

            return CreatedAtAction(nameof(ObterClientePorId), new { id = novoCliente.Id }, novoCliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCliente(Guid id, [FromBody] ClienteRequisicao clienteRequisicao)
        {
            var clienteAtualizado = await _clienteService.AtualizarClienteAsync(id, clienteRequisicao);

            if (clienteAtualizado == false)
                return NotFound();

            return Ok(clienteAtualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarCliente(Guid id)
        {
            var clienteDeletado = await _clienteService.RemoverClienteAsync(id);

            if (clienteDeletado == false)
                return NotFound();

            return NoContent();
        }
    }
}