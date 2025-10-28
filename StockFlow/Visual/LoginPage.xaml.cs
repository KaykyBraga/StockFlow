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

            // Aqui o 'tipoFuncionario' receberá "Vendedor", "Gerente" ou "Estoquista"
            controleUsuario.LoginUsuario(email, senha, out string tipoFuncionario);

            if (controleUsuario.mensagem == "")
            {
                // ===============================================
                // CENÁRIO 1: VENDEDOR (Mudei de "Caixa" para "Vendedor")
                // ===============================================
                if (tipoFuncionario == "Vendedor")
                {
                    // Navega para a Tela_Vendas (que é uma Page)
                    Tela_Vendas tela_Vendas = new Tela_Vendas();
                    tela_Vendas.Show();
                    Window.GetWindow(this).Close();
                }

                // ===============================================
                // CENÁRIO 2: GERENTE
                // ===============================================
                else if (tipoFuncionario == "Gerente")
                {
                    // 1. Cria a MainWindow passando o cargo "Gerente"
                    MainWindow menu = new MainWindow(tipoFuncionario);

                    // 2. Mostra a Janela Principal
                    menu.Show();

                    // 3. Fecha a janela de Login atual
                    Window.GetWindow(this).Close();
                }

                // ===============================================
                // CENÁRIO 3: ESTOQUISTA
                // ===============================================
                else if (tipoFuncionario == "Estoquista")
                {
                    // 1. Cria a MainWindow passando o cargo "Estoquista"
                    MainWindow menu = new MainWindow(tipoFuncionario);

                    // 2. Mostra a Janela Principal
                    menu.Show();

                    // 3. Fecha a janela de Login atual
                    Window.GetWindow(this).Close();
                }
                else// (Opcional) Caso o login retorne um tipo não esperado
                {
                    MessageBox.Show($"Tipo de funcionário '{tipoFuncionario}' não reconhecido.", "Erro de Login", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if(controleUsuario.mensagem != "")
                {
                    MessageBox.Show(controleUsuario.mensagem, "Erro de Login", MessageBoxButton.OK, MessageBoxImage.Error);
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