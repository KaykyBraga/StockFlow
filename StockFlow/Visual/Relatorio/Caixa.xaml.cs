using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockFlow.Visual.Relatorio
{
    public partial class Caixa : Page
    {
        public Caixa()
        {
            InitializeComponent();
            CarregarDadosIniciais();
        }

        /// <summary>
        /// Carrega os dados iniciais da tela, como a lista de IDs de usuário no ComboBox.
        /// </summary>
        private void CarregarDadosIniciais()
        {
            try
            {
                // Busca os IDs de usuário do banco de dados para popular o filtro
                List<int> idsDeUsuario = BuscarIdsDeUsuarioDoBanco();

                // Adiciona a opção "0" para representar "Todos"
                idsDeUsuario.Insert(0, 0);

                // Define a lista como a fonte de dados do ComboBox e seleciona '0' como padrão
                CboIdUsuario.ItemsSource = idsDeUsuario;
                CboIdUsuario.SelectedItem = 0;

                // Garante que a grade comece vazia
                dgCaixa.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados iniciais: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Coleta os parâmetros dos filtros da tela
                DateTime? dataInicio = DpDataInicio.SelectedDate;
                DateTime? dataFim = DpDataFim.SelectedDate;
                int idUsuarioSelecionado = (int)(CboIdUsuario.SelectedItem ?? 0);

                // 2. Chama o método de busca no banco de dados com os filtros
                List<SessaoCaixa> resultado = BuscarSessoesCaixaDoBanco(dataInicio, dataFim, idUsuarioSelecionado);

                // 3. Atualiza a grade (DataGrid) com os resultados da busca
                dgCaixa.ItemsSource = resultado;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar relatório de caixa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        public int IdUsuario { get; set; }
        public DateTime DataAbertura { get; set; }
        public decimal ValorAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public decimal? ValorFechamento { get; set; }
        public decimal Diferenca { get; set; } // O cálculo da diferença deve vir do banco ou da sua camada de negócios
        public string Status => DataFechamento.HasValue ? "Fechado" : "Aberto";
        public bool DiferencaNegativa => Diferenca < 0;
    }
}