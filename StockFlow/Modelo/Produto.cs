using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Produto
    {
        private int ProdutoId { get; set; }
        private String Sku { get; set; }
        private String NomeCompleto { get; set; }
        private double PrecoVenda { get; set; }
        private double PrecoCusto { get; set; }
        private bool Ativo { get; set; }
        private int EstoqueAtual { get; set; } 
        private int EstoqueMinimo { get; set; }
        private DateTime DataCadastro { get; set; }
        private int MarcaId { get; set; }
        private int CategoriaId { get; set; }
        private int FornecedorId { get; set; }
        private Marca Marca { get; set; }
        private Categoria Categoria { get; set; }
        private Fornecedor Fornecedor { get; set; }

    }
}
