using AutorizacaoAutenticacao.Auth;
using AutorizacaoAutenticacao.Enum;
using AutorizacaoAutenticacao.Model;
using AutorizacaoAutenticacao.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutorizacaoAutenticacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("teste")]
        [CustomAuthorize(UserRoles = new[] { Roles.Admin })]
        public ActionResult Teste()
        {
            return Ok("1");
        }

        [HttpGet("teste2")]
        [CustomAuthorize(UserRoles = new[] { Roles.Admin, Roles.Manager })]
        public ActionResult Teste2()
        {
            return Ok("2");
        }

        [HttpPost("login")]
        public ActionResult Login(string nome)
        {
            var usuario = UsuarioRepository.BuscaUsuario(nome);
            var token = CreateToken(usuario);
            return Ok(token);
        }

        private string CreateToken(Usuario usuario)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            foreach (var role in usuario.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, role.GetDisplayName()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
