using StockFlow.Controles;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel; // Necessário para Dispatcher
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Necessário para KeyEventArgs
using System.Windows.Media;
using System.Windows.Threading;



namespace StockFlow.Visual

{

    public partial class Tela_Vendas : Window

    {
        private decimal valorAberturaAtual = 0; // guarda o valor inicial
        private decimal valorVendasDinheiro = 0; // NOVO: Guarda o total de vendas pagas em dinheiro
        private decimal valorAdicionadoTroco = 0; // NOVO: Guarda o total de troco adicionado
        private decimal valorSangria = 0;

        #region Classes de Dados

        // Sua classe Produto precisa ter uma propriedade 'Nome' (ou ajuste DisplayMemberPath)

        public class Produto

        {
            // Adicione outras propriedades como Id, se tiver

            public string Nome { get; set; }
            public decimal Preco { get; set; }
            public int Id { get; set; }
            public override string ToString() => $"{Nome} - R$ {Preco:F2}";

        }

        private class ItemVenda
        {
            public string Nome { get; set; }
            public decimal PrecoUnitario { get; set; }
            public int Quantidade { get; set; }
            public int ProdutoId { get; set; }
            public decimal PrecoTotal => PrecoUnitario * Quantidade;
        }

        #endregion



        #region Variáveis de Controle

        // Renomeado para refletir que guarda todos os produtos

        private List<Produto> listaDeTodosOsProdutos;

        private List<ItemVenda> itensVenda = new List<ItemVenda>();

        private string metodoPagamentoSelecionado = "";

        private bool isCaixaAberto = false;

        #endregion



        public Tela_Vendas()
        {
            CarregarProdutos(); // Carrega listaDeTodosOsProdutos
            ControleVenda controleVenda = new ControleVenda();
            controleVenda.DesativarPromocoesExpiradas(); // Desativa promoções expiradas ao iniciar a tela de vendas
            InitializeComponent();

            // Configura o ItemsSource e o que será exibido no ComboBox           

  
            AtualizarEstadoVisualCaixa(false);

        }

