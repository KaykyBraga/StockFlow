using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Fornecedor
    {
         public int FornecedorId { get; set; }
         public string NomeFantasia { get; set; }
         public string RazaoSocial { get; set; }
         public string Cnpj { get; set; }
         public string EmailPrincipal { get; set; }
         public string TelefonePrincipal { get; set; }
         public bool Ativo { get; set; }
         public DateTime DataCadastro { get; set; }
    }
}
