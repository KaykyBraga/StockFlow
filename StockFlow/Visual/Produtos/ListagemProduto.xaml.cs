using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFlow.Visual.Produtos
{
    public class Produto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Quantidade { get; set; }
        public string Categoria { get; set; }
    }

    // ✅ CORREÇÃO AQUI: Adicionado ": UserControl"
    public partial class ListagemProduto : UserControl
    {
        private List<Produto> listaDeProdutos;

        public ListagemProduto()
        {
            InitializeComponent();
            CarregarProduto();
        }

        private void CarregarProduto()
        {
            listaDeProdutos = new List<Produto>
            {
                new Produto { Id = "1", Nome = "Ana Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "bna Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "cna Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "dna Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "ena Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "fna Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "gna Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "hna Silva", Quantidade = "12", Categoria = "Trator" },
                new Produto { Id = "1", Nome = "ina Silva", Quantidade = "12", Categoria = "Trator" }
            };
            DgProdutos.ItemsSource = listaDeProdutos;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Implemente aqui a lógica de filtragem da tabela.
            string termoBusca = TxtBusca.Text;
            MessageBox.Show($"Iniciando busca por: {termoBusca}");
        }

        private void Button_Click_Remover(object sender, RoutedEventArgs e)
        {
            Produto ProdutoParaRemover = (sender as Button).DataContext as Produto;
            if (ProdutoParaRemover != null)
            {
                MessageBoxResult resultado = MessageBox.Show($"Tem certeza que deseja remover o Produto? '{ProdutoParaRemover.Nome}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultado == MessageBoxResult.Yes)
                {
                    listaDeProdutos.Remove(ProdutoParaRemover);
                    // ✅ CORREÇÃO PARA O AVISO CS8600: Força a atualização da lista de forma segura
                    DgProdutos.ItemsSource = new List<Produto>(listaDeProdutos);
                    MessageBox.Show("Produto removido com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);

            if (navigationService != null)
            {
                // Limpa o conteúdo do Frame navegando para um URI nulo.
                // Isso simula o "fechamento" da página e deixa o Frame vazio.
                navigationService.Navigate(null as Uri);

                // Opcional: Se você quer ter certeza de que o histórico não guarda essa entrada de 'null':
                // navigationService.RemoveBackEntry();
            }
        }
    }
}
