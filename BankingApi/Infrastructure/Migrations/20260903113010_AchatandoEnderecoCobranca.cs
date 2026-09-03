using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AchatandoEnderecoCobranca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnderecoCobranca_Rua",
                table: "Contas",
                newName: "Rua");

            migrationBuilder.RenameColumn(
                name: "EnderecoCobranca_Estado",
                table: "Contas",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "EnderecoCobranca_Cidade",
                table: "Contas",
                newName: "Cidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rua",
                table: "Contas",
                newName: "EnderecoCobranca_Rua");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Contas",
                newName: "EnderecoCobranca_Estado");

            migrationBuilder.RenameColumn(
                name: "Cidade",
                table: "Contas",
                newName: "EnderecoCobranca_Cidade");
        }
    }
}
