using AutorizacaoAutenticacao.Enum;
using AutorizacaoAutenticacao.Model;

namespace AutorizacaoAutenticacao.Repository
{
    public static class UsuarioRepository
    {
        public static Usuario? Login(string nome, string senha)
        {
            var users = new List<Usuario>()
            {
                new() { Nome = "gabriel", Senha = "123", UserRoles = new [] { Roles.Funcionario } },
                new() { Nome = "leonardo", Senha = "321", UserRoles = new [] { Roles.Funcionario } },
                new() { Nome = "pedro", Senha = "456", UserRoles = new [] { Roles.Gerente } },
                new() { Nome = "william", Senha = "654", UserRoles = new [] { Roles.Diretor } }
            };

            return users.FirstOrDefault(u => u.Nome == nome && u.Senha == senha);
        }
    }
}
