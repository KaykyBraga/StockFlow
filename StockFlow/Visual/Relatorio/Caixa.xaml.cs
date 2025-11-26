using StockFlow.Controles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockFlow.Visual.Relatorio
{

    public partial class Caixa : Page
    {
        private List<SessaoCaixa> sessoesCaixa;
        private List<string> nomeFuncionario;
        public Caixa()
        {
            CarregarDadosIniciais();
            InitializeComponent();
        }

        /// <summary>
        /// Carrega os dados iniciais da tela, como a lista de IDs de usuário no ComboBox.
        /// </summary>
        private async void CarregarDadosIniciais()
        {
            try
            {
                ControleVenda controleVenda = new ControleVenda();
                var todasSessoesCaixa = await controleVenda.ObterTodosOsCaixasParaGridAsync();
                sessoesCaixa = todasSessoesCaixa;

                // Busca os IDs de usuário do banco de dados para popular o filtro
                nomeFuncionario = new List<string>();
                nomeFuncionario.Add("Todos");

                foreach (var nomes in todasSessoesCaixa)
                {
                    if (!nomeFuncionario.Contains(nomes.IdUsuario))
                        nomeFuncionario.Add(nomes.IdUsuario);
                }

                // Adiciona a opção "0" para representar "Todos"


                // Define a lista como a fonte de dados do ComboBox e seleciona '0' como padrão
                CboIdUsuario.ItemsSource = nomeFuncionario;
                CboIdUsuario.SelectedItem = "Todos";

                // Garante que a grade comece vazia
                dgCaixa.ItemsSource = sessoesCaixa;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados iniciais: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<SessaoCaixa> movimentacoesFiltradas = sessoesCaixa;

            // Filtro por Data de Início
            if (DpDataInicio.SelectedDate.HasValue)
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.DataAbertura.Date >= DpDataInicio.SelectedDate.Value.Date);
            }

            // Filtro por Data de Fim
            if (DpDataFim.SelectedDate.HasValue)
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.DataFechamento.Value.Date <= DpDataFim.SelectedDate.Value.Date);
            }

            // Filtro por Tipo de Movimentação
            if (CboIdUsuario.SelectedItem?.ToString() != "Todos")
            {
                movimentacoesFiltradas = movimentacoesFiltradas.Where(m => m.IdUsuario == CboIdUsuario.SelectedItem.ToString());
            }
           dgCaixa.ItemsSource = movimentacoesFiltradas.ToList();
        }

        
        private List<int> BuscarIdsDeUsuarioDoBanco()
        {
            // AQUI ENTRA A SUA LÓGICA DE BANCO DE DADOS
            
            return new List<int>();
        }

        /// <summary>
        /// MÉTODO PLACEHOLDER: Busca as sessões de caixa no banco de dados com base nos filtros.
        /// </summary>
        /// <param name="dataInicio">Data inicial do filtro.</param>
        /// <param name="dataFim">Data final do filtro.</param>
        /// <param name="idUsuario">ID do usuário (0 para todos).</param>
        /// <returns>Uma lista de sessões de caixa que correspondem aos filtros.</returns>
        private List<SessaoCaixa> BuscarSessoesCaixaDoBanco(DateTime? dataInicio, DateTime? dataFim, int idUsuario)
        {
            // AQUI ENTRA A SUA LÓGICA DE BANCO DE DADOS
            
            return new List<SessaoCaixa>();
        }
    }

    
    public class SessaoCaixa
    {
        public int IdCaixa { get; set; }
        public string IdUsuario { get; set; }
        public DateTime DataAbertura { get; set; }
        public decimal ValorAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public decimal? ValorFechamento { get; set; }
        public decimal? Diferenca { get; set; } // O cálculo da diferença deve vir do banco ou da sua camada de negócios
        public string Status => DataFechamento.HasValue ? "Fechado" : "Aberto";
        public bool DiferencaNegativa => Diferenca < 0;
    }
}