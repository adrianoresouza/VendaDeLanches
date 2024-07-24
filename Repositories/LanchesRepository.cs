using System.Runtime.InteropServices;
using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Repositories;

public class LanchesRepository : ILanchesRepository
{
    private readonly AppDbContext _contexto;

    public LanchesRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public IEnumerable<Lanche> Lanches => _contexto.Lanches.Include(c => c.Categoria);

    public IEnumerable<Lanche> LanchesPreferidos => _contexto.Lanches.Where( l => l.IsLanchePreferido).Include(c=>c.Categoria);

    public Lanche GetLancheById(int lancheId) => _contexto.Lanches.FirstOrDefault(l => l.LancheId == lancheId);
    
}