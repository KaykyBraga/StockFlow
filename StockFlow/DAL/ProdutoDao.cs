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
        private readonly AppDbContext _context;

        // O construtor agora recebe a instância do AppDbContext
        public ProdutoDao(AppDbContext context)
        {
            _context = context;
        }

        public bool CadastrarProduto(Produto produto)
        {
            this.mensagem = "";
            try
            {
                // Usa o _context em vez de criar um novo
                bool ProduotoJaCadastrado = _context.Produtos.Any(p => p.Ean == produto.Ean);
                if (ProduotoJaCadastrado)
                {
                    this.mensagem = "Produto já cadastrado!";
                    return false;
                }
                _context.Produtos.Add(produto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // ... (seu tratamento de erro continua o mesmo)
                this.mensagem = "Erro ao cadastrar o produto. Causa: " + ex.Message;
                return false;
            }
            return true;
        }

        public List<Produto> BuscarProdutoPorNome(string nome)
        {
            this.mensagem = "";
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new List<Produto>();
            }
            try
            {
                // Usa o _context em vez de criar um novo
                var termoBusca = nome.Trim().ToUpper();
                var produtos = _context.Produtos
                    .Where(p => p.NomeCompleto.ToUpper().Contains(termoBusca)).ToList();
                return produtos;
            }
            catch (Exception ex)
            {
                this.mensagem = "Ocorreu um erro ao consultar o banco de dados." + ex.Message;
                return new List<Produto>();
            }
        }

        public Produto BuscarProdutoPorId(int produtoId)
        {
            this.mensagem = "";
            try
            {
                // Usa o _context em vez de criar um novo
                var produto = _context.Produtos.Find(produtoId);
                if (produto == null)
                {
                    this.mensagem = "Produto não encontrado.";
                }
                return produto;
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
                // Usa o _context em vez de criar um novo
                var produto = _context.Produtos.Find(produtoId);
                if (produto != null)
                {
                    produto.EstoqueAtual += quantidade;
                    _context.SaveChanges();
                }
                else
                {
                    this.mensagem = "Produto não encontrado.";
                }
            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao adicionar estoque: " + ex.Message;
            }
        }

        public void DesativarProduto(int produtoId) // Alterado para receber ID para ser mais testável
        {
            this.mensagem = "";
            try
            {
                var produto = _context.Produtos.Find(produtoId);
                if (produto == null)
                {
                    this.mensagem = "Produto não encontrado para desativação.";
                    return;
                }
                produto.Ativo = false;
                _context.Produtos.Update(produto);
                _context.SaveChanges();
                this.mensagem = "Produto desativado com sucesso!";
            }
            catch (Exception ex)
            {
                this.mensagem = "Erro ao desativar o produto! " + ex.Message;
            }
        }

        public void EditarProduto(Produto produto)
        {
            this.mensagem = "";
            try
            {
                // Usa o _context em vez de criar um novo
                _context.Produtos.Update(produto);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao editar o produto!";
                return;
            }
        }

        public async Task<List<Produto>> ObterTodosOsProdutosAsync()
        {
            // Usa o _context em vez de criar um novo
            return await _context.Produtos.ToListAsync();
        }

        public async Task<List<Produto>> ObterProdutosAtivosAsync()
        {
            // Usa o _context em vez de criar um novo
            return await _context.Produtos
                .Include(p => p.Marca)
                .Include(p => p.Fornecedor)
                .Include(p => p.Categoria)
                .Include(p => p.PromocaoProdutos).ThenInclude(p => p.Promocao)
                .Where(u => u.Ativo == true).ToListAsync();
        }

        public async Task<List<Produto>> ObterProdutosComEstoqueBaixoAsync()
        {
            // Usa o _context em vez de criar um novo
            return await _context.Produtos
                .Include(p => p.Marca)
                .Include(p => p.Fornecedor)
                .Include(p => p.Categoria)
                .Where(p => p.EstoqueAtual < p.EstoqueMinimo)
                .ToListAsync();
        }
    }


}
