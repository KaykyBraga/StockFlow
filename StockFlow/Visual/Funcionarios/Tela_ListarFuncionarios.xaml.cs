using StockFlow.Controles;
using StockFlow.Visual.Produtos;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    public class Funcionario
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    // ✅ CORREÇÃO AQUI: Adicionado ": UserControl"
    public partial class Tela_ListarFuncionarios : UserControl
    {
        private List<Funcionario> listaDeFuncionarios;

        public Tela_ListarFuncionarios()
        {
            CarregarFuncionarios();

            InitializeComponent();
        }

        private async void CarregarFuncionarios()
        {
            ControleUsuario controleUsuario = new ControleUsuario();
            var listaUsuarios = await controleUsuario.ObterTodosOsUsuariosAtivosDataGridAsync();
            listaDeFuncionarios = new List<Funcionario>();
            listaDeFuncionarios = listaUsuarios;      
            DgFuncionarios.ItemsSource = listaDeFuncionarios;
        }

        private void Button_Click_Editar(object sender, RoutedEventArgs e)
        {
            Funcionario funcionarioSelecionado = (sender as Button).DataContext as Funcionario;

            if (funcionarioSelecionado == null)
            {
                MessageBox.Show("Não foi possível identificar o funcionário selecionado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ControleUsuario controleUsuario = new ControleUsuario();
            var funcionario = controleUsuario.BuscarUsuarioPorIdentificador(funcionarioSelecionado.Id);
            // 2. Pegar o ID do produto
            string idDoFuncionario = funcionario.IdentificadorFuncionario;
            string nomeDoFuncionario = funcionario.NomeCompleto;
            string emailDoFuncionario = funcionario.Email;
            string tipoDoFuncionario = funcionario.PerfilAcesso;

            // 3. Criar a nova página de edição, passando o ID para o construtor dela
            Tela_EditarFuncionario paginaEditar = new Tela_EditarFuncionario(idDoFuncionario, nomeDoFuncionario, emailDoFuncionario, tipoDoFuncionario);

            // 4. Navegar para a página
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            if (navigationService != null)
            {
                navigationService.Navigate(paginaEditar);
            }
        }

        private void Button_Click_Inativar(object sender, RoutedEventArgs e)
        {
            Funcionario funcionarioParaInativar = (sender as Button).DataContext as Funcionario;
            if (funcionarioParaInativar != null)
            {
                MessageBoxResult resultado = MessageBox.Show($"Tem certeza que deseja inativar o funcionário '{funcionarioParaInativar.Nome}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultado == MessageBoxResult.Yes)
                {
                    ControleUsuario controleUsuario = new ControleUsuario();
                    var funcionario = controleUsuario.BuscarUsuarioPorIdentificador(funcionarioParaInativar.Id);
                    controleUsuario.DesativarUsuario(funcionario.UsuarioId.ToString());
                    listaDeFuncionarios.Remove(funcionarioParaInativar);
                    // ✅ CORREÇÃO PARA O AVISO CS8600: Força a atualização da lista de forma segura
                    DgFuncionarios.ItemsSource = new List<Funcionario>(listaDeFuncionarios);
                    MessageBox.Show("Funcionário inativado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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

    }
}