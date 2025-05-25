using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;

namespace PBL_N2_1BI.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Consulta(UsuarioViewModel usuarioConsulta)
        {
            List<UsuarioViewModel> listaUauarios = new List<UsuarioViewModel>();
            listaUauarios = new UsuarioDAO().ListarUsuarios(usuarioConsulta);

            string TesteExtraiLogin = HttpContext.Session.GetString("Login");

            ViewBag.Filtros = usuarioConsulta;

            return View(listaUauarios);
        }

        public IActionResult Adicionar()
        {
            UsuarioViewModel usuarioNovo = new UsuarioViewModel();
            return View("Cadastro", usuarioNovo);
        }

        public IActionResult Editar(int idUsuario)
        {
            UsuarioViewModel usuarioNovo = new UsuarioViewModel();
            usuarioNovo = new UsuarioDAO().PesquisarPorId(idUsuario);

            return View("Cadastro", usuarioNovo);
        }

        public IActionResult Salvar(UsuarioViewModel usuario)
        {
            UsuarioDAO dao = new UsuarioDAO();
            try
            {
                if (usuario.IsPrimeiroAcesso)
                {
                    if (!usuario.Id.HasValue)
                    {
                        dao.Inserir(usuario);
                        TempData["Mensagem"] = "Usuário salvo com sucesso!";
                    }
                    else
                    {
                        dao.Alterar(usuario);
                        TempData["Mensagem"] = "Usuário alterado com sucesso!";
                    }
                }
                else
                {
                    dao.Inserir(usuario);

                    TempData["Mensagem"] = "Usuário salvo com sucesso!";
                    return RedirectToAction("Login", "Login");
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }

            return RedirectToAction("Consulta");
        }

        public IActionResult Excluir(int Id)
        {
            try
            {
                new UsuarioDAO().Excluir(Id);
                TempData["Mensagem"] = "Usuário excluído com sucesso!";
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }

            return RedirectToAction("Consulta");
        }
    }
}
