using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    public partial class Tela_EditarFuncionario : UserControl
    {
        private string funcionarioId;
        public Tela_EditarFuncionario(string idFuncionario)
        {
            InitializeComponent();
            this.funcionarioId = idFuncionario;
            CarregarDadosFuncionario();
        }

        private void CarregarDadosFuncionario()
        {
           TxtIdFuncionario.Text = this.funcionarioId;



        }

        private void Button_Click_Salvar(object sender, RoutedEventArgs e)
        {
            // ... (seu código para salvar continua o mesmo)
            string nome = TxtNomeCompleto.Text;
            MessageBox.Show($"Alterações para '{nome}' salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ✅ MÉTODO ADICIONADO PARA O BOTÃO DE REMOVER
        

        // Métodos para mostrar/esconder senha (continuam os mesmos)
        private void ChkMostrarSenha_Checked(object sender, RoutedEventArgs e)
        {
            TxtSenhaVisivel.Text = TxtSenha.Password;
            TxtSenhaVisivel.Visibility = Visibility.Visible;
            TxtSenha.Visibility = Visibility.Collapsed;
        }

        private void ChkMostrarSenha_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtSenha.Password = TxtSenhaVisivel.Text;
            TxtSenha.Visibility = Visibility.Visible;
            TxtSenhaVisivel.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtém o NavigationService associado ao controle atual
            NavigationService navigationService = NavigationService.GetNavigationService(this);

            if (navigationService != null && navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
            else
            {
                navigationService?.Navigate(null as Uri);
            }
        }

        private void Button_Click_Limpar(object sender, RoutedEventArgs e)
        {
            TxtNomeCompleto.Text = string.Empty;
            TxtIdFuncionario.Text = string.Empty;
            TxtEmail.Text = string.Empty;
            TxtSenhaVisivel.Text = string.Empty;
        }
    }
}