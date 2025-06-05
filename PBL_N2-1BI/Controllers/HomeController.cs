using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PBL_N2_1BI.Models;
using System;

namespace PBL_N2_1BI.Controllers
{
    public class HomeController : Controller
    {

        #region Logger

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Direcionamentos

        public IActionResult Index()
        {
            try
            {
                ViewBag.MensagemErro = HttpContext.Session.GetString("MensagemErro");
                HttpContext.Session.Remove("MensagemErro");

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Sobre()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        #endregion

        #region Tratamento de Erros

        public IActionResult Erro(Exception ex)
        {
            return View("Error", new ErrorViewModel(ex.ToString()));
        }

        #endregion
    }
}
