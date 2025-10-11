using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class VendaItens
    {
        private int VendaItemId { get; set; }
        private int VendaId { get; set; }
        private int ProdutoId { get; set; }
        private int Quantidade { get; set; }
        private double PrecoUnitarioMomento { get; set; }
        private double DescontoItem { get; set; }
        private Venda venda { get; set; }
        private Produto produto { get; set; }
    }
}
