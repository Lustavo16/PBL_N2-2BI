using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PBL_N2_1BI.Models;
using System.Collections.Generic;
using System.Linq;

namespace PBL_N2_1BI.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuarioLogado = context.HttpContext.Session.GetString("Login");
            List<PermissaoViewModel> listaPermissoes = new List<PermissaoViewModel>();

            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            var urlAnterior = context.HttpContext.Request.Headers["Referer"].ToString();

            // Verifica se está logado
            if (string.IsNullOrEmpty(usuarioLogado))
            {
                context.Result = new RedirectToActionResult("Login", "Conta", null);
                return;
            }
            else
            {
                var permissoesSession = context.HttpContext.Session.GetString("Permissoes");
                if (!string.IsNullOrEmpty(permissoesSession))
                {
                    listaPermissoes = JsonConvert.DeserializeObject<List<PermissaoViewModel>>(permissoesSession);
                }
                else
                {
                    if (action == "Excluir")
                        context.HttpContext.Session.SetString("MensagemErro", "Você não tem permissão realizar esta ação.");
                    else
                        context.HttpContext.Session.SetString("MensagemErro", "Você não tem permissão para acessar esta página.");

                    if (!string.IsNullOrEmpty(urlAnterior))
                    {
                        context.Result = new RedirectResult(urlAnterior);
                    }
                    else
                    {
                        context.Result = new RedirectToActionResult("Index", "Home", null);
                    }

                    return;
                }

                bool temPermissao = listaPermissoes.Any(xs => xs.NomeController == controller && xs.NomeAction == action);

                if (!temPermissao)
                {
                    if (action == "Excluir")
                        context.HttpContext.Session.SetString("MensagemErro", "Você não tem permissão para realizar esta ação.");
                    else
                        context.HttpContext.Session.SetString("MensagemErro", "Você não tem permissão para acessar esta página.");

                    if (!string.IsNullOrEmpty(urlAnterior))
                    {
                        context.Result = new RedirectResult(urlAnterior);
                    }
                    else
                    {
                        context.Result = new RedirectToActionResult("Index", "Home", null);
                    }

                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
