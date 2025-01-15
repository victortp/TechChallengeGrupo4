﻿namespace ContatosGrupo4.Domain.Entities
{
    public class Contato : BaseEntity
    {
        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int CodigoArea { get; set; } = 0;
        public virtual Usuario Usuario { get; set; } = null!;
    }
}