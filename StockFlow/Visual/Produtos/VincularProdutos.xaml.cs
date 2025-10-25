using StockFlow.Controles;
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

        private async void CarregarProdutos()
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            var listaProdutos = await controleEstoque.ObterTodosOsProdutosAtivosAsync();
            var todosOsProdutos = new List<Produto>();
            foreach (var produto in listaProdutos)
            {
                todosOsProdutos.Add(new Produto
                {
                    Id = produto.ProdutoId.ToString(),
                    Nome = produto.NomeCompleto
                });
            }

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
            ControleVenda controleVenda = new ControleVenda();
            foreach (var item in listaProdutosVinculados)
            {
                controleVenda.DesvincularPromocaoProduto(promocaoAlvo.Id, int.Parse(item.Produto.Id));
            }
            promocaoAlvo.ProdutoIds.Clear();
            foreach (var item in listaProdutosVinculados)
            {
                if (item.IsVinculado)
                {                 
                        controleVenda.VincularPromocaoProduto(promocaoAlvo.Id, int.Parse(item.Produto.Id));
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