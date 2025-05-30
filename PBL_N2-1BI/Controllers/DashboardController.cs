﻿using Microsoft.AspNetCore.Mvc;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class DashboardController : Controller
{
    public IActionResult Dashboard1(DateTime? dataInicio, DateTime? dataFim)
    {
        RegistroDAO dao = new RegistroDAO();

        List<RegistroViewModel> model = dao.ListarRegistros();

        if (dataInicio.HasValue)
        {
            model = model.Where(xs => xs.DataRegistro >= dataInicio).ToList();
            ViewBag.DataInicio = FormataData(dataInicio);
        }
        if (dataFim.HasValue)
        {
            model = model.Where(xs => xs.DataRegistro <= dataFim).ToList();
            ViewBag.DataFim = FormataData(dataFim);
        }

        return View(model);
    }

    public IActionResult Dashboard2(DateTime? dataInicio, DateTime? dataFim)
    {
        RegistroDAO dao = new RegistroDAO();
       
        List<RegistroViewModel> model = dao.ListarRegistros();

        if (dataInicio.HasValue)
        {
            model = model.Where(xs => xs.DataRegistro >= dataInicio).ToList();
            ViewBag.DataInicio = FormataData(dataInicio);
        }
        if(dataFim.HasValue)
        {
            model = model.Where(xs => xs.DataRegistro <= dataFim).ToList();
            ViewBag.DataFim = FormataData(dataFim);
        }

        return View(model);
    }

    public IActionResult ConsultaRegistros()
    {
        RegistroDAO dao = new RegistroDAO();
        Random random = new Random();

        try
        {
            return Json(dao.ListarRegistros());
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel(ex.ToString()));
        }
    }

    public string FormataData(DateTime? data)
    {
        string dataStr = data.ToString();

        string dia = dataStr.Substring(0, 2);
        string mes = dataStr.Substring(3, 2);
        string ano = dataStr.Substring(6, 4);

        dataStr = ano + "-" + mes + "-" + dia;

        return dataStr;
    }

    public async Task<IActionResult> ObterDadosDispositivo(string ip, string tipoSensor, string idSensor, string atributo, string quantidadeValores)
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

        // Lista para armazenar todos os dados agregados por segundo
        var todosRegistros = new List<(DateTime timestamp, double valor)>();

        while (temMaisDados)
        {
            var url = $"http://{ip}:8666/STH/v1/contextEntities/type/{tipoSensor}/id/{idSensor}/attributes/{atributo}" +
                      $"?hLimit=100&hOffset={offset}";

            if(dateFrom.ToShortDateString() != "01/01/0001")
            {
                url += $"&dateFrom ={dateFromStr}";             
            }
            if (dateFrom.ToShortDateString() != "01/01/0001")
            {
                url += $"&dateTo ={dateToStr}";
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

            // Extrair todos os registros (timestamp, valor) e armazenar na lista
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

        // Agora, agrupamos os dados em janelas de 15 segundos e calculamos a média
        var agrupados = todosRegistros
            .OrderBy(x => x.timestamp)
            .GroupBy(x => (long)(x.timestamp - todosRegistros[0].timestamp).TotalSeconds / 15)
            .Select(g => new
            {
                timestamp = g.First().timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                valorMedio = g.Average(x => x.valor)
            })
            .ToList();

        // Montar o JSON de saída com o formato esperado (a propriedade "values" é um array de arrays [timestamp, valor])
        var valuesJsonArray = new JsonArray();

        foreach (var grupo in agrupados)
        {
            valuesJsonArray.Add(new JsonArray { grupo.timestamp, grupo.valorMedio });
        }

        // Construir o JSON que corresponde à propriedade "values" no seu retorno atual
        var jsonFinal = valuesJsonArray.ToJsonString();

        return Content(jsonFinal, "application/json");
    }

    public static List<DateTime> ConvertToBrasiliaTime(List<string> timestamps)
    {
        var converted = new List<DateTime>();

        // Detectar o timezone correto dependendo do sistema operacional
        string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "E. South America Standard Time"
            : "America/Sao_Paulo";

        var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        foreach (var ts in timestamps)
        {
            var cleanTs = ts.Replace("T", " ").Replace("Z", "");

            DateTime dt;
            if (!DateTime.TryParseExact(cleanTs, "yyyy-MM-dd HH:mm:ss.fff",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dt))
            {
                DateTime.TryParseExact(cleanTs, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dt);
            }

            // Converte de UTC para horário de Brasília
            var dtBrasilia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dt, DateTimeKind.Utc), brasiliaTimeZone);

            converted.Add(dtBrasilia);
        }

        return converted;
    }
}
