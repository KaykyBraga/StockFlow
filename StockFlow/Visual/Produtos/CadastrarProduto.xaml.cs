using StockFlow.Controles;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace StockFlow.Visual.Produtos
{
    public partial class CadastrarProduto : Page
    {
        List<StockFlow.Modelo.Marca> marcas;
        List<StockFlow.Modelo.Fornecedor> fornecedores;
        List<StockFlow.Modelo.Categoria> categorias;

        public CadastrarProduto()
        {
            Carregar();
            InitializeComponent();
            
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            if (navigationService != null)
            {
                navigationService.Navigate(null as Uri);
            }
        }

        #region Controle de Pop-ups de Cadastro (Marca, Fornecedor, Categoria)


        private async void Carregar()
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            marcas = await controleEstoque.ObterTodasAsMarcasAsync();
            List<string> listaMarcas = new List<string>();
            foreach (var marca in marcas)
            {
                if (marca.Ativo)
                {
                    listaMarcas.Add(marca.NomeMarca);
                }
            }
            cmbMarca.ItemsSource = listaMarcas;

            fornecedores = await controleEstoque.ObterTodosOsFornecedoresAsync();
            List<string> listaFornecedores = new List<string>();
            foreach (var fornecedor in fornecedores)
            {
                if (fornecedor.Ativo)
                {
                    listaFornecedores.Add(fornecedor.NomeFantasia);
                }
            }
            cmbFornecedor.ItemsSource = listaFornecedores;

            categorias = await controleEstoque.ObterTodasAsCategoriasAsync();
            List<string> listaCategorias = new List<string>();
            foreach (var categoria in categorias)
            {
                if (categoria.Ativo)
                {
                    listaCategorias.Add(categoria.NomeCategoria);
                }
            }
            cmbCategoria.ItemsSource = listaCategorias;

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // NOVO: Evento TextChanged para formatar o CNPJ
        private void TxtCnpj_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // 1. Remove qualquer formatação existente para trabalhar só com números
            string digitsOnly = new String(textBox.Text.Where(char.IsDigit).ToArray());

            // Evita loop infinito desanexando temporariamente o handler
            textBox.TextChanged -= TxtCnpj_TextChanged;

            // 2. Aplica a máscara XX.XXX.XXX/XXXX-XX
            string formatted = digitsOnly;
            if (digitsOnly.Length > 12)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2, 3)}.{digitsOnly.Substring(5, 3)}/{digitsOnly.Substring(8, 4)}-{digitsOnly.Substring(12)}";
            else if (digitsOnly.Length > 8)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2, 3)}.{digitsOnly.Substring(5, 3)}/{digitsOnly.Substring(8)}";
            else if (digitsOnly.Length > 5)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2, 3)}.{digitsOnly.Substring(5)}";
            else if (digitsOnly.Length > 2)
                formatted = $"{digitsOnly.Substring(0, 2)}.{digitsOnly.Substring(2)}";

            // 3. Atualiza o texto e a posição do cursor
            int caretPosition = textBox.CaretIndex;
            textBox.Text = formatted;
            // Tenta manter o cursor no fim ou na posição correta após a formatação
            textBox.CaretIndex = Math.Min(formatted.Length, Math.Max(caretPosition + (formatted.Length - digitsOnly.Length), 0));

            // Reanexa o handler
            textBox.TextChanged += TxtCnpj_TextChanged;
        }

        // NOVO: Evento TextChanged para formatar o Telefone
        private void TxtTelefonePrincipal_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // 1. Remove qualquer formatação existente
            string digitsOnly = new String(textBox.Text.Where(char.IsDigit).ToArray());

            // Evita loop infinito
            textBox.TextChanged -= TxtTelefonePrincipal_TextChanged;

            // 2. Aplica a máscara (XX) XXXXX-XXXX ou (XX) XXXX-XXXX
            string formatted = digitsOnly;
            if (digitsOnly.Length == 11) // Celular com 9 dígitos
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 5)}-{digitsOnly.Substring(7)}";
            else if (digitsOnly.Length == 10) // Fixo ou celular antigo
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 4)}-{digitsOnly.Substring(6)}";
            else if (digitsOnly.Length > 6) // Após o DDD e os 4/5 primeiros dígitos
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, Math.Min(digitsOnly.Length - 2, 5))}{(digitsOnly.Length > 7 ? "-" : "")}{digitsOnly.Substring(Math.Min(digitsOnly.Length, 7))}";
            else if (digitsOnly.Length > 2) // Após o DDD
                formatted = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2)}";
            else if (digitsOnly.Length > 0) // Durante o DDD
                formatted = $"({digitsOnly}";


            // 3. Atualiza o texto e a posição do cursor
            int caretPosition = textBox.CaretIndex;
            textBox.Text = formatted;
            textBox.CaretIndex = Math.Min(formatted.Length, Math.Max(caretPosition + (formatted.Length - digitsOnly.Length), 0));


            // Reanexa o handler
            textBox.TextChanged += TxtTelefonePrincipal_TextChanged;
        }

        private void btnAddMarca_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupMarca.Visibility = Visibility.Visible;
            
        }

        private void btnAddFornecedor_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupFornecedor.Visibility = Visibility.Visible;
        }

        private void btnAddCategoria_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
            PopupCategoria.Visibility = Visibility.Visible;
        }

        private void btnAdicinarMarca_Click(object sender, RoutedEventArgs e)
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            if (string.IsNullOrWhiteSpace(txtNomeMarca.Text))
            {
                MessageBox.Show("O campo 'Nome da Marca' é obrigatório.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            controleEstoque.AdicionarMarca(txtNomeMarca.Text);

            if (!string.IsNullOrEmpty(controleEstoque.mensagem))
            {
                MessageBox.Show(controleEstoque.mensagem, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Marca cadastrada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            txtNomeMarca.Clear();
            Carregar();
            btnCancelar_Click(sender, e);

        }

        private void btnAdicinarFornecedor_Click(object sender, RoutedEventArgs e)
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            if (string.IsNullOrWhiteSpace(txtRazaoSocial.Text) ||
                string.IsNullOrWhiteSpace(txtNomeFantasia.Text) ||
                string.IsNullOrWhiteSpace(txtCnpj.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtTelefonePrincipal.Text)) 
            {
                MessageBox.Show("Os campos 'Nome do Fornecedor' e 'CNPJ' são obrigatórios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            controleEstoque.AdicionarFornecedor(new List<string>
            {
                txtNomeFantasia.Text,
                txtRazaoSocial.Text,             
                txtCnpj.Text,
                txtEmail.Text,
                txtTelefonePrincipal.Text              
            });
            if (!string.IsNullOrEmpty(controleEstoque.mensagem))
            {
                MessageBox.Show(controleEstoque.mensagem, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Fornecedor cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            txtNomeFantasia.Clear();
            txtRazaoSocial.Clear();
            txtCnpj.Clear();
            txtEmail.Clear();
            txtTelefonePrincipal.Clear();
            Carregar();
            btnCancelar_Click(sender, e);

        }

        private void btnAdicinarCategoria_Click(object sender, RoutedEventArgs e)
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            if (string.IsNullOrWhiteSpace(txtNomeCategoria.Text))
            {
                MessageBox.Show("O campo 'Nome da Categoria' é obrigatório.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            controleEstoque.AdicionarCategoria(txtNomeCategoria.Text);

            if (!string.IsNullOrEmpty(controleEstoque.mensagem))
            {
                MessageBox.Show(controleEstoque.mensagem, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Categoria cadastrada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            txtNomeCategoria.Clear();
            Carregar();
            btnCancelar_Click(sender, e);

        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Esconde todos os pop-ups de cadastro
            PopupMarca.Visibility = Visibility.Collapsed;
            PopupFornecedor.Visibility = Visibility.Collapsed;
            PopupCategoria.Visibility = Visibility.Collapsed;

            // Esconde a camada de sobreposição
            PopupOverlay.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Lógica Principal e Pop-ups de Notificação

        // Exemplo de como usar os pop-ups de sucesso/erro
        private void CadastrarButton_Click(object sender, RoutedEventArgs e)
        {
            // --- AQUI VAI A SUA LÓGICA DE VALIDAÇÃO E CADASTRO ---
            // Por exemplo, verificar se o campo nome do produto está preenchido
            if (string.IsNullOrWhiteSpace(txtNomeProduto.Text))
            {
                // Se estiver vazio, mostra pop-up de erro
                ShowErrorPopup("O campo 'Nome Produto' é obrigatório.");
            }
            else
            {
                // Se tudo estiver certo (simulação):
                // 1. Salve os dados no banco.
                // 2. Mostre o pop-up de sucesso.
                ShowSuccessPopup("Produto cadastrado com sucesso!");
            }
        }

        // NOVO: Mostra o pop-up de sucesso com uma mensagem customizada
        private void ShowSuccessPopup(string message)
        {
            PopupMensagemSucesso.Text = message;
            PopupOverlay.Visibility = Visibility.Visible;
            PopupSucesso.Visibility = Visibility.Visible;
        }

        // NOVO: Mostra o pop-up de erro com uma mensagem customizada
        private void ShowErrorPopup(string message)
        {
            PopupMensagemErro.Text = message;
            PopupOverlay.Visibility = Visibility.Visible;
            PopupErro.Visibility = Visibility.Visible;
        }

        // NOVO: Evento do botão "OK" para fechar os pop-ups de notificação
        private void btnPopupOk_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            PopupSucesso.Visibility = Visibility.Collapsed;
            PopupErro.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}