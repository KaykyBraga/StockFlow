using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual.Produtos
{
    public partial class EditarProduto : Page
    {
        private string produtoId;

        public EditarProduto(string idProduto)
        {
            InitializeComponent();
            this.produtoId = idProduto;

            // 2. Chama um método para carregar os dados do produto nos campos
            CarregarDadosDoProduto();
        }

        private void CarregarDadosDoProduto()
        {
            // É AQUI QUE VOCÊ DEVE BUSCAR NO BANCO DE DADOS:
            // 1. Use 'this.produtoId' para buscar o produto completo no banco.
            //    Ex: Produto produto = MeuBanco.GetProdutoPorId(this.produtoId);

            // 2. Preencha os campos (TextBoxes, ComboBoxes, etc.) da sua
            //    tela 'EditarProduto.xaml' com os dados encontrados.
            //    Ex: txtNomeProduto.Text = produto.Nome;
            //        txtPrecoVenda.Text = produto.Preco.ToString();
            //        cmbCategoria.SelectedValue = produto.CategoriaId;

            // Apenas como exemplo, vamos supor que você tem um TextBlock
            // chamado 'txtTitulo' e vamos exibir o ID nele:

            // Ex: txtTitulo.Text = $"Editando Produto ID: {this.produtoId}";
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