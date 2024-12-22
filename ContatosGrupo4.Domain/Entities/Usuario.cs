namespace ContatosGrupo4.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Login { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public virtual ICollection<Contato> Contato { get; set; } = null!;
    }
}