        private void Tela_Vendas_Closing(object sender, CancelEventArgs e)
        {
            // Verifica se o caixa está aberto
            if (isCaixaAberto)
            {
                // 1. Mostra a mesma mensagem de aviso do botão "Sair"
                MessageBox.Show("Você precisa fechar o caixa antes de fechar o sistema.",
                                "Caixa Aberto",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                // 2. CANCELA o evento de fechamento da janela
                e.Cancel = true;
            }
        }

        #region LÓGICA DE ESTADO DO CAIXA

        // ... (Seus métodos AtualizarEstadoVisualCaixa, AlertaCaixaFechado, BtnAbrirCaixa_Click, BtnFecharCaixa_Click, BtnSair_Click permanecem os mesmos) ...

        // Cole aqui os métodos da região LÓGICA DE ESTADO DO CAIXA do seu código original

        private void AtualizarEstadoVisualCaixa(bool caixaAberto)

        {

            if (caixaAberto)

            {

                BtnAbrirCaixa.Visibility = Visibility.Collapsed;

                BtnFecharCaixa.Visibility = Visibility.Visible;

                GridVendaPrincipal.Opacity = 1.0;

                GridVendaPrincipal.IsEnabled = true; // Habilita a área de venda

            }

            else
            {
                BtnAbrirCaixa.Visibility = Visibility.Visible;
                BtnFecharCaixa.Visibility = Visibility.Collapsed;
                GridVendaPrincipal.Opacity = 0.5;
                GridVendaPrincipal.IsEnabled = false; // Desabilita a área de venda
                LimparVendaAtual(); // Limpa a venda ao fechar o caixa
            }

        }

        private bool AlertaCaixaFechado()
        {
            if (!isCaixaAberto)
            {
                MessageBox.Show("É necessário abrir o caixa para utilizar esta função.", "Caixa Fechado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }



        private void BtnAbrirCaixa_Click(object sender, RoutedEventArgs e)
        {
            decimal? valorInicial = CriarPopupAberturaCaixa();
            if (valorInicial.HasValue)
            {
                this.valorAberturaAtual = valorInicial.Value;

                // --- ZERA AS VARIÁVEIS DE TRANSAÇÃO ---
                this.valorVendasDinheiro = 0;
                this.valorAdicionadoTroco = 0;
                this.valorSangria = 0;
                // ----------------------------------------

                isCaixaAberto = true;
                AtualizarEstadoVisualCaixa(true);              
                MessageBox.Show($"Caixa aberto com sucesso com um valor inicial de {this.valorAberturaAtual:C}!", "Caixa Aberto", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
        }



        private void BtnFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;

            // ==========================================================
            // 2. CALCULAR O VALOR ESPERADO NO CAIXA
            // ==========================================================
            decimal valorEsperado = this.valorAberturaAtual
                                   + this.valorVendasDinheiro
                                   + this.valorAdicionadoTroco
                                   - this.valorSangria;
            // ==========================================================

            // 3. CHAMAR O POPUP DE FECHAMENTO, PASSANDO O VALOR ESPERADO
            // ==========================================================
            bool? resultadoPopup = CriarPopupFechamentoCaixa(valorEsperado); // Passa o valor calculado

            // ==========================================================
            // 4. VERIFICAR SE O USUÁRIO CONFIRMOU (APÓS VER A DIFERENÇA NO POPUP)
            // ==========================================================
            if (resultadoPopup == true)
            {
                // O popup já mostrou a diferença e o usuário confirmou (clicou OK na msgbox).
                // Agora só finalizamos o processo.
                MessageBox.Show("Caixa fechado com sucesso!", "Fechamento de Caixa", MessageBoxButton.OK, MessageBoxImage.Information);
                isCaixaAberto = false;
                AtualizarEstadoVisualCaixa(false);
                SessaoUsuario.EncerrarSessao();
                Tela_Login login = new Tela_Login();
                login.Show();
                Window.GetWindow(this).Close();
                // Chama LimparVendaAtual e reseta variáveis

                // --- Adicione aqui a lógica para registrar o fechamento no banco ---
                // Você pode querer registrar valorAberturaAtual, valorEsperado,
                // o valor contado (que está dentro do popup), e a diferença.
                // Para pegar o valor contado, precisaria ajustar o popup para retorná-lo.
                // Ex: RegistrarFechamentoCaixa(this.valorAberturaAtual, valorEsperado, valorContadoNoPopup, diferencaCalculadaNoPopup);
                // Por ora, a chamada ao FecharCaixa do controle está dentro do popup.
            }
            // else { /* Usuário cancelou no popup */ }
        }





        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            if (isCaixaAberto)
            {
                MessageBox.Show("Você precisa fechar o caixa antes de sair.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            MessageBoxResult resultado = MessageBox.Show("Tem certeza que deseja sair?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (resultado == MessageBoxResult.Yes)
            {                 
                SessaoUsuario.EncerrarSessao();
                Tela_Login login = new Tela_Login();
                login.Show();
                Window.GetWindow(this).Close();
            }
        
        }

        #endregion



        #region Lógica de Venda



        // MÉTODO ATUALIZADO: Apenas carrega a lista principal

        private async void CarregarProdutos()

        {

            // --- SIMULAÇÃO --- Substitua pela busca real no banco de dados
            ControleVenda controleVenda = new ControleVenda();
            var lista = await controleVenda.ObterTodosOsProdutosParaVendaAsync();
            listaDeTodosOsProdutos = lista;

            ComboBuscarProduto.ItemsSource = listaDeTodosOsProdutos;

            // A definição do ItemsSource foi movida para o construtor

        }



        // MÉTODO REMOVIDO: TxtBuscarProduto_TextChanged não existe mais

        // private void TxtBuscarProduto_TextChanged(object sender, TextChangedEventArgs e) { ... }



        // NOVO MÉTODO: Evento KeyUp para o ComboBox editável

        private void ComboBuscarProduto_KeyUp(object sender, KeyEventArgs e)

        {

            var comboBox = sender as ComboBox;

            if (comboBox == null || listaDeTodosOsProdutos == null) return; // Verificação extra



            // Pega o texto atual DENTRO do ComboBox

            string textoBusca = comboBox.Text.ToLower();



            // Evita re-filtragem desnecessária quando um item é selecionado via clique ou Enter/Tab

            // Exceto se o usuário estiver apagando o texto (Backspace/Delete)

            if (comboBox.SelectedItem != null &&

                (comboBox.SelectedItem as Produto)?.Nome.ToLower() == textoBusca &&

                e.Key != Key.Back && e.Key != Key.Delete)

            {

                // Se o texto é igual ao item selecionado E não foi Backspace/Delete, não faz nada.

                return;

            }



            // Filtra a lista

            if (string.IsNullOrWhiteSpace(textoBusca))

            {

                // Se não há texto, mostra a lista completa

                comboBox.ItemsSource = listaDeTodosOsProdutos;

            }

            else

            {

                // Filtra a lista principal baseada no texto digitado

                var produtosFiltrados = listaDeTodosOsProdutos

                    .Where(p => p.Nome.ToLower().Contains(textoBusca)) // Busca se o Nome CONTÉM o texto

                    .ToList();

                comboBox.ItemsSource = produtosFiltrados;

            }



            // Força a abertura/manutenção do DropDown enquanto digita,

            // exceto se for Enter/Tab (seleção) ou Escape (fechar)

            if (e.Key != Key.Enter && e.Key != Key.Tab && e.Key != Key.Escape)

            {

                // Garante que o dropdown abra APÓS a atualização do ItemsSource

                Dispatcher.BeginInvoke(new Action(() =>
                {

                    // Só abre se estiver fechado E se houver itens para mostrar

                    if (!comboBox.IsDropDownOpen && comboBox.HasItems)

                    {

                        comboBox.IsDropDownOpen = true;

                    }

                }), DispatcherPriority.Background);

            }

        }





        // MÉTODO ATUALIZADO: Usa ComboBuscarProduto e limpa corretamente

        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)

        {

            if (AlertaCaixaFechado()) return;



            // Pega o item do NOVO ComboBox

            if (ComboBuscarProduto.SelectedItem == null)

            {

                MessageBox.Show("Por favor, selecione um produto válido da lista!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;

            }



            var produtoSelecionado = ComboBuscarProduto.SelectedItem as Produto;



            // Lógica para adicionar ou incrementar quantidade (sem alterações)

            var itemExistente = itensVenda.FirstOrDefault(i => i.Nome == produtoSelecionado.Nome); // Idealmente, compare por ID

            if (itemExistente != null)

            {

                itemExistente.Quantidade++;

            }

            else

            {

                itensVenda.Add(new ItemVenda { Nome = produtoSelecionado.Nome, PrecoUnitario = produtoSelecionado.Preco, Quantidade = 1, ProdutoId = produtoSelecionado.Id });

            }



            AtualizarListaItens();

            AtualizarTotal();



            // Limpa o ComboBox de busca corretamente

            ComboBuscarProduto.Text = string.Empty;          // Limpa o texto digitado

            ComboBuscarProduto.SelectedItem = null;       // Desseleciona o item

            ComboBuscarProduto.ItemsSource = listaDeTodosOsProdutos; // Restaura a lista completa

            ComboBuscarProduto.IsDropDownOpen = false;       // Fecha o dropdown

        }





        // ... (Seus métodos AtualizarListaItens, AdicionarItemVisual, Aumentar/Diminuir/Remover Quantidade, AtualizarTotal permanecem os mesmos) ...

        // Cole aqui os métodos da região Lógica de Venda do seu código original (exceto CarregarProdutos e TxtBuscarProduto_TextChanged)



        private void AtualizarListaItens()

        {

            PainelItensVenda.Children.Clear();

            foreach (var item in itensVenda) { AdicionarItemVisual(item); }

        }



        private void AdicionarItemVisual(ItemVenda item)

        {

            var itemBorder = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(5), Margin = new Thickness(0, 0, 5, 5) }; // Adicionado Margin bottom

            var itemGrid = new Grid();

            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Nome e Preço

            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Botões +/-

            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Botão Remover



            var infoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

            infoPanel.Children.Add(new TextBlock { Text = item.Nome, FontWeight = FontWeights.SemiBold, FontSize = 14 }); // Fonte um pouco maior

            infoPanel.Children.Add(new TextBlock { Text = $"{item.Quantidade} x {item.PrecoUnitario:C} = {item.PrecoTotal:C}", Foreground = Brushes.Gray, FontSize = 12 }); // Usando :C para moeda

            Grid.SetColumn(infoPanel, 0);



            var qtdPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            var btnDiminuir = new Button { Content = "-", Width = 25, Height = 25, Margin = new Thickness(5, 0, 2, 0), FontSize = 14, FontWeight = FontWeights.Bold }; // Ajuste visual

            var btnAumentar = new Button { Content = "+", Width = 25, Height = 25, Margin = new Thickness(2, 0, 5, 0), FontSize = 14, FontWeight = FontWeights.Bold }; // Ajuste visual

            btnDiminuir.Click += (s, ev) => DiminuirQuantidade(item);

            btnAumentar.Click += (s, ev) => AumentarQuantidade(item);

            qtdPanel.Children.Add(btnDiminuir);

            qtdPanel.Children.Add(btnAumentar);

            Grid.SetColumn(qtdPanel, 1);



            var btnRemover = new Button { Content = "✕", Width = 25, Height = 25, Foreground = Brushes.Red, FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0), Background = Brushes.Transparent, BorderBrush = Brushes.Transparent, VerticalAlignment = VerticalAlignment.Center, ToolTip = "Remover item" }; // Adicionado ToolTip

            btnRemover.Click += (s, ev) => RemoverItem(item);

            Grid.SetColumn(btnRemover, 2);



            itemGrid.Children.Add(infoPanel);

            itemGrid.Children.Add(qtdPanel);

            itemGrid.Children.Add(btnRemover);

            itemBorder.Child = itemGrid;

            PainelItensVenda.Children.Add(itemBorder);

        }





        private void AumentarQuantidade(ItemVenda item) { item.Quantidade++; AtualizarListaItens(); AtualizarTotal(); }

        private void DiminuirQuantidade(ItemVenda item) { item.Quantidade--; if (item.Quantidade <= 0) { itensVenda.Remove(item); } AtualizarListaItens(); AtualizarTotal(); }

        private void RemoverItem(ItemVenda item) { itensVenda.Remove(item); AtualizarListaItens(); AtualizarTotal(); }

        private void AtualizarTotal() { TxtTotalVenda.Text = itensVenda.Sum(i => i.PrecoTotal).ToString("C", CultureInfo.GetCultureInfo("pt-BR")); } // Formata como moeda BR

        #endregion



        #region Lógica de Pagamento e Finalização

        // ... (Seus métodos BtnPagamento_Click, ResetarBordasBotoesPagamento, BtnFinalizarVenda_Click e LimparVendaAtual) ...

        // MÉTODO ATUALIZADO: LimparVendaAtual também limpa o ComboBox de busca

        // Cole aqui os métodos da região Lógica de Pagamento e Finalização do seu código original, MAS SUBSTITUA o LimparVendaAtual por este:



        private void BtnPagamento_Click(object sender, RoutedEventArgs e)

        {

            if (AlertaCaixaFechado()) return;

            ResetarBordasBotoesPagamento();



            Button botaoClicado = sender as Button;

            if (botaoClicado != null)

            {

                botaoClicado.BorderBrush = Brushes.Gold;

                botaoClicado.BorderThickness = new Thickness(4);

                metodoPagamentoSelecionado = botaoClicado.Content.ToString();

            }

        }



        private void ResetarBordasBotoesPagamento()

        {

            BtnPagamentoCartao.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3498DB"));

            BtnPagamentoDinheiro.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#229954"));

            BtnPagamentoPix.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E67E22"));

            BtnPagamentoCartao.BorderThickness = new Thickness(2);

            BtnPagamentoDinheiro.BorderThickness = new Thickness(2);

            BtnPagamentoPix.BorderThickness = new Thickness(2);

        }



        private void BtnFinalizarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            if (itensVenda.Count == 0) { /*...*/ return; }
            if (string.IsNullOrEmpty(metodoPagamentoSelecionado)) { /*...*/ return; }

            // --- Calcula o total ANTES de limpar ---
            decimal totalVendaAtual = itensVenda.Sum(i => i.PrecoTotal);

            // --- Adicione aqui a lógica para registrar a venda no banco de dados ---
            List<Modelo.VendaItem> listaDeVenda = new List<Modelo.VendaItem>();
            foreach (var item in itensVenda)
            {
                listaDeVenda.Add(new Modelo.VendaItem
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,                   
                });
            }
            ControleVenda controleVenda = new ControleVenda();
            controleVenda.RegistrarVenda(listaDeVenda, metodoPagamentoSelecionado);

            if (controleVenda.mensagem != "")
            {
                MessageBox.Show(controleVenda.mensagem, "Erro de Venda", MessageBoxButton.OK, MessageBoxImage.Error);
                // NÃO limpa a venda se deu erro no registro
            }
            else
            {
                // --- SOMA O VALOR SE FOR DINHEIRO ---
                if (metodoPagamentoSelecionado.Equals("Dinheiro", StringComparison.OrdinalIgnoreCase))
                {
                    this.valorVendasDinheiro += totalVendaAtual;
                }
                // ------------------------------------

                MessageBox.Show($"Venda finalizada com sucesso!\nTotal: {totalVendaAtual:C}\nMétodo: {metodoPagamentoSelecionado}", "Venda Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
                LimparVendaAtual(); // Limpa a venda APÓS o sucesso
            }
        }





        // MÉTODO ATUALIZADO

        private void LimparVendaAtual()

        {

            itensVenda.Clear();

            PainelItensVenda.Children.Clear();

            AtualizarTotal();

            metodoPagamentoSelecionado = "";

            ResetarBordasBotoesPagamento();



            // Limpa o ComboBox de busca corretamente

            ComboBuscarProduto.Text = string.Empty;

            ComboBuscarProduto.SelectedItem = null;

            ComboBuscarProduto.ItemsSource = listaDeTodosOsProdutos; // Restaura lista completa

            ComboBuscarProduto.IsDropDownOpen = false;

        }

        #endregion



        #region Lógica de Operações de Caixa

        // ... (Seus métodos BtnSangria_Click e BtnAdicionarTroco_Click permanecem os mesmos) ...

        // Cole aqui os métodos da região Lógica de Operações de Caixa do seu código original

        private void BtnSangria_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            var r = CriarPopupEntradaValorMotivo("Sangria de Caixa", "Digite o valor a ser RETIRADO do caixa e o motivo.", (SolidColorBrush)new BrushConverter().ConvertFrom("#E67E22"));
            if (r != null)
            {
                // --- REGISTRA O VALOR RETIRADO ---
                this.valorSangria += r.Item1;
                // ---------------------------------

                // ... (sua chamada ao ControleVenda.SangriaCaixa e MessageBox) ...
                MessageBox.Show($"Sangria de {r.Item1:C} registrada com sucesso."); // Movido para fora do else
                ControleVenda controleVenda = new ControleVenda();
                controleVenda.SangriaCaixa(r.Item1.ToString().Replace(',', '.'), r.Item2);
                // if (controleVenda.mensagem != "") { /*...*/ }
            }
        }



        private void BtnAdicionarTroco_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            var r = CriarPopupEntradaValorMotivo("Adicionar Troco", "Digite o valor a ser ADICIONADO ao caixa e o motivo.", (SolidColorBrush)new BrushConverter().ConvertFrom("#2ECC71"));
            if (r != null)
            {
                // --- REGISTRA O VALOR ADICIONADO ---
                this.valorAdicionadoTroco += r.Item1;
                // -----------------------------------

                // ... (sua chamada ao ControleVenda.AdicionarTroca e MessageBox) ...
                MessageBox.Show($"Troco de {r.Item1:C} adicionado com sucesso."); // Movido para fora do else
                ControleVenda controleVenda = new ControleVenda();
                controleVenda.AdicionarTroca(r.Item1.ToString().Replace(',', '.'), r.Item2);
                // if(controleVenda.mensagem != "") { /*...*/ }
            }
        }

        #endregion



        #region Métodos de Criação de Popups

        // ... (Cole aqui os métodos CriarPopupAberturaCaixa, CriarPopupFechamentoCaixa, CriarPopupConfirmacaoSimples e CriarPopupEntradaValorMotivo) ...

        // Cole aqui os métodos da região Métodos de Criação de Popups do seu código original

        private decimal? CriarPopupAberturaCaixa()

        {

            var popupWindow = new Window { Title = "Abertura de Caixa", Width = 400, Height = 250, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize };

            var mainGrid = new Grid { Margin = new Thickness(20) };

            var contentStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

            var titleText = new TextBlock { Text = "Abrir Caixa", FontSize = 22, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 15) };

            var messageText = new TextBlock { Text = "Digite o valor inicial do troco (fundo de caixa):", Margin = new Thickness(0, 0, 0, 5) };

            var txtValorInicial = new TextBox { Name = "txtValorInicial", Height = 30, FontSize = 14, Padding = new Thickness(5) };

            txtValorInicial.PreviewTextInput += (s, args) => { args.Handled = !System.Text.RegularExpressions.Regex.IsMatch(args.Text, @"^[0-9]*(,|\.)?[0-9]*$"); };

            contentStack.Children.Add(titleText); contentStack.Children.Add(messageText); contentStack.Children.Add(txtValorInicial);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 20, 0, 0) };

