using StockFlow.Controles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StockFlow.Visual
{
    // ... (As classes Produto e ProdutoVinculado continuam as mesmas)
    public class Produto
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";
    }
    public class ProdutoVinculado
    {
        public Produto Produto { get; set; } = new Produto();
        public bool IsVinculado { get; set; }
    }


    // ✅ NOME DA CLASSE CORRIGIDO
    public partial class VincularProdutos : Window
    {
        private Promocao promocaoAlvo;
        private List<ProdutoVinculado> listaProdutosVinculados;

        // ✅ NOME DO CONSTRUTOR CORRIGIDO
        public VincularProdutos(Promocao promocao)
        {
            CarregarProdutos();
            InitializeComponent();
            promocaoAlvo = promocao;
            LblNomePromocao.Text = promocao.Nome;
        }

        private async void CarregarProdutos()
        {
            ControleEstoque controleEstoque = new ControleEstoque();

            // 1. Busca todos os produtos do banco (com os Includes de PromocaoProdutos e Promocao)
            var listaDoBanco = await controleEstoque.ObterTodosOsProdutosAtivosAsync();

            // 2. APLICANDO O FILTRO DE VISIBILIDADE
            var produtosVisiveis = listaDoBanco.Where(p =>
                // A condição é: NÃO PODE existir nenhum vínculo que seja "Problemático".
                // O que é um vínculo problemático?
                !p.PromocaoProdutos.Any(pp =>
                    pp.Promocao.Ativo == true &&           // A outra promoção está Ativa
                    pp.PromocaoId != promocaoAlvo.Id       // E NÃO é a promoção que estamos editando agora
                )
            ).ToList();

            // 3. Transformando para a lista visual (ProdutoVinculado)
            // Fiz tudo em um passo só pra ficar mais rápido e limpo que os dois foreachs
            listaProdutosVinculados = produtosVisiveis.Select(produto => new ProdutoVinculado
            {
                // Cria o objeto visual do Produto
                Produto = new Produto
                {
                    Id = produto.ProdutoId.ToString(),
                    Nome = produto.NomeCompleto
                },

                // Verifica se ele já faz parte DA NOSSA promoção alvo para marcar o checkbox
                IsVinculado = promocaoAlvo.ProdutoIds.Contains(produto.ProdutoId.ToString())

            }).ToList();

            DgProdutos.ItemsSource = listaProdutosVinculados;
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            ControleVenda controleVenda = new ControleVenda();
            foreach (var item in listaProdutosVinculados)
            {
                controleVenda.DesvincularPromocaoProduto(promocaoAlvo.Id, int.Parse(item.Produto.Id));
            }
            promocaoAlvo.ProdutoIds.Clear();
            foreach (var item in listaProdutosVinculados)
            {
                if (item.IsVinculado)
                {                 
                        controleVenda.VincularPromocaoProduto(promocaoAlvo.Id, int.Parse(item.Produto.Id));
                        promocaoAlvo.ProdutoIds.Add(item.Produto.Id);                
                }               
            }

            MessageBox.Show("Produtos vinculados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}