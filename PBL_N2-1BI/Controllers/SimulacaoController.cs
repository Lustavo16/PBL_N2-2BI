using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System.Collections.Generic;
using System;
using PBL_N2_1BI.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<ContentResult> ObterDadosAgregadosMedia(string ip, string tipoSensor, string idSensor, string atributo, DateTime dateFrom, DateTime dateTo)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("fiware-service", "smart");
            client.DefaultRequestHeaders.Add("fiware-servicepath", "/");

            string dateFromStr = dateFrom.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            DateTime dateToCorrigido = dateTo.AddDays(1);
            string dateToStr = dateToCorrigido.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            int offset = 0;
            bool temMaisDados = true;

            var todosRegistros = new List<(DateTime timestamp, double valor)>();

            while (temMaisDados)
            {
                var url = $"http://{ip}:8666/STH/v1/contextEntities/type/{tipoSensor}/id/{idSensor}/attributes/{atributo}" +
                          $"?hLimit=100&hOffset={offset}";

                if (dateFrom.ToShortDateString() != "01/01/0001")
                {
                    url += $"&dateFrom={dateFromStr}";
                }
                if (dateFrom.ToShortDateString() != "01/01/0001")
                {
                    url += $"&dateTo={dateToStr}";
                }

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    break;

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                var valuesElement = root
                    .GetProperty("contextResponses")[0]
                    .GetProperty("contextElement")
                    .GetProperty("attributes")[0]
                    .GetProperty("values");

                if (valuesElement.GetArrayLength() == 0)
                {
                    temMaisDados = false;
                    break;
                }

                foreach (var item in valuesElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        var tsStr = item.GetProperty("recvTime").GetString();
                        var val = item.GetProperty("attrValue").GetDouble();

                        if (DateTime.TryParse(tsStr, out DateTime dt))
                        {
                            todosRegistros.Add((dt, val));
                        }
                    }
                }

                offset += 100;
            }

            var agrupados = todosRegistros
                .OrderBy(x => x.timestamp)
                .GroupBy(x => (long)(x.timestamp - todosRegistros[0].timestamp).TotalSeconds / 10)
                .Select(g => new
                {
                    timestamp = g.First().timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    valorMedio = g.Average(x => x.valor)
                })
                .ToList();

            var valuesJsonArray = new JsonArray();

            foreach (var grupo in agrupados)
            {
                valuesJsonArray.Add(new JsonArray { grupo.timestamp, grupo.valorMedio });
            }

            var jsonFinal = valuesJsonArray.ToJsonString();

            return Content(jsonFinal, "application/json");
        }
    }
}
