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

            TestarConexao();

        }

        private async void TestarConexao()
        {
            // Substitua "SeuDbContext" pelo nome da sua classe de contexto
            using (var dbContext = new AppDbContext())
            {
                try
                {
                    // Tenta se conectar ao banco de dados
                    bool podeConectar = await dbContext.Database.CanConnectAsync();

                    if (podeConectar)
                    {
                        MessageBox.Show("Conexão com o banco de dados bem-sucedida!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // Este caso é raro, geralmente uma exceção é lançada primeiro
                        MessageBox.Show("Não foi possível conectar ao banco de dados (CanConnectAsync retornou false).", "Falha", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    // A exceção vai te dar a mensagem de erro detalhada
                    MessageBox.Show($"Erro ao conectar ao banco de dados: \n\n{ex.Message}", "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}