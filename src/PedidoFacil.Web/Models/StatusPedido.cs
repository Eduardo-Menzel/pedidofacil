namespace PedidoFacil.Web.Models;

/// <summary>Situação do pedido (RN06). Gravado no banco como número: 0 a 3.</summary>
public enum StatusPedido
{
    Aberto = 0,
    Aprovado = 1,
    Faturado = 2,
    Cancelado = 3
}

public static class StatusPedidoExtensions
{
    /// <summary>Classe de cor do Bootstrap usada na etiqueta de status.</summary>
    public static string CorEtiqueta(this StatusPedido status) => status switch
    {
        StatusPedido.Aberto => "text-bg-primary",
        StatusPedido.Aprovado => "text-bg-warning",
        StatusPedido.Faturado => "text-bg-success",
        StatusPedido.Cancelado => "text-bg-secondary",
        _ => "text-bg-light"
    };
}
