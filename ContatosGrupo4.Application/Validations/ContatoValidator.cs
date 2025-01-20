using System.Text.RegularExpressions;

namespace ContatosGrupo4.Application.Validations
{
    public static class ContatoValidator
    {
        private static readonly string TelefoneRegex = @"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$";
        private static readonly string EmailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public static bool ValidarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone)) return false;

            return Regex.IsMatch(telefone, TelefoneRegex);
        }

        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return Regex.IsMatch(email, EmailRegex);
        }
    }
}
