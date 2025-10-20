using StockFlow.DAL;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

    }
}