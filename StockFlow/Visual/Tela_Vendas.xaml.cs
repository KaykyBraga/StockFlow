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
        // ... (Classes Produto e ItemVenda permanecem as mesmas)
        private class Produto
        {
            public string Nome { get; set; }
            public decimal Preco { get; set; }
            public override string ToString() => $"{Nome} - R$ {Preco:F2}";
        }

        private class ItemVenda
        {
            public string Nome { get; set; }
            public decimal PrecoUnitario { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoTotal => PrecoUnitario * Quantidade;
        }

        private List<Produto> listaCompletaProdutos = new List<Produto>();
        private List<ItemVenda> itensVenda = new List<ItemVenda>();
        private decimal totalVenda = 0;

        public Tela_Vendas()
        {
            InitializeComponent();
            CarregarProdutos();
        }

        // ... (Seus métodos existentes: CarregarProdutos, TxtBuscarProduto_TextChanged, BtnAdicionar_Click, etc., permanecem os mesmos)

        #region Métodos de Venda (Existentes - Sem Alteração)

        private void CarregarProdutos()
        {
            // É AQUI que você deve adicionar sua lógica para buscar os produtos do banco de dados
            // e preencher a 'listaCompletaProdutos'.
            // Exemplo: listaCompletaProdutos = seuBancoDeDados.GetProdutos();
            ComboProdutos.ItemsSource = listaCompletaProdutos;
        }

        private void TxtBuscarProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textoBusca = TxtBuscarProduto.Text.ToLower();
            if (string.IsNullOrWhiteSpace(textoBusca))
            {
                ComboProdutos.ItemsSource = listaCompletaProdutos;
            }
            else
            {
                ComboProdutos.ItemsSource = listaCompletaProdutos
                    .Where(p => p.Nome.ToLower().Contains(textoBusca))
                    .ToList();
            }
            ComboProdutos.IsDropDownOpen = true;
        }

        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (ComboProdutos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um produto!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var produtoSelecionado = ComboProdutos.SelectedItem as Produto;
            var itemExistente = itensVenda.FirstOrDefault(i => i.Nome == produtoSelecionado.Nome);
            if (itemExistente != null)
            {
                itemExistente.Quantidade++;
                AtualizarListaItens();
            }
            else
            {
                var novoItem = new ItemVenda
                {
                    Nome = produtoSelecionado.Nome,
                    PrecoUnitario = produtoSelecionado.Preco,
                    Quantidade = 1
                };
                itensVenda.Add(novoItem);
                AdicionarItemVisual(novoItem);
            }
            AtualizarTotal();
            TxtBuscarProduto.Clear();
            ComboProdutos.SelectedItem = null;
        }

        private void AdicionarItemVisual(ItemVenda item) { /* ... seu código aqui ... */ }
        private void AumentarQuantidade(ItemVenda item) { /* ... seu código aqui ... */ }
        private void DiminuirQuantidade(ItemVenda item) { /* ... seu código aqui ... */ }
        private void AtualizarListaItens() { /* ... seu código aqui ... */ }
        private void AtualizarTotal() { /* ... seu código aqui ... */ }
        private void BtnFecharCaixa_Click(object sender, RoutedEventArgs e) { /* ... seu código aqui ... */ }

        #endregion

        #region NOVOS MÉTODOS PARA SANGRIA E TROCO

        private void BtnSangria_Click(object sender, RoutedEventArgs e)
        {
            // Chama o popup genérico para a operação de Sangria
            var resultado = CriarPopupEntradaValorMotivo(
                "Sangria de Caixa",
                "Digite o valor a ser RETIRADO do caixa e o motivo.",
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#E67E22")) // Cor Laranja
            );

            if (resultado != null)
            {
                decimal valor = resultado.Item1;
                string motivo = resultado.Item2;

                // AQUI você adicionaria a lógica para salvar essa operação no banco de dados.
                // Ex: seuBancoDeDados.RegistrarMovimentacaoCaixa("Sangria", valor, motivo);

                MessageBox.Show($"Sangria de R$ {valor:F2} registrada com sucesso!\nMotivo: {motivo}",
                                "Operação Realizada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAdicionarTroco_Click(object sender, RoutedEventArgs e)
        {
            // Chama o popup genérico para a operação de Adicionar Troco
            var resultado = CriarPopupEntradaValorMotivo(
                "Adicionar Troco",
                "Digite o valor a ser ADICIONADO ao caixa e o motivo (ex: 'início de turno').",
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#2ECC71")) // Cor Verde
            );

            if (resultado != null)
            {
                decimal valor = resultado.Item1;
                string motivo = resultado.Item2;

                // AQUI você adicionaria a lógica para salvar essa operação no banco de dados.
                // Ex: seuBancoDeDados.RegistrarMovimentacaoCaixa("AdicaoTroco", valor, motivo);

                MessageBox.Show($"Troco de R$ {valor:F2} adicionado com sucesso!\nMotivo: {motivo}",
                                "Operação Realizada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Método genérico para criar um popup que solicita um valor monetário e um motivo.
        /// </summary>
        /// <returns>Um Tuple com (decimal valor, string motivo) ou null se cancelado.</returns>
        private Tuple<decimal, string> CriarPopupEntradaValorMotivo(string titulo, string mensagem, SolidColorBrush corBotaoConfirmar)
        {
            // Cria a janela do popup
            var popupWindow = new Window
            {
                Title = titulo,
                Width = 450,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.WhiteSmoke
            };

            var mainGrid = new Grid { Margin = new Thickness(20) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Título
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Conteúdo
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Botões

            // Título
            var titleText = new TextBlock
            {
                Text = titulo,
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(titleText, 0);
            mainGrid.Children.Add(titleText);

            // Conteúdo (inputs)
            var contentStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(contentStack, 1);

            contentStack.Children.Add(new TextBlock { Text = mensagem, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 15) });

            contentStack.Children.Add(new TextBlock { Text = "Valor (R$):", FontWeight = FontWeights.SemiBold });
            var txtValor = new TextBox { Name = "txtValor", Height = 30, FontSize = 14, Padding = new Thickness(5), Margin = new Thickness(0, 5, 0, 15) };
            contentStack.Children.Add(txtValor);

            contentStack.Children.Add(new TextBlock { Text = "Motivo:", FontWeight = FontWeights.SemiBold });
            var txtMotivo = new TextBox { Name = "txtMotivo", Height = 30, FontSize = 14, Padding = new Thickness(5), Margin = new Thickness(0, 5, 0, 0) };
            contentStack.Children.Add(txtMotivo);

            mainGrid.Children.Add(contentStack);

            // Botões
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            Grid.SetRow(buttonPanel, 2);

            var btnCancelar = new Button
            {
                Content = "Cancelar",
                Width = 120,
                Height = 40,
                Margin = new Thickness(0, 0, 10, 0),
                Background = Brushes.LightGray
            };
            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };

            var btnConfirmar = new Button
            {
                Content = "Confirmar",
                Width = 120,
                Height = 40,
                Background = corBotaoConfirmar,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            btnConfirmar.Click += (s, args) =>
            {
                if (!decimal.TryParse(txtValor.Text, out decimal valor) || valor <= 0)
                {
                    MessageBox.Show("Por favor, insira um valor numérico válido e maior que zero.", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show("O motivo não pode estar em branco.", "Motivo Inválido", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                popupWindow.DialogResult = true;
                popupWindow.Close();
            };

            buttonPanel.Children.Add(btnCancelar);
            buttonPanel.Children.Add(btnConfirmar);
            mainGrid.Children.Add(buttonPanel);

            popupWindow.Content = mainGrid;

            // Mostra o popup e aguarda o resultado
            bool? resultadoDialog = popupWindow.ShowDialog();

            if (resultadoDialog == true)
            {
                // Retorna os valores se o usuário confirmou
                return new Tuple<decimal, string>(decimal.Parse(txtValor.Text), txtMotivo.Text);
            }

            // Retorna null se o usuário cancelou
            return null;
        }

        #endregion
    }
}