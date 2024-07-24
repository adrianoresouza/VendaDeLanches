using LanchesMac.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminGraficoController : Controller
{
    private readonly GraficoVendasService _graficoVendas;

    public AdminGraficoController(GraficoVendasService graficoVendas)
    {
        _graficoVendas = graficoVendas ?? throw new ArgumentNullException(nameof(graficoVendas));
    }

    public JsonResult VendasLanches(int dias)
    {
        var lanchesVendasTotais = _graficoVendas.GetVendasLanches(dias);
        return Json(lanchesVendasTotais);
    }

    public IActionResult Index(int dias)
    {
        return View();
    }

    public IActionResult VendasMensal(int dias)
    {
        return View();
    }

    public IActionResult VendasSemanal(int dias)
    {
        return View();
    }
}