using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Linq;

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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // MÉTODO ADICIONADO: Máscara de CNPJ
        private void TxtCnpj_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string digitsOnly = new String(textBox.Text.Where(char.IsDigit).ToArray());

            textBox.TextChanged -= TxtCnpj_TextChanged; // Desanexa

            string formatted = digitsOnly;
            if (digitsOnly.Length > 12)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2, 3)}.{digitsOnly.Substring(5, 3)}/{digitsOnly.Substring(8, 4)}-{digitsOnly.Substring(12)}";
            else if (digitsOnly.Length > 8)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2, 3)}.{digitsOnly.Substring(5, 3)}/{digitsOnly.Substring(8)}";
            else if (digitsOnly.Length > 5)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2, 3)}.{digitsOnly.Substring(5)}";
            else if (digitsOnly.Length > 2)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2)}";

            int caretPosition = textBox.CaretIndex;
            textBox.Text = formatted;
            textBox.CaretIndex = Math.Min(formatted.Length, Math.Max(caretPosition + (formatted.Length - digitsOnly.Length), 0));

            textBox.TextChanged += TxtCnpj_TextChanged; // Reanexa
        }

        // MÉTODO ADICIONADO: Máscara de Telefone
        private void TxtTelefonePrincipal_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string digitsOnly = new String(textBox.Text.Where(char.IsDigit).ToArray());

            textBox.TextChanged -= TxtTelefonePrincipal_TextChanged; // Desanexa

            string formatted = digitsOnly;
            if (digitsOnly.Length == 11) // (XX) XXXXX-XXXX
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 5)}-{digitsOnly.Substring(7)}";
            else if (digitsOnly.Length == 10) // (XX) XXXX-XXXX
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 4)}-{digitsOnly.Substring(6)}";
            else if (digitsOnly.Length > 6)
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, Math.Min(digitsOnly.Length - 2, 5))}{(digitsOnly.Length > 7 ? "-" : "")}{digitsOnly.Substring(Math.Min(digitsOnly.Length, 7))}";
            else if (digitsOnly.Length > 2)
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2)}";
            else if (digitsOnly.Length > 0)
                formatted = $"({digitsOnly}";

            int caretPosition = textBox.CaretIndex;
            textBox.Text = formatted;
            textBox.CaretIndex = Math.Min(formatted.Length, Math.Max(caretPosition + (formatted.Length - digitsOnly.Length), 0));

            textBox.TextChanged += TxtTelefonePrincipal_TextChanged; // Reanexa
        }

        private void btnExcluirMarca_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupEditarMarca.Visibility = Visibility.Visible;
        }

        private void btnEditarFornecedor_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupEditarFornecedor.Visibility = Visibility.Visible;
        }

        private void btnExcluirCategoria_Click(object sender, RoutedEventArgs e)
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