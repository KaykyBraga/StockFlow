using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Linq;
using StockFlow.Controles;

namespace StockFlow.Visual.Produtos
{
    public partial class EditarProduto : Page
    {
        private string produtoId;
        List<StockFlow.Modelo.Marca> marcas;
        List<StockFlow.Modelo.Fornecedor> fornecedores;
        List<StockFlow.Modelo.Categoria> categorias;

        public EditarProduto(string idProduto)
        {
            this.produtoId = idProduto;
            CarregarDadosDoProduto();
            InitializeComponent();

            // 2. Chama um método para carregar os dados do produto nos campos
        }

        private async void CarregarDadosDoProduto()
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            var produto = await controleEstoque.BuscarProdutoPorId(this.produtoId);


            txtNomeProduto.Text = produto.NomeCompleto;
            txtEstoqueAtual.Text = produto.EstoqueAtual.ToString();
            txtSku.Text = produto.Sku;
            txtEan.Text = produto.Ean;
            txtPrecoCusto.Text = produto.PrecoCusto.ToString();




            List<string> listaMarcas = new List<string>();
            marcas = await controleEstoque.ObterTodasAsMarcasAsync();
            var nomeMarca = marcas.FirstOrDefault(m => m.MarcaId == produto.MarcaId);
            foreach ( var item in marcas)
            {
                listaMarcas.Add(item.NomeMarca);
            }
            cmbMarca.ItemsSource = listaMarcas;
            cmbMarca.Text = nomeMarca.NomeMarca;

            List<string> listaCategorias = new List<string>();
            categorias = await controleEstoque.ObterTodasAsCategoriasAsync();
            var nomeCategorias = categorias.FirstOrDefault(c => c.CategoriaId == produto.CategoriaId);
            foreach( var item in categorias)
            {
                listaCategorias.Add(item.NomeCategoria);
            }
            cmbCategoria.ItemsSource = listaCategorias;
            cmbCategoria.Text = nomeCategorias.NomeCategoria;

            List<string> listaFornecedores = new List<string>();
            fornecedores = await controleEstoque.ObterTodosOsFornecedoresAsync();
            var nomeFantasia = fornecedores.FirstOrDefault(f => f.FornecedorId == produto.FornecedorId);
            foreach ( var item in fornecedores)
            {
                listaFornecedores.Add(item.NomeFantasia);
            }
            cmbFornecedor.ItemsSource = listaFornecedores;
            cmbFornecedor.Text = nomeFantasia.NomeFantasia;


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