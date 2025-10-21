using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    // Classe auxiliar para representar os dados de um funcionário
    public class Funcionario
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public partial class Tela_ListarFuncionarios : UserControl
    {
        // Lista para guardar os funcionários (simulando um banco de dados)
        private List<Funcionario> listaDeFuncionarios;

        public Tela_ListarFuncionarios()
        {
            InitializeComponent();
            CarregarFuncionarios();
        }

        private void CarregarFuncionarios()
        {
            // --- SIMULAÇÃO ---
            // Aqui você faria a busca no seu banco de dados para popular a lista.
            listaDeFuncionarios = new List<Funcionario>
            {
                new Funcionario { Id = "FUNC-001", Nome = "Ana Silva", Email = "ana.silva@empresa.com" },
                new Funcionario { Id = "FUNC-002", Nome = "Bruno Costa", Email = "bruno.costa@empresa.com" },
                new Funcionario { Id = "FUNC-003", Nome = "Carlos Pereira", Email = "carlos.p@empresa.com" },
                new Funcionario { Id = "FUNC-004", Nome = "Daniela Souza", Email = "daniela.souza@empresa.com" }
            };

            // Atribui a lista de funcionários ao DataGrid para exibição
            DgFuncionarios.ItemsSource = listaDeFuncionarios;
        }

        private void Button_Click_Editar(object sender, RoutedEventArgs e)
        {
            // Pega o funcionário da linha em que o botão foi clicado
            Funcionario funcionarioSelecionado = (sender as Button).DataContext as Funcionario;

            if (funcionarioSelecionado != null)
            {
                // Navega para a tela de edição, passando o ID do funcionário selecionado
                NavigationService.GetNavigationService(this).Navigate(new Tela_EditarFuncionario(funcionarioSelecionado.Id));
            }
        }

        private void Button_Click_Remover(object sender, RoutedEventArgs e)
        {
            // Pega o funcionário da linha em que o botão foi clicado
            Funcionario funcionarioParaRemover = (sender as Button).DataContext as Funcionario;

            if (funcionarioParaRemover != null)
            {
                MessageBoxResult resultado = MessageBox.Show(
                    $"Tem certeza que deseja remover o funcionário '{funcionarioParaRemover.Nome}'?",
                    "Confirmar Remoção",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    // --- SIMULAÇÃO ---
                    // Aqui você chamaria a lógica para remover do banco de dados.
                    // Ex: FuncionarioController.Remover(funcionarioParaRemover.Id);

                    // Remove da lista local e atualiza a tela
                    listaDeFuncionarios.Remove(funcionarioParaRemover);
                    DgFuncionarios.ItemsSource = null; // Limpa a fonte de dados
                    DgFuncionarios.ItemsSource = listaDeFuncionarios; // Reatribui a lista atualizada

                    MessageBox.Show("Funcionário removido com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}