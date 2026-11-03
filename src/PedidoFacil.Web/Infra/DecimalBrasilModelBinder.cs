using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PedidoFacil.Web.Infra;

/// <summary>
/// Lê valores decimais digitados nos formulários aceitando o formato brasileiro
/// (45,90 · 1.234,56) e também o ponto como separador decimal (45.90),
/// para evitar que "45.90" vire 4590 por engano.
/// </summary>
public class DecimalBrasilModelBinder : IModelBinder
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var resultado = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (resultado == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, resultado);
        var texto = resultado.FirstValue;
        var nomeCampo = bindingContext.ModelMetadata.GetDisplayName();

        if (string.IsNullOrWhiteSpace(texto))
        {
            if (Nullable.GetUnderlyingType(bindingContext.ModelType) is not null)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"O campo {nomeCampo} é obrigatório.");
            }
            return Task.CompletedTask;
        }

        if (TentarConverter(texto, out var valor))
        {
            bindingContext.Result = ModelBindingResult.Success(valor);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"O campo {nomeCampo} deve ser um número.");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Converte o texto em decimal.
    /// Com vírgula: formato brasileiro. Só com ponto: se depois do último ponto houver
    /// exatamente 3 dígitos é separador de milhar (1.234), senão é separador decimal (45.90).
    /// </summary>
    public static bool TentarConverter(string texto, out decimal valor)
    {
        texto = texto.Replace("R$", string.Empty).Replace(" ", string.Empty).Trim();

        if (texto.Contains(','))
        {
            return decimal.TryParse(texto, NumberStyles.Number, Brasil, out valor);
        }

        var ultimoPonto = texto.LastIndexOf('.');
        if (ultimoPonto >= 0 && texto.Length - ultimoPonto - 1 != 3)
        {
            return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
        }

        return decimal.TryParse(texto, NumberStyles.Number, Brasil, out valor);
    }
}

/// <summary>Registra o DecimalBrasilModelBinder para campos decimal e decimal?.</summary>
public class DecimalBrasilModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.UnderlyingOrModelType == typeof(decimal)
            ? new DecimalBrasilModelBinder()
            : null;
    }
}
