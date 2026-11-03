using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidoFacil.Web.Data;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Web.Controllers;

/// <summary>Cadastro de produtos – incluir, listar, editar e excluir (RF03 / UC03).</summary>
[Authorize]
public class ProdutosController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProdutosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Produtos
    public async Task<IActionResult> Index()
    {
        var produtos = await _context.Produtos.AsNoTracking().OrderBy(p => p.Codigo).ToListAsync();
        return View(produtos);
    }

    // GET: /Produtos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return produto is null ? NotFound() : View(produto);
    }

    // GET: /Produtos/Create
    public IActionResult Create()
    {
        return View(new Produto());
    }

    // POST: /Produtos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Codigo,Descricao,PrecoUnitario,Ativo")] Produto produto)
    {
        await ValidarCodigoUnicoAsync(produto);
        if (!ModelState.IsValid)
        {
            return View(produto);
        }

        _context.Produtos.Add(produto);
        if (!await SalvarAsync())
        {
            return View(produto);
        }

        TempData["Sucesso"] = Mensagens.ProdutoCadastrado;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Produtos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos.FindAsync(id);
        return produto is null ? NotFound() : View(produto);
    }

    // POST: /Produtos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Descricao,PrecoUnitario,Ativo")] Produto dados)
    {
        if (id != dados.Id)
        {
            return NotFound();
        }

        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null)
        {
            return NotFound();
        }

        await ValidarCodigoUnicoAsync(dados);
        if (!ModelState.IsValid)
        {
            return View(dados);
        }

        // Alterar o preço aqui NÃO muda os pedidos já feitos (RN09 – preço histórico).
        produto.Codigo = dados.Codigo;
        produto.Descricao = dados.Descricao;
        produto.PrecoUnitario = dados.PrecoUnitario;
        produto.Ativo = dados.Ativo;

        if (!await SalvarAsync())
        {
            return View(dados);
        }

        TempData["Sucesso"] = Mensagens.DadosAtualizados;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Produtos/Delete/5 – tela de confirmação (MSG07)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (produto is null)
        {
            return NotFound();
        }

        ViewData["EmUso"] = await _context.ItensPedido.AnyAsync(i => i.ProdutoId == id);
        return View(produto);
    }

    // POST: /Produtos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null)
        {
            return NotFound();
        }

        // Produto que já foi vendido não pode ser excluído (integridade referencial).
        if (await _context.ItensPedido.AnyAsync(i => i.ProdutoId == id))
        {
            TempData["Erro"] = Mensagens.ProdutoEmUso;
            return RedirectToAction(nameof(Index));
        }

        _context.Produtos.Remove(produto);
        try
        {
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = Mensagens.RegistroExcluido;
        }
        catch (DbUpdateException)
        {
            TempData["Erro"] = Mensagens.ProdutoEmUso;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarCodigoUnicoAsync(Produto produto)
    {
        if (string.IsNullOrWhiteSpace(produto.Codigo))
        {
            return;
        }

        produto.Codigo = produto.Codigo.Trim().ToUpperInvariant();
        var existe = await _context.Produtos.AnyAsync(p => p.Codigo == produto.Codigo && p.Id != produto.Id);
        if (existe)
        {
            ModelState.AddModelError(nameof(Produto.Codigo), Mensagens.CodigoDuplicado);
        }
    }

    private async Task<bool> SalvarAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(Produto.Codigo), Mensagens.CodigoDuplicado);
            return false;
        }
    }
}
