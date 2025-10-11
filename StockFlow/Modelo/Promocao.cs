using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Promocao
    {
        private int PromocaoId { get; set; }
        private string NomePromocao { get; set; }
        private string TipoDesconto { get; set; }
        private double ValorDesconto { get; set; }
        private DateTime DataInicio { get; set; }
        private DateTime DataFim { get; set; }
        private bool Ativo { get; set; }


    }
}
