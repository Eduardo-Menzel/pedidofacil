using PedidoFacil.Web.Infra;

namespace PedidoFacil.Tests;

/// <summary>
/// Testa a leitura dos valores digitados nos formulários (preço e desconto),
/// que aceita o formato brasileiro e o ponto decimal.
/// </summary>
public class ConversaoDecimalTests
{
    [Theory(DisplayName = "T18 – Valores digitados são convertidos corretamente")]
    [InlineData("45,90", 45.90)]
    [InlineData("45.90", 45.90)]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("1.234", 1234)]
    [InlineData("10", 10)]
    [InlineData("R$ 32,90", 32.90)]
    [InlineData("2.5", 2.5)]
    public void T18_Converte_FormatosAceitos(string texto, double esperado)
    {
        var convertido = DecimalBrasilModelBinder.TentarConverter(texto, out var valor);

        Assert.True(convertido);
        Assert.Equal((decimal)esperado, valor);
    }

    [Theory(DisplayName = "T18b – Texto que não é número é recusado")]
    [InlineData("abc")]
    [InlineData("12,3,4")]
    public void T18b_Recusa_TextoInvalido(string texto)
    {
        Assert.False(DecimalBrasilModelBinder.TentarConverter(texto, out _));
    }
}
