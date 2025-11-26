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
        string fornecedorId;
        List<StockFlow.Modelo.Marca> marcas;
        List<StockFlow.Modelo.Fornecedor> fornecedores;
        List<StockFlow.Modelo.Categoria> categorias;
        DateTime? dataCadastro;
        DateTime? dataCadastroFor;

        public EditarProduto(string idProduto)
        {
            CarregarDadosDoProduto();
            InitializeComponent(); // Esta linha DEVE vir primeiro

            this.produtoId = idProduto;

            // ADICIONE ESTAS 4 LINHAS:
            txtPrecoCusto.GotFocus += TxtPrice_GotFocus;
            txtPrecoCusto.LostFocus += TxtPrice_LostFocus;

            txtPrecoVenda.GotFocus += TxtPrice_GotFocus;
            txtPrecoVenda.LostFocus += TxtPrice_LostFocus;

            txtEan.MaxLength = 13;            
            txtEan.PreviewTextInput += NumberValidationTextBox;

            
        }

        private async void CarregarDadosDoProduto()
        {
            ControleEstoque controleEstoque = new ControleEstoque();
            var produto = await controleEstoque.BuscarProdutoPorId(this.produtoId);


            txtNomeProduto.Text = produto.NomeCompleto;
            txtEstoqueAtual.Text = produto.EstoqueAtual.ToString();
            txtSku.Text = produto.Sku;
            txtEan.Text = produto.Ean;
            txtPrecoCusto.Text = $"R$: {produto.PrecoCusto:F2}";
            txtEstoqueMinimo.Text = produto.EstoqueMinimo.ToString();
            txtPrecoVenda.Text = $"R$: {produto.PrecoVenda:F2}";
            txtLocalizacao.Text = produto.LocalizacaoEstoque;
            dataCadastro = produto.DataCadastro;




            List<string> listaMarcas = new List<string>();
            marcas = await controleEstoque.ObterTodasAsMarcasAsync();
            var nomeMarca = marcas.FirstOrDefault(m => m.MarcaId == produto.MarcaId);
            foreach (var item in marcas)
            {
                listaMarcas.Add(item.NomeMarca);
            }
            cmbMarca.ItemsSource = listaMarcas;
            cmbMarca.Text = nomeMarca.NomeMarca;

            List<string> listaCategorias = new List<string>();
            categorias = await controleEstoque.ObterTodasAsCategoriasAsync();
            var nomeCategorias = categorias.FirstOrDefault(c => c.CategoriaId == produto.CategoriaId);
            foreach (var item in categorias)
            {
                listaCategorias.Add(item.NomeCategoria);
            }
            cmbCategoria.ItemsSource = listaCategorias;
            cmbCategoria.Text = nomeCategorias.NomeCategoria;

            List<string> listaFornecedores = new List<string>();
            fornecedores = await controleEstoque.ObterTodosOsFornecedoresAsync();
            var nomeFantasia = fornecedores.FirstOrDefault(f => f.FornecedorId == produto.FornecedorId);
            foreach (var item in fornecedores)
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

        private void PriceValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // Se o texto não for um dígito
            if (!char.IsDigit(e.Text, 0))
            {
                // Permite UM ponto ou UMA vírgula
                if ((e.Text == "," || e.Text == ".") &&
                    !textBox.Text.Contains(",") &&
                    !textBox.Text.Contains("."))
                {
                    // Substitui ponto por vírgula para padronizar
                    e.Handled = true;
                    textBox.Text = textBox.Text.Insert(textBox.CaretIndex, ",");
                    textBox.CaretIndex = textBox.Text.Length;
                }
                else
                {
                    // Rejeita qualquer outro caractere
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// FORMATAÇÃO (Ao Sair): Formata o texto para "R$: 123,45" quando o usuário sai do campo.
        /// </summary>
        private void TxtPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // Padroniza o separador para vírgula (caso o usuário digite ponto)
            string text = textBox.Text.Replace('.', ',');

            if (decimal.TryParse(text, out decimal price))
            {
                // Formata como R$: 123,45 (Formato "F2" usa vírgula)
                textBox.Text = $"R$: {price:F2}";
            }
            else
            {
                textBox.Text = "R$: 0,00";
            }
        }

        /// <summary>
        /// MÉTODO 2: Ao ENTRAR no campo (GotFocus)
        /// Remove o "R$:" para facilitar a digitação
        /// </summary>
        private void TxtPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // 1. Limpa o "R$:"
            string cleanText = textBox.Text.Replace("R$:", "").Trim();

            // 2. Padroniza para vírgula
            string text = cleanText.Replace('.', ',');

            if (decimal.TryParse(text, out decimal price))
            {
                // 3. Mostra SÓ o número formatado com vírgula (Ex: "123,45")
                textBox.Text = price.ToString("F2");
            }
            else
            {
                textBox.Text = "0,00";
            }

            // 4. Seleciona tudo para o usuário digitar por cima
            textBox.SelectAll();
        }

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
            MessageBoxResult resultado = MessageBox.Show($"Tem certeza que deseja remover a Marca? '{cmbMarca.Text}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultado == MessageBoxResult.Yes)
            {
                var m = marcas.FirstOrDefault(m => m.NomeMarca == cmbMarca.Text);
                ControleEstoque controleEstoque = new ControleEstoque();
                controleEstoque.DesatiarMarca(m);
                if (controleEstoque.mensagem != "")
                {
                    MessageBox.Show(controleEstoque.mensagem);
                }
                else
                {
                    MessageBox.Show("Marca deletada com sucesso!");
                }
            }


        }

        private void btnEditarFornecedor_Click(object sender, RoutedEventArgs e)
        {
            var f = fornecedores.FirstOrDefault(f => f.NomeFantasia == cmbFornecedor.Text);
            txtPopupFornecedorNomeFantasia.Text = f.NomeFantasia;
                txtPopupFornecedorRazaoSocial.Text = f.RazaoSocial;
                txtPopupFornecedorTelefone.Text = f.TelefonePrincipal;
            txtPopupFornecedorEmail.Text = f.EmailPrincipal;
            txtPopupFornecedorCnpj.Text = f.Cnpj;
            fornecedorId = f.FornecedorId.ToString();
            dataCadastroFor = f.DataCadastro;

            PopupOverlay.Visibility = Visibility.Visible;
            PopupEditarFornecedor.Visibility = Visibility.Visible;
        }

        private void btnExcluirCategoria_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado = MessageBox.Show($"Tem certeza que deseja remover a Categoria? '{cmbCategoria.Text}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultado == MessageBoxResult.Yes)
            {
                var c = categorias.FirstOrDefault(c => c.NomeCategoria == cmbCategoria.Text);
                ControleEstoque controleEstoque = new ControleEstoque();
                controleEstoque.DesativarCategoria(c);
                if (controleEstoque.mensagem != "")
                {
                    MessageBox.Show(controleEstoque.mensagem);
                }
                else
                {
                    MessageBox.Show("Categoria deletada com sucesso!");
                }
            }
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
            txtSku.Text = string.Empty;
            txtEan.Text = string.Empty;
            txtLocalizacao.Text = string.Empty;
        }

        // 1. O botão Salvar APENAS abre o pop-up de confirmação
        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeProduto.Text) ||
                string.IsNullOrWhiteSpace(txtSku.Text) ||
                string.IsNullOrWhiteSpace(txtPrecoCusto.Text) ||
                string.IsNullOrWhiteSpace(txtPrecoVenda.Text) ||
                string.IsNullOrWhiteSpace(txtEstoqueAtual.Text) ||
                string.IsNullOrWhiteSpace(txtEan.Text) ||
                string.IsNullOrWhiteSpace(txtEstoqueMinimo.Text) ||
                string.IsNullOrWhiteSpace(txtLocalizacao.Text))
            {
                MessageBox.Show("Preencha todos os campos corretamente", "Erro campos nao preenchidos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PopupOverlay.Visibility = Visibility.Visible;
            PopupConfirmacao.Visibility = Visibility.Visible;

        }

        private void BtnSalvarEdicao_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPopupFornecedorNomeFantasia.Text) ||
                string.IsNullOrWhiteSpace(txtPopupFornecedorRazaoSocial.Text) ||
                string.IsNullOrWhiteSpace(txtPopupFornecedorTelefone.Text) ||
                string.IsNullOrWhiteSpace(txtPopupFornecedorEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPopupFornecedorCnpj.Text))
            {
                    MessageBox.Show("Preencha todos os campos!!");

            }
            else
            {
                string textoComMascara = txtPopupFornecedorCnpj.Text;
                string soNumeros = new string(textoComMascara.Where(char.IsDigit).ToArray());
                ControleEstoque controleEstoque = new ControleEstoque();
                List<string> listaDados = new List<string>
                {
                    fornecedorId,
                    txtPopupFornecedorNomeFantasia.Text,
                    txtPopupFornecedorRazaoSocial.Text,
                    soNumeros,
                    txtPopupFornecedorEmail.Text,
                    txtPopupFornecedorTelefone.Text,
                    dataCadastroFor.ToString(),

                };
                controleEstoque.EditarFornecedor(listaDados);
                if (controleEstoque.mensagem != "")
                {
                    MessageBox.Show(controleEstoque.mensagem);
                    return;
                }
                else
                {
                    ShowSuccessPopup("Fornecedor atualizado com sucesso!");
                    CarregarDadosDoProduto();
                }
            }
        }

        // 2. O botão "Sim, Salvar" dentro do pop-up executa a lógica
        private void btnConfirmarSalvar_Click(object sender, RoutedEventArgs e)
        {
            PopupConfirmacao.Visibility = Visibility.Collapsed;

            string precoVendaLimpo = txtPrecoVenda.Text.Replace("R$:", "").Trim().Replace(',', '.');
            string precoCustoLimpo = txtPrecoCusto.Text.Replace("R$:", "").Trim().Replace(',', '.');


            List<string> listaDados = new List<string>();
            var m = marcas.FirstOrDefault(m => m.NomeMarca == cmbMarca.Text);
            if (m.Ativo == false)
            {
                MessageBox.Show("coloque uma marca que esteja ativa");
                PopupOverlay.Visibility = Visibility.Collapsed;
                PopupConfirmacao.Visibility = Visibility.Collapsed;
                return;
            }
            var f = fornecedores.FirstOrDefault(f => f.NomeFantasia == cmbFornecedor.Text);
            if (f.Ativo == false)
            {
                MessageBox.Show("coloque um fornecedor que esteja ativa");
                PopupOverlay.Visibility = Visibility.Collapsed;
                PopupConfirmacao.Visibility = Visibility.Collapsed;
                return;
            }
            var c = categorias.FirstOrDefault(c => c.NomeCategoria == cmbCategoria.Text);
            if (c.Ativo == false)
            {
                MessageBox.Show("coloque uma categoria que esteja ativa");
                PopupOverlay.Visibility = Visibility.Collapsed;
                PopupConfirmacao.Visibility = Visibility.Collapsed;
                return;
            }
            listaDados.Add(this.produtoId);
            listaDados.Add(txtSku.Text);
            listaDados.Add(txtEan.Text);
            listaDados.Add(txtNomeProduto.Text);
            listaDados.Add(precoVendaLimpo);
            listaDados.Add(precoCustoLimpo);
            listaDados.Add(txtEstoqueAtual.Text);
            listaDados.Add(txtEstoqueMinimo.Text);
            listaDados.Add(dataCadastro.ToString());
            listaDados.Add(m.MarcaId.ToString());
            listaDados.Add(c.CategoriaId.ToString());
            listaDados.Add(f.FornecedorId.ToString());
            listaDados.Add(txtLocalizacao.Text);

            ControleEstoque controleEstoque = new ControleEstoque();
            controleEstoque.EditarProduto(listaDados);
            if (controleEstoque.mensagem != "")
            {
                MessageBox.Show(controleEstoque.mensagem);
                PopupOverlay.Visibility = Visibility.Collapsed;
                PopupConfirmacao.Visibility = Visibility.Collapsed;
                return;
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