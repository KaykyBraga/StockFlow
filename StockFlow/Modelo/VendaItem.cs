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
         public virtual Venda Venda { get; set; }
         public virtual Produto Produto { get; set; }
    }
}