            var btnCancelar = new Button { Content = "Cancelar", Width = 100, Height = 35, Margin = new Thickness(0, 0, 10, 0), IsCancel = true };

            var btnConfirmar = new Button { Content = "Confirmar", Width = 100, Height = 35, FontWeight = FontWeights.Bold, Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#4CAF50"), Foreground = Brushes.White, IsDefault = true };

            btnConfirmar.Click += (s, args) =>
            {
                if (decimal.TryParse(txtValorInicial.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal valor) && valor >= 0)
                {
                    ControleVenda controleVenda = new ControleVenda();

                    // --- ALTERAÇÃO AQUI ---
                    // Converte o valor para string e força a troca da vírgula por ponto
                    string valorFormatado = valor.ToString().Replace(',', '.');

                    controleVenda.AbrirCaixa(valorFormatado);
                    // ----------------------

                    popupWindow.DialogResult = true;
                    popupWindow.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, insira um valor monetário válido (ex: 50,00).", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };

            buttonPanel.Children.Add(btnCancelar); buttonPanel.Children.Add(btnConfirmar);

            contentStack.Children.Add(buttonPanel);

            mainGrid.Children.Add(contentStack);

            popupWindow.Content = mainGrid;

            if (popupWindow.ShowDialog() == true)

            {

                if (decimal.TryParse(txtValorInicial.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal valorFinal)) { return valorFinal; }

            }

            return null;

        }



        private bool? CriarPopupFechamentoCaixa(decimal valorEsperadoCaixa)
        {
            // --- Cores ---
            SolidColorBrush corConfirmar = (SolidColorBrush)new BrushConverter().ConvertFrom("#28A745");
            SolidColorBrush corConfirmarHover = (SolidColorBrush)new BrushConverter().ConvertFrom("#218838");
            SolidColorBrush corCancelar = (SolidColorBrush)new BrushConverter().ConvertFrom("#AAAAAA");
            SolidColorBrush corCancelarHover = (SolidColorBrush)new BrushConverter().ConvertFrom("#888888");
            SolidColorBrush corValorDisplay = Brushes.Gray;

            // --- Janela Popup (COM TAMANHO DEFINIDO) ---
            var popupWindow = new Window
            {
                Title = "Fechamento de Caixa",
                SizeToContent = SizeToContent.Height, // Ajusta a altura ao conteúdo
                Width = 450, // Define uma LARGURA FIXA
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow
            };

            // --- Layout ---
            var mainGrid = new Grid { Margin = new Thickness(25) };
            var contentStack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch // Deixa o StackPanel preencher o Grid
            };

            // Título (Centralizado)
            var titleText = new TextBlock
            {
                Text = "Fechamento de Caixa",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center, // <-- Centraliza
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Ícone (Centralizado)
            var iconText = new TextBlock
            {
                Text = "💰",
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center, // <-- Centraliza
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Valor ESPERADO (Centralizado)
            var labelValorEsperado = new TextBlock
            {
                Text = "Valor Esperado (Calculado):",
                FontSize = 12,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center, // <-- Centraliza
                Margin = new Thickness(0, 10, 0, 2)
            };

            var txtValorEsperadoDisplay = new TextBox
            {
                Text = valorEsperadoCaixa.ToString("C", CultureInfo.GetCultureInfo("pt-BR")),
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = corValorDisplay,
                TextAlignment = TextAlignment.Center, // <-- Centraliza o texto
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Valor Final (Alinhado à Esquerda)
            var labelValorFinal = new TextBlock
            {
                Text = "Digite o Valor Final Contado em Caixa (R$):",
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Left, // <-- Alinha à esquerda
                Margin = new Thickness(0, 0, 0, 5)
            };

            var txtValorFinalContado = new TextBox
            {
                Name = "txtValorFinalContado",
                Height = 35,
                FontSize = 16,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 25),
                HorizontalAlignment = HorizontalAlignment.Stretch // <-- Estica (comportamento padrão)
            };
            txtValorFinalContado.PreviewTextInput += (s, args) => { args.Handled = !new Regex(@"^[0-9]*(,|\.)?[0-9]*$").IsMatch(args.Text); };

            // Adiciona elementos
            contentStack.Children.Add(titleText);
            contentStack.Children.Add(iconText);
            contentStack.Children.Add(labelValorEsperado);
            contentStack.Children.Add(txtValorEsperadoDisplay);
            contentStack.Children.Add(labelValorFinal);
            contentStack.Children.Add(txtValorFinalContado);

            // --- Painel de Botões (Alinhado à Direita) ---
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right // <-- Alinha os botões à direita
            };

            // --- Botão Cancelar ---
            var btnCancelar = new Button
            {
                Content = "Cancelar",
                Width = 120,
                Height = 40,
                Margin = new Thickness(0, 0, 10, 0),
                Background = corCancelar,
                Foreground = Brushes.White,
                IsCancel = true,
                Padding = new Thickness(5),
                Cursor = Cursors.Hand
            };
            btnCancelar.MouseEnter += (s, e) => btnCancelar.Background = corCancelarHover;
            btnCancelar.MouseLeave += (s, e) => btnCancelar.Background = corCancelar;
            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };

            // --- Botão Confirmar ---
            var btnConfirmar = new Button
            {
                Content = "Confirmar Fechamento",
                Width = 180,
                Height = 40,
                FontWeight = FontWeights.Bold,
                Background = corConfirmar,
                Foreground = Brushes.White,
                IsDefault = true,
                Padding = new Thickness(5),
                Cursor = Cursors.Hand
            };
            btnConfirmar.MouseEnter += (s, e) => btnConfirmar.Background = corConfirmarHover;
            btnConfirmar.MouseLeave += (s, e) => btnConfirmar.Background = corConfirmar;
            btnConfirmar.Click += (s, args) =>
            {
                // ... (Sua lógica de clique para validar, calcular diferença e mostrar MessageBox) ...
                // (A lógica interna do clique não precisa mudar)
                if (!decimal.TryParse(txtValorFinalContado.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal valorFinalContado) || valorFinalContado < 0)
                { MessageBox.Show(popupWindow, "Por favor, insira um valor monetário válido...", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning); txtValorFinalContado.Focus(); return; }

                decimal diferenca = valorFinalContado - valorEsperadoCaixa;
                string tituloMsgBox = "Resultado do Fechamento";
               
                string msgResultado;
                MessageBoxImage iconeMsgBox = MessageBoxImage.Information;

                if (diferenca == 0) { msgResultado = "O caixa fechou sem diferença."; }
                else if (diferenca > 0) { msgResultado = $"SOBRA: O caixa fechou com {diferenca:C} a mais."; iconeMsgBox = MessageBoxImage.Warning; }
                else { msgResultado = $"FALTA: O caixa fechou com {(-diferenca):C} a menos."; iconeMsgBox = MessageBoxImage.Warning; }

                string msgCompleta = $"{msgResultado}\n\nValor Esperado: {valorEsperadoCaixa:C}\nValor Contado: {valorFinalContado:C}";
                MessageBox.Show(popupWindow, msgCompleta, tituloMsgBox, MessageBoxButton.OK, iconeMsgBox);

                ControleVenda controleVenda = new ControleVenda();
                string valorFormatado = valorFinalContado.ToString().Replace(',', '.');
                controleVenda.FecharCaixa(valorFormatado);

                popupWindow.DialogResult = true;
                popupWindow.Close();
            };

            // Adiciona os botões ao painel de botões
            buttonPanel.Children.Add(btnCancelar);
            buttonPanel.Children.Add(btnConfirmar);

            // Adiciona o painel de botões ao StackPanel principal
            contentStack.Children.Add(buttonPanel);

            // Adiciona o StackPanel ao Grid
            mainGrid.Children.Add(contentStack);
            popupWindow.Content = mainGrid;

            popupWindow.Loaded += (s, e) => txtValorFinalContado.Focus();

            return popupWindow.ShowDialog();
        }



        private Tuple<decimal, string> CriarPopupEntradaValorMotivo(string titulo, string mensagem, SolidColorBrush corBotaoConfirmar)

        {

            var popupWindow = new Window { Title = titulo, Width = 450, Height = 350, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize, Background = Brushes.WhiteSmoke };

            var mainGrid = new Grid { Margin = new Thickness(20) };

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var titleText = new TextBlock { Text = titulo, FontSize = 22, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 20) };

            Grid.SetRow(titleText, 0); mainGrid.Children.Add(titleText);

            var contentStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center }; Grid.SetRow(contentStack, 1);

