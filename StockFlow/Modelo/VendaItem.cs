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
         public decimal PrecoUnitarioMomento { get; set; }
         public decimal DescontoItem { get; set; }
         public Venda Venda { get; set; }
         public Produto Produto { get; set; }
    }
}
