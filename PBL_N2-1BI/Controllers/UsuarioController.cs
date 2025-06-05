using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Filters;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;

namespace PBL_N2_1BI.Controllers
{
    public class UsuarioController : Controller
    {
        [SessionAuthorize]
        public IActionResult Consulta(UsuarioViewModel usuarioConsulta)
        {
            try
            {
                ViewBag.MensagemErro = HttpContext.Session.GetString("MensagemErro");
                HttpContext.Session.Remove("MensagemErro");

                List<UsuarioViewModel> listaUauarios = new List<UsuarioViewModel>();
                listaUauarios = new UsuarioDAO().ListarUsuarios(usuarioConsulta);

                string TesteExtraiLogin = HttpContext.Session.GetString("Login");

                ViewBag.Filtros = usuarioConsulta;

                return View(listaUauarios);

            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        [SessionAuthorize]
        public IActionResult Adicionar()
        {
            try
            {
                UsuarioViewModel usuarioNovo = new UsuarioViewModel();
                ViewBag.Perfis = new PerfilDAO().ListarPerfis(new PerfilViewModel());

                return View("Cadastro", usuarioNovo);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        [SessionAuthorize]
        public IActionResult Editar(int idUsuario)
        {
            try
            {
                UsuarioViewModel usuarioNovo = new UsuarioViewModel();

                usuarioNovo = new UsuarioDAO().PesquisarPorId(idUsuario);

                usuarioNovo.FotoBase64 = usuarioNovo.Foto != null ? Convert.ToBase64String(usuarioNovo.Foto) : null;

                ViewBag.Perfis = new PerfilDAO().ListarPerfis(new PerfilViewModel());

                return View("Cadastro", usuarioNovo);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        [HttpPost]
        public IActionResult Salvar(UsuarioViewModel usuario)
        {
            try
            {
                UsuarioDAO dao = new UsuarioDAO();
                var arquivo = Request.Form.Files["Foto"];
                if (arquivo != null && arquivo.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        arquivo.CopyTo(ms);
                        usuario.Foto = ms.ToArray();
                        usuario.FotoBase64 = Convert.ToBase64String(usuario.Foto);
                    }
                }
                else if (!string.IsNullOrEmpty(usuario.FotoBase64))
                {
                    usuario.Foto = Convert.FromBase64String(usuario.FotoBase64);
                }

                if (usuario.IsPrimeiroAcesso)
                {
                    if (!usuario.Id.HasValue)
                    {
                        dao.Inserir(usuario);
                        TempData["Mensagem"] = "Usuário salvo com sucesso!";
                    }
                    else
                    {
                        dao.AlterarCadastro(usuario);
                        TempData["Mensagem"] = "Usuário alterado com sucesso!";

                        var login = HttpContext.Session.GetString("Login");

                        if (login != null)
                        {
                            LoginViewModel usuarioLogin = JsonConvert.DeserializeObject<LoginViewModel>(login);

                            if (usuarioLogin != null && usuarioLogin.Login != null)
                            {
                                if (usuarioLogin.Login == usuario.Login)
                                {
                                    usuarioLogin.FotoBase64 = usuario.FotoBase64;

                                    HttpContext.Session.SetString("Login", JsonConvert.SerializeObject(usuarioLogin));
                                }
                            }
                        }
                    }
                }
                else
                {
                    dao.Inserir(usuario);

                    TempData["Mensagem"] = "Usuário salvo com sucesso!";
                    return RedirectToAction("Login", "Login");
                }

                return RedirectToAction("Consulta");

            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }

        }

        [SessionAuthorize]
        public IActionResult Excluir(int Id)
        {
            try
            {
                new UsuarioDAO().Excluir(Id);

                TempData["Mensagem"] = "Usuário excluído com sucesso!";

                return RedirectToAction("Consulta");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }
    }
}
