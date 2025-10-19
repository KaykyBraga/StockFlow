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

            CaixaDao caixaDao = new CaixaDao();
            caixaDao.AbrirCaixa(novoCaixa);

            if (caixaDao.mensagemErro != "")
            {
                this.mensagem = caixaDao.mensagemErro;
                return;
            }

            this.mensagem = "Caixa aberto com sucesso.";
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
            CaixaDao caixaDao = new CaixaDao();
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
            VendaDao vendaDao = new VendaDao();
            vendaDao.RegistrarVenda(SessaoUsuario.UsuarioId, itensDto, metodoPagamento);
            if (vendaDao.mensagemErro != "")
            {
                this.mensagem = vendaDao.mensagemErro;
                return;
            }
            this.mensagem = "Venda registrada com sucesso.";
        }

        public void CriarPromocao(List<string> listaDados)
        {
            Promocao novaPromocao = new Promocao();
            ValidacaoEstoque validacao = new ValidacaoEstoque();            
            PromocaoDao promocaoDao = new PromocaoDao();


            validacao.TentarConverterParaDecimal(listaDados[2], out decimal valorDesconto);
            novaPromocao.NomePromocao = listaDados[0];
            novaPromocao.TipoDesconto = listaDados[1];
            novaPromocao.ValorDesconto = valorDesconto;
            novaPromocao.DataInicio = Convert.ToDateTime(listaDados[3]);
            novaPromocao.DataFim = Convert.ToDateTime(listaDados[4]);
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
            PromocaoProdutoDao promocaoProdutoDao = new PromocaoProdutoDao();
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

        public void DesativarPromocoesExpiradas()
        {
            PromocaoDao promocaoDao = new PromocaoDao();
            promocaoDao.DesativarPromocoesExpiradas();
            if (promocaoDao.mensagemErro != "")
            {
                this.mensagem = promocaoDao.mensagemErro;
                return;
            }
          
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
            CaixaDao caixaDao = new CaixaDao();
            caixaDao.FazerSangria(valorSangria,motivo);
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
            CaixaDao caixaDao = new CaixaDao();
            caixaDao.AdicionarReforco(valorReforco, motivo);
            if (caixaDao.mensagemErro != "")
            {
                this.mensagem = caixaDao.mensagemErro;
                return;
            }
            this.mensagem = "Reforço registrado com sucesso.";
        }

        public async Task<List<Venda>> ObterTodasAsVendasAsync()
        {
            VendaDao vendaDao = new VendaDao();
            List<Venda> listaVendas = new List<Venda>();
            listaVendas = await vendaDao.ObterTodaAsVendasAsync();
            return listaVendas;
        }

        public async Task<List<VendaItem>> ObterTodasAsVendasItemAsync()
        {
            VendaItemDao vendaItemDao = new VendaItemDao();
            List<VendaItem> listaVendaItem = new List<VendaItem>();
            listaVendaItem = await vendaItemDao.ObterTodasAsVendaItensAsync();
            return listaVendaItem;
        }

        public async Task<List<Promocao>> ObterTodasAsPromocoesAsync()
        {
            PromocaoDao promocaoDao = new PromocaoDao();
            List<Promocao> listaPromocoes = new List<Promocao>();
            listaPromocoes = await promocaoDao.ObterTodosAsPromocoesAsync();
            return listaPromocoes;
        }

        public async Task<List<Promocao>> ObterTodasAsPromocoesAtivasAsync()
        {
            PromocaoDao promocaoDao = new PromocaoDao();
            List<Promocao> listaPromocoes = new List<Promocao>();
            listaPromocoes = await promocaoDao.ObterPromocoesAtivasAsync();
            return listaPromocoes;
        }

        public async Task<List<PromocaoProduto>> ObterTodasAsPromocaoProdutosAsync()
        {
            PromocaoProdutoDao promocaoProdutoDao = new PromocaoProdutoDao();
            List<PromocaoProduto> listaPromocaoProdutos = new List<PromocaoProduto>();
            listaPromocaoProdutos = await promocaoProdutoDao.ObterTodosAsPromocaoProdutoAsync();
            return listaPromocaoProdutos;
        }

        public async Task<List<Caixa>> ObterTodosOsCaixasAsync()
        {
            CaixaDao caixaDao = new CaixaDao();
            List<Caixa> listaCaixas = new List<Caixa>();
            listaCaixas = await caixaDao.ObterTodosOsCaixasAsync();
            return listaCaixas;
        }

        public async Task<List<MovimentacaoCaixa>> ObterTodasAsMovimentacaoCaixasAsync()
        {
            MovimetacaoCaixaDao movimetacaoCaixaDao = new MovimetacaoCaixaDao();
            List<MovimentacaoCaixa> listaMovimentacaoCaixas = new List<MovimentacaoCaixa>();
            listaMovimentacaoCaixas = await movimetacaoCaixaDao.ObterTodosAsMovimentacoesDoCaixaAsync();
            return listaMovimentacaoCaixas;
        }
    }
}
