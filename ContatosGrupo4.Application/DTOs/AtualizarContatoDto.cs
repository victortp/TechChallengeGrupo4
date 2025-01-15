namespace ContatosGrupo4.Application.DTOs;

public class AtualizarContatoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int CodigoArea { get; set; } = 0;
}