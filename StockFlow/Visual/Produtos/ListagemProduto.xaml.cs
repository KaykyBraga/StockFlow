using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO; // Necessário para StringReader
using System.Xml; // Necessário para XmlReader
using System.Windows.Markup;
using System.Xml;
using StockFlow.Controles;

namespace StockFlow.Visual.Produtos
{
    public class Produto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Quantidade { get; set; }
        public string Categoria { get; set; }
    }

    // ✅ CORREÇÃO AQUI: Adicionado ": UserControl"
    public partial class ListagemProduto : UserControl
    {
        private List<Produto> listaDeProdutos;

        public ListagemProduto()
        {
            InitializeComponent();
            CarregarProduto();
        }

        private async void CarregarProduto()
        {
            listaDeProdutos = new List<Produto>();
            ControleEstoque controleEstoque = new ControleEstoque();
            listaDeProdutos = await controleEstoque.ObterTodosOsProdutosParaOGridAtivosAsync();

            DgProdutos.ItemsSource = listaDeProdutos;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Implemente aqui a lógica de filtragem da tabela.
            string termoBusca = TxtBusca.Text.ToLower();
            //MessageBox.Show($"Iniciando busca por: {termoBusca}");
            // Se a barra de pesquisa estiver vazia, mostramos a lista COMPLETA de novo.
            if (string.IsNullOrWhiteSpace(termoBusca))
            {
                DgProdutos.ItemsSource =  listaDeProdutos;
                return;
            }

            // Filtra a lista mestra usando o termo da busca.
            var listaFiltrada = listaDeProdutos.Where(item =>

                // Defina aqui EM QUAIS COLUNAS você quer pesquisar
                item.Nome.ToLower().Contains(termoBusca)  // Busca no Código
                

            ).ToList();

            // Atualiza o DataGrid para mostrar APENAS os itens da lista filtrada.
            DgProdutos.ItemsSource = listaFiltrada;
        }

        private void Button_Click_Adicionar(object sender, RoutedEventArgs e)
        {
            // Você precisa garantir que seu XAML tenha o botão de adicionar com este evento:
            // <Button Content="Adicionar" Click="Button_Click_Adicionar"/>

            Produto produtoSelecionado = (sender as Button).DataContext as Produto;

            if (produtoSelecionado != null)
            {
                // 1. Confirmação inicial via MessageBox
                MessageBoxResult resultadoMsg = MessageBox.Show(
                    $"Deseja adicionar estoque para o produto '{produtoSelecionado.Nome}'?",
                    "Confirmar Entrada",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultadoMsg == MessageBoxResult.Yes)
                {
                    // 2. Chama o PopUp customizado para obter a quantidade a adicionar
                    int? quantidadeAdicionada = CriarPopupAdicionarEstoque(produtoSelecionado);

                    if (quantidadeAdicionada.HasValue)
                    {
                        // 3. Processa a adição
                        if (int.TryParse(produtoSelecionado.Quantidade, out int quantidadeAtual))
                        {
                            int novaQuantidade = quantidadeAtual + quantidadeAdicionada.Value;
                            ControleEstoque controleEstoque = new ControleEstoque();
                            controleEstoque.AdicionarProduto(new List<string> { produtoSelecionado.Id, quantidadeAdicionada.Value.ToString() });
                            // ATUALIZAÇÃO DO MODELO
                            produtoSelecionado.Quantidade = novaQuantidade.ToString();

                            // Atualiza visualmente o DataGrid
                            DgProdutos.Items.Refresh();

                            MessageBox.Show(
                                $"Estoque de '{produtoSelecionado.Nome}' atualizado com sucesso!\n" +
                                $"{quantidadeAdicionada.Value} unidades adicionadas.\n" +
                                $"Novo Total: {novaQuantidade} unidades.",
                                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                           
                        }
                    }
                }
            }
        }

        // Adicione este método dentro da classe ListagemProduto : UserControl
        private Style GetBaseButtonStyle()
        {
            // Corrigido: Usar XamlReader para carregar o template, eliminando FrameworkElementFactory

            // 1. Definição do ControlTemplate como uma string XAML
            string templateXaml = @"
        <Style xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
               xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
               TargetType='{x:Type Button}'>
            <Setter Property='Cursor' Value='Hand'/>
            <Setter Property='Padding' Value='10, 5'/>
            <Setter Property='BorderThickness' Value='0'/>

            <Setter Property='Template'>
                <Setter.Value>
                    <ControlTemplate TargetType='{x:Type Button}'>
                        <Border x:Name='ButtonBorder' 
                                Background='{TemplateBinding Background}' 
                                BorderBrush='{TemplateBinding BorderBrush}' 
                                BorderThickness='{TemplateBinding BorderThickness}'
                                CornerRadius='5'>
                            <ContentPresenter HorizontalAlignment='Center' 
                                              VerticalAlignment='Center'
                                              Margin='{TemplateBinding Padding}'/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>";

            // 2. Carregar a string XAML como um objeto Style
            using (StringReader stringReader = new StringReader(templateXaml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    // O XamlReader.Load retorna o Style
                    return (Style)XamlReader.Load(xmlReader);
                }
            }
        }

        private int? CriarPopupAdicionarEstoque(Produto produto)
        {
            // Tenta obter o estoque atual como número
            if (!int.TryParse(produto.Quantidade, out int estoqueAtual))
            {
                MessageBox.Show("Erro: A quantidade de estoque atual é inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            int adicaoAtual = 0; // VARIÁVEL DE CONTROLE LOCAL

            // --- CORES ---
            SolidColorBrush corVerde = (SolidColorBrush)new BrushConverter().ConvertFrom("#4CAF50");
            SolidColorBrush corVerdeHover = (SolidColorBrush)new BrushConverter().ConvertFrom("#388E3C");
            SolidColorBrush corCinza = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE0E0E0");
            SolidColorBrush corCinzaHover = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFC0C0C0");
            SolidColorBrush corTextoEscuro = (SolidColorBrush)new BrushConverter().ConvertFrom("#333333");

            Style baseButtonStyle = GetBaseButtonStyle();

            // --- SETUP DA JANELA POPUP ---
            var popupWindow = new Window
            {
                Title = $"Adicionar Estoque para {produto.Nome}",
                Width = 550,
                Height = 420, // Ajuste para ficar mais espaçoso
                WindowStyle = WindowStyle.ToolWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
            };

            var mainGrid = new Grid { Margin = new Thickness(30) }; // Margem maior para "respiro"
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Conteúdo Central
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Botões

            // --- TÍTULO ---
            var titleText = new TextBlock
            {
                Text = $"Entrada de Estoque: {produto.Nome}",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(titleText, 0);
            mainGrid.Children.Add(titleText);

            // ====================================================================
            // CONTEÚDO CENTRALIZADO (Inputs e Resultado)
            // ====================================================================
            var contentGrid = new Grid { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
            Grid.SetRow(contentGrid, 1);

            contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int internalRow = 0;

            // --- R0: Estoque Atual (Exibição) ---
            var txtEstoqueAtual = new TextBlock
            {
                Text = $"Estoque Atual: {estoqueAtual} Unidades",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = corTextoEscuro
            };
            Grid.SetRow(txtEstoqueAtual, internalRow++);
            contentGrid.Children.Add(txtEstoqueAtual);

            // --- R1: Campo Quantidade a Adicionar ---
            var labelAdicionar = new TextBlock { Text = "Quantidade a Adicionar:", Margin = new Thickness(0, 0, 0, 5), FontSize = 16 };
            Grid.SetRow(labelAdicionar, internalRow++);
            contentGrid.Children.Add(labelAdicionar);

            var txtQuantiaAdicionar = new TextBox { Name = "TxtQuantiaAdicionar", Height = 45, FontSize = 18, Padding = new Thickness(10), Margin = new Thickness(0, 0, 0, 30) };
            Grid.SetRow(txtQuantiaAdicionar, internalRow++);
            contentGrid.Children.Add(txtQuantiaAdicionar);

            // --- R2: Novo Total (Label) ---
            var labelNovoTotal = new TextBlock { Text = "Novo Total:", Margin = new Thickness(0, 0, 0, 5), FontWeight = FontWeights.Bold, FontSize = 16 };
            Grid.SetRow(labelNovoTotal, internalRow++);
            contentGrid.Children.Add(labelNovoTotal);

            // --- R3: Novo Total (Resultado) ---
            var resultadoBorder = new Border
            {
                BorderBrush = corVerde,
                BorderThickness = new Thickness(2),
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#E8F5E9"),
                Padding = new Thickness(8),
                CornerRadius = new CornerRadius(5),
                Height = 55,
                Margin = new Thickness(0, 0, 0, 0)
            };
            Grid.SetRow(resultadoBorder, internalRow++);
            contentGrid.Children.Add(resultadoBorder);

            var txtNovoTotal = new TextBlock
            {
                Name = "TxtNovoTotal",
                Text = $"{estoqueAtual} Unidades",
                FontSize = 28,
                FontWeight = FontWeights.ExtraBold,
                Foreground = corVerde,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            resultadoBorder.Child = txtNovoTotal;

            mainGrid.Children.Add(contentGrid);

            // --- Lógica de Cálculo em Tempo Real ---
            txtQuantiaAdicionar.PreviewTextInput += (s, args) => { args.Handled = new Regex("[^0-9]+").IsMatch(args.Text); };
            txtQuantiaAdicionar.TextChanged += (s, args) =>
            {
                if (int.TryParse(txtQuantiaAdicionar.Text, out int adicao))
                {
                    adicaoAtual = adicao;
                    int novoTotal = estoqueAtual + adicao;
                    txtNovoTotal.Text = $"{novoTotal} Unidades";
                }
                else
                {
                    adicaoAtual = 0;
                    txtNovoTotal.Text = $"{estoqueAtual} Unidades";
                }
            };

            // --- Botões ---
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 30, 0, 0) };
            Grid.SetRow(buttonPanel, 2);

            // 1. BOTÃO CANCELAR
            var btnCancelar = new Button
            {
                Content = "Cancelar",
                Width = 130,
                Height = 45,
                Margin = new Thickness(0, 0, 15, 0),
                FontSize = 16,
                Foreground = corTextoEscuro,
                Background = corCinza,
                IsCancel = true
            };
            // CRIAÇÃO DE CÓPIA MODIFICÁVEL E APLICAÇÃO DO HOVER
            Style cancelStyle = new Style(typeof(Button), baseButtonStyle); // Cria a cópia
            var cancelHoverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            cancelHoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, corCinzaHover));
            cancelHoverTrigger.Setters.Add(new Setter(Button.ForegroundProperty, corTextoEscuro));
            cancelStyle.Triggers.Add(cancelHoverTrigger); // Adiciona o Trigger ANTES de atribuir
            btnCancelar.Style = cancelStyle; // ⬅️ Atribuição após adicionar Triggers

            btnCancelar.Click += (s, args) => { popupWindow.DialogResult = false; popupWindow.Close(); };

            // 2. BOTÃO CONFIRMAR
            var btnConfirmar = new Button
            {
                Content = "Confirmar Adição",
                Width = 180,
                Height = 45,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = corVerde,
                IsDefault = true
            };
            // CRIAÇÃO DE CÓPIA MODIFICÁVEL E APLICAÇÃO DO HOVER
            Style confirmStyle = new Style(typeof(Button), baseButtonStyle); // Cria a cópia
            var confirmHoverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            confirmHoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, corVerdeHover));
            confirmHoverTrigger.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
            confirmStyle.Triggers.Add(confirmHoverTrigger); // Adiciona o Trigger ANTES de atribuir
            btnConfirmar.Style = confirmStyle; // ⬅️ Atribuição após adicionar Triggers

            btnConfirmar.Click += (s, args) =>
            {
                if (!int.TryParse(txtQuantiaAdicionar.Text, out int adicao) || adicao <= 0)
                {
                    MessageBox.Show("Por favor, insira uma quantidade válida para adicionar.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Define o valor de retorno
                adicaoAtual = adicao;

                popupWindow.DialogResult = true;
                popupWindow.Close();
            };

            buttonPanel.Children.Add(btnCancelar);
            buttonPanel.Children.Add(btnConfirmar);
            mainGrid.Children.Add(buttonPanel);

            popupWindow.Content = mainGrid;

            bool? resultadoDialog = popupWindow.ShowDialog();

            if (resultadoDialog == true)
            {
                return adicaoAtual;
            }

            return null;
        }


        private void Button_Click_Editar(object sender, RoutedEventArgs e)
        {
            // 1. Obter o Produto da linha clicada
            // (Exatamente como você faz no botão de Remover)
            Produto produtoSelecionado = (sender as Button).DataContext as Produto;

            if (produtoSelecionado == null)
            {
                MessageBox.Show("Não foi possível identificar o produto selecionado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Pegar o ID do produto
            string idDoProduto = produtoSelecionado.Id;

            // 3. Criar a nova página de edição, passando o ID para o construtor dela
            EditarProduto paginaEditar = new EditarProduto(idDoProduto);

            // 4. Navegar para a página
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            if (navigationService != null)
            {
                navigationService.Navigate(paginaEditar);
            }
        }


        private void Button_Click_Remover(object sender, RoutedEventArgs e)
        {
            Produto ProdutoParaRemover = (sender as Button).DataContext as Produto;
            if (ProdutoParaRemover != null)
            {
                MessageBoxResult resultado = MessageBox.Show($"Tem certeza que deseja remover o Produto? '{ProdutoParaRemover.Nome}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultado == MessageBoxResult.Yes)
                {
                    ControleEstoque controleEstoque = new ControleEstoque();
                    controleEstoque.DesativarProduto(ProdutoParaRemover.Id);
                    listaDeProdutos.Remove(ProdutoParaRemover);
                    // ✅ CORREÇÃO PARA O AVISO CS8600: Força a atualização da lista de forma segura
                    DgProdutos.ItemsSource = new List<Produto>(listaDeProdutos);
                    MessageBox.Show("Produto removido com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);

            if (navigationService != null)
            {
                // Limpa o conteúdo do Frame navegando para um URI nulo.
                // Isso simula o "fechamento" da página e deixa o Frame vazio.
                navigationService.Navigate(null as Uri);

                // Opcional: Se você quer ter certeza de que o histórico não guarda essa entrada de 'null':
                // navigationService.RemoveBackEntry();
            }
        }

        
    }
}
