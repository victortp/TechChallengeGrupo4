namespace ContatosGrupo4.Application.DTOs
{
    public class UsuarioAtualizarDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }
}
