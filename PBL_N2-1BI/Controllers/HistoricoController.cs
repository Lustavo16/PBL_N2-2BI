using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PBL_N2_1BI.Controllers
{
    public class HistoricoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ContentResult> ObterDadosAgregadosMedia(string ip, string tipoSensor, string idSensor, string atributo, DateTime dateFrom, DateTime dateTo, int intervalo = 1)
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
                .GroupBy(x => (long)(x.timestamp - todosRegistros[0].timestamp).TotalSeconds / intervalo)
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
