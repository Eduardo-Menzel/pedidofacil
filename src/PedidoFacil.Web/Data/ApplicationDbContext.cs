using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PedidoFacil.Web.Models;

namespace PedidoFacil.Web.Data;

/// <summary>
/// Acesso ao banco de dados (Entity Framework Core + SQLite).
/// Herda de IdentityDbContext para incluir as tabelas de usuários do ASP.NET Core Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Necessário para criar as tabelas AspNet* do Identity.
        base.OnModelCreating(builder);

        builder.Entity<Cliente>(e =>
        {
            e.ToTable("Clientes");
            e.HasIndex(c => c.CnpjCpf).IsUnique();   // RN01
            e.HasIndex(c => c.Nome);                  // busca por nome (RF02)
        });

        builder.Entity<Produto>(e =>
        {
            e.ToTable("Produtos", t => t.HasCheckConstraint("CK_Produtos_Preco", "CAST(\"PrecoUnitario\" AS REAL) > 0"));
            e.HasIndex(p => p.Codigo).IsUnique();
            e.Property(p => p.PrecoUnitario).HasPrecision(10, 2);
        });

        builder.Entity<Pedido>(e =>
        {
            e.ToTable("Pedidos", t =>
            {
                t.HasCheckConstraint("CK_Pedidos_Status", "\"Status\" BETWEEN 0 AND 3");                                  // RN06
                t.HasCheckConstraint("CK_Pedidos_Desconto", "CAST(\"DescontoPercentual\" AS REAL) BETWEEN 0 AND 30");      // RN03
            });
            e.Property(p => p.DescontoPercentual).HasPrecision(5, 2);
            e.HasIndex(p => p.Status);                // filtro por status (RF08)

            // RN08: cliente com pedidos não pode ser excluído.
            e.HasOne(p => p.Cliente)
             .WithMany(c => c.Pedidos)
             .HasForeignKey(p => p.ClienteId)
             .OnDelete(DeleteBehavior.Restrict);

            // Composição: excluir o pedido exclui os itens.
            e.HasMany(p => p.Itens)
             .WithOne(i => i.Pedido)
             .HasForeignKey(i => i.PedidoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ItemPedido>(e =>
        {
            e.ToTable("ItensPedido", t => t.HasCheckConstraint("CK_ItensPedido_Quantidade", "\"Quantidade\" >= 1"));   // RN02
            e.Property(i => i.PrecoUnitario).HasPrecision(10, 2);

            // Produto que já foi vendido não pode ser excluído.
            e.HasOne(i => i.Produto)
             .WithMany()
             .HasForeignKey(i => i.ProdutoId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
