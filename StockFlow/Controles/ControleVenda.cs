using StockFlow.DAL;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Controles
{
    public class ControleVenda
    {
        public string mensagem = "";

        public void AbrirCaixa(string ValorDeAbertura)
        {
            Caixa novoCaixa = new Caixa();
            DataHoraCorreta dataHoraCorreta = new DataHoraCorreta();
            decimal valorAbertura;
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();

            if (!validacaoEstoque.TentarConverterParaDecimal(ValorDeAbertura, out valorAbertura))
            {
                this.mensagem = "Valor de abertura inválido.";
                return;
            }


            novoCaixa.UsuarioAberturaId = SessaoUsuario.UsuarioId;
            novoCaixa.ValorAbertura = valorAbertura;
            novoCaixa.Status = "Aberto";


            // Cria a movimentação inicial de abertura
            var movimentacao = new MovimentacaoCaixa
            {
                Valor = valorAbertura,
                TipoMovimentacao = "Abertura"
            };
            dataHoraCorreta.ObterHoraCorretaComCallback(horaAtual =>
            {

                if (horaAtual.HasValue)
                {
                    novoCaixa.DataHoraAbertura = horaAtual.Value;
                    movimentacao.DataHora = horaAtual.Value;
                }
                else
                {
                    this.mensagem = "Não foi possível obter a hora correta. Fornecedor não cadastrado.";
                    return;
                }
            });

            novoCaixa.MovimentacaoCaixa.Add(movimentacao);
            var context = new AppDbContext();
            CaixaDao caixaDao = new CaixaDao(context);
            caixaDao.AbrirCaixa(novoCaixa);

            if (caixaDao.mensagemErro != "")
            {
                this.mensagem = caixaDao.mensagemErro;
                return;
            }

            this.mensagem = "";
        }

        public Caixa FecharCaixa(string ValorInformado)
        {
            decimal valorFechamento;
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();
            if (!validacaoEstoque.TentarConverterParaDecimal(ValorInformado, out valorFechamento))
            {
                this.mensagem = "Valor de fechamento inválido.";
                return null;
            }
            var context = new AppDbContext();
            CaixaDao caixaDao = new CaixaDao(context);
            var caixaFechado = caixaDao.FecharCaixa(SessaoUsuario.UsuarioId, valorFechamento);
            if (caixaDao.mensagemErro != "")
            {
                this.mensagem = caixaDao.mensagemErro;
                return null;
            }
            this.mensagem = "Caixa fechado com sucesso.";
            return caixaFechado;
        }

        public void RegistrarVenda( List<VendaItem> itensDto, string metodoPagamento)
        {
            var context = new AppDbContext();
            VendaDao vendaDao = new VendaDao(context);
            vendaDao.RegistrarVenda(SessaoUsuario.UsuarioId, itensDto, metodoPagamento);
            if (vendaDao.mensagemErro != "")
            {
                this.mensagem = vendaDao.mensagemErro;
                return;
            }
            this.mensagem = "";
        }

        public void CriarPromocao(List<string> listaDados)
        {
            Promocao novaPromocao = new Promocao();
            ValidacaoEstoque validacao = new ValidacaoEstoque();  
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);


            validacao.TentarConverterParaDecimal(listaDados[1], out decimal valorDesconto);
            novaPromocao.NomePromocao = listaDados[0];
            novaPromocao.TipoDesconto = "Porcentagem";
            novaPromocao.ValorDesconto = valorDesconto;
            novaPromocao.DataInicio = Convert.ToDateTime(listaDados[2]);
            novaPromocao.DataFim = Convert.ToDateTime(listaDados[3]);
            novaPromocao.Ativo = true;
            if(validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }
            promocaoDao.CadastrarPromocao(novaPromocao);
            if (promocaoDao.mensagemErro != "")
            {
                this.mensagem = promocaoDao.mensagemErro;
                return;
            }
            this.mensagem = "Promoção cadastrada com sucesso.";
        }

        public void VincularPromocaoProduto(int promocaoId, int produtoId)
        {
            var context = new AppDbContext();
            PromocaoProdutoDao promocaoProdutoDao = new PromocaoProdutoDao(context);
            PromocaoProduto novaPromocaoProduto = new PromocaoProduto();
            novaPromocaoProduto.PromocaoId = promocaoId;
            novaPromocaoProduto.ProdutoId = produtoId;


            promocaoProdutoDao.VincularPromocaoProduto(novaPromocaoProduto);
            if (promocaoProdutoDao.mensagemErro != "")
            {
                this.mensagem = promocaoProdutoDao.mensagemErro;
                return;
            }
            this.mensagem = "Promoção vinculada ao produto com sucesso.";
        }

       public void DesvincularPromocaoProduto(int promocaoId, int produtoId)
        {
            var context = new AppDbContext();
            PromocaoProdutoDao promocaoProdutoDao = new PromocaoProdutoDao(context);
            promocaoProdutoDao.DesvincularPromocaoProduto(promocaoId, produtoId);
            if (promocaoProdutoDao.mensagemErro != "")
            {
                this.mensagem = promocaoProdutoDao.mensagemErro;
                return;
            }
            this.mensagem = "Promoção desvinculada do produto com sucesso.";
        }

        public void DesativarPromocoesExpiradas()
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            promocaoDao.DesativarPromocoesExpiradas();
            if (promocaoDao.mensagemErro != "")
            {
                this.mensagem = promocaoDao.mensagemErro;
                return;
            }
          
        }

        public void DesativarPromocao(int promocaoId)
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            promocaoDao.DesativarPromocao(promocaoId);
            if (promocaoDao.mensagemErro != "")
            {
                this.mensagem = promocaoDao.mensagemErro;
                return;
            }
            this.mensagem = "Promoção desativada com sucesso.";
        }

        public void ReativarPromocao(int promocaoId)
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            promocaoDao.ReativarPromocao(promocaoId);
            if (promocaoDao.mensagemErro != "")
            {
                this.mensagem = promocaoDao.mensagemErro;
                return;
            }
            this.mensagem = "Promoção reativada com sucesso.";
        }

        public void RemoverPromocao(int promocaoId)
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            promocaoDao.RemoverPromocao(promocaoId);
            if (promocaoDao.mensagemErro != "")
            {
                this.mensagem = promocaoDao.mensagemErro;
                return;
            }
            this.mensagem = "Promoção removida com sucesso.";
        }

        public void SangriaCaixa(string ValorSangria, string motivo)
        {
            decimal valorSangria;
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();
            validacaoEstoque.TentarConverterParaDecimal(ValorSangria, out valorSangria);
            if (validacaoEstoque.mensagem != "")
            {
                this.mensagem = validacaoEstoque.mensagem;
                return;
            }
            var context = new AppDbContext();
            CaixaDao caixaDao = new CaixaDao(context);
            caixaDao.FazerSangria(SessaoUsuario.UsuarioId,valorSangria,motivo);
            if (caixaDao.mensagemErro != "")
            {
                this.mensagem = caixaDao.mensagemErro;
                return;
            }
            this.mensagem = "Sangria registrada com sucesso.";
        }

        public void AdicionarTroca(string ValorReforco, string motivo)
        {
            decimal valorReforco;
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();
            validacaoEstoque.TentarConverterParaDecimal(ValorReforco, out valorReforco);
            if (validacaoEstoque.mensagem != "")
            {
                this.mensagem = validacaoEstoque.mensagem;
                return;
            }
            var context = new AppDbContext();
            CaixaDao caixaDao = new CaixaDao(context);
            caixaDao.AdicionarReforco(SessaoUsuario.UsuarioId, valorReforco, motivo);
            if (caixaDao.mensagemErro != "")
            {
                this.mensagem = caixaDao.mensagemErro;
                return;
            }
            this.mensagem = "";
        }

        public async Task<List<Venda>> ObterTodasAsVendasAsync()
        {
            var context = new AppDbContext();
            VendaDao vendaDao = new VendaDao(context);
            List<Venda> listaVendas = new List<Venda>();
            listaVendas = await vendaDao.ObterTodaAsVendasAsync();
            return listaVendas;
        }

        public async Task<List<StockFlow.Visual.Venda>> ObterTodasAsVendasDataGridAsync()
        {
            var context = new AppDbContext();
            VendaDao vendaDao = new VendaDao(context);
            List<Venda> listaVendas = new List<Venda>();
            listaVendas = await vendaDao.ObterTodaAsVendasAsync();
            var listaFinalParaGrid = listaVendas.Select(venda => new StockFlow.Visual.Venda
            {
                IdVenda = venda.VendaId.ToString(),
                Data = venda.DataVenda,
                NomeFuncionario = venda.Usuario.NomeCompleto,
                QuantidadeItens = venda.VendaItems.Count,
                DescontoTotal = venda.DescontoTotal,
                MetodoDePagamento = venda.MetodoPagamento,              
                ValorTotal = venda.ValorTotal
            }).ToList();
            return listaFinalParaGrid;
        }

        public async Task<List<VendaItem>> ObterTodasAsVendasItemAsync()
        {
            var context = new AppDbContext();
            VendaItemDao vendaItemDao = new VendaItemDao(context);
            List<VendaItem> listaVendaItem = new List<VendaItem>();
            listaVendaItem = await vendaItemDao.ObterTodasAsVendaItensAsync();
            return listaVendaItem;
        }

        public async Task<List<StockFlow.Visual.Relatorio.ItemVenda>> ObterTodasAsVendasItemParaGridAsync(int id)
        {
            var context = new AppDbContext();
            VendaItemDao vendaItemDao = new VendaItemDao(context);
            List<VendaItem> listaVendaItem = new List<VendaItem>();
            listaVendaItem = await vendaItemDao.ObterTodasAsVendaItensPorVendaIdAsync(id);
            var listaFinalParaGrid = listaVendaItem.Select(vendaItem => new StockFlow.Visual.Relatorio.ItemVenda
            {
                NomeProduto = vendaItem.Produto.NomeCompleto,
                Quantidade = vendaItem.Quantidade,
                PrecoUnitario = vendaItem.PrecoUnitarioMomento,
                PrecoTotal = vendaItem.PrecoUnitarioMomento * vendaItem.Quantidade,
                DescontoTotal = vendaItem.DescontoItem
            }).ToList();
            return listaFinalParaGrid;
        }

        public async Task<List<Promocao>> ObterTodasAsPromocoesAsync()
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            List<Promocao> listaPromocoes = new List<Promocao>();
            listaPromocoes = await promocaoDao.ObterTodosAsPromocoesAsync();
            return listaPromocoes;
        }

        public async Task<List<StockFlow.Visual.Promocao>> ObterTodasAsPromocoesParaDataGridAsync()
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            List<Promocao> listaPromocoes = new List<Promocao>();
            listaPromocoes = await promocaoDao.ObterTodosAsPromocoesAsync();
            var listaFinalParaGrid = listaPromocoes.Select(promocao => new StockFlow.Visual.Promocao
            {
                Ativa = promocao.Ativo,
                DataFim = promocao.DataFim,
                DataInicio = promocao.DataInicio,
                Desconto = promocao.ValorDesconto,
                Id = promocao.PromocaoId,
                Nome = promocao.NomePromocao,
                ProdutoIds = promocao.PromocaoProdutos.Select(pp => pp.ProdutoId.ToString()).ToList()
            }).ToList();
            return listaFinalParaGrid;
        }

        public async Task<List<Promocao>> ObterTodasAsPromocoesAtivasAsync()
        {
            var context = new AppDbContext();
            PromocaoDao promocaoDao = new PromocaoDao(context);
            List<Promocao> listaPromocoes = new List<Promocao>();
            listaPromocoes = await promocaoDao.ObterPromocoesAtivasAsync();
            return listaPromocoes;
        }

        public async Task<List<PromocaoProduto>> ObterTodasAsPromocaoProdutosAsync()
        {
            var context = new AppDbContext();
            PromocaoProdutoDao promocaoProdutoDao = new PromocaoProdutoDao(context);
            List<PromocaoProduto> listaPromocaoProdutos = new List<PromocaoProduto>();
            listaPromocaoProdutos = await promocaoProdutoDao.ObterTodosAsPromocaoProdutoAsync();
            return listaPromocaoProdutos;
        }

        public async Task<List<StockFlow.Visual.Relatorio.SessaoCaixa>> ObterTodosOsCaixasParaGridAsync()
        {
            var context = new AppDbContext();
            CaixaDao caixaDao = new CaixaDao(context);
            List<Caixa> listaCaixas = new List<Caixa>();
            listaCaixas = await caixaDao.ObterTodosOsCaixasAsync();
            var listaFinalParaGrid = listaCaixas.Select(caixa => new StockFlow.Visual.Relatorio.SessaoCaixa
            {
                IdCaixa = caixa.CaixaId,
                IdUsuario = caixa.UsuarioAbertura.NomeCompleto,
                DataAbertura = caixa.DataHoraAbertura,
                ValorAbertura = caixa.ValorAbertura,
                DataFechamento = caixa.DataHoraFechamento,
                ValorFechamento = caixa.ValorFechamentoInformado ?? 0,
                Diferenca = caixa.Diferenca ?? 0
            }).ToList();
            return listaFinalParaGrid;
        }

        public async Task<List<MovimentacaoCaixa>> ObterTodasAsMovimentacaoCaixasAsync()
        {
            var context = new AppDbContext();
            MovimentacaoCaixaDao movimetacaoCaixaDao = new MovimentacaoCaixaDao(context);
            List<MovimentacaoCaixa> listaMovimentacaoCaixas = new List<MovimentacaoCaixa>();
            listaMovimentacaoCaixas = await movimetacaoCaixaDao.ObterTodosAsMovimentacoesDoCaixaAsync();
            return listaMovimentacaoCaixas;
        }

        public async Task<List<StockFlow.Visual.Tela_Vendas.Produto>> ObterTodosOsProdutosParaVendaAsync()
        {
            var context = new AppDbContext();
            ProdutoDao produtoDao = new ProdutoDao(context);            
            var produtosDoBanco = await produtoDao.ObterProdutosAtivosAsync();

            var listaFinal = produtosDoBanco.Select(produto =>
            {
                // --- INÍCIO DA LÓGICA DO PREÇO PARA CADA PRODUTO ---

                // a) Encontra a promoção ATIVA para este produto, se houver.
                var promocaoAtiva = produto.PromocaoProdutos
                    .Select(pp => pp.Promocao) // Pega o objeto da promoção
                    .FirstOrDefault(promo =>
                        promo.Ativo &&
                        DateTime.Now >= promo.DataInicio &&
                        DateTime.Now <= promo.DataFim);

                // b) Começa com o preço normal.
                decimal precoFinal = produto.PrecoVenda;

                // c) Se encontrou uma promoção ativa, calcula o desconto.
                if (promocaoAtiva != null)
                {
                    if (promocaoAtiva.TipoDesconto == "Porcentagem")
                    {
                        decimal valorDoDesconto = precoFinal * (promocaoAtiva.ValorDesconto / 100);
                        precoFinal -= valorDoDesconto;
                    }
                    else // Assume que é "Fixo"
                    {
                        precoFinal -= promocaoAtiva.ValorDesconto;
                    }
                }

                // d) Cria o objeto final para a lista da tela
                return new StockFlow.Visual.Tela_Vendas.Produto
                {
                    Nome = produto.NomeCompleto,
                    Preco = precoFinal,
                    Id = produto.ProdutoId
                };

                // --- FIM DA LÓGICA ---

            }).ToList();

            return listaFinal;
        }
    }
}
