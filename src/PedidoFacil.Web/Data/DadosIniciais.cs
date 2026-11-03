using PedidoFacil.Web.Models;

namespace PedidoFacil.Web.Data;

/// <summary>
/// Cria dados de exemplo FICTÍCIOS na primeira vez que o sistema é iniciado,
/// para que os avaliadores já encontrem clientes, produtos e pedidos.
/// </summary>
public static class DadosIniciais
{
    public static void Popular(ApplicationDbContext db)
    {
        if (db.Clientes.Any())
        {
            return; // o banco já tem dados
        }

        var clientes = new List<Cliente>
        {
            new() { Nome = "Mercado Bom Preço Ltda", CnpjCpf = "12.345.678/0001-90", Email = "compras@bompreco.exemplo", Telefone = "(49) 99999-0001", Cidade = "Chapecó", UF = "SC" },
            new() { Nome = "Padaria Pão Quente ME", CnpjCpf = "98.765.432/0001-10", Email = "contato@paoquente.exemplo", Telefone = "(49) 99999-0002", Cidade = "Xaxim", UF = "SC" },
            new() { Nome = "Oficina Silva Ltda", CnpjCpf = "11.222.333/0001-44", Telefone = "(49) 99999-0003", Cidade = "Concórdia", UF = "SC" },
            new() { Nome = "João da Silva", CnpjCpf = "123.456.789-09", Email = "joao.silva@email.exemplo", Telefone = "(49) 99999-0004", Cidade = "Chapecó", UF = "SC" },
            new() { Nome = "Maria Oliveira", CnpjCpf = "987.654.321-00", Telefone = "(49) 99999-0005", Cidade = "Pinhalzinho", UF = "SC" }
        };
        db.Clientes.AddRange(clientes);

        var produtos = new List<Produto>
        {
            new() { Codigo = "PRD-001", Descricao = "Caneta esferográfica azul (cx 50)", PrecoUnitario = 45.00m },
            new() { Codigo = "PRD-002", Descricao = "Lápis preto nº 2 (cx 72)", PrecoUnitario = 38.50m },
            new() { Codigo = "PRD-003", Descricao = "Caderno universitário 200 folhas", PrecoUnitario = 24.90m },
            new() { Codigo = "PRD-004", Descricao = "Papel A4 75g (resma)", PrecoUnitario = 32.90m },
            new() { Codigo = "PRD-005", Descricao = "Pasta suspensa (pct 10)", PrecoUnitario = 27.00m },
            new() { Codigo = "PRD-006", Descricao = "Clips nº 2 (cx 500)", PrecoUnitario = 12.50m },
            new() { Codigo = "PRD-007", Descricao = "Grampeador de mesa", PrecoUnitario = 49.00m },
            new() { Codigo = "PRD-008", Descricao = "Grampos 26/6 (cx 5000)", PrecoUnitario = 9.80m },
            new() { Codigo = "PRD-009", Descricao = "Fita adesiva transparente (pct 10)", PrecoUnitario = 35.00m },
            new() { Codigo = "PRD-010", Descricao = "Marca-texto amarelo (cx 12)", PrecoUnitario = 42.00m, Ativo = false }
        };
        db.Produtos.AddRange(produtos);

        // Grava clientes e produtos primeiro para que recebam seus Ids.
        db.SaveChanges();

        // Pedido 1 – Aberto, com 10% de desconto: subtotal 936,00 → total 842,40.
        var pedido1 = new Pedido { Cliente = clientes[0], DataPedido = DateTime.Today.AddDays(-2), DescontoPercentual = 10 };
        pedido1.AdicionarItem(produtos[0], 4);
        pedido1.AdicionarItem(produtos[3], 20);
        pedido1.AdicionarItem(produtos[6], 2);

        // Pedido 2 – Aprovado.
        var pedido2 = new Pedido { Cliente = clientes[1], DataPedido = DateTime.Today.AddDays(-5) };
        pedido2.AdicionarItem(produtos[2], 10);
        pedido2.AdicionarItem(produtos[5], 3);
        pedido2.AlterarStatus(StatusPedido.Aprovado);

        // Pedido 3 – Faturado, com 5% de desconto.
        var pedido3 = new Pedido { Cliente = clientes[2], DataPedido = DateTime.Today.AddDays(-9), DescontoPercentual = 5 };
        pedido3.AdicionarItem(produtos[1], 2);
        pedido3.AdicionarItem(produtos[8], 4);
        pedido3.AlterarStatus(StatusPedido.Aprovado);
        pedido3.AlterarStatus(StatusPedido.Faturado);

        db.Pedidos.AddRange(pedido1, pedido2, pedido3);
        db.SaveChanges();
    }
}
