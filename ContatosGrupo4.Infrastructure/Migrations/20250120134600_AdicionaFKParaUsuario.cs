using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContatosGrupo4.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaFKParaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Contato",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Contato");
        }
    }
}
