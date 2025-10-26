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
using StockFlow.Controles;
using StockFlow.Visual.Relatorio;

namespace StockFlow.Visual.Relatorio
{
    public class ItemVenda
    {
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal PrecoTotal { get; set; }
        public decimal DescontoTotal { get; set; }
    }
    /// <summary>
    /// Interação lógica para InfoVenda.xam
    /// </summary>
    public partial class InfoVenda : Page
    {
        List<ItemVenda> itensVenda;
        // Construtor padrão (pode ser removido se você SEMPRE for passar um ID)
        public InfoVenda()
        {
            InitializeComponent();
        }

        // NOVO CONSTRUTOR: Este é o que recebe o ID da tela anterior
        public InfoVenda(string idVenda, string nomeFuncionario, string metodoDePagamento)
        {
            InitializeComponent();

            // Agora você tem o ID. Chame um método para carregar os dados.
            CarregarDadosVenda(idVenda, nomeFuncionario, metodoDePagamento);
        }

        private async void CarregarDadosVenda(string idVenda, string nomeFuncionario, string metodoDePagamento)
        {
            
            txtIdVenda.Text = $"{idVenda}";
            txtFuncionario.Text = $"{nomeFuncionario}";
            txtMetodoPgto.Text = $"{metodoDePagamento}";
            ControleVenda controleVenda = new ControleVenda();
            var carregamento = await controleVenda.ObterTodasAsVendasItemParaGridAsync(int.Parse(idVenda));
            itensVenda = carregamento;
            decimal totalVenda = 0;
            foreach (var item in itensVenda)
            {
                totalVenda += item.PrecoTotal;
            }
            txtTotalVenda.Text = $"R$ {totalVenda:F2}";
            decimal descontoTotal = 0;
            foreach (var item in itensVenda)
            {
                descontoTotal += item.DescontoTotal;
            }
            txtDesconto.Text = $"R$ {descontoTotal:F2}";
            decimal subtotalVenda = totalVenda + descontoTotal;
            txtSubtotal.Text = $"R$ {subtotalVenda:F2}";

            DgItensVenda.ItemsSource = itensVenda;
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
