using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using LanchesMac.Areas.Admin.FastReportUtils;
using LanchesMac.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminLanchesReportController : Controller
{   
    private readonly IWebHostEnvironment _environment;
    private readonly RelatorioLanchesService _relatorioLanchesService;

    public AdminLanchesReportController(IWebHostEnvironment environment, RelatorioLanchesService relatorioLanchesService)
    {
        _environment = environment;
        _relatorioLanchesService = relatorioLanchesService;
    }

    public async Task<IActionResult> LanchesCategoriaReport()
    {
        var webReport = new WebReport();
        var mySqlConnection = new MySqlDataConnection();

        webReport.Report.Dictionary.AddChild(mySqlConnection);

        webReport.Report.Load(Path.Combine(_environment.ContentRootPath, "wwwroot/reports", "LanchesCategoria.frx"));
        
        var lanches = HelperFastReport.GetTable(await _relatorioLanchesService.GetLanchesReport(), "LanchesReport");
        var categorias = HelperFastReport.GetTable(await _relatorioLanchesService.GetCategoriasReport(), "CategoriasReport");

        webReport.Report.RegisterData(lanches, "LanchesReport");
        webReport.Report.RegisterData(categorias, "CategoriasReport");

        return View(webReport);

    }

    [Route("LanchesCategoriaPDF")]
    public async Task<ActionResult> LanchesCategoriaPDF()
    {
        var webReport = new WebReport();
        var mssqlDataConnection = new MySqlDataConnection();

        webReport.Report.Dictionary.AddChild(mssqlDataConnection);

        webReport.Report.Load(Path.Combine(_environment.ContentRootPath, "wwwroot/reports",
                                           "lanchesCategoria.frx"));

        var lanches = HelperFastReport.GetTable(await _relatorioLanchesService.GetLanchesReport(), "LanchesReport");
        var categorias = HelperFastReport.GetTable(await _relatorioLanchesService.GetCategoriasReport(), "CategoriasReport");

        webReport.Report.RegisterData(lanches, "LancheReport");
        webReport.Report.RegisterData(categorias, "CategoriasReport");

        webReport.Report.Prepare(); //Prepara o relatório para 

        Stream stream = new MemoryStream();

        webReport.Report.Export(new PDFSimpleExport(), stream);
        stream.Position = 0;

        return File(stream, "application/zip", "LancheCategoria.pdf");
        //return new FileStreamResult(stream, "application/pdf");
    }
}