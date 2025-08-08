using System;
using api.crud_cliente.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.crud_cliente.src.Data;

public class ClienteContext : DbContext
{
    public ClienteContext(DbContextOptions<ClienteContext> options) : base(options)
    {

    }
    
    public DbSet<Cliente> Clientes { get; set; }
}
