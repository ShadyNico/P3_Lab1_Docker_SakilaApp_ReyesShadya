using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SakilaApp.Models;
using Microsoft.AspNetCore.Authorization;


namespace SakilaApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // ANTES: return View(); mostraba Views/Home/Index.cshtml, que estaba vacio.
        // CORREGIDO: redirige al listado de peliculas.
        return RedirectToAction("Index", "Films");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult PanelAdministrador()
    {
        return View();
    }
}

