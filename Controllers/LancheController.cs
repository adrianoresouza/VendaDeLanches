using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers;

public class LancheController : Controller
{
    private readonly ILanchesRepository _lanchesRepository;

    public LancheController(ILanchesRepository lanchesRepository) => _lanchesRepository = lanchesRepository;

    // public LancheController(ILanchesRepository lanchesRepository)
    // {
    //     _lanchesRepository = lanchesRepository;
    // }

    public ActionResult List(string categoria)
    {
        IEnumerable<Lanche> lanches = null;
        string categoriaAtual = string.Empty;

        if (string.IsNullOrEmpty(categoria))
        {
            lanches = _lanchesRepository.Lanches.OrderBy(l => l.LancheId);
            categoriaAtual = "Todos os lanches";
        }
        else
        {
            //     if(string.Equals("Normal", categoria, StringComparison.OrdinalIgnoreCase))
            //     {
            //         lanches = _lanchesRepository.Lanches
            //         .Where(l=>l.Categoria.CategoriaNome.Equals("Normal"))
            //         .OrderBy(l=>l.Nome);
            //         categoria = "Normal";
            //     }
            //     else if(string.Equals("Natural", categoria, StringComparison.OrdinalIgnoreCase))
            //     {
            //         lanches = _lanchesRepository.Lanches
            //         .Where(l=>l.Categoria.CategoriaNome.Equals("Natural"))
            //         .OrderBy(l=>l.Nome);
            //         categoria = "Natural";
            //     }
            //     categoriaAtual = categoria;

            lanches = _lanchesRepository.Lanches.Where(l => l.Categoria.CategoriaNome.Equals(categoria))
            .OrderBy(l => l.Categoria.CategoriaNome);
            categoriaAtual = categoria;
        }
        var lanchesListViewModel = new LancheListViewModel
        {
            Lanches = lanches,
            CategoriaAtual = categoriaAtual
        };
        return View(lanchesListViewModel);
    }

    public IActionResult Details(int lancheId)
    {
        var lanche = _lanchesRepository.GetLancheById(lancheId);
        return View(lanche);
    }

    public ViewResult Search(string searchString)
    {
        IEnumerable<Lanche> lanches;
        string categoriaAtual = string.Empty;

        if(String.IsNullOrEmpty(searchString))
        {
            lanches = _lanchesRepository.Lanches.OrderBy(l=>l.LancheId);
            categoriaAtual = "Todos os Lanches";

        }
        else
        {
            lanches = _lanchesRepository.Lanches.Where(l=>l.Nome.ToLower()
            .Contains(searchString.ToLower()));

            if(lanches.Any())
            {
                categoriaAtual = "Lanches";
            }
            else
            {
                categoriaAtual = "Nenhum lanche foi encontrado";
            }


        }

        return View("~/Views/Lanche/List.cshtml", new LancheListViewModel
        {
            Lanches = lanches,
            CategoriaAtual = categoriaAtual
        });
    }
}