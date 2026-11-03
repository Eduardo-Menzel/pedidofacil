using System.ComponentModel.DataAnnotations;

namespace PedidoFacil.Web.Models;

/// <summary>Produto que pode ser vendido (tabela Produtos).</summary>
public class Produto
{
    public int Id { get; set; }

    [Display(Name = "Código")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [StringLength(30, MinimumLength = 1, ErrorMessage = Mensagens.TamanhoTexto)]
    public string Codigo { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [StringLength(150, MinimumLength = 3, ErrorMessage = Mensagens.TamanhoTexto)]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>RN02: preço maior que zero.</summary>
    [Display(Name = "Preço unitário")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [Range(0.01, 9999999.99, ErrorMessage = Mensagens.PrecoInvalido)]
    [DataType(DataType.Currency)]
    public decimal PrecoUnitario { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    /// <summary>Texto mostrado na lista de produtos do pedido.</summary>
    public string CodigoDescricao => $"{Codigo} – {Descricao}";
}
