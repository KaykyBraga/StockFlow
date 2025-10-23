using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            InitializeComponent();
            CarregarFuncionarios();
        }

        private void CarregarFuncionarios()
        {
            listaDeFuncionarios = new List<Funcionario>
            {
                new Funcionario { Id = "FUNC-001", Nome = "Ana Silva", Email = "ana.silva@empresa.com" },
                new Funcionario { Id = "FUNC-002", Nome = "Bruno Costa", Email = "bruno.costa@empresa.com" },
                new Funcionario { Id = "FUNC-003", Nome = "Carlos Pereira", Email = "carlos.p@empresa.com" },
                new Funcionario { Id = "FUNC-004", Nome = "Daniela Souza", Email = "daniela.souza@empresa.com" }
            };
            DgFuncionarios.ItemsSource = listaDeFuncionarios;
        }

        private void Button_Click_Editar(object sender, RoutedEventArgs e)
        {
            Funcionario funcionarioSelecionado = (sender as Button).DataContext as Funcionario;
            if (funcionarioSelecionado != null)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ContentFrame.Navigate(new Tela_EditarFuncionario(funcionarioSelecionado.Id));
            }
        }

        private void Button_Click_Remover(object sender, RoutedEventArgs e)
        {
            Funcionario funcionarioParaRemover = (sender as Button).DataContext as Funcionario;
            if (funcionarioParaRemover != null)
            {
                MessageBoxResult resultado = MessageBox.Show($"Tem certeza que deseja remover o funcionário '{funcionarioParaRemover.Nome}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultado == MessageBoxResult.Yes)
                {
                    listaDeFuncionarios.Remove(funcionarioParaRemover);
                    // ✅ CORREÇÃO PARA O AVISO CS8600: Força a atualização da lista de forma segura
                    DgFuncionarios.ItemsSource = new List<Funcionario>(listaDeFuncionarios);
                    MessageBox.Show("Funcionário removido com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}