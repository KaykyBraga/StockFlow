using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class MovimentacaoCaixaDao
    {
        private readonly AppDbContext _context;

        // O construtor agora recebe a instância do AppDbContext
        public MovimentacaoCaixaDao(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovimentacaoCaixa>> ObterTodosAsMovimentacoesDoCaixaAsync()
        {
            return await _context.MovimentacaoCaixas.ToListAsync();
        }
    }
}
