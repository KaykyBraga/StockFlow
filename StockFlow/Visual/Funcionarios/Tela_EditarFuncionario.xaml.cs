using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    public partial class Tela_EditarFuncionario : UserControl
    {
        public Tela_EditarFuncionario(string funcionarioId)
        {
            InitializeComponent();
            CarregarDadosFuncionario(funcionarioId);
        }

        private void CarregarDadosFuncionario(string id)
        {
            // Simulação de carregamento de dados do banco
            TxtIdFuncionario.Text = id;
            TxtNomeCompleto.Text = "Nome do Funcionário Carregado";
            TxtEmail.Text = "email.do.banco@exemplo.com";
            // A senha não é carregada por segurança, o campo fica pronto para receber uma nova
        }

        private void Button_Click_Salvar(object sender, RoutedEventArgs e)
        {
            // ... (seu código para salvar continua o mesmo)
            string nome = TxtNomeCompleto.Text;
            MessageBox.Show($"Alterações para '{nome}' salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ✅ MÉTODO ADICIONADO PARA O BOTÃO DE REMOVER
        private void Button_Click_Remover(object sender, RoutedEventArgs e)
        {
            string id = TxtIdFuncionario.Text;
            string nome = TxtNomeCompleto.Text;

            // Pop-up de confirmação para uma ação destrutiva
            MessageBoxResult resultado = MessageBox.Show(
                $"TEM CERTEZA que deseja remover o funcionário '{nome}'?\n\nEsta ação não pode ser desfeita.",
                "Confirmar Remoção",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning // Ícone de aviso para indicar perigo
            );

            if (resultado == MessageBoxResult.Yes)
            {
                // AQUI, você chamaria a lógica para fazer o DELETE no banco de dados
                // Ex: FuncionarioController.Remover(id);

                MessageBox.Show("Funcionário removido com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpa a tela após a remoção
                TxtNomeCompleto.Clear();
                TxtIdFuncionario.Clear();
                TxtEmail.Clear();
                TxtSenha.Clear();
                TxtSenhaVisivel.Clear();
                ChkMostrarSenha.IsChecked = false;
            }
        }

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
    }
}