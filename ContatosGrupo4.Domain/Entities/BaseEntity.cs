namespace ContatosGrupo4.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime DataAtualizacao { get; private set; }

        public void SetDataCriacao()
        {
            DataCriacao = DateTime.Now;
            SetDataAtualizacao();
        }

        public void SetDataAtualizacao()
        {
            DataAtualizacao = DateTime.Now;
        }
    }
}
