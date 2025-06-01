using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PBL_N2_1BI.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using PBL_N2_1BI.Filters;
using Microsoft.AspNetCore.Http;

namespace PBL_N2_1BI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.MensagemErro = HttpContext.Session.GetString("MensagemErro");
            HttpContext.Session.Remove("MensagemErro");

            return View();
        }
      
        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
