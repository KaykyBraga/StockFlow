using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class MovimentacaoCaixa
    {
        public int MovimentacaoCaixaId { get; set; }
        public int CaixaId { get; set; }
        public int? VendaId { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Valor { get; set; }
        public string TipoMovimentacao { get; set; } // "Entrada", "Saída", "Abertura", "Sangria"
        public string MetodoDePagamento { get; set; } // "Dinheiro", "Cartão"
        public string Descricao { get; set; }
        public virtual Caixa Caixa { get; set; }
        public virtual Venda Venda { get; set; }
    }
}
