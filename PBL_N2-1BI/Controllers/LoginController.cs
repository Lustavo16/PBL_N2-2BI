using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System;

namespace PBL_N2_1BI.Controllers
{
    public class LoginController : Controller
    {
        #region Login

        public IActionResult Login(LoginViewModel loginUsuario)
        {
            try
            {
                HttpContext.Session.Clear();
                TempData["Login"] = "Login";

                return View(loginUsuario);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Entrar(LoginViewModel loginUsuario)
        {
            try
            {
                UsuarioDAO dao = new UsuarioDAO();

                if (dao.ValidarLogin(loginUsuario))
                {
                    string login = JsonConvert.SerializeObject(loginUsuario);
                    HttpContext.Session.SetString("Login", login);

                    TempData["Login"] = null;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("IsUsuarioValido", "Usuário ou senha inválidos.");

                    TempData["Login"] = "Login";
                    return View("Login", loginUsuario);
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }
        #endregion

        #region Cadastro

        public IActionResult Cadastrar(UsuarioViewModel usuario, bool isLoginEmUso = false)
        {
            try
            {
                TempData["Login"] = "Login";

                if (!isLoginEmUso || usuario == null)
                    return View("Cadastro", new UsuarioViewModel());
                else
                {
                    ModelState.AddModelError("Login", "Este Login já está em uso!");
                    return View("Cadastro", usuario);
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }

        }

        public IActionResult CadastrarSenha(string loginUsuario, bool IsEmailValido = true)
        {
            try
            {
                TempData["Login"] = "Login";

                UsuarioViewModel model = new UsuarioDAO().PesquisarPorLogin(loginUsuario);
                ModelState.AddModelError("Email", "Preencha o Email para provar sua titularidade da conta!");

                if (!IsEmailValido)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Email", "Email incorreto!");
                }

                return View("Cadastro", model);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Salvar(UsuarioViewModel usuario)
        {

            try
            {
                UsuarioDAO dao = new UsuarioDAO();
                LoginViewModel model = new LoginViewModel();

                model.Login = usuario.Login;

                if (!usuario.Id.HasValue)
                {
                    if (dao.VerificarLoginCadastro(usuario.Login))
                    {
                        return Cadastrar(usuario, true);
                    }

                    dao.Inserir(usuario);
                    TempData["Mensagem"] = "Usuário salvo com sucesso!";
                }
                else
                {
                    if (!VerificarEmail(usuario))
                    {
                        return CadastrarSenha(model.Login, false);
                    }

                    dao.Alterar(usuario);
                    TempData["Mensagem"] = "Senha cadastrada com sucesso!";
                }

                TempData["Login"] = "Login";
                return View("Login", model);

            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        #endregion

        #region Validações

        public bool VerificarExistenciaLogin(string loginUsuario)
        {
            try
            {
                bool retorno = new UsuarioDAO().VerificarPrimeiroAcesso(loginUsuario);

                return retorno;
            }
            catch (Exception ex)
            {
                Erro(ex);
                return false;
            }
        }

        public bool VerificarEmail(UsuarioViewModel usuario)
        {
            try
            {
                bool retorno = new UsuarioDAO().ValidarEmail(usuario);

                return retorno;
            }
            catch (Exception ex)
            {
                Erro(ex);
                return false;
            }
        }

        public IActionResult Erro(Exception ex)
        {
            return View("Error", new ErrorViewModel(ex.ToString()));
        }

        #endregion
    }
}
