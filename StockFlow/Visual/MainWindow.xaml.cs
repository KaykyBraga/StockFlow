using StockFlow.DAL;
using StockFlow.Visual;
using StockFlow.Visual.Produtos;
using StockFlow.Visual.Relatorio; // Adicionado para a nova tela de Caixa
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFlow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnResumoEstoque_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ResumoEstoquePage());
        }

        private void btnAlertas_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AlertasPage());
        }

        private void btnAddProduto_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CadastrarProduto());
        }

        private void btnListEstoque_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ListagemProduto());
        }

        private void btnAddFun_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_AddFuncionario());
        }

        private void btnListFun_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_ListarFuncionarios());
        }

        private void btnRelatVendas_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_RelatorioVendas());
        }

        private void btnRelatMoviment_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_RelatorioMovimentacao());
        }

        private void btnPromocoes_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_Promocoes());
        }

        // ✅ MÉTODO ADICIONADO PARA O RELATÓRIO DE CAIXA
        private void btnRelatCaixa_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Caixa());
        }
    }
}