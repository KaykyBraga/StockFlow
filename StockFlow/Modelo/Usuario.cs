using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Usuario
    {
         public int UsuarioID { get; set; }
         public string NomeCompleto { get; set; }
         public string Email { get; set; }
         public string SenhaHash { get; set; }
         public string PerfilAcesso { get; set; }
         public bool Ativo { get; set; }
         public DateTime DataCadastro { get; set; }
    }
}
