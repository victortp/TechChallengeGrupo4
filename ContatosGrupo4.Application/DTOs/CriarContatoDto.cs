namespace ContatosGrupo4.Application.DTOs;

public class CriarContatoDto
{
    public string? Nome { get; set; }
    public string Telefone { get; set; } = null!;
    public string Email { get; set; } = null!;
}