using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class PromocaoProdutoDao
    {
        public string mensagemErro = "";
        private readonly AppDbContext _context;

        // O construtor agora recebe a instância do AppDbContext
        public PromocaoProdutoDao(AppDbContext context)
        {
            _context = context;
        }

        public void VincularPromocaoProduto(PromocaoProduto novaPromocaoProduto)
        {
            this.mensagemErro = ""; // Limpa a mensagem no início
            try
            {
                _context.PromocaoProdutos.Add(novaPromocaoProduto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Erro ao cadastrar a promoção do produto. Causa: " + ex.InnerException?.Message ?? ex.Message;
            }
        }

        public async Task<List<PromocaoProduto>> ObterTodosAsPromocaoProdutoAsync()
        {
            // Nota: O Include em 'ProdutoId' não é válido, pois é um int. 
            // A intenção provavelmente era 'Produto'. A lógica de busca continua a mesma.
            return await _context.PromocaoProdutos
                .Include(pp => pp.Promocao)
                .Include(pp => pp.Produto) // Corrigido de pp.ProdutoId para pp.Produto
                .ToListAsync();
        }
    }
}
