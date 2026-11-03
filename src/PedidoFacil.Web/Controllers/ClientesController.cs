using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidoFacil.Web.Data;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Web.Controllers;

/// <summary>Cadastro de clientes – incluir, listar, buscar, editar e excluir (RF02 / UC02).</summary>
[Authorize]
public class ClientesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Clientes?busca=texto
    public async Task<IActionResult> Index(string? busca)
    {
        var consulta = _context.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            consulta = consulta.Where(c => EF.Functions.Like(c.Nome, $"%{termo}%"));
        }

        ViewData["Busca"] = busca;
        return View(await consulta.OrderBy(c => c.Nome).ToListAsync());
    }

    // GET: /Clientes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes
            .Include(c => c.Pedidos).ThenInclude(p => p.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return cliente is null ? NotFound() : View(cliente);
    }

    // GET: /Clientes/Create
    public IActionResult Create()
    {
        return View(new Cliente());
    }

    // POST: /Clientes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,CnpjCpf,Email,Telefone,Cidade,UF")] Cliente cliente)
    {
        await ValidarDocumentoUnicoAsync(cliente);
        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        cliente.DataCadastro = DateTime.Now;
        _context.Clientes.Add(cliente);

        if (!await SalvarAsync(cliente))
        {
            return View(cliente);
        }

        TempData["Sucesso"] = Mensagens.ClienteCadastrado;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Clientes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FindAsync(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    // POST: /Clientes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CnpjCpf,Email,Telefone,Cidade,UF")] Cliente dados)
    {
        if (id != dados.Id)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        await ValidarDocumentoUnicoAsync(dados);
        if (!ModelState.IsValid)
        {
            return View(dados);
        }

        cliente.Nome = dados.Nome;
        cliente.CnpjCpf = dados.CnpjCpf;
        cliente.Email = dados.Email;
        cliente.Telefone = dados.Telefone;
        cliente.Cidade = dados.Cidade;
        cliente.UF = dados.UF;

        if (!await SalvarAsync(dados))
        {
            return View(dados);
        }

        TempData["Sucesso"] = Mensagens.DadosAtualizados;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Clientes/Delete/5 – tela de confirmação (MSG07)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null)
        {
            return NotFound();
        }

        ViewData["TemPedidos"] = await _context.Pedidos.AnyAsync(p => p.ClienteId == id);
        return View(cliente);
    }

    // POST: /Clientes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        // RN08: cliente com pedidos não pode ser excluído.
        if (await _context.Pedidos.AnyAsync(p => p.ClienteId == id))
        {
            TempData["Erro"] = Mensagens.ClienteComPedidos;
            return RedirectToAction(nameof(Index));
        }

        _context.Clientes.Remove(cliente);
        try
        {
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = Mensagens.RegistroExcluido;
        }
        catch (DbUpdateException)
        {
            TempData["Erro"] = Mensagens.ClienteComPedidos;
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>RN01: CNPJ/CPF não pode se repetir.</summary>
    private async Task ValidarDocumentoUnicoAsync(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.CnpjCpf))
        {
            return;
        }

        cliente.CnpjCpf = cliente.CnpjCpf.Trim();
        var existe = await _context.Clientes.AnyAsync(c => c.CnpjCpf == cliente.CnpjCpf && c.Id != cliente.Id);
        if (existe)
        {
            ModelState.AddModelError(nameof(Cliente.CnpjCpf), Mensagens.CnpjCpfDuplicado);
        }
    }

    /// <summary>Grava no banco e transforma erros técnicos em mensagem amigável.</summary>
    private async Task<bool> SalvarAsync(Cliente cliente)
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            // Ex.: dois cadastros com o mesmo CNPJ/CPF ao mesmo tempo (índice único do banco).
            ModelState.AddModelError(nameof(Cliente.CnpjCpf), Mensagens.CnpjCpfDuplicado);
            return false;
        }
    }
}
