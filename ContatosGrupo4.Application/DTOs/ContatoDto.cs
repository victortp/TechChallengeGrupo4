using System.Diagnostics.CodeAnalysis;

namespace ContatosGrupo4.Application.DTOs
{
    [ExcludeFromCodeCoverage]
    public class ContatoDto
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
