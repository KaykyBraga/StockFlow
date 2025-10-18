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
    public class FornecedorDao
    {
        public string mensagem = "";

        public void CadastrarFornecedor(Fornecedor fornecedor)
        {
            this.mensagem = "";
            try
            {
                var contexto = new AppDbContext();
                contexto.Fornecedores.Add(fornecedor);
                contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao cadastrar fornecedor!" + ex;
            }
        }

        public void DesativarFornecedor(Fornecedor fornecedor)
        {
            this.mensagem = "";

            fornecedor = BuscarUsuarioPorId(fornecedor.FornecedorId);
            if (fornecedor == null)
            {
                this.mensagem = "Fornecedor não encontrado para desativalo!";
                return;
            }
            fornecedor.Ativo = false;

            try
            {

                var context = new AppDbContext();
                context.Fornecedores.Update(fornecedor);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao desativar o fornecedor!";
                return;
            }
            this.mensagem = "fornecedor desativar com sucesso!";
        }

        public List<Fornecedor> BuscarFornecedorPorNome(string nome)
        {
            this.mensagem = "";
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new List<Fornecedor>();
            }
            try
            {
                
                var context = new AppDbContext();
                var termoBusca = nome.Trim().ToLower();
                var fornecedores = context.Fornecedores.Where(u => u.NomeFantasia.ToLower().Contains(termoBusca)).ToList();
                return fornecedores;
            }
            catch (Exception)
            {

                this.mensagem = "Erro ao buscar Fornecedores por nome!";
                return new List<Fornecedor>();
            }
        }

        public Fornecedor BuscarUsuarioPorId(int id)
        {
            this.mensagem = "";


            try
            {

                var context = new AppDbContext();
                var fornecedor = context.Fornecedores.FirstOrDefault(u => u.FornecedorId == id);
                return fornecedor;
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao buscar usuário por esse Id!";
                return null;
            }
        }

        public void EditarFornecedor(Fornecedor fornecedor)
        {
            this.mensagem = "";
            try
            {
                var context = new AppDbContext();
                context.Fornecedores.Update(fornecedor);
                context.SaveChanges();
            }
            catch (Exception)
            {
               this.mensagem = "Erro ao editar o usuário!";
                return;
            }
            this.mensagem = "Fornecedor editado com sucesso!";
        }

        public async Task<List<Fornecedor>> ObterTodosOsFornecedoresAsync()
        {
            await using (var context = new AppDbContext())
            {
                
                List<Fornecedor> todosOsFornecedores = await context.Fornecedores.ToListAsync();

                return todosOsFornecedores;
            }
        }

        public async Task<List<Fornecedor>> ObterFornecedoresAtivosAsync()
        {

            await using (var context = new AppDbContext())
            {

                List<Fornecedor> fornecedoresAtivos = await context.Fornecedores.Where(u => u.Ativo == true).ToListAsync();

                return fornecedoresAtivos;
            }
        }
    }
}
