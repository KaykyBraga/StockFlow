using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class CategoriaDao
    {
        public string mensagem = "";

        public void CadastrarCategoria(Categoria categoria)
        {
            this.mensagem = "";
            try
            {
                var contexto = new AppDbContext();
                contexto.Categorias.Add(categoria);
                contexto.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao cadastrar categoria!";
            }
        }


        public void DesativarCategoria(Categoria categoria)
        {
            this.mensagem = "";
            categoria.Ativo = false;
            try
            {
                var context = new AppDbContext();
                context.Categorias.Update(categoria);
                context.SaveChanges();
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao desativar a categoria!";
                return;
            }
            this.mensagem = "Categoria desativada com sucesso!";
        }

        public async Task<List<Categoria>> ObterTodasAsCategoriasAsync()
        {
            await using (var context = new AppDbContext())
            {
                
                List<Categoria> todosAsCategorias = await context.Categorias.ToListAsync();

                return todosAsCategorias;
            }
        }

        public async Task<List<Categoria>> ObterCategoriasAtivasAsync()
        {

            await using (var context = new AppDbContext())
            {

                List<Categoria> CategoriasAtivas = await context.Categorias.Where(u => u.Ativo == true).ToListAsync();

                return CategoriasAtivas;
            }
        }
    } 
}

