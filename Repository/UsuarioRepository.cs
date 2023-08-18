using AutorizacaoAutenticacao.Enum;
using AutorizacaoAutenticacao.Model;

namespace AutorizacaoAutenticacao.Repository
{
    public static class UsuarioRepository
    {
        public static Usuario? BuscaUsuario(string nome)
        {
            var users = new List<Usuario>()
            {
                new(){Nome = "gabriel", UserRoles = new [] { Roles.Admin } },
                new(){Nome = "leonardo", UserRoles = new [] { Roles.Manager } }
            };

            return users.FirstOrDefault(u => u.Nome == nome);
        }
    }
}
