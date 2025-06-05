using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class DashboardController : Controller
{
    public IActionResult Dashboard1(DateTime? dataInicio, DateTime? dataFim)
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

    public IActionResult Dashboard2(DateTime? dataInicio, DateTime? dataFim)
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

    public string FormataData(DateTime? data)
    {
        try
        {
            string dataStr = data.ToString();

            string dia = dataStr.Substring(0, 2);
            string mes = dataStr.Substring(3, 2);
            string ano = dataStr.Substring(6, 4);

            dataStr = ano + "-" + mes + "-" + dia;

            return dataStr;
        }
        catch (Exception ex)
        {
            Erro(ex);
            return null;
        }
    }

    public async Task<IActionResult> ObterDadosDispositivo(string ip, string tipoSensor, string idSensor, string atributo, string quantidadeValores)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("fiware-service", "smart");
            client.DefaultRequestHeaders.Add("fiware-servicepath", "/");

            var response = await client.GetAsync($"http://{ip}:8666/STH/v1/contextEntities/type/{tipoSensor}/id/{idSensor}/attributes/{atributo}?lastN={quantidadeValores}");
            var content = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(content);

            var root = doc.RootElement;
            var value = root
                .GetProperty("contextResponses")[0]
                .GetProperty("contextElement")
                .GetProperty("attributes")[0]
                .GetProperty("values")
                .ToString();

            ContentResult retorno = Content(value, "application/json");

            return retorno;
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel(ex.ToString()));
        }
    }

    public async Task<ContentResult> ObterDadosAgregadosMedia(string ip, string tipoSensor, string idSensor, string atributo, DateTime dateFrom, DateTime dateTo)
    {
        try
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
                .GroupBy(x => (long)(x.timestamp - todosRegistros[0].timestamp).TotalSeconds / 15)
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
        catch (Exception ex)
        {
            Erro(ex);
            return null;
        }
    }

    public async Task<bool> OnOffLed(string ip, bool onOff)
    {
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Patch, "http://35.171.156.216:1026/v2/entities/urn:ngsi-ld:Temp:001/attrs");
            StringContent content = new StringContent("");

            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");

            if (onOff)
                content = new StringContent("{\n  \"on\": {\n   \"type\" : \"command\",\n      \"value\" : \"\"\n  }\n}", null, "application/json");
            else
                content = new StringContent("{\n  \"off\": {\n   \"type\" : \"command\",\n      \"value\" : \"\"\n  }\n}", null, "application/json");

            request.Content = content;

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
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
}
