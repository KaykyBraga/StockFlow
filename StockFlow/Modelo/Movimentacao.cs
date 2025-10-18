using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Movimentacao
    {
         public int MovimentacaoId { get; set; }
         public int ProdutoId { get; set; }
         public int UsuarioId { get; set; }
         public string TipoMovimentacao { get; set; }
         public DateTime Data { get; set; }
         public int Quantidade { get; set; }
         public string Observacao { get; set; }

         public virtual Produto Produto { get; set; }
         public virtual Usuario Usuario { get; set; }    
        
    }
}
