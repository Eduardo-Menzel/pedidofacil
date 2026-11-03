using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PedidoFacil.Web.Data;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Tests;

/// <summary>
/// Testes de integração com o banco SQLite (em memória):
/// gravação, leitura e integridade referencial.
/// </summary>
public class BancoDeDadosTests : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly DbContextOptions<ApplicationDbContext> _opcoes;

    public BancoDeDadosTests()
    {
        _conexao = new SqliteConnection("Data Source=:memory:");
        _conexao.Open();
        _opcoes = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_conexao).Options;

        using var db = NovoContexto();
        db.Database.EnsureCreated();
    }

    private ApplicationDbContext NovoContexto() => new(_opcoes);

    public void Dispose() => _conexao.Dispose();

    /// <summary>Grava um cliente com um pedido de 3 itens e 10% de desconto; devolve os ids.</summary>
    private (int clienteId, int pedidoId) CriarPedidoDeExemplo()
    {
        using var db = NovoContexto();
        var cliente = new Cliente { Nome = "Cliente Teste Ltda", CnpjCpf = "11.111.111/0001-11" };
        var caneta = new Produto { Codigo = "T-1", Descricao = "Caneta", PrecoUnitario = 45.00m };
        var papel = new Produto { Codigo = "T-2", Descricao = "Papel A4", PrecoUnitario = 32.90m };
        var grampeador = new Produto { Codigo = "T-3", Descricao = "Grampeador", PrecoUnitario = 49.00m };
        db.AddRange(cliente, caneta, papel, grampeador);
        db.SaveChanges();

        var pedido = new Pedido { ClienteId = cliente.Id, DescontoPercentual = 10 };
        pedido.AdicionarItem(caneta, 4);
        pedido.AdicionarItem(papel, 20);
        pedido.AdicionarItem(grampeador, 2);
        db.Pedidos.Add(pedido);
        db.SaveChanges();

        return (cliente.Id, pedido.Id);
    }

    [Fact(DisplayName = "T12 – RN04: pedido gravado no banco volta com o total correto")]
    public void T12_SalvarPedido_RecuperaComTotalCorreto()
    {
        var (_, pedidoId) = CriarPedidoDeExemplo();

        using var db = NovoContexto();
        var pedido = db.Pedidos.Include(p => p.Itens).Single(p => p.Id == pedidoId);

        Assert.Equal(3, pedido.Itens.Count);
        Assert.Equal(StatusPedido.Aberto, pedido.Status);
        Assert.Equal(936.00m, pedido.CalcularSubtotal());
        Assert.Equal(842.40m, pedido.CalcularTotal());
    }

    [Fact(DisplayName = "T13 – RN08: banco impede excluir cliente que tem pedido")]
    public void T13_ExcluirClienteComPedido_EhBloqueadoPeloBanco()
    {
        var (clienteId, _) = CriarPedidoDeExemplo();

        using var db = NovoContexto();
        var cliente = db.Clientes.Single(c => c.Id == clienteId);
        db.Clientes.Remove(cliente);

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());
    }

    [Fact(DisplayName = "T13b – RN01: banco impede CNPJ/CPF repetido")]
    public void T13b_CnpjRepetido_EhBloqueadoPeloBanco()
    {
        CriarPedidoDeExemplo();

        using var db = NovoContexto();
        db.Clientes.Add(new Cliente { Nome = "Outro cliente", CnpjCpf = "11.111.111/0001-11" });

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());
    }

    [Fact(DisplayName = "T13c – Excluir o pedido exclui os itens (cascata)")]
    public void T13c_ExcluirPedido_ExcluiItens()
    {
        var (_, pedidoId) = CriarPedidoDeExemplo();

        using (var db = NovoContexto())
        {
            db.Pedidos.Remove(db.Pedidos.Single(p => p.Id == pedidoId));
            db.SaveChanges();
        }

        using var verificacao = NovoContexto();
        Assert.Equal(0, verificacao.ItensPedido.Count(i => i.PedidoId == pedidoId));
    }

    [Fact(DisplayName = "T13d – Status alterado é gravado no banco")]
    public void T13d_AlterarStatus_GravaNoBanco()
    {
        var (_, pedidoId) = CriarPedidoDeExemplo();

        using (var db = NovoContexto())
        {
            var pedido = db.Pedidos.Include(p => p.Itens).Single(p => p.Id == pedidoId);
            pedido.AlterarStatus(StatusPedido.Aprovado);
            db.SaveChanges();
        }

        using var verificacao = NovoContexto();
        Assert.Equal(StatusPedido.Aprovado, verificacao.Pedidos.Single(p => p.Id == pedidoId).Status);
    }

    [Fact(DisplayName = "T13e – Dados de exemplo são criados uma única vez")]
    public void T13e_DadosIniciais_CriadosUmaVez()
    {
        using (var db = NovoContexto())
        {
            DadosIniciais.Popular(db);
            DadosIniciais.Popular(db); // segunda chamada não duplica
        }

        using var verificacao = NovoContexto();
        Assert.Equal(5, verificacao.Clientes.Count());
        Assert.Equal(10, verificacao.Produtos.Count());
        Assert.Equal(3, verificacao.Pedidos.Count());
        var pedido1 = verificacao.Pedidos.Include(p => p.Itens).OrderBy(p => p.Id).First();
        Assert.Equal(842.40m, pedido1.CalcularTotal());
    }
}
