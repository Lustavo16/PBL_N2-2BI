using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System.Collections.Generic;
using System;
using PBL_N2_1BI.Filters;
using Microsoft.AspNetCore.Http;

namespace PBL_N2_1BI.Controllers
{
    public class SimulacaoController : Controller
    {
        [SessionAuthorize]
        public IActionResult Consulta(SimulacaoViewModel simulacaoConsulta)
        {
            ViewBag.MensagemErro = HttpContext.Session.GetString("MensagemErro");
            HttpContext.Session.Remove("MensagemErro");

            List<SimulacaoViewModel> listaSimulacao = new List<SimulacaoViewModel>();

            listaSimulacao = new SimulacaoDAO().ListarSimulacao(simulacaoConsulta);

            ViewBag.Motores = new MotorDAO().ListarMotores(new MotorViewModel());
            ViewBag.Usuarios = new UsuarioDAO().ListarUsuarios(new UsuarioViewModel());

            ViewBag.Filtros = simulacaoConsulta;

            return View(listaSimulacao);
        }

        [SessionAuthorize]
        public IActionResult Adicionar()
        {
            SimulacaoViewModel SimulacaoNovo = new SimulacaoViewModel();
            ViewBag.Motores = new MotorDAO().ListarMotores(new MotorViewModel());
            ViewBag.Usuarios = new UsuarioDAO().ListarUsuarios(new UsuarioViewModel());
            
            return View("Cadastro", SimulacaoNovo);
        }

        [SessionAuthorize]
        public IActionResult Editar(int idSimulacao)
        {
            SimulacaoViewModel SimulacaoNovo = new SimulacaoViewModel();

            SimulacaoNovo = new SimulacaoDAO().PesquisarPorId(idSimulacao);

            ViewBag.Motores = new MotorDAO().ListarMotores(new MotorViewModel());
            ViewBag.Usuarios = new UsuarioDAO().ListarUsuarios(new UsuarioViewModel());

            return View("Cadastro", SimulacaoNovo);
        }

        public IActionResult Salvar(SimulacaoViewModel Simulacao)
        {
            SimulacaoDAO dao = new SimulacaoDAO();
            try
            {
                if (!Simulacao.Id.HasValue)
                {
                    dao.Inserir(Simulacao);
                    TempData["Mensagem"] = "Simulação salva com sucesso!";
                }
                else
                {
                    dao.Alterar(Simulacao);
                    TempData["Mensagem"] = "Simulação alterada com sucesso!";
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
                new SimulacaoDAO().Excluir(Id);
                TempData["Mensagem"] = "Simulação excluída com sucesso!";
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
            return RedirectToAction("Consulta");
        }
    }
}
