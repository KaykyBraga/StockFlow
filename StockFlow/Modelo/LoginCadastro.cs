using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class LoginCadastro
    {
        public bool LoginUsuario(string senhaDigitada,Usuario usuario)
        {           
                return this.verificar(senhaDigitada, usuario.SenhaHash);
        }

        public string Hash(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public bool verificar(string senha, string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }
       

    }
}
