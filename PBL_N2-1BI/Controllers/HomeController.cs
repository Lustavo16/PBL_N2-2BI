﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PBL_N2_1BI.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<IActionResult> ObterDispositivos()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("fiware-service", "smart");
            client.DefaultRequestHeaders.Add("fiware-servicepath", "/");

            var response = await client.GetAsync("http://44.207.2.184:4041/iot/devices");
            var content = await response.Content.ReadAsStringAsync();

            return Content(content, "application/json");
        }       
    }
}
