using System;
using console.cartao_credito.src.Models;
using Microsoft.EntityFrameworkCore;

namespace console.cartao_credito.src.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    
    public DbSet<ClienteCartaoCredito> ClientesCartaoCredito { get; set; }
}
