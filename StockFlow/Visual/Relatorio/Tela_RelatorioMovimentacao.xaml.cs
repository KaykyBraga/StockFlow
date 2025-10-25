using StockFlow.Controles;
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

        private async void CarregarDadosIniciais()
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            var carregamento = await controleEstoque.ObterTodasAsMovimentacoesParaOGridAsync();
            List<string> tiposMovimentacao = new List<string>();
            List<string> nomesFuncionarios = new List<string>();
            tiposMovimentacao.Add("Todos");
            nomesFuncionarios.Add("Todos");
            foreach (var tipos in carregamento)
            {
                if (!tiposMovimentacao.Contains(tipos.Tipo))
                    tiposMovimentacao.Add(tipos.Tipo);
                
            }
            foreach (var nome in carregamento)
            {
                if (!nomesFuncionarios.Contains(nome.NomeFuncionario))
                    nomesFuncionarios.Add(nome.NomeFuncionario);
            }
            // --- SIMULAÇÃO ---
            // Carrega opções para os filtros
            CboTipoMovimentacao.ItemsSource = tiposMovimentacao;
            CboTipoMovimentacao.SelectedIndex = 0;

            CboFuncionarios.ItemsSource = nomesFuncionarios;
            CboFuncionarios.SelectedIndex = 0;

            // Carrega a lista completa de movimentações (simulação do banco)
            todasAsMovimentacoes = new List<MovimentacaoEstoque>();
            todasAsMovimentacoes = carregamento;

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