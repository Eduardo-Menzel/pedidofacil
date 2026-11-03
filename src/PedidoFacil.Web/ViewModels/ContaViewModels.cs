using System.ComponentModel.DataAnnotations;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Web.ViewModels;

/// <summary>Dados da tela de login.</summary>
public class EntrarViewModel
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [EmailAddress(ErrorMessage = Mensagens.EmailInvalido)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Lembrar de mim")]
    public bool LembrarDeMim { get; set; }
}

/// <summary>Dados da tela de criação de conta.</summary>
public class RegistrarViewModel
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [EmailAddress(ErrorMessage = Mensagens.EmailInvalido)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Confirmar senha")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [DataType(DataType.Password)]
    [Compare(nameof(Senha), ErrorMessage = Mensagens.SenhasDiferentes)]
    public string ConfirmarSenha { get; set; } = string.Empty;
}

/// <summary>Números mostrados na página inicial.</summary>
public class InicioViewModel
{
    public int TotalClientes { get; set; }
    public int TotalProdutosAtivos { get; set; }
    public int PedidosAbertos { get; set; }
    public int PedidosAprovados { get; set; }
}
