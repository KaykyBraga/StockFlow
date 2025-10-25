using Microsoft.Identity.Client;
using StockFlow.DAL;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Controles
{
    public class ControleUsuario
    {

        public string mensagem = "";
        public String usuarioAcesso = "";
        public void CadastrarUsuario(List<String> listaDadosUsuario)
        {
            this.mensagem = "";
            Usuario usuario1 = new Usuario();
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            ValidacaoUsuario validacao = new ValidacaoUsuario();
            DataHoraCorreta dataHoraCorreta = new DataHoraCorreta();
            LoginCadastro loginCadastro = new LoginCadastro();
            usuario1.NomeCompleto = listaDadosUsuario[0];
            usuario1.SenhaHash = listaDadosUsuario[1];
            usuario1.IdentificadorFuncionario = listaDadosUsuario[2];
            usuario1.Email = listaDadosUsuario[3];
            usuario1.PerfilAcesso = listaDadosUsuario[4];
            usuario1.Ativo = true;
            
            dataHoraCorreta.ObterHoraCorretaComCallback(horaRecebida =>
            {

                if (horaRecebida.HasValue)
                {
                    // Agora você tem o resultado!
                    usuario1.DataCadastro = horaRecebida.Value;
                }
                else
                {
                    this.mensagem = "Falha ao obter a hora. O salvamento foi cancelado.";
                    return;
                }
            });




            // Valida se o email é valido
            validacao.EmailValido(usuario1.Email);
            if (validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }

            // Valida se o email ou o Identificador ja existe
            validacao.EmailExistenteOuIdentificadorExistente(usuario1.Email, usuario1.IdentificadorFuncionario);

            // Se o email ja existir, retorna a mensagem de erro
            if (validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }

            //Hash da senha
            usuario1.SenhaHash = loginCadastro.Hash(usuario1.SenhaHash);

            //adiciona o usuario
            usuarioDao.AdcionarUsuario(usuario1);
            this.mensagem = usuarioDao.mensagem;
        }


        public void LoginUsuario(string email, string senha, out string TipoUsuario)
        {

            this.mensagem = "";
            this.usuarioAcesso = "";
            Usuario usuario = new Usuario();
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            LoginCadastro loginCadastro = new LoginCadastro();

            // Busca o usuario pelo email
            usuario = usuarioDao.BuscarUsuarioPorEmail(email);

            // Verifica se o usuario existe
            if (usuarioDao.mensagem == "")
            {
                // Verifica se a senha esta correta
                if (loginCadastro.LoginUsuario(senha, usuario))
                {
                    this.mensagem = "";
                    this.usuarioAcesso = usuario.PerfilAcesso;
                    SessaoUsuario.IniciarSessao(usuario.UsuarioId);
                    TipoUsuario = usuario.PerfilAcesso;
                    return;
                }

            }
            this.mensagem = "Erro ao realizar login, email ou senha invalidos ";
            TipoUsuario = "";
            return;
        }


        public void EditarUsuario(List<string> listaUsuario)
        {
            Usuario usuario = new Usuario();
            ValidacaoUsuario validacao = new ValidacaoUsuario();
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);

            usuario.NomeCompleto = listaUsuario[0]; // [0]
            usuario.Email = listaUsuario[1]; // [1]
            usuario.PerfilAcesso = listaUsuario[2]; // [2]
            usuario.IdentificadorFuncionario = listaUsuario[3]; // [3]
            usuario.Ativo = validacao.ValidarAtivo(listaUsuario[4]); // [4]
            usuario.UsuarioId = validacao.ValidarId(listaUsuario[5]); // [5]
            usuario.DataCadastro = Convert.ToDateTime(listaUsuario[6]); // [6]
            usuario.SenhaHash = listaUsuario[7];
            if (validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }

            usuarioDao.EditarUsuario(usuario);
            this.mensagem = usuarioDao.mensagem;
            return;
        }

        public List<Usuario> BuscarUsuarioPorNome(string nome)
        {
            this.mensagem = "";
            List<Usuario> listaDeUsuarios = new List<Usuario>();
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            listaDeUsuarios = usuarioDao.BuscarUsuarioPorNome(nome);
            if (usuarioDao.mensagem != "")
            {
                this.mensagem = usuarioDao.mensagem;
                return null;
            }
            return listaDeUsuarios;


        }

        public Usuario BuscarUsuarioPorIdentificador(string indentificador)
        {
            this.mensagem = "";
            Usuario usuario = new Usuario();
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            usuario = usuarioDao.BuscarUsuarioPorIdentificador(indentificador);
            if (usuarioDao.mensagem != "")
            {
                this.mensagem = usuarioDao.mensagem;
                return null;
            }
            return usuario;
        }

        public void DesativarUsuario(string id)
        {
            Usuario usuario = new Usuario();
            ValidacaoUsuario validacao = new ValidacaoUsuario();
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);

            
            usuario.UsuarioId = validacao.ValidarId(id);
            
            if (validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }

            usuarioDao.DesativarUsuario(usuario.UsuarioId);
            this.mensagem = usuarioDao.mensagem;
            return;
        }

        public void RedefinirSenha(string email, string identificador, string novaSenha)
        {
            this.mensagem = "";
            this.usuarioAcesso = "";
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            ValidacaoUsuario validacao = new ValidacaoUsuario();
            LoginCadastro loginCadastro = new LoginCadastro();
            string senha = loginCadastro.Hash(novaSenha);


            // Busca o usuario pelo email
            var usuario = usuarioDao.BuscarUsuarioPorEmail(email);

            // Verifica se o usuario existe
            if (usuarioDao.mensagem != "")
            {
                this.mensagem = "Erro ao redefinir senha, email ou identificador invalidos ";
                return;
            }
            if (usuario == null)
            {
                this.mensagem = "Erro ao redefinir senha, email ou identificador invalidos ";
                return;
            }

            if (loginCadastro.verificar(novaSenha, usuario.SenhaHash))
            {
                this.mensagem = "A nova senha não pode ser igual a senha antiga";
                return;
            }

            if (usuario.IdentificadorFuncionario == identificador)
            {
                usuario.SenhaHash = senha;
                usuarioDao.EditarUsuario(usuario);
                this.mensagem = "";

            }
            else
            {
                this.mensagem = "Erro ao redefinir senha, email ou identificador invalidos ";
                return;
            }


        }

        public async Task<List<Usuario>> ObterTodosOsUsuariosAsync()
        {
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            List<Usuario> listaUsuarios = new List<Usuario>();
            listaUsuarios = await usuarioDao.ObterTodosOsUsuariosAsync();
            return listaUsuarios;
        }

        public async Task<List<Usuario>> ObterTodosOsUsuariosAtivosAsync()
        {
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            List<Usuario> listaUsuarios = new List<Usuario>();
            listaUsuarios = await usuarioDao.ObterUsuariosAtivosAsync();
            return listaUsuarios;
        }

        public async Task<List<StockFlow.Visual.Funcionario>> ObterTodosOsUsuariosAtivosDataGridAsync()
        {
            var context = new AppDbContext();
            UsuarioDao usuarioDao = new UsuarioDao(context);
            List<Usuario> listaUsuarios = new List<Usuario>();
            listaUsuarios = await usuarioDao.ObterUsuariosAtivosAsync();

            var listaFinalParaGrid = listaUsuarios.Select(Usuario => new StockFlow.Visual.Funcionario
            {                
                Id = Usuario.IdentificadorFuncionario,
                Nome = Usuario.NomeCompleto,
                Email = Usuario.Email
            }).ToList();
            return listaFinalParaGrid;
        }
       
    }

}

