using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PedidoFacil.Web.Models;
using PedidoFacil.Web.ViewModels;

namespace PedidoFacil.Web.Controllers;

/// <summary>Criar conta, entrar e sair do sistema (RF01 / UC01).</summary>
public class ContaController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public ContaController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Entrar(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new EntrarViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Entrar(EntrarViewModel modelo, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var resultado = await _signInManager.PasswordSignInAsync(
            modelo.Email, modelo.Senha, modelo.LembrarDeMim, lockoutOnFailure: false);

        if (resultado.Succeeded)
        {
            return VoltarPara(returnUrl);
        }

        ModelState.AddModelError(string.Empty, Mensagens.LoginInvalido);
        return View(modelo);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        return View(new RegistrarViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistrarViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = new IdentityUser { UserName = modelo.Email, Email = modelo.Email };
        var resultado = await _userManager.CreateAsync(usuario, modelo.Senha);

        if (resultado.Succeeded)
        {
            await _signInManager.SignInAsync(usuario, isPersistent: false);
            TempData["Sucesso"] = Mensagens.ContaCriada;
            return RedirectToAction("Index", "Home");
        }

        foreach (var erro in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, erro.Description);
        }
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Entrar));
    }

    /// <summary>Volta para a página que o usuário tentou abrir antes do login (somente endereços do próprio sistema).</summary>
    private IActionResult VoltarPara(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
