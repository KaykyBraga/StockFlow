using StockFlow.Visual.Relatorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using StockFlow.Visual.Relatorio;

namespace StockFlow.Visual
{
    // Classe auxiliar para representar uma venda
    public class Venda
    {
        public string IdVenda { get; set; }
        public DateTime Data { get; set; }
        public string NomeFuncionario { get; set; }        
        public int QuantidadeItens { get; set; }
        public decimal ValorTotal { get; set; }
    }

    // ✅ A CORREÇÃO É AQUI: Adicionar ": UserControl"
    public partial class Tela_RelatorioVendas : UserControl
    {
        private List<Venda> todasAsVendas;

        public Tela_RelatorioVendas()
        {
            InitializeComponent();
            CarregarDadosIniciais();
        }

        private void CarregarDadosIniciais()
        {
            // --- SIMULAÇÃO ---
            var funcionarios = new List<string> { "Todos", "Ana Silva", "Bruno Costa", "Carlos Pereira" };
            CboFuncionarios.ItemsSource = funcionarios;
            CboFuncionarios.SelectedIndex = 0;

            todasAsVendas = new List<Venda>
            {
                new Venda { IdVenda = "VEN-001", Data = new DateTime(2025, 10, 20, 10, 30, 0), NomeFuncionario = "Ana Silva", QuantidadeItens = 3, ValorTotal = 150.75m },
                new Venda { IdVenda = "VEN-002", Data = new DateTime(2025, 10, 20, 14, 0, 0), NomeFuncionario = "Bruno Costa", QuantidadeItens = 1, ValorTotal = 89.90m },
                new Venda { IdVenda = "VEN-003", Data = new DateTime(2025, 10, 21, 9, 15, 0), NomeFuncionario = "Ana Silva", QuantidadeItens = 5, ValorTotal = 320.00m },
                new Venda { IdVenda = "VEN-004", Data = new DateTime(2025, 10, 21, 11, 45, 0), NomeFuncionario = "Carlos Pereira", QuantidadeItens = 2, ValorTotal = 110.50m },
                new Venda { IdVenda = "VEN-005", Data = new DateTime(2025, 10, 19, 16, 20, 0), NomeFuncionario = "Bruno Costa", QuantidadeItens = 8, ValorTotal = 540.20m }
            };

            DgVendas.ItemsSource = todasAsVendas;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Venda> vendasFiltradas = todasAsVendas;

            if (DpDataInicio.SelectedDate.HasValue)
            {
                vendasFiltradas = vendasFiltradas.Where(v => v.Data.Date >= DpDataInicio.SelectedDate.Value.Date);
            }

            if (DpDataFim.SelectedDate.HasValue)
            {
                vendasFiltradas = vendasFiltradas.Where(v => v.Data.Date <= DpDataFim.SelectedDate.Value.Date);
            }

            if (CboFuncionarios.SelectedItem != null && CboFuncionarios.SelectedItem.ToString() != "Todos")
            {
                string funcionarioSelecionado = CboFuncionarios.SelectedItem.ToString();
                vendasFiltradas = vendasFiltradas.Where(v => v.NomeFuncionario == funcionarioSelecionado);
            }

            DgVendas.ItemsSource = vendasFiltradas.ToList();
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

        private void BtnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Verificar se um item está realmente selecionado
            if (DgVendas.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma venda na tabela para visualizar.",
                                "Nenhuma Venda Selecionada",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // 2. Obter o objeto "Venda" da linha selecionada
            Venda vendaSelecionada = (Venda)DgVendas.SelectedItem;
            string idDaVenda = vendaSelecionada.IdVenda;

            // 3. Criar a instância da página de InfoVenda, passando o ID para o construtor dela
            InfoVenda paginaDetalhes = new InfoVenda(idDaVenda);

            // 4. Navegar para a nova página
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            if (navigationService != null)
            {
                navigationService.Navigate(paginaDetalhes);
            }
        }
    }
}