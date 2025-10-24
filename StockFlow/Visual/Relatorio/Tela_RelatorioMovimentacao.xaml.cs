using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual
{
    // Classe auxiliar para representar uma movimentação
    public class MovimentacaoEstoque
    {
        public DateTime Data { get; set; }
        public string NomeProduto { get; set; }
        public string Tipo { get; set; } // "Entrada" ou "Saída"
        public int Quantidade { get; set; }
        public string NomeFuncionario { get; set; }
        public string Observacao { get; set; }
    }

    public partial class Tela_RelatorioMovimentacao : UserControl
    {
        private List<MovimentacaoEstoque> todasAsMovimentacoes;

        public Tela_RelatorioMovimentacao()
        {
            InitializeComponent();
            CarregarDadosIniciais();
        }

        private void CarregarDadosIniciais()
        {
            // --- SIMULAÇÃO ---
            // Carrega opções para os filtros
            CboTipoMovimentacao.ItemsSource = new List<string> { "Todos", "Entrada", "Saída" };
            CboTipoMovimentacao.SelectedIndex = 0;

            CboFuncionarios.ItemsSource = new List<string> { "Todos", "Ana Silva", "Bruno Costa", "Carlos Pereira" };
            CboFuncionarios.SelectedIndex = 0;

            // Carrega a lista completa de movimentações (simulação do banco)
            todasAsMovimentacoes = new List<MovimentacaoEstoque>
            {
                new MovimentacaoEstoque { Data = new DateTime(2025, 10, 20, 9, 0, 0), NomeProduto = "Notebook Gamer X", Tipo = "Entrada", Quantidade = 10, NomeFuncionario = "Ana Silva", Observacao = "Recebimento do fornecedor TecMaster." },
                new MovimentacaoEstoque { Data = new DateTime(2025, 10, 20, 10, 35, 0), NomeProduto = "Notebook Gamer X", Tipo = "Saída", Quantidade = 1, NomeFuncionario = "Ana Silva", Observacao = "Venda #VEN-001" },
                new MovimentacaoEstoque { Data = new DateTime(2025, 10, 20, 14, 5, 0), NomeProduto = "Mouse Óptico Sem Fio", Tipo = "Saída", Quantidade = 1, NomeFuncionario = "Bruno Costa", Observacao = "Venda #VEN-002" },
                new MovimentacaoEstoque { Data = new DateTime(2025, 10, 21, 8, 30, 0), NomeProduto = "Teclado Mecânico RGB", Tipo = "Entrada", Quantidade = 20, NomeFuncionario = "Carlos Pereira", Observacao = "Recebimento do fornecedor CompShop." },
                new MovimentacaoEstoque { Data = new DateTime(2025, 10, 21, 9, 20, 0), NomeProduto = "Notebook Gamer X", Tipo = "Saída", Quantidade = 2, NomeFuncionario = "Ana Silva", Observacao = "Venda #VEN-003" }
            };

            // Exibe todas as movimentações inicialmente
            DgMovimentacoes.ItemsSource = todasAsMovimentacoes;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<MovimentacaoEstoque> movimentacoesFiltradas = todasAsMovimentacoes;

            // Filtro por Data de Início
            if (DpDataInicio.SelectedDate.HasValue)
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.Data.Date >= DpDataInicio.SelectedDate.Value.Date);
            }

            // Filtro por Data de Fim
            if (DpDataFim.SelectedDate.HasValue)
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.Data.Date <= DpDataFim.SelectedDate.Value.Date);
            }

            // Filtro por Tipo de Movimentação
            if (CboTipoMovimentacao.SelectedItem?.ToString() != "Todos")
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.Tipo == CboTipoMovimentacao.SelectedItem.ToString());
            }

            // Filtro por Funcionário
            if (CboFuncionarios.SelectedItem?.ToString() != "Todos")
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.NomeFuncionario == CboFuncionarios.SelectedItem.ToString());
            }

            DgMovimentacoes.ItemsSource = movimentacoesFiltradas.ToList();
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