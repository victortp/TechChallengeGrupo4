using Bogus;
using ContatosGrupo4.Domain.Entities;

namespace ContatosGrupo4.Tests
{
    public static class FakeData
    {
        public static List<Contato> ContatoFake()
        {
            var contatoFake = new Faker<Contato>("pt_BR")
                .RuleFor(c => c.Nome, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email(f.Person.FirstName.ToLower()))
                .RuleFor(c => c.Telefone, f => f.Phone.PhoneNumber("###########"));

            return contatoFake.Generate(2);
        }
    }
}
