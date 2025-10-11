using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Movimentacao
    {
        private int MovimentacaoId { get; set; }
        private int ProdutoId { get; set; }
        private int UsuarioId { get; set; }
        private string TipoMovimentacao { get; set; }
        private DateTime Data { get; set; }
        private int Quantidade { get; set; }
        private string Observacao { get; set; }

        private Produto Produto { get; set; }
        private Usuario Usuario { get; set; }
    }
}
