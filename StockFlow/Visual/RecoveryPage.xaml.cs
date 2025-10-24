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
    /// Interação lógica para RecoveryPage.xam
    /// </summary>
    public partial class RecoveryPage : Page
    {
        public RecoveryPage()
        {
            InitializeComponent();
        }

        private void Hyperlink_Login_Click(object sender, RoutedEventArgs e)
        {
            // Navega de volta para a LoginPage
            NavigationService.GetNavigationService(this)?.Navigate(new LoginPage());
        }
    }
}
