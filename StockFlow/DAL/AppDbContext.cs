using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class AppDbContext : DbContext
    {


        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Fornecedor> Fornecedores { get; set; }
        public virtual DbSet<Marca> Marcas { get; set; }
        public virtual DbSet<Movimentacao> movimentacoes { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Promocao> Promocoes { get; set; }
        public virtual DbSet<PromocaoProduto> PromocaoProdutos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Venda> Vendas { get; set; }
        public virtual DbSet<VendaItem> VendaItens { get; set; }
        public virtual DbSet<Caixa> Caixas { get; set; }
        public virtual DbSet<MovimentacaoCaixa> MovimentacaoCaixas { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source = 100.124.148.100; Initial Catalog = Db_StockFlow; User ID = sa; Password = 21122005; Encrypt = False");
            }            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PromocaoProduto>().HasOne(pp => pp.Promocao).WithMany(p => p.PromocaoProdutos).HasForeignKey(pp => pp.PromocaoId).OnDelete(DeleteBehavior.Cascade);
            
            // Se um Produto for deletado, os vínculos dele nas promoções também somem.
            modelBuilder.Entity<PromocaoProduto>()
                .HasOne(pp => pp.Produto)
                .WithMany(p => p.PromocaoProdutos)
                .HasForeignKey(pp => pp.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
