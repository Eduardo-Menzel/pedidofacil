namespace PedidoFacil.Web.Models;

/// <summary>
/// Erro lançado quando uma regra de negócio é violada.
/// A mensagem já está pronta para ser mostrada ao usuário.
/// </summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}
