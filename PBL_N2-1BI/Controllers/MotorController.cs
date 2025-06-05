using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Filters;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;

namespace PBL_N2_1BI.Controllers
{
    public class MotorController : Controller
    {
        [SessionAuthorize]
        public IActionResult Consulta(MotorViewModel motorConsulta)
        {
            try
            {
                ViewBag.MensagemErro = HttpContext.Session.GetString("MensagemErro");
                HttpContext.Session.Remove("MensagemErro");

                List<MotorViewModel> listaMotores = new List<MotorViewModel>();
                listaMotores = new MotorDAO().ListarMotores(motorConsulta);

                ViewBag.Filtros = motorConsulta;

                return View(listaMotores);
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
                MotorViewModel motorNovo = new MotorViewModel();
                return View("Cadastro", motorNovo);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        [SessionAuthorize]
        public IActionResult Editar(int idMotor)
        {
            try
            {
                MotorViewModel motorNovo = new MotorViewModel();
                motorNovo = new MotorDAO().PesquisarPorId(idMotor);

                return View("Cadastro", motorNovo);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Salvar(MotorViewModel motor)
        {
            try
            {
                MotorDAO dao = new MotorDAO();

                if (!motor.Id.HasValue)
                {
                    dao.Inserir(motor);
                    TempData["Mensagem"] = "Motor salvo com sucesso!";
                }
                else
                {
                    dao.Alterar(motor);
                    TempData["Mensagem"] = "Motor alterado com sucesso!";
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
                new MotorDAO().Excluir(Id);
                TempData["Mensagem"] = "Motor excluído com sucesso!";

                return RedirectToAction("Consulta");

            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }

        }
    }
}
