using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        public String Sku { get; set; }
        public String NomeCompleto { get; set; }
        public double PrecoVenda { get; set; }
        public double PrecoCusto { get; set; }
        public bool Ativo { get; set; }
        public int EstoqueAtual { get; set; } 
        public int EstoqueMinimo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int MarcaId { get; set; }
        public int CategoriaId { get; set; }
        public int FornecedorId { get; set; }
        public Marca Marca { get; set; }
        public Categoria Categoria { get; set; }
        public Fornecedor Fornecedor { get; set; }

    }
}