            contentStack.Children.Add(new TextBlock { Text = mensagem, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 15) });

            contentStack.Children.Add(new TextBlock { Text = "Valor (R$):", FontWeight = FontWeights.SemiBold });

            var txtValor = new TextBox { Name = "txtValor", Height = 30, FontSize = 14, Padding = new Thickness(5), Margin = new Thickness(0, 5, 0, 15) };

            txtValor.PreviewTextInput += (s, args) => { args.Handled = !System.Text.RegularExpressions.Regex.IsMatch(args.Text, @"^[0-9]*(,|\.)?[0-9]*$"); };

            contentStack.Children.Add(txtValor);

            contentStack.Children.Add(new TextBlock { Text = "Motivo:", FontWeight = FontWeights.SemiBold });

            var txtMotivo = new TextBox { Name = "txtMotivo", Height = 30, FontSize = 14, Padding = new Thickness(5), Margin = new Thickness(0, 5, 0, 0) };

            contentStack.Children.Add(txtMotivo);

            mainGrid.Children.Add(contentStack);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right }; Grid.SetRow(buttonPanel, 2);

            var btnCancelar = new Button { Content = "Cancelar", Width = 120, Height = 40, Margin = new Thickness(0, 0, 10, 0), Background = Brushes.LightGray, IsCancel = true };

            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };

            var btnConfirmar = new Button { Content = "Confirmar", Width = 120, Height = 40, Background = corBotaoConfirmar, Foreground = Brushes.White, FontWeight = FontWeights.Bold, IsDefault = true };

            btnConfirmar.Click += (s, args) =>
            {

                if (!decimal.TryParse(txtValor.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal valor) || valor <= 0)

                { MessageBox.Show("Por favor, insira um valor numérico válido e maior que zero.", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (string.IsNullOrWhiteSpace(txtMotivo.Text)) { MessageBox.Show("O motivo não pode estar em branco.", "Motivo Inválido", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                popupWindow.DialogResult = true; popupWindow.Close();

            };

            buttonPanel.Children.Add(btnCancelar); buttonPanel.Children.Add(btnConfirmar);

            mainGrid.Children.Add(buttonPanel);

            popupWindow.Content = mainGrid;

            if (popupWindow.ShowDialog() == true)

            {

                if (decimal.TryParse(txtValor.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal valorFinal)) { return new Tuple<decimal, string>(valorFinal, txtMotivo.Text); }

            }

            return null;

        }

        #endregion

    }
}