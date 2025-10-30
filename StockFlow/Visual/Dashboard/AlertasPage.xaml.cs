using StockFlow.Controles;
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

namespace StockFlow.Visual
{
    /// <summary>
    /// Interação lógica para AlertasPage.xam
    /// </summary>
    
    public class Alertagrid
    {
        public string EstadoDeAtencao { get; set; }
        public string PrecisaDeReposicao { get; set; }

    }
    public partial class AlertasPage : Page
    {
        public AlertasPage()
        {
            Carregar();
            InitializeComponent();
        }

        private async void Carregar()
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            var listaAtencao = await controleEstoque.ObterTodosOsProdutosParaGridAlertaAtencaoAsync();
            var listaReposicao = await controleEstoque.ObterTodosOsProdutosParaGridAlertaReposicaoAsync();

            DgAtencao.ItemsSource = listaAtencao;
            DgReposicao.ItemsSource = listaReposicao;
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
