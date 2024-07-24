using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers;

public class CarrinhoCompraController : Controller
{
    private readonly ILanchesRepository _lanchesRepository;
    private readonly CarrinhoCompra _carrinhoCompra;

    public CarrinhoCompraController(ILanchesRepository lanchesRepository, CarrinhoCompra carrinhoCompra)
    {
        _lanchesRepository = lanchesRepository;
        _carrinhoCompra = carrinhoCompra;
    }

    public IActionResult Index()
    {
        var itens = _carrinhoCompra.GetCarrinhoCompraItems();
        _carrinhoCompra.CarrinhoCompraItems = itens;
        var carrinhoCompraVM = new CarrinhoCompraViewModel
        {
            CarrinhoCompra = _carrinhoCompra,
            CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
        };
        return View(carrinhoCompraVM);
    }

    [Authorize]
    public IActionResult AdicionarItemCarrinhoCompra(int lancheId)
    {
        var lancheSelecionado = _lanchesRepository.Lanches.FirstOrDefault(l=>l.LancheId == lancheId);
        if(lancheSelecionado!= null)
        {
            _carrinhoCompra.AdicionaAoCarrinho(lancheSelecionado);
        }
        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult RemoverItemCarrinhoCompra(int lancheId)
    {
        var lancheSelecionado = _lanchesRepository.Lanches.FirstOrDefault(l=>l.LancheId == lancheId);
        if(lancheSelecionado!= null)
        {
            _carrinhoCompra.RemoverItemDoCarrinho(lancheSelecionado);
        }
        return RedirectToAction("Index");
    }
}