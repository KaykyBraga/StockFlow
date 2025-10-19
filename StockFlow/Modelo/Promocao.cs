using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Promocao
    {
         public int PromocaoId { get; set; }
         public string NomePromocao { get; set; }
         public string TipoDesconto { get; set; } // "Porcentagem" ou "ValorFixo"
        public decimal ValorDesconto { get; set; }
         public DateTime DataInicio { get; set; }
         public DateTime DataFim { get; set; }
         public bool Ativo { get; set; }
        public virtual ICollection<PromocaoProduto> PromocaoProdutos { get; set; }


    }
}
