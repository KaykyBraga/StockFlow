using StockFlow.Controles;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    public partial class Tela_EditarFuncionario : UserControl
    {
        private string funcionarioId;
        private string funcionarioName;
        private string funcionarioEmail;
        private string funcionarioTipo;
        public Tela_EditarFuncionario(string idFuncionario, string funcionarioName, string funcionarioEmail, string funcionarioTipo)
        {
            InitializeComponent();
            this.funcionarioId = idFuncionario;
            this.funcionarioName = funcionarioName;
            this.funcionarioEmail = funcionarioEmail;
            this.funcionarioTipo = funcionarioTipo;
            CarregarDadosFuncionario();
        }

        private void CarregarDadosFuncionario()
        {
            TxtIdFuncionario.Text = this.funcionarioId;
            TxtEmail.Text = this.funcionarioEmail;
            TxtNomeCompleto.Text = this.funcionarioName;
            CmbTipoFuncionario.Text = this.funcionarioTipo;


        }

        private void Button_Click_Salvar(object sender, RoutedEventArgs e)
        {
            // ... (seu código para salvar continua o mesmo)
            if (string.IsNullOrWhiteSpace(TxtNomeCompleto.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                CmbTipoFuncionario.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ControleUsuario controleUsuario = new ControleUsuario();
            var usuarioExistente = controleUsuario.BuscarUsuarioPorIdentificador(TxtIdFuncionario.Text);
            controleUsuario.EditarUsuario(new System.Collections.Generic.List<string>
            {
                TxtNomeCompleto.Text,
                TxtEmail.Text,
                CmbTipoFuncionario.Text,
                usuarioExistente.IdentificadorFuncionario,
                usuarioExistente.Ativo.ToString(),
                usuarioExistente.UsuarioId.ToString(),
                usuarioExistente.DataCadastro.ToString("o"),
                usuarioExistente.SenhaHash

            });
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
            CmbTipoFuncionario.SelectedIndex = -1;
        }
    }
}