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
    /// Interação lógica para LoginPage.xam
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }



        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para recuperação de senha
            NavigationService.GetNavigationService(this)?.Navigate(new RecoveryPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtUsuario.Text;
            string senha = TxtSenha.Password;

            //validar se nao tem campos vazios
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ControleUsuario controleUsuario = new ControleUsuario();
            controleUsuario.LoginUsuario(email, senha, out string TipoFuncionario);
            if (controleUsuario.mensagem == "")
            {
                if (TipoFuncionario == "Caixa")
                {
                    // Navegar para a tela de vendas
                }

                if(TipoFuncionario == "Gerente")
                {
                    // Navegar para a tela de Gerente
                }

                if(TipoFuncionario == "Estoquista")
                {
                    // Navegar para a tela de Estoquista
                }

            }
            else
            {
                MessageBox.Show(controleUsuario.mensagem, "Erro de Login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
