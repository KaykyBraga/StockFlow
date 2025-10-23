using StockFlow.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    internal class ValidacaoUsuario
    {
        public string mensagem = "";
        public void EmailExistenteOuIdentificadorExistente(string email, string identificador)
        {
            this.mensagem = "";
            var contexto = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(contexto);
            Usuario usuario = new Usuario();
            Usuario usuario2 = new Usuario();

            usuario = usuarioDao.BuscarUsuarioPorEmail(email);
            usuario2 = usuarioDao.BuscarUsuarioPorIdentificador(identificador);

            if (usuario == null)
                return;

            if (usuario2 == null)
                return;

            // Verifica se o email já está cadastrado
            if (usuario.Email == email)
            {
                this.mensagem = "Email já cadastrado!";
                return;
            }


            // Verifica se o identificador já está cadastrado
            if (usuario2.IdentificadorFuncionario == identificador)
            {
                this.mensagem = "Identificador já cadastrado!";
                return;
            }

        }

        public void EmailValido(string email)
        {
            this.mensagem = "";
            string padraoEmail = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (!Regex.IsMatch(email, padraoEmail, RegexOptions.IgnoreCase))
            {
                this.mensagem = "Formato de e-mail inválido.";
                return;
            }
        }

        public int ValidarId(string Id)
        {
            this.mensagem = "";
            int convercao;
            try
            {
                convercao = Convert.ToInt32(Id);
            }
            catch (Exception)
            {
                this.mensagem = "ID inválido, insira um número inteiro.";
                return 0;
            }
            return convercao;
        }

        public bool ValidarAtivo (string ativo)
        {
            this.mensagem = "";
            bool convercao;
            try
            {
                convercao = Convert.ToBoolean(ativo);
            }
            catch (Exception)
            {
                this.mensagem = "Valor inválido para saber se o usuario esta ativo";
                return false;
            }
            return convercao;
        }

    }
}
