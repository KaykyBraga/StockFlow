using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Caixa
    {
        public int CaixaId { get; set; }
        public int UsuarioAberturaId { get; set; }
        public DateTime DataHoraAbertura { get; set; }
        public decimal ValorAbertura { get; set; }
        public int? UsuarioFechamentoId { get; set; }
        public DateTime? DataHoraFechamento { get; set; }
        public decimal? ValorFechamentoCalculado { get; set; }
        public decimal? ValorFechamentoInformado { get; set; }
        public decimal? ValorFechamentoCaixa { get; set; }
        public decimal? Diferenca { get; set; }
        public string Status { get; set; } // "Aberto" ou "Fechado"
        
        [ForeignKey("UsuarioAberturaId")]
        public virtual Usuario UsuarioAbertura { get; set; }

        public virtual ICollection<MovimentacaoCaixa> MovimentacaoCaixa { get; set; }

        public Caixa()
        {

            MovimentacaoCaixa = new HashSet<MovimentacaoCaixa>();
        }
    }
}
