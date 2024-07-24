using System.Net.Sockets;
using System.Runtime.CompilerServices;
using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LanchesMac.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminImagensController : Controller
{
    private readonly ConfigurationImagens _myConfig;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AdminImagensController(IWebHostEnvironment hostingEnvironment, IOptions<ConfigurationImagens> myConfiguration)
    {
        _hostingEnvironment = hostingEnvironment;
        _myConfig = myConfiguration.Value;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> UploadFiles(List<IFormFile> files)
    {
        if(files == null || files.Count == 0)
        {
            ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
            return View();
        }

        if(files.Count > 10)
        {
            ViewData["Erro"] = "Error: Quantidade de arquios excedeu o limite";
            return View();
        }

        long size = files.Sum(f => f.Length); //Calcula o tamanho em bytes

        var filePathNames = new List<string>(); //Armazena os nomes do arquivo

        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProduto);

        foreach (var formFile in files)
        {
            if(formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") || formFile.FileName.Contains(".png"))
            {
                var fileNameWithPath = string.Concat(filePath, "/", formFile.FileName);
                filePathNames.Add(fileNameWithPath);
                using(var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
        }    
        ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao servidor, " + $"com tamanho total de {size} bytes";
        ViewBag.Arquivos = filePathNames;

        return View(ViewData);
    }

    public IActionResult GetImages()
    {
        FileManagerModel model = new FileManagerModel();

        var userImagesPath = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProduto);

        DirectoryInfo dir = new DirectoryInfo(userImagesPath);

        FileInfo[] files = dir.GetFiles();

        model.PathImagesProduto = _myConfig.NomePastaImagensProduto;

        var size = files.Length;

        if(files.Length == 0)
        {
            ViewData["Erro"] = $"Nenhum arquivo encontrado na pasta: {userImagesPath}";
        }

        model.Files = files;

        return View(model);
    }

    public IActionResult Deletefile(string fname)
    {
        string _imagemDeleta = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProduto + "/",fname);

        if(System.IO.File.Exists(_imagemDeleta))
        {
            System.IO.File.Delete(_imagemDeleta);
            ViewData["Deletado"] = $"Arquivo (s) {_imagemDeleta} deletado com sucesso";
        }

        return View("index");
    }
}