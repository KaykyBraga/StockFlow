using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class Categoria
    {
         public int CategoriaId { get; set; }
         public string NomeCategoria { get; set; }
        public bool Ativo { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }

        public Categoria()
        {
            Produtos = new HashSet<Produto>();
        }
    }
}
