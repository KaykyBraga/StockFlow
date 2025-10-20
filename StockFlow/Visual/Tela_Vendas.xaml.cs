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
        public Tela_Vendas()
        {
            InitializeComponent();
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