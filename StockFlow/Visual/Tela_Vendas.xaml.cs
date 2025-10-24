using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StockFlow.Visual
{
    public partial class Tela_Vendas : Window
    {
        #region Classes de Dados
        private class Produto { public string Nome { get; set; } public decimal Preco { get; set; } public override string ToString() => $"{Nome} - R$ {Preco:F2}"; }
        private class ItemVenda { public string Nome { get; set; } public decimal PrecoUnitario { get; set; } public int Quantidade { get; set; } public decimal PrecoTotal => PrecoUnitario * Quantidade; }
        #endregion

        #region Variáveis de Controle
        private List<Produto> listaCompletaProdutos = new List<Produto>();
        private List<ItemVenda> itensVenda = new List<ItemVenda>();
        private string metodoPagamentoSelecionado = "";
        private bool isCaixaAberto = false;
        #endregion

        public Tela_Vendas()
        {
            InitializeComponent();
            CarregarProdutos();
            AtualizarEstadoVisualCaixa(false);
        }

        #region LÓGICA DE ESTADO DO CAIXA

        private void AtualizarEstadoVisualCaixa(bool caixaAberto)
        {
            if (caixaAberto)
            {
                BtnAbrirCaixa.Visibility = Visibility.Collapsed;
                BtnFecharCaixa.Visibility = Visibility.Visible;
                GridVendaPrincipal.Opacity = 1.0;
            }
            else
            {
                BtnAbrirCaixa.Visibility = Visibility.Visible;
                BtnFecharCaixa.Visibility = Visibility.Collapsed;
                GridVendaPrincipal.Opacity = 0.5;
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
                isCaixaAberto = true;
                AtualizarEstadoVisualCaixa(true);
                MessageBox.Show($"Caixa aberto com sucesso com um valor inicial de {valorInicial.Value:C}!", "Caixa Aberto", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;

            SolidColorBrush laranjaConfirmar = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9800"));
            bool fecharConfirmado = CriarPopupConfirmacaoSimples("Fechar Caixa", "Deseja realmente fechar o caixa?", "Sim, Fechar", laranjaConfirmar);
            if (fecharConfirmado)
            {
                MessageBox.Show("Caixa fechado com sucesso!", "Fechamento de Caixa", MessageBoxButton.OK, MessageBoxImage.Information);
                isCaixaAberto = false;
                LimparVendaAtual();
                AtualizarEstadoVisualCaixa(false);
            }
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            if (isCaixaAberto)
            {
                MessageBox.Show("Você precisa fechar o caixa antes de sair.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.Close();
        }
        #endregion

        #region Lógica de Venda
        private void CarregarProdutos()
        {
            listaCompletaProdutos = new List<Produto> { new Produto { Nome = "Água Mineral 500ml", Preco = 3.00m } };
            ComboProdutos.ItemsSource = listaCompletaProdutos;
        }

        private void TxtBuscarProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AlertaCaixaFechado()) { TxtBuscarProduto.Clear(); return; }
            string textoBusca = TxtBuscarProduto.Text.ToLower();
            ComboProdutos.ItemsSource = string.IsNullOrWhiteSpace(textoBusca) ? listaCompletaProdutos : listaCompletaProdutos.Where(p => p.Nome.ToLower().Contains(textoBusca)).ToList();
            ComboProdutos.IsDropDownOpen = true;
        }

        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            if (ComboProdutos.SelectedItem == null) { MessageBox.Show("Por favor, selecione um produto!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            var produtoSelecionado = ComboProdutos.SelectedItem as Produto;
            var itemExistente = itensVenda.FirstOrDefault(i => i.Nome == produtoSelecionado.Nome);
            if (itemExistente != null) { itemExistente.Quantidade++; }
            else { itensVenda.Add(new ItemVenda { Nome = produtoSelecionado.Nome, PrecoUnitario = produtoSelecionado.Preco, Quantidade = 1 }); }
            AtualizarListaItens();
            AtualizarTotal();
            TxtBuscarProduto.Clear();
            ComboProdutos.SelectedItem = null;
        }

        private void AtualizarListaItens()
        {
            PainelItensVenda.Children.Clear();
            foreach (var item in itensVenda) { AdicionarItemVisual(item); }
        }

        private void AdicionarItemVisual(ItemVenda item)
        {
            var itemBorder = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(5), Margin = new Thickness(0, 0, 5, 0) };
            var itemGrid = new Grid();
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var infoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            infoPanel.Children.Add(new TextBlock { Text = item.Nome, FontWeight = FontWeights.SemiBold });
            infoPanel.Children.Add(new TextBlock { Text = $"{item.Quantidade}x R$ {item.PrecoUnitario:F2} = R$ {item.PrecoTotal:F2}", Foreground = Brushes.Gray });
            Grid.SetColumn(infoPanel, 0);
            var qtdPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var btnDiminuir = new Button { Content = "-", Width = 25, Height = 25, Margin = new Thickness(5, 0, 5, 0) };
            var btnAumentar = new Button { Content = "+", Width = 25, Height = 25, Margin = new Thickness(5, 0, 5, 0) };
            btnDiminuir.Click += (s, ev) => DiminuirQuantidade(item);
            btnAumentar.Click += (s, ev) => AumentarQuantidade(item);
            qtdPanel.Children.Add(btnDiminuir);
            qtdPanel.Children.Add(btnAumentar);
            Grid.SetColumn(qtdPanel, 1);
            var btnRemover = new Button { Content = "✕", Width = 25, Height = 25, Foreground = Brushes.Red, FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0), Background = Brushes.Transparent, BorderBrush = Brushes.Transparent };
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
        private void AtualizarTotal() { TxtTotalVenda.Text = itensVenda.Sum(i => i.PrecoTotal).ToString("C"); }
        #endregion

        #region Lógica de Pagamento e Finalização
        private void BtnPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            BtnPagamentoCartao.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3498DB"));
            BtnPagamentoDinheiro.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#229954"));
            BtnPagamentoPix.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E67E22"));
            BtnPagamentoCartao.BorderThickness = new Thickness(2);
            BtnPagamentoDinheiro.BorderThickness = new Thickness(2);
            BtnPagamentoPix.BorderThickness = new Thickness(2);
            Button botaoClicado = sender as Button;
            botaoClicado.BorderBrush = Brushes.Gold;
            botaoClicado.BorderThickness = new Thickness(4);
            metodoPagamentoSelecionado = botaoClicado.Content.ToString();
        }

        private void BtnFinalizarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            if (itensVenda.Count == 0) { MessageBox.Show("Adicione pelo menos um item para finalizar a venda.", "Venda Vazia", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (string.IsNullOrEmpty(metodoPagamentoSelecionado)) { MessageBox.Show("Por favor, selecione um método de pagamento.", "Pagamento não Selecionado", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            MessageBox.Show($"Venda finalizada com sucesso!\nTotal: {TxtTotalVenda.Text}\nMétodo: {metodoPagamentoSelecionado}", "Venda Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            LimparVendaAtual();
        }

        private void LimparVendaAtual()
        {
            itensVenda.Clear();
            PainelItensVenda.Children.Clear();
            AtualizarTotal();
            metodoPagamentoSelecionado = "";
            BtnPagamentoCartao.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3498DB"));
            BtnPagamentoDinheiro.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#229954"));
            BtnPagamentoPix.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E67E22"));
            BtnPagamentoCartao.BorderThickness = new Thickness(2);
            BtnPagamentoDinheiro.BorderThickness = new Thickness(2);
            BtnPagamentoPix.BorderThickness = new Thickness(2);
            TxtBuscarProduto.Clear();
        }
        #endregion

        #region Lógica de Operações de Caixa
        private void BtnSangria_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            var r = CriarPopupEntradaValorMotivo("Sangria de Caixa", "Digite o valor a ser RETIRADO do caixa e o motivo.", (SolidColorBrush)new BrushConverter().ConvertFrom("#E67E22"));
            if (r != null) MessageBox.Show($"Sangria de {r.Item1:C} registrada.");
        }

        private void BtnAdicionarTroco_Click(object sender, RoutedEventArgs e)
        {
            if (AlertaCaixaFechado()) return;
            var r = CriarPopupEntradaValorMotivo("Adicionar Troco", "Digite o valor a ser ADICIONADO ao caixa e o motivo.", (SolidColorBrush)new BrushConverter().ConvertFrom("#2ECC71"));
            if (r != null) MessageBox.Show($"Troco de {r.Item1:C} adicionado.");
        }
        #endregion

        #region Métodos de Criação de Popups
        private decimal? CriarPopupAberturaCaixa()
        {
            var popupWindow = new Window { Title = "Abertura de Caixa", Width = 400, Height = 250, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize };
            var mainGrid = new Grid { Margin = new Thickness(20) };
            var contentStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            var titleText = new TextBlock { Text = "Abrir Caixa", FontSize = 22, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 15) };
            var messageText = new TextBlock { Text = "Digite o valor inicial do troco (fundo de caixa):", Margin = new Thickness(0, 0, 0, 5) };
            var txtValorInicial = new TextBox { Name = "txtValorInicial", Height = 30, FontSize = 14, Padding = new Thickness(5) };
            contentStack.Children.Add(titleText); contentStack.Children.Add(messageText); contentStack.Children.Add(txtValorInicial);
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 20, 0, 0) };
            var btnCancelar = new Button { Content = "Cancelar", Width = 100, Height = 35, Margin = new Thickness(0, 0, 10, 0) };
            var btnConfirmar = new Button { Content = "Confirmar", Width = 100, Height = 35, FontWeight = FontWeights.Bold, Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#4CAF50"), Foreground = Brushes.White };
            btnConfirmar.Click += (s, args) => { if (decimal.TryParse(txtValorInicial.Text, out decimal valor) && valor >= 0) { popupWindow.DialogResult = true; popupWindow.Close(); } else { MessageBox.Show("Por favor, insira um valor monetário válido (ex: 50,00).", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Error); } };
            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };
            buttonPanel.Children.Add(btnCancelar); buttonPanel.Children.Add(btnConfirmar);
            contentStack.Children.Add(buttonPanel);
            mainGrid.Children.Add(contentStack);
            popupWindow.Content = mainGrid;
            if (popupWindow.ShowDialog() == true) { return decimal.Parse(txtValorInicial.Text); }
            return null;
        }

        private bool CriarPopupConfirmacaoSimples(string titulo, string mensagem, string textoBotaoConfirmar, SolidColorBrush corIcone)
        {
            var popupWindow = new Window { Title = titulo, Width = 400, Height = 250, WindowStyle = WindowStyle.ToolWindow, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize, Background = Brushes.White };
            var mainGrid = new Grid { Margin = new Thickness(20) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var iconStack = new StackPanel { Margin = new Thickness(0, 0, 0, 10), HorizontalAlignment = HorizontalAlignment.Center }; Grid.SetRow(iconStack, 0);
            var iconText = new TextBlock { Text = "💵", FontSize = 48, HorizontalAlignment = HorizontalAlignment.Center, Foreground = corIcone }; iconStack.Children.Add(iconText);
            var titleText = new TextBlock { Text = titulo, FontSize = 24, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Center }; iconStack.Children.Add(titleText);
            mainGrid.Children.Add(iconStack);
            var msgText = new TextBlock { Text = mensagem, FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.Gray }; Grid.SetRow(msgText, 1);
            mainGrid.Children.Add(msgText);
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center }; Grid.SetRow(buttonPanel, 3);
            var buttonStyle = (Style)this.FindResource("ButtonStyle");
            var btnCancelar = new Button { Content = "Cancelar", Width = 120, Height = 40, Margin = new Thickness(10, 0, 10, 10), Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE0E0E0"), Style = buttonStyle, BorderBrush = Brushes.Gray, IsCancel = true };
            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; };
            var btnConfirmar = new Button { Content = textoBotaoConfirmar, Width = 120, Height = 40, Margin = new Thickness(10, 0, 10, 10), Foreground = Brushes.White, Background = corIcone, Style = buttonStyle, BorderBrush = corIcone, IsDefault = true };
            btnConfirmar.Click += (s, args) => { popupWindow.DialogResult = true; };
            buttonPanel.Children.Add(btnCancelar); buttonPanel.Children.Add(btnConfirmar); mainGrid.Children.Add(buttonPanel);
            popupWindow.Content = mainGrid;
            return popupWindow.ShowDialog() == true;
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
            contentStack.Children.Add(txtValor);
            contentStack.Children.Add(new TextBlock { Text = "Motivo:", FontWeight = FontWeights.SemiBold });
            var txtMotivo = new TextBox { Name = "txtMotivo", Height = 30, FontSize = 14, Padding = new Thickness(5), Margin = new Thickness(0, 5, 0, 0) };
            contentStack.Children.Add(txtMotivo);
            mainGrid.Children.Add(contentStack);
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right }; Grid.SetRow(buttonPanel, 2);
            var btnCancelar = new Button { Content = "Cancelar", Width = 120, Height = 40, Margin = new Thickness(0, 0, 10, 0), Background = Brushes.LightGray };
            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };
            var btnConfirmar = new Button { Content = "Confirmar", Width = 120, Height = 40, Background = corBotaoConfirmar, Foreground = Brushes.White, FontWeight = FontWeights.Bold };
            btnConfirmar.Click += (s, args) => { if (!decimal.TryParse(txtValor.Text, out decimal valor) || valor <= 0) { MessageBox.Show("Por favor, insira um valor numérico válido e maior que zero.", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Error); return; } if (string.IsNullOrWhiteSpace(txtMotivo.Text)) { MessageBox.Show("O motivo não pode estar em branco.", "Motivo Inválido", MessageBoxButton.OK, MessageBoxImage.Error); return; } popupWindow.DialogResult = true; popupWindow.Close(); };
            buttonPanel.Children.Add(btnCancelar); buttonPanel.Children.Add(btnConfirmar);
            mainGrid.Children.Add(buttonPanel);
            popupWindow.Content = mainGrid;
            if (popupWindow.ShowDialog() == true) { return new Tuple<decimal, string>(decimal.Parse(txtValor.Text), txtMotivo.Text); }
            return null;
        }
        #endregion
    }
}