using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockFlow.DAL
{
    public class ProdutoDao
    {
        public string mensagem = "";

        public bool CadastrarProduto(Produto produto)
        {
            this.mensagem = "";

            try
            {

                var contexto = new AppDbContext();
                bool ProduotoJaCadastrado = contexto.Produtos.Any(p => p.Ean == produto.Ean);
                if (ProduotoJaCadastrado)
                {
                    this.mensagem = "Produto já cadastrado!";
                    return false;
                }
                contexto.Produtos.Add(produto);
                contexto.SaveChanges();


            }
            catch (Exception ex)
            {
                // Para depuração, esta linha imprime TUDO no console de saída (muito útil)
                Console.WriteLine(ex.ToString());

                // Pega a exceção mais interna, que é a que realmente importa
                Exception innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                // Sua variável 'mensagem' agora terá a mensagem de erro específica do banco
                this.mensagem = "Erro ao cadastrar o produto. Causa: " + innerEx.Message;
                return false;
            }
            return true;
        }

        public List<Produto> BuscarProdutoPorNome(string nome)
        {
            this.mensagem = "";

            // Se o termo de busca for nulo ou vazio, retorna uma lista vazia imediatamente.
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new List<Produto>();
            }

            try
            {               
                using (var context = new AppDbContext())
                {
                    var termoBusca = nome.Trim().ToUpper();

                    //BUSCA case-insensitive, convertendo ambos os lados para ToUpper().
                    var produtos = context.Produtos
                        .Where(p => p.NomeCompleto.ToUpper().Contains(termoBusca)).ToList();

                    return produtos;
                }
            }
            catch (Exception ex)
            {
                
                this.mensagem = "Ocorreu um erro ao consultar o banco de dados." + ex.Message;

                // RETORNAR uma lista vazia.
                return new List<Produto>();
            }
        }

        public Produto BuscarProdutoPorId(int produtoId)
        {
            this.mensagem = "";
            try
            {
                using (var context = new AppDbContext())
                {
                    
                    var produto = context.Produtos.Find(produtoId);
                    if (produto != null)
                    {
                        return produto;
                    }
                    else
                    {
                        this.mensagem = "Produto não encontrado.";
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao buscar produto: " + ex.Message;
                return null;
            }
        }

        public void AdicionarEstoque(int produtoId, int quantidade)
        {
            this.mensagem = "";
            try
            {
                using (var context = new AppDbContext())
                {
                    var produto = context.Produtos.Find(produtoId);
                    if (produto != null)
                    {
                        produto.EstoqueAtual += quantidade;
                        context.SaveChanges();
                    }
                    else
                    {
                        this.mensagem = "Produto não encontrado.";
                    }
                }
            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao adicionar estoque: " + ex.Message;
            }
        }

        public void DesativarProduto(Produto produto)
        {
            this.mensagem = "";
            produto = BuscarProdutoPorId(produto.ProdutoId);
            if (produto == null)
            {
                this.mensagem = "Produto não encontrado para desativação.";
                return;
            }
            produto.Ativo = false;
            try
            {
                var context = new AppDbContext();
                context.Produtos.Update(produto);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao deletar o produto!";
                return;
            }
            this.mensagem = "Produto desativado com sucesso!";
        }

        public void EditarProduto(Produto produto)
        {
            this.mensagem = "";
            try
            {
                var context = new AppDbContext();
                context.Produtos.Update(produto);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao editar o produto!";
                return;
            }
            
        }

        public async Task<List<Produto>> ObterTodosOsProdutosAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<Produto> todosOsProdutos = await context.Produtos.ToListAsync();

                return todosOsProdutos;
            }
        }


        public async Task<List<Produto>> ObterProdutosAtivosAsync()
        {

            await using (var context = new AppDbContext())
            {

                List<Produto> produtosAtivos = await context.Produtos.Include(p => p.Marca)
                    .Include(p => p.Fornecedor)
                    .Include(p => p.Categoria)
                    .Where(u => u.Ativo == true).ToListAsync();

                return produtosAtivos;
            }
        }

        public async Task<List<Produto>> ObterProdutosComEstoqueBaixoAsync()
        {
            
            await using (var context = new AppDbContext())
            {
                
                List<Produto> produtosComEstoqueBaixo = await context.Produtos.Include(p => p.Marca)
                    .Include(p => p.Fornecedor)
                    .Include(p => p.Categoria)
                    .Where(p => p.EstoqueAtual < p.EstoqueMinimo)
                    .ToListAsync();

                return produtosComEstoqueBaixo;
            }
        }

    }


}
