using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class VendaItemDao
    {
        private readonly AppDbContext _context;
        public VendaItemDao(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<VendaItem>> ObterTodasAsVendaItensAsync()
        {

            List<VendaItem> todasAsVendaItens = await _context.VendaItens.Include(vi => vi.Venda).ToListAsync();
            return todasAsVendaItens;
        }

        public async Task<List<VendaItem>> ObterTodasAsVendaItensPorVendaIdAsync(int vendaId)
        {

            List<VendaItem> todasAsVendaItens = await _context.VendaItens.Where(Venda => Venda.VendaId == vendaId).Include(p => p.Produto).ToListAsync();
            return todasAsVendaItens;
        }
    }
}

