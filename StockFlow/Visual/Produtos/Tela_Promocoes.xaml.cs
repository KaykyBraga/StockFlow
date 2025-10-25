using StockFlow.Controles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    public class Promocao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Desconto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Ativa { get; set; }
        public string Status => Ativa ? "Ativa" : "Inativa";

        // Lista para guardar os IDs dos produtos vinculados
        public List<string> ProdutoIds { get; set; } = new List<string>();
    }

    public partial class Tela_Promocoes : UserControl
    {
        private List<Promocao> listaDePromocoes;
        private int proximoId = 1;

        public Tela_Promocoes()
        {
            InitializeComponent();
            CarregarPromocoes();
        }

        private async Task CarregarPromocoes()
        {
            ControleVenda controleVenda = new ControleVenda();
            listaDePromocoes = new List<Promocao>();
            var listaPromocoes = await controleVenda.ObterTodasAsPromocoesParaDataGridAsync();
            listaDePromocoes = listaPromocoes;           

            DgPromocoes.ItemsSource = listaDePromocoes;
        }

        private void BtnCriar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNomePromocao.Text) || !decimal.TryParse(TxtDesconto.Text, out decimal desconto) || !DpDataInicio.SelectedDate.HasValue || !DpDataFim.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ControleVenda controleVenda = new ControleVenda(); 
            controleVenda.CriarPromocao(new List<string> { TxtNomePromocao.Text, desconto.ToString(), DpDataInicio.SelectedDate.Value.ToString("yyyy-MM-dd"), DpDataFim.SelectedDate.Value.ToString("yyyy-MM-dd") });
            var novaPromocao = new Promocao { Id = proximoId++, Nome = TxtNomePromocao.Text, Desconto = desconto, DataInicio = DpDataInicio.SelectedDate.Value, DataFim = DpDataFim.SelectedDate.Value, Ativa = true };
            listaDePromocoes.Add(novaPromocao);
            DgPromocoes.ItemsSource = new List<Promocao>(listaDePromocoes);
            TxtNomePromocao.Clear(); TxtDesconto.Clear(); DpDataInicio.SelectedDate = null; DpDataFim.SelectedDate = null;
            MessageBox.Show("Promoção criada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click_Vincular(object sender, RoutedEventArgs e)
        {
            Promocao promocaoSelecionada = (sender as Button).DataContext as Promocao;
            if (promocaoSelecionada != null)
            {
                // Cria uma instância da nova janela, passando a promoção selecionada
                VincularProdutos janelaVinculo = new VincularProdutos(promocaoSelecionada);

                // Abre a janela como um diálogo (bloqueia a janela principal até ser fechada)
                janelaVinculo.ShowDialog();
            }
        }

        private void Button_Click_Desativar(object sender, RoutedEventArgs e)
        {
            Promocao promocaoSelecionada = (sender as Button).DataContext as Promocao;
            ControleVenda controleVenda = new ControleVenda();
            string acao = promocaoSelecionada.Ativa ? "desativar" : "reativar";
            if(acao == "desativar")
            {
                controleVenda.DesativarPromocao(promocaoSelecionada.Id);
            }
            else
            {
                controleVenda.ReativarPromocao(promocaoSelecionada.Id);
            }
                MessageBoxResult resultado = MessageBox.Show($"Deseja realmente {acao} a promoção '{promocaoSelecionada.Nome}'?", "Confirmar Alteração", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                promocaoSelecionada.Ativa = !promocaoSelecionada.Ativa;
                ICollectionView view = System.Windows.Data.CollectionViewSource.GetDefaultView(DgPromocoes.ItemsSource);
                view.Refresh();
            }
        }

        private void Button_Click_Remover(object sender, RoutedEventArgs e)
        {
            Promocao promocaoParaRemover = (sender as Button).DataContext as Promocao;
            ControleVenda controleVenda = new ControleVenda();
            MessageBoxResult resultado = MessageBox.Show($"TEM CERTEZA que deseja remover permanentemente a promoção '{promocaoParaRemover.Nome}'? " , "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultado == MessageBoxResult.Yes)
            {
                controleVenda.RemoverPromocao(promocaoParaRemover.Id);
                listaDePromocoes.Remove(promocaoParaRemover);

                DgPromocoes.ItemsSource = new List<Promocao>(listaDePromocoes);
                MessageBox.Show("Promoção removida com sucesso." + controleVenda.mensagem, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DgPromocoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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