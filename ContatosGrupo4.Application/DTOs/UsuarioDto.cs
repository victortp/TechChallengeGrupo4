using System.Diagnostics.CodeAnalysis;

namespace ContatosGrupo4.Application.DTOs
{
    [ExcludeFromCodeCoverage]
    public class UsuarioDto
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string Login { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public ICollection<ContatoDto> Contato { get; set; } = null!;
    }
}
