using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class PromocaoProduto
    {
        private int PromocaoProdutoId { get; set; }
        private int PromocaoId { get; set; }
        private int ProdutoId { get; set; }
        private Produto Produto { get; set; }
        private Promocao Promocao { get; set; }
    }
}
