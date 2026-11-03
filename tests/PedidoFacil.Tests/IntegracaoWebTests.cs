using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PedidoFacil.Tests;

/// <summary>
/// Testes de integração: front-end (HTML das telas) + back-end (controllers) + banco,
/// fazendo requisições HTTP de verdade para a aplicação.
/// </summary>
public class IntegracaoWebTests : IClassFixture<FabricaAplicacao>
{
    private readonly FabricaAplicacao _fabrica;

    public IntegracaoWebTests(FabricaAplicacao fabrica)
    {
        _fabrica = fabrica;
    }

    private HttpClient CriarCliente() =>
        _fabrica.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Theory(DisplayName = "T09 – RNF04: telas de negócio sem login levam para o login")]
    [InlineData("/Clientes")]
    [InlineData("/Produtos")]
    [InlineData("/Pedidos")]
    [InlineData("/Pedidos/Details/1")]
    public async Task T09_SemLogin_RedirecionaParaLogin(string endereco)
    {
        var resposta = await CriarCliente().GetAsync(endereco);

        Assert.Equal(HttpStatusCode.Redirect, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
        Assert.Contains("/Conta/Entrar", resposta.Headers.Location!.OriginalString);
    }

    [Fact(DisplayName = "T10 – Página inicial abre e mostra o nome do sistema")]
    public async Task T10_PaginaInicial_Responde200()
    {
        var resposta = await CriarCliente().GetAsync("/");
        var html = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Contains("PedidoFácil", html);
    }

    [Fact(DisplayName = "T11 – RF01: tela de login abre com o formulário")]
    public async Task T11_TelaDeLogin_TemFormulario()
    {
        var resposta = await CriarCliente().GetAsync("/Conta/Entrar");
        var html = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Contains("Entrar no PedidoFácil", html);
        Assert.Contains("name=\"Email\"", html);
        Assert.Contains("name=\"Senha\"", html);
    }

    [Fact(DisplayName = "T11b – RF01: tela de criar conta abre com o formulário")]
    public async Task T11b_TelaDeCriarConta_TemFormulario()
    {
        var resposta = await CriarCliente().GetAsync("/Conta/Registrar");
        var html = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Contains("name=\"ConfirmarSenha\"", html);
    }

    [Fact(DisplayName = "T19 – RNF05: endereço inexistente mostra página amigável (404)")]
    public async Task T19_EnderecoInexistente_MostraPaginaAmigavel()
    {
        var resposta = await CriarCliente().GetAsync("/pagina-que-nao-existe");
        var html = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        Assert.Contains("Página não encontrada", html);
    }
}
