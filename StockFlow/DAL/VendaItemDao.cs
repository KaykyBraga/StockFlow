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
        public async Task<List<VendaItem>> ObterTodasAsVendaItensAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<VendaItem> todasAsVendaItens = await context.VendaItens.Include(vi => vi.Venda).ToListAsync();

                return todasAsVendaItens;
            }
        }
    }
}
