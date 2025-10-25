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
using StockFlow.Visual.Relatorio;

namespace StockFlow.Visual.Relatorio
{
    /// <summary>
    /// Interação lógica para InfoVenda.xam
    /// </summary>
    public partial class InfoVenda : Page
    {
        // Construtor padrão (pode ser removido se você SEMPRE for passar um ID)
        public InfoVenda()
        {
            InitializeComponent();
        }

        // NOVO CONSTRUTOR: Este é o que recebe o ID da tela anterior
        public InfoVenda(string idVenda)
        {
            InitializeComponent();

            // Agora você tem o ID. Chame um método para carregar os dados.
            CarregarDadosVenda(idVenda);
        }

        private void CarregarDadosVenda(string idVenda)
        {
            // É AQUI QUE VOCÊ DEVE FAZER A LÓGICA:
            // 1. Usar o 'idVenda' para buscar os detalhes no banco de dados.
            // 2. Preencher os TextBlocks e o DataGrid que criamos no XAML.

            // Exemplo (usando os nomes do XAML que eu te passei):
            // VendaDetalhada venda = seuBancoDeDados.BuscarVendaPorId(idVenda);
            // List<ItemVenda> itens = seuBancoDeDados.BuscarItensDaVenda(idVenda);

            // txtIdVenda.Text = venda.IdVenda;
            // txtDataVenda.Text = venda.Data.ToString("g");
            // txtFuncionario.Text = venda.NomeFuncionario;
            // txtMetodoPgto.Text = venda.MetodoPagamento;
            //
            // DgItensVenda.ItemsSource = itens;
            //
            // txtSubtotal.Text = venda.Subtotal.ToString("C");
            // txtDesconto.Text = venda.Desconto.ToString("C");
            // txtTotalVenda.Text = venda.TotalPago.ToString("C");


            // Por enquanto, vamos só exibir o ID no TextBlock para confirmar que funcionou
            txtIdVenda.Text = $"{idVenda}";
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
