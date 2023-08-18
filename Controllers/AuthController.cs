using AutorizacaoAutenticacao.Repository;
using AutorizacaoAutenticacao.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutorizacaoAutenticacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult Login(string nome, string senha)
        {
            try
            {
                var usuario = UsuarioRepository.Login(nome, senha);
                if (usuario == null)
                    return new JsonResult(new { Sucesso = false, Mensagem = "Login ou Senha inválido." }) { StatusCode = 400 };

                var token = _tokenService.CreateToken(usuario);

                return new JsonResult(new { Sucesso = true, Mensagem = "Usuário autenticado.", Token = token }) { StatusCode = 200 };
            } catch
            {
                return new JsonResult(new { Sucesso = false, Mensagem = "Erro interno." }) { StatusCode = 500 };
            }
        }
    }
}
