using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class MovimetacaoCaixaDao
    {
        public async Task<List<MovimentacaoCaixa>> ObterTodosAsMovimentacoesDoCaixaAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<MovimentacaoCaixa> todosAsMovimentacoesDoCaixa = await context.MovimentacaoCaixas.ToListAsync();

                return todosAsMovimentacoesDoCaixa;
            }
        }
    }
}
