using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class PromocaoProduto
    {
        public int PromocaoProdutoId { get; set; }
        public int PromocaoId { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public Promocao Promocao { get; set; }
    }
}
