using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PedidoFacil.Web.Data;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Web.Controllers;

/// <summary>
/// Pedidos de venda: registrar, adicionar/remover itens, desconto, status e consulta
/// (RF04 a RF08 / UC04 a UC07). As regras ficam na classe Pedido (Model);
/// o controller só busca os dados, chama as regras e grava.
/// </summary>
[Authorize]
public class PedidosController : Controller
{
    private readonly ApplicationDbContext _context;

    public PedidosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Pedidos?status=Aprovado  (RF08)
    public async Task<IActionResult> Index(StatusPedido? status)
    {
        var consulta = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .AsNoTracking();

        if (status.HasValue)
        {
            consulta = consulta.Where(p => p.Status == status.Value);
        }

        ViewData["Status"] = new SelectList(Enum.GetValues<StatusPedido>(), status);
        ViewData["StatusSelecionado"] = status;

        var pedidos = await consulta.OrderByDescending(p => p.Id).ToListAsync();
        return View(pedidos);
    }

    // GET: /Pedidos/Create
    public async Task<IActionResult> Create()
    {
        await CarregarClientesAsync(null);
        return View(new Pedido());
    }

    // POST: /Pedidos/Create  (RF04)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ClienteId,DescontoPercentual,Observacao")] Pedido pedido)
    {
        if (pedido.ClienteId > 0 && !await _context.Clientes.AnyAsync(c => c.Id == pedido.ClienteId))
        {
            ModelState.AddModelError(nameof(Pedido.ClienteId), Mensagens.SelecioneCliente);
        }

        if (!ModelState.IsValid)
        {
            await CarregarClientesAsync(pedido.ClienteId);
            return View(pedido);
        }

        pedido.DataPedido = DateTime.Now;   // RN10
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = Mensagens.PedidoCriado;
        return RedirectToAction(nameof(Details), new { id = pedido.Id });
    }

    // GET: /Pedidos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var pedido = await BuscarPedidoAsync(id.Value, rastrear: false);
        if (pedido is null)
        {
            return NotFound();
        }

        ViewData["Produtos"] = await _context.Produtos
            .Where(p => p.Ativo)
            .OrderBy(p => p.Codigo)
            .AsNoTracking()
            .ToListAsync();

        return View(pedido);
    }

    // POST: /Pedidos/AdicionarItem/5  (RF05 – RN02, RN05, RN09)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarItem(int id, int produtoId, int quantidade)
    {
        var pedido = await BuscarPedidoAsync(id, rastrear: true);
        if (pedido is null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos.FindAsync(produtoId);
        if (produto is null)
        {
            TempData["Erro"] = Mensagens.SelecioneProduto;
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            pedido.AdicionarItem(produto, quantidade);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = Mensagens.ItemAdicionado;
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Pedidos/RemoverItem/5  (RF05 – RN05)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverItem(int id, int itemId)
    {
        var pedido = await BuscarPedidoAsync(id, rastrear: true);
        if (pedido is null)
        {
            return NotFound();
        }

        try
        {
            var item = pedido.RemoverItem(itemId);
            _context.ItensPedido.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = Mensagens.ItemRemovido;
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Pedidos/AtualizarDesconto/5  (RF06 – RN03, RN05)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarDesconto(int id, decimal? desconto)
    {
        var pedido = await BuscarPedidoAsync(id, rastrear: true);
        if (pedido is null)
        {
            return NotFound();
        }

        if (desconto is null || !ModelState.IsValid)
        {
            TempData["Erro"] = Mensagens.DescontoInvalido;
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            pedido.DefinirDesconto(desconto.Value);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = Mensagens.DescontoAtualizado;
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Pedidos/AlterarStatus/5  (RF07 – RN06, RN07)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, StatusPedido novoStatus)
    {
        var pedido = await BuscarPedidoAsync(id, rastrear: true);
        if (pedido is null)
        {
            return NotFound();
        }

        try
        {
            pedido.AlterarStatus(novoStatus);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = string.Format(Mensagens.StatusAlterado, novoStatus);
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<Pedido?> BuscarPedidoAsync(int id, bool rastrear)
    {
        var consulta = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .AsQueryable();

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(p => p.Id == id);
    }

    private async Task CarregarClientesAsync(int? selecionado)
    {
        var clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        ViewData["Clientes"] = new SelectList(clientes, nameof(Cliente.Id), nameof(Cliente.Nome), selecionado);
    }
}
