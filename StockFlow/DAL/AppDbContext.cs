using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
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
                optionsBuilder.UseSqlServer(@"Data Source = KAYKY; Initial Catalog = Db_StockFlow; User ID = sa; Password = 21122005; Encrypt = False");
            }
        }

        
    }
}
