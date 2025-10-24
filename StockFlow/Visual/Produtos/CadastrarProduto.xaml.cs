using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockFlow.Visual.Produtos
{
    public partial class CadastrarProduto : Page
    {
        public CadastrarProduto()
        {
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