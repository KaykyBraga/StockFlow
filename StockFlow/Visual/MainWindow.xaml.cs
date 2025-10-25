using StockFlow.DAL;
using StockFlow.Visual;
using StockFlow.Visual.Produtos;
using StockFlow.Visual.Relatorio; // Adicionado para a nova tela de Caixa
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFlow
{
    public partial class MainWindow : Window
    {
        private string cargoDoUsuarioLogado;

        public MainWindow(string tipoFuncionario)
        {
            InitializeComponent();

            // 1. Armazena o cargo na variável da classe
            this.cargoDoUsuarioLogado = tipoFuncionario;

            // 2. Chama um método para fazer a verificação que você precisa
            ConfigurarVisibilidadeDosMenus();   
        }

        private void ConfigurarVisibilidadeDosMenus()
        {
            // Exemplo: Se você tem um botão no menu chamado 'BtnMenuFuncionarios'
            // e só o Gerente pode vê-lo.

            if (this.cargoDoUsuarioLogado == "Estoquista")
            {
                // O Estoquista não pode ver o menu de funcionários
                SubMenuFun.Visibility = Visibility.Collapsed;
                SubMenuRelat.Visibility = Visibility.Collapsed;
                // Ex: BtnMenuRelatorios.Visibility = Visibility.Collapsed;
            }
            else if (this.cargoDoUsuarioLogado == "Gerente")
            {
                // O Gerente pode ver tudo
                SubMenuFun.Visibility = Visibility.Visible;
                SubMenuRelat.Visibility = Visibility.Visible;
            }

            // Você também pode querer exibir o cargo em algum lugar
            // Ex: TxtBoasVindas.Text = $"Bem-vindo, {this.cargoDoUsuarioLogado}!";
        }

        private void btnResumoEstoque_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ResumoEstoquePage());
        }

        private void btnAlertas_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AlertasPage());
        }

        private void btnAddProduto_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CadastrarProduto());
        }

        private void btnListEstoque_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ListagemProduto());
        }

        private void btnAddFun_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_AddFuncionario());
        }

        private void btnListFun_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_ListarFuncionarios());
        }

        private void btnRelatVendas_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_RelatorioVendas());
        }

        private void btnRelatMoviment_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_RelatorioMovimentacao());
        }

        private void btnPromocoes_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Tela_Promocoes());
        }

        // ✅ MÉTODO ADICIONADO PARA O RELATÓRIO DE CAIXA
        private void btnRelatCaixa_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Caixa());
        }
    }
}