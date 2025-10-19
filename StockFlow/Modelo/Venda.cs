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
        public int VendaId { get; set; }
        public int UsuarioId { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal DescontoTotal { get; set; }
        public String MetodoPagamento { get; set; }
        public DateTime DataVenda { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<VendaItem> VendaItems { get; set; }

        public Venda()
        {            
            VendaItems = new HashSet<VendaItem>(); 
        }

    }
}
