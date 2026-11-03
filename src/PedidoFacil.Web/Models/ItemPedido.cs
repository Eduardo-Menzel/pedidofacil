using System.ComponentModel.DataAnnotations;

namespace PedidoFacil.Web.Models;

/// <summary>Produto incluído em um pedido (tabela ItensPedido).</summary>
public class ItemPedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    [Display(Name = "Produto")]
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    /// <summary>RN02: quantidade mínima 1.</summary>
    [Display(Name = "Quantidade")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [Range(1, int.MaxValue, ErrorMessage = Mensagens.QuantidadeInvalida)]
    public int Quantidade { get; set; }

    /// <summary>RN09: preço do produto no momento em que o item foi adicionado (preço histórico).</summary>
    [Display(Name = "Preço unitário")]
    [DataType(DataType.Currency)]
    public decimal PrecoUnitario { get; set; }

    /// <summary>RN04: quantidade × preço unitário.</summary>
    public decimal CalcularSubtotal() => Quantidade * PrecoUnitario;
}
