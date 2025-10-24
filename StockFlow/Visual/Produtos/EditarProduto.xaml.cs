using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual.Produtos
{
    public partial class EditarProduto : Page
    {
        public EditarProduto()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #region Controle de Pop-ups de Edição (Marca, Fornecedor, Categoria)

        private void btnEditarMarca_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupEditarMarca.Visibility = Visibility.Visible;
        }

        private void btnEditarFornecedor_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupEditarFornecedor.Visibility = Visibility.Visible;
        }

        private void btnEditarCategoria_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupEditarCategoria.Visibility = Visibility.Visible;
        }

        private void btnCancelarEdicao_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            PopupEditarMarca.Visibility = Visibility.Collapsed;
            PopupEditarFornecedor.Visibility = Visibility.Collapsed;
            PopupEditarCategoria.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Lógica Principal e Pop-ups de Confirmação/Notificação

        /// <summary>
        /// Limpa todos os campos de entrada da tela.
        /// </summary>
        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            // Limpa todos os campos de texto
            txtNomeProduto.Text = string.Empty;
            txtPrecoCusto.Text = string.Empty;
            txtPrecoVenda.Text = string.Empty;
            txtEstoqueMinimo.Text = string.Empty;
            txtDescricao.Text = string.Empty;

            // O campo Estoque Inicial é ReadOnly, mas limpá-lo pode ser útil
            // se o valor for carregado programaticamente.
            txtEstoqueInicial.Text = string.Empty;

            // Limpa a seleção do ComboBox, removendo qualquer item selecionado
            cmbUnidadeMedida.SelectedIndex = -1;
        }

        // 1. O botão Salvar APENAS abre o pop-up de confirmação
        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupConfirmacao.Visibility = Visibility.Visible;
        }

        // 2. O botão "Sim, Salvar" dentro do pop-up executa a lógica
        private void btnConfirmarSalvar_Click(object sender, RoutedEventArgs e)
        {
            PopupConfirmacao.Visibility = Visibility.Collapsed;

            // --- AQUI VAI A SUA LÓGICA DE VALIDAÇÃO E EDIÇÃO ---
            if (string.IsNullOrWhiteSpace(txtNomeProduto.Text))
            {
                ShowErrorPopup("O campo 'Nome Produto' não pode estar vazio.");
            }
            else
            {
                ShowSuccessPopup("Produto atualizado com sucesso!");
            }
        }

        // Fecha o pop-up de confirmação sem fazer nada
        private void btnCancelarConfirmacao_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            PopupConfirmacao.Visibility = Visibility.Collapsed;
        }

        private void ShowSuccessPopup(string message)
        {
            PopupMensagemSucesso.Text = message;
            PopupOverlay.Visibility = Visibility.Visible;
            PopupSucesso.Visibility = Visibility.Visible;
        }

        private void ShowErrorPopup(string message)
        {
            PopupMensagemErro.Text = message;
            PopupOverlay.Visibility = Visibility.Visible;
            PopupErro.Visibility = Visibility.Visible;
        }

        private void btnPopupOk_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            PopupSucesso.Visibility = Visibility.Collapsed;
            PopupErro.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}