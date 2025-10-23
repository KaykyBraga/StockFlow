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
        private readonly AppDbContext _context;

        // O construtor agora recebe a instância do AppDbContext
        public UsuarioDao(AppDbContext context)
        {
            _context = context;
        }

        public void AdcionarUsuario(Usuario usuario)
        {
            this.mensagem = "";
            try
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                this.mensagem = "Usuário adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao adicionar usuário! " + ex.Message;
            }
        }

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            this.mensagem = "";
            try
            {
                var termoBusca = email.Trim().ToLower();
                return _context.Usuarios.FirstOrDefault(u => u.Email.ToLower() == termoBusca);
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao buscar usuário por email!";
                return null;
            }
        }

        public Usuario BuscarUsuarioPorIdentificador(string identificador)
        {
            this.mensagem = "";
            try
            {
                var termoBusca = identificador.Trim().ToLower();
                return _context.Usuarios.FirstOrDefault(u => u.IdentificadorFuncionario.ToLower() == termoBusca);
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao buscar usuário por esse Id!";
                return null;
            }
        }

        public Usuario BuscarUsuarioPorId(int id)
        {
            this.mensagem = "";
            try
            {
                // Usar Find é mais otimizado para busca por chave primária
                return _context.Usuarios.Find(id);
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
                var termoBusca = nome.Trim().ToLower();
                return _context.Usuarios.Where(u => u.NomeCompleto.ToLower().Contains(termoBusca)).ToList();
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
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                this.mensagem = "Usuário editado com sucesso!";
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao editar usuário!";
            }
        }

        // Alterado para receber ID para ser mais testável e robusto
        public void DesativarUsuario(int usuarioId)
        {
            this.mensagem = "";
            try
            {
                var usuario = _context.Usuarios.Find(usuarioId);
                if (usuario == null)
                {
                    this.mensagem = "Usuário não encontrado para desativar!";
                    return;
                }
                usuario.Ativo = false;
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                this.mensagem = "Usuário desativado com sucesso!";
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao desativar usuário!";
            }
        }

        public async Task<List<Usuario>> ObterTodosOsUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<List<Usuario>> ObterUsuariosAtivosAsync()
        {
            return await _context.Usuarios.Where(u => u.Ativo).ToListAsync();
        }
    }
}
