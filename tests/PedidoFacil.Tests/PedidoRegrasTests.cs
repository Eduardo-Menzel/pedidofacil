using System.ComponentModel.DataAnnotations;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Tests;

/// <summary>
/// Testes unitários das regras de negócio do pedido (camada Model).
/// Não usam banco de dados nem telas: testam só a lógica.
/// </summary>
public class PedidoRegrasTests
{
    private static Produto Produto(int id, decimal preco, bool ativo = true) =>
        new() { Id = id, Codigo = $"PRD-{id:000}", Descricao = $"Produto {id}", PrecoUnitario = preco, Ativo = ativo };

    /// <summary>Pedido do exemplo da documentação: 4 × 45,00 + 20 × 32,90 + 2 × 49,00 = 936,00.</summary>
    private static Pedido PedidoComTresItens(decimal desconto = 0)
    {
        var pedido = new Pedido { DescontoPercentual = desconto };
        pedido.AdicionarItem(Produto(1, 45.00m), 4);
        pedido.AdicionarItem(Produto(4, 32.90m), 20);
        pedido.AdicionarItem(Produto(7, 49.00m), 2);
        return pedido;
    }

    private static List<ValidationResult> Validar(object objeto)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(objeto, new ValidationContext(objeto), resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact(DisplayName = "T01 – RN04: total sem desconto é a soma dos itens")]
    public void T01_CalcularTotal_SemDesconto_RetornaSomaDosItens()
    {
        var pedido = PedidoComTresItens();

        Assert.Equal(936.00m, pedido.CalcularSubtotal());
        Assert.Equal(936.00m, pedido.CalcularTotal());
    }

    [Fact(DisplayName = "T02 – RN04: total com 10% de desconto")]
    public void T02_CalcularTotal_ComDezPorCento_AplicaDesconto()
    {
        var pedido = PedidoComTresItens(desconto: 10);

        Assert.Equal(842.40m, pedido.CalcularTotal());
        Assert.Equal(93.60m, pedido.CalcularValorDesconto());
    }

    [Theory(DisplayName = "T03 – RN03: desconto fora de 0% a 30% é recusado")]
    [InlineData(-1)]
    [InlineData(30.01)]
    [InlineData(35)]
    public void T03_DefinirDesconto_ForaDoLimite_GeraErro(double percentual)
    {
        var pedido = PedidoComTresItens();

        var erro = Assert.Throws<RegraNegocioException>(() => pedido.DefinirDesconto((decimal)percentual));
        Assert.Equal(Mensagens.DescontoInvalido, erro.Message);
        Assert.Equal(0m, pedido.DescontoPercentual);

        // A mesma regra também vale para o formulário (DataAnnotations)
        var pedidoFormulario = new Pedido { ClienteId = 1, DescontoPercentual = (decimal)percentual };
        Assert.Contains(Validar(pedidoFormulario), r => r.ErrorMessage == Mensagens.DescontoInvalido);
    }

    [Theory(DisplayName = "T03b – RN03: desconto entre 0% e 30% é aceito")]
    [InlineData(0)]
    [InlineData(12.5)]
    [InlineData(30)]
    public void T03b_DefinirDesconto_DentroDoLimite_Aceita(double percentual)
    {
        var pedido = PedidoComTresItens();

        pedido.DefinirDesconto((decimal)percentual);

        Assert.Equal((decimal)percentual, pedido.DescontoPercentual);
    }

    [Fact(DisplayName = "T04 – RN05: pedido faturado não pode ser editado")]
    public void T04_PedidoFaturado_NaoPodeSerEditado()
    {
        var pedido = PedidoComTresItens();
        pedido.AlterarStatus(StatusPedido.Aprovado);
        pedido.AlterarStatus(StatusPedido.Faturado);

        Assert.False(pedido.PodeEditar());
        var erroItem = Assert.Throws<RegraNegocioException>(() => pedido.AdicionarItem(Produto(9, 10m), 1));
        Assert.Equal(Mensagens.PedidoNaoEditavel, erroItem.Message);
        var erroDesconto = Assert.Throws<RegraNegocioException>(() => pedido.DefinirDesconto(5));
        Assert.Equal(Mensagens.PedidoNaoEditavel, erroDesconto.Message);
        Assert.Equal(3, pedido.Itens.Count);
    }

    [Fact(DisplayName = "T05 – RN07: não aprova pedido sem itens")]
    public void T05_AprovarPedidoSemItens_GeraErro()
    {
        var pedido = new Pedido();

        var erro = Assert.Throws<RegraNegocioException>(() => pedido.AlterarStatus(StatusPedido.Aprovado));

        Assert.Equal(Mensagens.PedidoSemItens, erro.Message);
        Assert.Equal(StatusPedido.Aberto, pedido.Status);
    }

    [Theory(DisplayName = "T06 – RN06: mudança de status não permitida gera erro")]
    [InlineData(StatusPedido.Aberto)]
    [InlineData(StatusPedido.Aprovado)]
    [InlineData(StatusPedido.Cancelado)]
    public void T06_PedidoFaturado_NaoMudaDeStatus(StatusPedido novoStatus)
    {
        var pedido = PedidoComTresItens();
        pedido.AlterarStatus(StatusPedido.Aprovado);
        pedido.AlterarStatus(StatusPedido.Faturado);

        var erro = Assert.Throws<RegraNegocioException>(() => pedido.AlterarStatus(novoStatus));

        Assert.Equal(Mensagens.TransicaoInvalida, erro.Message);
        Assert.Equal(StatusPedido.Faturado, pedido.Status);
    }

