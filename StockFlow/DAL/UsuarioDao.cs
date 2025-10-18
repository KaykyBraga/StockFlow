using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockFlow.DAL
{
    public class UsuarioDao
    {
        public string mensagem;

        public void AdcionarUsuario(Usuario usuario)
        {
            this.mensagem = "";
            try
            {
                var context = new AppDbContext();
                context.Usuarios.Add(usuario);
                context.SaveChanges();


            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao adicionar usuário!" + ex;

            }
            this.mensagem = "Usuário adicionado com sucesso!";


        }

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            this.mensagem = "";
                    

            try
            {
                var termoBusca = email.Trim().ToLower();

                var context = new AppDbContext();
                var usuario = context.Usuarios.FirstOrDefault(u => u.Email.ToLower() == termoBusca);
                return usuario;
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao buscar usuário por email!";
                return null;
            }
        }

        public Usuario BuscarUsuarioPorIdentificador(string indentificador)
        {
            this.mensagem = "";
            

            try
            {
                var termoBusca = indentificador.Trim().ToLower();
                var context = new AppDbContext();
                var usuario = context.Usuarios.FirstOrDefault(u => u.IdentificadorFuncionario.ToLower() == termoBusca);
                return usuario;
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao buscar usuário por esse Id!";
                return null;
            }
        }

        public Usuario BuscarUsuarioPorId( int id)
        {
            this.mensagem = "";


            try
            {
                
                var context = new AppDbContext();
                var usuario = context.Usuarios.FirstOrDefault(u => u.UsuarioId == id);
                return usuario;
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao buscar usuário por esse Id!";
                return null;
            }
        }

        public List<Usuario> BuscarUsuarioPorNome(string nome)
        {
            this.mensagem = "";
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new List<Usuario>();
            }
            try
            {
                var context = new AppDbContext();
                var termoBusca = nome.Trim().ToLower();
                var Usuarios =  context.Usuarios.Where(u => u.NomeCompleto.ToLower().Contains(termoBusca)).ToList();
                return Usuarios;
            }
            catch (Exception)
            {

                this.mensagem = "Erro ao buscar usuário por nome!";
                return new List<Usuario>();
            }
        }

        public void EditarUsuario(Usuario usuario) 
        {
            this.mensagem = "";
            try
            {
                var context = new AppDbContext();               
                context.Usuarios.Update(usuario);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao editar usuário!";
                return;
            }
            this.mensagem = "Usuário editado com sucesso!";
        }

        public void DesativarUsuario(Usuario usuario)
        {
            this.mensagem = "";
            usuario = BuscarUsuarioPorId(usuario.UsuarioId);
            if (usuario == null)
            {
                this.mensagem = "Usuário não encontrado para deletar!";
                return;
            }
            usuario.Ativo = false;
            try
            {
                var context = new AppDbContext();
                context.Usuarios.Update(usuario);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao desativar usuário!";
                return;
            }
            this.mensagem = "Usuário desativado com sucesso!";
        }

        public async Task<List<Usuario>> ObterTodosOsUsuariosAsync()
        {
            await using (var context = new AppDbContext())
            {
                
                List<Usuario> todosOsUsuarios = await context.Usuarios.ToListAsync();

                return todosOsUsuarios;
            }
        }


        public async Task<List<Usuario>> ObterUsuariosAtivosAsync()
        {
            
            await using (var context = new AppDbContext())
            {
                
                List<Usuario> usuariosAtivos = await context.Usuarios.Where(u => u.Ativo == true).ToListAsync();

                return usuariosAtivos;
            }
        }
    }
}
