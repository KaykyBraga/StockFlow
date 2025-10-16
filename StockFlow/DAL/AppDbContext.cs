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
       
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Movimentacao> movimentacoes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Promocao> promocaos { get; set; }
        public DbSet<PromocaoProduto> PromocaoProdutos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<VendaItem> VendaItens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = KAYKY; Initial Catalog = Db_StockFlow; User ID = sa; Password = 21122005; Encrypt = False");
        }
    }
}
