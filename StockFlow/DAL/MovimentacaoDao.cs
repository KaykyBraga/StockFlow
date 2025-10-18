using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class MovimentacaoDao
    {
        public string mensagem = "";
        public void CadastrarMovimentacao(Movimentacao movimentacao)
        {
            this.mensagem = "";
            try
            {
                var contexto = new AppDbContext();
                contexto.movimentacoes.Add(movimentacao);
                contexto.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao cadastrar a movimentação.";
            }
        }

        public async Task<List<Movimentacao>> ObterTodasAsMovimentacoesAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<Movimentacao> todosAsMovimentacoes = await context.movimentacoes.ToListAsync();

                return todosAsMovimentacoes;
            }
        }
    }
}
