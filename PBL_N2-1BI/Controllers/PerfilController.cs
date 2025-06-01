using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System.Collections.Generic;
using System;
using PBL_N2_1BI.Filters;
using Microsoft.AspNetCore.Http;

namespace PBL_N2_1BI.Controllers
{
    public class PerfilController : Controller
    {
        [SessionAuthorize]
        public IActionResult Consulta(PerfilViewModel perfilConsulta)
        {
            ViewBag.MensagemErro = HttpContext.Session.GetString("MensagemErro");
            HttpContext.Session.Remove("MensagemErro");

            List<PerfilViewModel> listaPerfis = new List<PerfilViewModel>();

            listaPerfis = new PerfilDAO().ListarPerfis(perfilConsulta);

            ViewBag.Filtros = perfilConsulta;

            return View("Index", listaPerfis);
        }

        [SessionAuthorize]
        public IActionResult Adicionar()
        {
            PerfilViewModel perfilNovo = new PerfilViewModel();
            return View("Cadastro", perfilNovo);
        }

        [SessionAuthorize]
        public IActionResult Editar(int idperfil)
        {
            PerfilViewModel perfilNovo = new PerfilViewModel();
            perfilNovo = new PerfilDAO().PesquisarPorId(idperfil);

            return View("Cadastro", perfilNovo);
        }

        public IActionResult Salvar(PerfilViewModel perfil)
        {
            PerfilDAO dao = new PerfilDAO();
            try
            {
                if (!perfil.Id.HasValue)
                {
                    dao.Inserir(perfil);
                    TempData["Mensagem"] = "Perfil salvo com sucesso!";
                }
                else
                {
                    dao.Alterar(perfil);
                    TempData["Mensagem"] = "Perfil alterado com sucesso!";
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
            return RedirectToAction("Consulta");
        }

        [SessionAuthorize]
        public IActionResult Excluir(int Id)
        {
            try
            {
                new PerfilDAO().Excluir(Id);
                TempData["Mensagem"] = "Perfil excluído com sucesso!";
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
            return RedirectToAction("Consulta");
        }
    }
}

