using AutorizacaoAutenticacao.Auth;
using AutorizacaoAutenticacao.Enum;
using AutorizacaoAutenticacao.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AutorizacaoAutenticacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("ApiBlock")]
    public class ProdutoController : ControllerBase
    {
        [HttpGet]
        [CustomAuthorize(UserRoles = new[] { Roles.Diretor, Roles.Gerente, Roles.Funcionario })]
        public ActionResult Get()
        {
            var produtos = new List<Produto>() 
            { 
                new() { Id = 1, Nome = "Bolacha", QuantidadeEstoque = 10 } ,
                new() { Id = 2, Nome = "Biscoito", QuantidadeEstoque = 15 } 
            };
            return new JsonResult(new { Sucesso = true, Mensagem = "Busca realizada com sucesso.", Produtos = produtos }) { StatusCode = 200 };
        }

        [HttpPost]
        [CustomAuthorize(UserRoles = new[] { Roles.Diretor, Roles.Gerente })]
        public ActionResult Post()
        {
            return new JsonResult(new { Sucesso = true, Mensagem = "Produto cadastrado com sucesso." }) { StatusCode = 201 };
        }

        [HttpDelete]
        [CustomAuthorize(UserRoles = new[] { Roles.Diretor })]
        public ActionResult Delete()
        {
            return new JsonResult(new { Sucesso = true, Mensagem = "Produto deletado com sucesso." }) { StatusCode = 200 };
        }
    }
}
