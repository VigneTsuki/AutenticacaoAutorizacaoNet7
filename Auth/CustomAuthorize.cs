using AutorizacaoAutenticacao.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Extensions;

namespace AutorizacaoAutenticacao.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public Roles[] UserRoles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new { Sucesso = false, Mensagem = "Não Autenticado." })
                {
                    StatusCode = 401
                };
                return;
            }

            if (!IsInRole(context))
            {
                context.Result = new JsonResult(new { Sucesso = false, Mensagem = "Não Autorizado." })
                {
                    StatusCode = 403
                };
                return;
            }
        }

        private bool IsInRole(AuthorizationFilterContext context)
        {
            foreach (var role in UserRoles)
                if (context.HttpContext.User.IsInRole(role.GetDisplayName()))
                    return true;

            return false;
        }
    }
}
