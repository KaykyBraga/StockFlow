using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    /// <summary>
    /// Lógica de interação para Tela_AddFuncionario.xaml
    /// </summary>
    public partial class Tela_AddFuncionario : UserControl
    {
        public Tela_AddFuncionario()
        {
            InitializeComponent();
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

        private void Button_Click_Adicionar(object sender, RoutedEventArgs e)
        {
            // Captura os dados dos campos da tela
            string nome = TxtNomeCompleto.Text;
            string id = TxtIdFuncionario.Text;
            string email = TxtEmail.Text;
            string tipoFun = CmbTipoFuncionario.Text;

            // Pega a senha do campo que estiver visível
            string senha;
            if (ChkMostrarSenha.IsChecked == true)
            {
                senha = TxtSenhaVisivel.Text;
            }
            else
            {
                senha = TxtSenha.Password;
            }

            // Validação simples
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Pop-up de confirmação
            MessageBoxResult resultado = MessageBox.Show($"Deseja realmente cadastrar o funcionário '{nome}'?", "Confirmação de Cadastro", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                // Lógica para salvar no banco de dados...

                // Pop-up de sucesso
                MessageBox.Show($"Funcionário '{nome}' adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpar os campos
                TxtNomeCompleto.Clear();
                TxtIdFuncionario.Clear();
                TxtEmail.Clear();
                TxtSenha.Clear();
                TxtSenhaVisivel.Clear();
                ChkMostrarSenha.IsChecked = false;
               
            }
        }

        // ✅ INÍCIO DA CORREÇÃO: Métodos que estavam faltando
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
        // ✅ FIM DA CORREÇÃO
    }
}