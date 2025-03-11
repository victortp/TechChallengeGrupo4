using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContatosGrupo4.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeFKParaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contato_Usuario_Id",
                table: "Contato");

            migrationBuilder.CreateIndex(
                name: "IX_Contato_UsuarioId",
                table: "Contato",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contato_Usuario_UsuarioId",
                table: "Contato",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contato_Usuario_UsuarioId",
                table: "Contato");

            migrationBuilder.DropIndex(
                name: "IX_Contato_UsuarioId",
                table: "Contato");

            migrationBuilder.AddForeignKey(
                name: "FK_Contato_Usuario_Id",
                table: "Contato",
                column: "Id",
                principalTable: "Usuario",
                principalColumn: "Id");
        }
    }
}
