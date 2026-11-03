using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidoFacil.Web.Data;
using PedidoFacil.Web.Models;
using PedidoFacil.Web.ViewModels;

namespace PedidoFacil.Web.Controllers;

/// <summary>Página inicial e páginas de erro.</summary>
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(model: null);
        }

        var modelo = new InicioViewModel
        {
            TotalClientes = await _context.Clientes.CountAsync(),
            TotalProdutosAtivos = await _context.Produtos.CountAsync(p => p.Ativo),
            PedidosAbertos = await _context.Pedidos.CountAsync(p => p.Status == StatusPedido.Aberto),
            PedidosAprovados = await _context.Pedidos.CountAsync(p => p.Status == StatusPedido.Aprovado)
        };
        return View(modelo);
    }

    /// <summary>Página mostrada quando acontece um erro inesperado (MSG08).</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    /// <summary>Página mostrada para erros como 404 – página não encontrada (MSG09).</summary>
    [Route("Home/StatusCode/{codigo:int}")]
    public IActionResult PaginaDeErro(int codigo)
    {
        return View("StatusCode", codigo);
    }
}
