using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Fornecedor
    {
        private int FornecedorId { get; set; }
        private string NomeFantasia { get; set; }
        private string RazaoSocial { get; set; }
        private string Cnpj { get; set; }
        private string EmailPrincipal { get; set; }
        private string TelefonePrincipal { get; set; }
        private bool Ativo { get; set; }
        private DateTime DataCadastro { get; set; }
    }
}
