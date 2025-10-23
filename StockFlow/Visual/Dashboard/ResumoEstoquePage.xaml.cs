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
    /// Interação lógica para ResumoEstoquePage.xam
    /// </summary>


    public class ProdutoResumo
    {
        public required string Produto { get; set; } // Resolvido o aviso CS8618
        public int Quantidade { get; set; }
        public required string Status { get; set; }      // Resolvido o aviso CS8618
    }

    public partial class ResumoEstoquePage : Page
    {
        public ResumoEstoquePage()
        {
            InitializeComponent();

            // Carrega os dados simulados no DataGrid
            List<ProdutoResumo> produtos = new List<ProdutoResumo>
            {
                // Os dados agora são inicializados corretamente com as propriedades 'required'
                new ProdutoResumo { Produto = "Monitor Gamer 27'", Quantidade = 50, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Teclado Mecânico RGB", Quantidade = 120, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Mouse Sem Fio Ultra", Quantidade = 15, Status = "Baixo Estoque" },
                new ProdutoResumo { Produto = "Webcam Full HD", Quantidade = 5, Status = "Crítico" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" },
                new ProdutoResumo { Produto = "Fone de Ouvido BT", Quantidade = 80, Status = "Em Estoque" }
            };
            DgEstoque.ItemsSource = produtos;
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

