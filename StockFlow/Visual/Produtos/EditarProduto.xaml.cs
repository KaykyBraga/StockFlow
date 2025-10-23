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
    /// <summary>
    /// Interação lógica para EditarProduto.xam
    /// </summary>
    public partial class EditarProduto : Page
    {
        public EditarProduto()
        {
            InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Tenta voltar para a página anterior no Frame
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            // Se não puder voltar, limpa o Frame
            else
            {
                NavigationService.GetNavigationService(this)?.Navigate(null as Uri);
            }
        }
    }

}
