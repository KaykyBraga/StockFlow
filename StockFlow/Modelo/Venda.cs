using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace StockFlow.Modelo
{
    public class Venda
    {
        private int VendaId { get; set; }
        private int ProdutoId { get; set; }
        private double ValorTotal { get; set; }
        private double DescontoTotal { get; set; }
        private String MetodoPagamento { get; set; }
        private Produto produto { get; set; }
    }
}
