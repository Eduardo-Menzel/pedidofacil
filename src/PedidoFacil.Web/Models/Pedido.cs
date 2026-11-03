using System.ComponentModel.DataAnnotations;

namespace PedidoFacil.Web.Models;

/// <summary>
/// Pedido de venda (tabela Pedidos). As regras de negócio do pedido ficam
/// dentro desta classe (camada Model do MVC), o que permite testá-las
/// sem banco de dados e sem telas.
/// </summary>
public class Pedido
{
    /// <summary>RN03: limite máximo de desconto, em %.</summary>
    public const decimal DescontoMaximo = 30m;

    /// <summary>RN06: mudanças de status permitidas.</summary>
    private static readonly Dictionary<StatusPedido, StatusPedido[]> TransicoesPermitidas = new()
    {
        [StatusPedido.Aberto] = new[] { StatusPedido.Aprovado, StatusPedido.Cancelado },
        [StatusPedido.Aprovado] = new[] { StatusPedido.Faturado, StatusPedido.Cancelado },
        [StatusPedido.Faturado] = Array.Empty<StatusPedido>(),
        [StatusPedido.Cancelado] = Array.Empty<StatusPedido>()
    };

    [Display(Name = "Nº")]
    public int Id { get; set; }

    [Display(Name = "Cliente")]
    [Required(ErrorMessage = Mensagens.SelecioneCliente)]
    [Range(1, int.MaxValue, ErrorMessage = Mensagens.SelecioneCliente)]
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    /// <summary>RN10: todo pedido novo nasce com a data do dia.</summary>
    [Display(Name = "Data")]
    [DataType(DataType.Date)]
    public DateTime DataPedido { get; set; } = DateTime.Now;

    /// <summary>RN10: todo pedido novo nasce Aberto. Só muda pelo método AlterarStatus.</summary>
    [Display(Name = "Status")]
    public StatusPedido Status { get; private set; } = StatusPedido.Aberto;

    /// <summary>RN03: entre 0% e 30%.</summary>
    [Display(Name = "Desconto (%)")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [Range(0.0, 30.0, ErrorMessage = Mensagens.DescontoInvalido)]
    public decimal DescontoPercentual { get; set; }

    [Display(Name = "Observação")]
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Observacao { get; set; }

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

    // ------------------------------------------------------------------
    // Cálculos (RN04)
    // ------------------------------------------------------------------

    /// <summary>Soma de quantidade × preço unitário de todos os itens.</summary>
    public decimal CalcularSubtotal() => Itens.Sum(i => i.CalcularSubtotal());

    /// <summary>Total = subtotal × (1 − desconto ÷ 100), arredondado em 2 casas.</summary>
    public decimal CalcularTotal() =>
        Math.Round(CalcularSubtotal() * (1 - DescontoPercentual / 100m), 2, MidpointRounding.AwayFromZero);

    /// <summary>Valor em reais do desconto concedido.</summary>
    public decimal CalcularValorDesconto() => CalcularSubtotal() - CalcularTotal();

    // ------------------------------------------------------------------
    // Regras de edição (RN05) e de status (RN06, RN07)
    // ------------------------------------------------------------------

    /// <summary>RN05: só pedidos Abertos podem ter itens ou desconto alterados.</summary>
    public bool PodeEditar() => Status == StatusPedido.Aberto;

    /// <summary>Indica se o pedido pode passar para o status informado.</summary>
    public bool PodeMudarPara(StatusPedido novoStatus) => TransicoesPermitidas[Status].Contains(novoStatus);

    /// <summary>
    /// Altera o status do pedido validando RN06 (transições permitidas)
    /// e RN07 (só aprova com pelo menos 1 item).
    /// </summary>
    public void AlterarStatus(StatusPedido novoStatus)
    {
        if (!PodeMudarPara(novoStatus))
        {
            throw new RegraNegocioException(Mensagens.TransicaoInvalida);
        }

        if (novoStatus == StatusPedido.Aprovado && Itens.Count == 0)
        {
            throw new RegraNegocioException(Mensagens.PedidoSemItens);
        }

        Status = novoStatus;
    }

    /// <summary>RN03 + RN05: altera o desconto de um pedido aberto.</summary>
    public void DefinirDesconto(decimal percentual)
    {
        GarantirQuePodeEditar();

        if (percentual < 0 || percentual > DescontoMaximo)
        {
            throw new RegraNegocioException(Mensagens.DescontoInvalido);
        }

        DescontoPercentual = percentual;
    }

    /// <summary>
    /// RN02 + RN05 + RN09: adiciona um produto ao pedido copiando o preço atual.
    /// Se o produto já estiver no pedido, soma a quantidade ao item existente.
    /// </summary>
    public ItemPedido AdicionarItem(Produto produto, int quantidade)
    {
        ArgumentNullException.ThrowIfNull(produto);
        GarantirQuePodeEditar();

        if (quantidade < 1)
        {
            throw new RegraNegocioException(Mensagens.QuantidadeInvalida);
        }

        if (!produto.Ativo)
        {
            throw new RegraNegocioException(Mensagens.ProdutoInativo);
        }

        var existente = produto.Id > 0 ? Itens.FirstOrDefault(i => i.ProdutoId == produto.Id) : null;
        if (existente is not null)
        {
            existente.Quantidade += quantidade;
            return existente;
        }

        var item = new ItemPedido
        {
            ProdutoId = produto.Id,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = produto.PrecoUnitario
        };
        Itens.Add(item);
        return item;
    }

    /// <summary>RN05: remove um item de um pedido aberto.</summary>
    public ItemPedido RemoverItem(int itemId)
    {
        GarantirQuePodeEditar();

        var item = Itens.FirstOrDefault(i => i.Id == itemId)
                   ?? throw new RegraNegocioException("Item não encontrado neste pedido.");
        Itens.Remove(item);
        return item;
    }

    private void GarantirQuePodeEditar()
    {
        if (!PodeEditar())
        {
            throw new RegraNegocioException(Mensagens.PedidoNaoEditavel);
        }
    }
}
