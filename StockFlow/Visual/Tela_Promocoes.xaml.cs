using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockFlow.Visual
{
    public class Promocao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Desconto { get; set; }
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

        private void CarregarPromocoes()
        {
            // Simulação de dados
            listaDePromocoes = new List<Promocao>
            {
                new Promocao { Id = proximoId++, Nome = "Queima de Estoque de Notebooks", Desconto = 15, DataInicio = DateTime.Now.AddDays(-5), DataFim = DateTime.Now.AddDays(10), Ativa = true },
                new Promocao { Id = proximoId++, Nome = "Promoção de Mouses e Teclados", Desconto = 20, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), Ativa = true },
                new Promocao { Id = proximoId++, Nome = "Promoção de Aniversário (Encerrada)", Desconto = 10, DataInicio = DateTime.Now.AddDays(-40), DataFim = DateTime.Now.AddDays(-10), Ativa = false }
            };

            DgPromocoes.ItemsSource = listaDePromocoes;
        }

        private void BtnCriar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNomePromocao.Text) || !double.TryParse(TxtDesconto.Text, out double desconto) || !DpDataInicio.SelectedDate.HasValue || !DpDataFim.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

            string acao = promocaoSelecionada.Ativa ? "desativar" : "reativar";
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
            MessageBoxResult resultado = MessageBox.Show($"TEM CERTEZA que deseja remover permanentemente a promoção '{promocaoParaRemover.Nome}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultado == MessageBoxResult.Yes)
            {
                listaDePromocoes.Remove(promocaoParaRemover);
                DgPromocoes.ItemsSource = new List<Promocao>(listaDePromocoes);
                MessageBox.Show("Promoção removida com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}