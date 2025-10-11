using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Usuario
    {
        private int UsuarioID { get; set; }
        private string NomeCompleto { get; set; }
        private string Email { get; set; }
        private string SenhaHash { get; set; }
        private string PerfilAcesso { get; set; }
        private bool Ativo { get; set; }
        private DateTime DataCadastro { get; set; }
    }
}
