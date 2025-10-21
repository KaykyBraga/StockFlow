using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockFlow.Visual
{
    /// <summary>
    /// Lógica interna para Tela_Vendas.xaml
    /// </summary>
    public partial class Tela_Vendas : Window
    {
        // Classe para representar um item da venda
        private class ItemVenda
        {
            public string Nome { get; set; }
            public decimal PrecoUnitario { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoTotal => PrecoUnitario * Quantidade;
        }

        private List<ItemVenda> itensVenda = new List<ItemVenda>();
        private decimal totalVenda = 0;

        public Tela_Vendas()
        {
            InitializeComponent();
            CarregarProdutos(); // Você pode implementar para carregar do banco de dados
        }

        private void CarregarProdutos()
        {
            // Exemplo: adicionar produtos ao ComboBox
            // Você deve substituir isso pela lógica de carregar do banco de dados
            ComboProdutos.Items.Add(new ComboBoxItem { Content = "Produto A - R$ 10,00", Tag = new { Nome = "Produto A", Preco = 10.00m } });
            ComboProdutos.Items.Add(new ComboBoxItem { Content = "Produto B - R$ 25,00", Tag = new { Nome = "Produto B", Preco = 25.00m } });
            ComboProdutos.Items.Add(new ComboBoxItem { Content = "Produto C - R$ 15,50", Tag = new { Nome = "Produto C", Preco = 15.50m } });
        }

        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (ComboProdutos.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, selecione um produto!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var itemSelecionado = ComboProdutos.SelectedItem as ComboBoxItem;
            dynamic produtoData = itemSelecionado.Tag;

            string nomeProduto = produtoData.Nome;
            decimal precoProduto = produtoData.Preco;

            // Verifica se o produto já está na lista
            var itemExistente = itensVenda.FirstOrDefault(i => i.Nome == nomeProduto);
            if (itemExistente != null)
            {
                itemExistente.Quantidade++;
                AtualizarListaItens();
            }
            else
            {
                var novoItem = new ItemVenda
                {
                    Nome = nomeProduto,
                    PrecoUnitario = precoProduto,
                    Quantidade = 1
                };
                itensVenda.Add(novoItem);
                AdicionarItemVisual(novoItem);
            }

            AtualizarTotal();
        }

        private void AdicionarItemVisual(ItemVenda item)
        {
            // Cria o Border para o item
            Border itemBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10),
                Background = new SolidColorBrush(Color.FromRgb(249, 249, 249)),
                Tag = item // Armazena referência ao item
            };

            // Grid principal do item
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Nome do produto
            TextBlock txtNome = new TextBlock
            {
                Text = item.Nome,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(txtNome, 0);
            mainGrid.Children.Add(txtNome);

            // Grid para controles (quantidade e preço)
            Grid controlGrid = new Grid();
            controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Grid.SetRow(controlGrid, 1);

            // StackPanel para os botões de quantidade
            StackPanel quantityPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            Grid.SetColumn(quantityPanel, 0);

            // Botão diminuir
            Button btnDiminuir = new Button
            {
                Content = "-",
                Width = 35,
                Height = 35,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(192, 57, 43)),
                BorderThickness = new Thickness(2),
                Cursor = Cursors.Hand
            };
            btnDiminuir.Click += (s, args) => DiminuirQuantidade(item);

            // TextBlock quantidade
            TextBlock txtQuantidade = new TextBlock
            {
                Text = item.Quantidade.ToString(),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 0, 15, 0),
                MinWidth = 30,
                TextAlignment = TextAlignment.Center,
                Tag = item // Para atualizar depois
            };

            // Botão aumentar
            Button btnAumentar = new Button
            {
                Content = "+",
                Width = 35,
                Height = 35,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(34, 153, 84)),
                BorderThickness = new Thickness(2),
                Cursor = Cursors.Hand
            };
            btnAumentar.Click += (s, args) => AumentarQuantidade(item);

            quantityPanel.Children.Add(btnDiminuir);
            quantityPanel.Children.Add(txtQuantidade);
            quantityPanel.Children.Add(btnAumentar);
            controlGrid.Children.Add(quantityPanel);

            // TextBlock preço
            TextBlock txtPreco = new TextBlock
            {
                Text = $"R$ {item.PrecoTotal:F2}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                VerticalAlignment = VerticalAlignment.Center,
                Tag = item // Para atualizar depois
            };
            Grid.SetColumn(txtPreco, 2);
            controlGrid.Children.Add(txtPreco);

            mainGrid.Children.Add(controlGrid);
            itemBorder.Child = mainGrid;

            PainelItensVenda.Children.Add(itemBorder);
        }

        private void AumentarQuantidade(ItemVenda item)
        {
            item.Quantidade++;
            AtualizarListaItens();
            AtualizarTotal();
        }

        private void DiminuirQuantidade(ItemVenda item)
        {
            if (item.Quantidade > 1)
            {
                item.Quantidade--;
                AtualizarListaItens();
                AtualizarTotal();
            }
            else
            {
                // Remove o item se a quantidade for 0
                var result = MessageBox.Show(
                    "Deseja remover este item da venda?",
                    "Confirmar Remoção",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    itensVenda.Remove(item);
                    AtualizarListaItens();
                    AtualizarTotal();
                }
            }
        }

        private void AtualizarListaItens()
        {
            // Limpa e reconstrói a lista visual
            PainelItensVenda.Children.Clear();
            foreach (var item in itensVenda)
            {
                AdicionarItemVisual(item);
            }
        }

        private void AtualizarTotal()
        {
            totalVenda = itensVenda.Sum(i => i.PrecoTotal);
            TxtTotalVenda.Text = $"R$ {totalVenda:F2}";
        }

        private void BtnFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            // Cria a janela de popup personalizada
            var popupWindow = new Window
            {
                Title = "Confirmação de Fechamento de Caixa",
                Width = 450,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
            };

            // Grid principal do popup
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Conteúdo da mensagem
            var contentStack = new StackPanel
            {
                Margin = new Thickness(30),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Ícone de aviso
            var iconText = new TextBlock
            {
                Text = "⚠️",
                FontSize = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };

            // Título
            var titleText = new TextBlock
            {
                Text = "Fechar Caixa",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = Brushes.Black
            };

            // Mensagem
            var messageText = new TextBlock
            {
                Text = "Deseja realmente fechar o caixa?",
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102))
            };

            contentStack.Children.Add(iconText);
            contentStack.Children.Add(titleText);
            contentStack.Children.Add(messageText);

            Grid.SetRow(contentStack, 0);
            mainGrid.Children.Add(contentStack);

            // Painel de botões
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            };

            // Botão Cancelar
            var btnCancelar = new Button
            {
                Content = "Cancelar",
                Width = 120,
                Height = 40,
                Margin = new Thickness(0, 0, 15, 0),
                Background = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                Foreground = Brushes.Black,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnCancelar.Click += (s, args) =>
            {
                popupWindow.DialogResult = false;
                popupWindow.Close();
            };

            // Botão Confirmar
            var btnConfirmar = new Button
            {
                Content = "Confirmar",
                Width = 120,
                Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnConfirmar.Click += (s, args) =>
            {
                popupWindow.DialogResult = true;
                popupWindow.Close();
            };

            buttonPanel.Children.Add(btnCancelar);
            buttonPanel.Children.Add(btnConfirmar);

            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);

            popupWindow.Content = mainGrid;

            // Mostra o popup e aguarda resposta
            bool? resultado = popupWindow.ShowDialog();

            if (resultado == true)
            {
                MessageBox.Show(
                    "Caixa fechado com sucesso!\n\nResumo do dia será gerado.",
                    "Caixa Fechado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}