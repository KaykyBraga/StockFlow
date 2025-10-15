using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public static class SessaoUsuario
    {
        public static int UsuarioId { get; private set; }

        public static bool IsLoggedIn => UsuarioId > 0;

        
        // Inicia a sessão do usuário após um login bem-sucedido.
        
        public static void IniciarSessao(int id)
        {
            UsuarioId = id;            
        }

        
        // Limpa os dados do usuário ao fazer logout.
        
        public static void EncerrarSessao()
        {
            UsuarioId = 0;           
        }
    }
}
