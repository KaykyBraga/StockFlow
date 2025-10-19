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
        public void VincularPromocaoProduto(Modelo.PromocaoProduto novaPromocaoProduto)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // Lógica para adicionar a nova promoção ao banco de dados
                    context.PromocaoProdutos.Add(novaPromocaoProduto);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Exception innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }
                this.mensagemErro = "Erro ao cadastrar a promoção do produto. Causa: " + innerEx.Message;
                return;
            }
        }

        public async Task<List<PromocaoProduto>> ObterTodosAsPromocaoProdutoAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<PromocaoProduto> todosAsPromocaoProdutos = await context.PromocaoProdutos.Include(pp => pp.Promocao).Include(pp => pp.ProdutoId).ToListAsync();

                return todosAsPromocaoProdutos;
            }
        }
    }
}
