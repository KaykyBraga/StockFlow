using StockFlow.DAL;
using StockFlow.Visual;
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
            // ContentFrame.Navigate(new ResumoEstoquePage()); // Removido para evitar erro de classe inexistente
        }

        private void btnAlertas_Click(object sender, RoutedEventArgs e)
        {
            // ContentFrame.Navigate(new AlertasPage()); // Removido para evitar erro de classe inexistente
        }

        private void btnAddFun_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_AddFuncionario());
        }

        private void btnEditFun_Click(object sender, RoutedEventArgs e)
        {
            string idDoFuncionarioParaEditar = "FUNC-123";
            ContentFrame.Navigate(new Tela_EditarFuncionario(idDoFuncionarioParaEditar));
        }

        private void btnListFun_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_ListarFuncionarios());
        }

        // ✅ CORREÇÃO AQUI: O método duplicado foi removido. Apenas uma versão existe agora.
        private void btnRelatVendas_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_RelatorioVendas());
        }
    }
}