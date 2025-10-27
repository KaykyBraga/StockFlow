using StockFlow.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        public String Sku { get; set; }
        public string Ean { get; set; }
        public String NomeCompleto { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal PrecoCusto { get; set; }
        public bool Ativo { get; set; }
        public int EstoqueAtual { get; set; } 
        public int EstoqueMinimo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int MarcaId { get; set; }
        public int CategoriaId { get; set; }
        public int FornecedorId { get; set; }
        public string LocalizacaoEstoque { get; set; }
        public virtual Marca Marca { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }

        public virtual ICollection<Movimentacao> Movimentacaos { get; set; }
        public virtual ICollection<PromocaoProduto> PromocaoProdutos { get; set; }


        public Produto()
        {           
            Movimentacaos = new HashSet<Movimentacao>();
            PromocaoProdutos = new HashSet<PromocaoProduto>();
        }

    }
}
