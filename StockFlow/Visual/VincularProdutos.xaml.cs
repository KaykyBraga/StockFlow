using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StockFlow.Visual
{
    // ... (As classes Produto e ProdutoVinculado continuam as mesmas)
    public class Produto
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";
    }
    public class ProdutoVinculado
    {
        public Produto Produto { get; set; } = new Produto();
        public bool IsVinculado { get; set; }
    }


    // ✅ NOME DA CLASSE CORRIGIDO
    public partial class VincularProdutos : Window
    {
        private Promocao promocaoAlvo;
        private List<ProdutoVinculado> listaProdutosVinculados;

        // ✅ NOME DO CONSTRUTOR CORRIGIDO
        public VincularProdutos(Promocao promocao)
        {
            InitializeComponent();
            promocaoAlvo = promocao;
            LblNomePromocao.Text = promocao.Nome;
            CarregarProdutos();
        }

        private void CarregarProdutos()
        {
            // ... (o resto do código continua igual)
            var todosOsProdutos = new List<Produto>
            {
                new Produto { Id = "PROD-001", Nome = "Notebook Gamer X" },
                new Produto { Id = "PROD-002", Nome = "Mouse Óptico Sem Fio" },
                new Produto { Id = "PROD-003", Nome = "Teclado Mecânico RGB" },
                new Produto { Id = "PROD-004", Nome = "Monitor 27' 4K" },
                new Produto { Id = "PROD-005", Nome = "Headset Gamer 7.1" }
            };

            listaProdutosVinculados = new List<ProdutoVinculado>();
            foreach (var produto in todosOsProdutos)
            {
                listaProdutosVinculados.Add(new ProdutoVinculado
                {
                    Produto = produto,
                    IsVinculado = promocaoAlvo.ProdutoIds.Contains(produto.Id)
                });
            }
            DgProdutos.ItemsSource = listaProdutosVinculados;
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            promocaoAlvo.ProdutoIds.Clear();
            foreach (var item in listaProdutosVinculados)
            {
                if (item.IsVinculado)
                {
                    promocaoAlvo.ProdutoIds.Add(item.Produto.Id);
                }
            }
            MessageBox.Show("Produtos vinculados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}