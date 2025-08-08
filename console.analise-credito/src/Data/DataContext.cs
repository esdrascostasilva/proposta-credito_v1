using console.analise_credito.src.Models;
using Microsoft.EntityFrameworkCore;

namespace console.analise_credito.src.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base (options)
    {
        
    }
    public DbSet<AvaliacaoCliente> AvaliacoesClientes { get; set; }
}
