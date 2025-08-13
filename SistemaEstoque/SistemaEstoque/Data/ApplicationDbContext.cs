using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SistemaEstoque.Models;

namespace SistemaEstoque.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItemVendas { get; set; }
        public DbSet<Servico> Servicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações para propriedades decimal
            modelBuilder.Entity<Produto>()
                .Property(p => p.PrecoCompra)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Produto>()
                .Property(p => p.PrecoVenda)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Servico>()
                .Property(s => s.ValorServico)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Venda>()
                .Property(v => v.ValorTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.PrecoUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.Subtotal)
                .HasColumnType("decimal(18,2)");

            // Configurações para relacionamentos
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId);

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Funcionario)
                .WithMany(f => f.Vendas)
                .HasForeignKey(v => v.FuncionarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemVenda>()
                .HasOne(i => i.Venda)
                .WithMany(v => v.ItemVendas)
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemVenda>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Servico>()
                .HasOne(s => s.Cliente)
                .WithMany(c => c.Servicos)
                .HasForeignKey(s => s.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Servico>()
                .HasOne(s => s.Funcionario)
                .WithMany(f => f.Servicos)
                .HasForeignKey(s => s.FuncionarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
