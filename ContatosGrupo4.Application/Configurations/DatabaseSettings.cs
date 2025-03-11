using System.Diagnostics.CodeAnalysis;

namespace ContatosGrupo4.Application.Configurations
{
    [ExcludeFromCodeCoverage]
    public class DatabaseSettings
    {
        public string? ConnectionString { get; set; } = null!;
    }
}
