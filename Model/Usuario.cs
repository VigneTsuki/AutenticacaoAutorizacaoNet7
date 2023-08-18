using AutorizacaoAutenticacao.Enum;

namespace AutorizacaoAutenticacao.Model
{
    public class Usuario
    {
        public string Nome { get; set; }
        public string Senha { get; set; }
        public Roles[] UserRoles { get; set; }
    }
}
