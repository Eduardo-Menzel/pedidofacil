using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace PedidoFacil.Tests;

/// <summary>
/// Sobe a aplicação inteira em memória para os testes de integração,
/// usando um banco SQLite temporário (separado do banco real).
/// </summary>
public class FabricaAplicacao : WebApplicationFactory<Program>
{
    private readonly string _arquivoBanco =
        Path.Combine(Path.GetTempPath(), $"pedidofacil-teste-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={_arquivoBanco}");
        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SqliteConnection.ClearAllPools();
        try
        {
            if (File.Exists(_arquivoBanco))
            {
                File.Delete(_arquivoBanco);
            }
        }
        catch (IOException)
        {
            // O arquivo temporário pode continuar em uso por alguns instantes; não afeta os testes.
        }
    }
}
