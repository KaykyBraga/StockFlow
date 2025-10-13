using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class VendaItem
    {
         public int VendaItemId { get; set; }
         public int VendaId { get; set; }
         public int ProdutoId { get; set; }
         public int Quantidade { get; set; }
         public double PrecoUnitarioMomento { get; set; }
         public double DescontoItem { get; set; }
         public Venda venda { get; set; }
         public Produto produto { get; set; }
    }
}
