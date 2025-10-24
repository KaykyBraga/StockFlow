using StockFlow.Controles;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtEmail.Text;
            string novaSenha = TxtNovaSenha.Text;
            string idFuncionario = txtIdFuncionario.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(idFuncionario))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Campos Incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ControleUsuario controleUsuario = new ControleUsuario();
            controleUsuario.RedefinirSenha(email, idFuncionario, novaSenha);
            
            if(controleUsuario.mensagem == "")
            {
                MessageBox.Show("Senha redefinida com sucesso!", "Redefinição de Senha", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GetNavigationService(this)?.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show(controleUsuario.mensagem, "Redefinição de Senha", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
