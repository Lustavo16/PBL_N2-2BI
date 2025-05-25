using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using PBL_N2_1BI.DAO;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class DashboardController : Controller
{
    public IActionResult Dashboard1(DateTime? dataInicio, DateTime? dataFim, string tipoDado)
    {
        RegistroDAO dao = new RegistroDAO();

        ViewBag.TipoDado = tipoDado;

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

    public IActionResult Dashboard2(DateTime? dataInicio, DateTime? dataFim, string tipoDado)
    {
        RegistroDAO dao = new RegistroDAO();
       
        ViewBag.TipoDado = tipoDado;

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
            RegistroViewModel registro = new RegistroViewModel()
            {
                DataRegistro = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond),
                ValorLuminosidade = random.Next(12, 26),
                ValorTemperatura = random.Next(41, 51),
                ValorUmidade = random.Next(39, 47),
            };

            dao.InserirRegistro(registro);

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
}