    [Fact(DisplayName = "T06b – RN06: Aberto não pode ir direto para Faturado")]
    public void T06b_PedidoAberto_NaoPodeSerFaturado()
    {
        var pedido = PedidoComTresItens();

        var erro = Assert.Throws<RegraNegocioException>(() => pedido.AlterarStatus(StatusPedido.Faturado));

        Assert.Equal(Mensagens.TransicaoInvalida, erro.Message);
    }

    [Fact(DisplayName = "T06c – RN06: fluxo válido Aberto → Aprovado → Faturado")]
    public void T06c_FluxoValido_AbertoAprovadoFaturado()
    {
        var pedido = PedidoComTresItens();

        pedido.AlterarStatus(StatusPedido.Aprovado);
        Assert.Equal(StatusPedido.Aprovado, pedido.Status);

        pedido.AlterarStatus(StatusPedido.Faturado);
        Assert.Equal(StatusPedido.Faturado, pedido.Status);
    }

    [Fact(DisplayName = "T06d – RN06: pedido aprovado pode ser cancelado")]
    public void T06d_PedidoAprovado_PodeSerCancelado()
    {
        var pedido = PedidoComTresItens();
        pedido.AlterarStatus(StatusPedido.Aprovado);

        pedido.AlterarStatus(StatusPedido.Cancelado);

        Assert.Equal(StatusPedido.Cancelado, pedido.Status);
    }

    [Theory(DisplayName = "T07 – RN02: quantidade menor que 1 é recusada")]
    [InlineData(0)]
    [InlineData(-3)]
    public void T07_AdicionarItem_QuantidadeInvalida_GeraErro(int quantidade)
    {
        var pedido = new Pedido();

        var erro = Assert.Throws<RegraNegocioException>(() => pedido.AdicionarItem(Produto(1, 10m), quantidade));

        Assert.Equal(Mensagens.QuantidadeInvalida, erro.Message);
        Assert.Empty(pedido.Itens);
    }

    [Fact(DisplayName = "T08 – RN01: cliente sem CNPJ/CPF é inválido")]
    public void T08_ClienteSemDocumento_EhInvalido()
    {
        var cliente = new Cliente { Nome = "Cliente Teste", CnpjCpf = "" };

        var erros = Validar(cliente);

        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(Cliente.CnpjCpf)));
    }

    [Theory(DisplayName = "T08b – CPF/CNPJ em formato válido é aceito")]
    [InlineData("123.456.789-09")]
    [InlineData("12345678909")]
    [InlineData("12.345.678/0001-90")]
    [InlineData("12345678000190")]
    public void T08b_DocumentoFormatoValido_EhAceito(string documento)
    {
        var cliente = new Cliente { Nome = "Cliente Teste", CnpjCpf = documento };

        Assert.Empty(Validar(cliente));
    }

    [Fact(DisplayName = "T08c – RN02: produto com preço zero é inválido")]
    public void T08c_ProdutoPrecoZero_EhInvalido()
    {
        var produto = new Produto { Codigo = "X-1", Descricao = "Produto teste", PrecoUnitario = 0m };

        var erros = Validar(produto);

        Assert.Contains(erros, e => e.ErrorMessage == Mensagens.PrecoInvalido);
    }

    [Fact(DisplayName = "T14 – RN09: item guarda o preço do dia da venda")]
    public void T14_Item_GuardaPrecoHistorico()
    {
        var produto = Produto(1, 45.00m);
        var pedido = new Pedido();
        pedido.AdicionarItem(produto, 2);

        produto.PrecoUnitario = 99.00m; // preço do produto mudou depois da venda

        Assert.Equal(45.00m, pedido.Itens.Single().PrecoUnitario);
        Assert.Equal(90.00m, pedido.CalcularTotal());
    }

    [Fact(DisplayName = "T15 – Adicionar o mesmo produto soma a quantidade")]
    public void T15_AdicionarMesmoProduto_SomaQuantidade()
    {
        var produto = Produto(1, 10m);
        var pedido = new Pedido();

        pedido.AdicionarItem(produto, 2);
        pedido.AdicionarItem(produto, 3);

        var item = Assert.Single(pedido.Itens);
        Assert.Equal(5, item.Quantidade);
    }

    [Fact(DisplayName = "T16 – Produto inativo não pode ser adicionado")]
    public void T16_ProdutoInativo_NaoPodeSerAdicionado()
    {
        var pedido = new Pedido();

        var erro = Assert.Throws<RegraNegocioException>(() => pedido.AdicionarItem(Produto(1, 10m, ativo: false), 1));

        Assert.Equal(Mensagens.ProdutoInativo, erro.Message);
    }

    [Fact(DisplayName = "T17 – RN10: pedido novo nasce Aberto, sem desconto e com a data de hoje")]
    public void T17_PedidoNovo_ValoresPadrao()
    {
        var pedido = new Pedido();

        Assert.Equal(StatusPedido.Aberto, pedido.Status);
        Assert.Equal(0m, pedido.DescontoPercentual);
        Assert.Equal(DateTime.Today, pedido.DataPedido.Date);
    }
}
