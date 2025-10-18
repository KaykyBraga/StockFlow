using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class MarcaDao
    {
        public string mensagem = "";

        public void CadastrarMarca(Marca marca)
        {
            
            this.mensagem = "";

            try
            {
                var contexto = new AppDbContext();
                contexto.Marcas.Add(marca);
                contexto.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao cadastrar marca!";
            }
        }

        public void DesativarMarca(Marca marca)
        {
            this.mensagem = "";
            marca.Ativo = false;
            try
            {
                var context = new AppDbContext();
                context.Marcas.Update(marca);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao deletar a Marca!";
                return;
            }
            this.mensagem = "Marca deletado com sucesso!";
        }

        public async Task<List<Marca>> ObterTodasAsMarcasAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<Marca> todosAsMarcas = await context.Marcas.ToListAsync();

                return todosAsMarcas;
            }
        }

        public async Task<List<Marca>> ObterMarcasAtivasAsync()
        {

            await using (var context = new AppDbContext())
            {

                List<Marca> marcasAtivas = await context.Marcas.Where(u => u.Ativo == true).ToListAsync();

                return marcasAtivas;
            }
        }
    }
}
