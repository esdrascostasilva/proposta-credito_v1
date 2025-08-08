using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace console.analise_credito.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvaliacoesClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RendaMensal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorCreditoOferecido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    estaElegivel = table.Column<bool>(type: "bit", nullable: false),
                    AvalidoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesClientes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaliacoesClientes");
        }
    }
}
