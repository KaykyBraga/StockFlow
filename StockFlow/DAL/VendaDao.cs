using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class VendaDao
    {
        public string mensagemErro = "";

        // 1. Campo privado para armazenar a instância do contexto do banco de dados.
        private readonly AppDbContext _context;

        // 2. Construtor que recebe o AppDbContext. Isso é a "Injeção de Dependência".
        // A classe agora não cria mais sua própria conexão, ela a recebe pronta.
        public VendaDao(AppDbContext context)
        {
            _context = context;
        }

        public void RegistrarVenda(int usuarioId, List<VendaItem> itensDto, string metodoPagamento)
        {
            try
            {
                // A lógica agora usa o "_context" que foi recebido pelo construtor,
                // em vez de criar um novo com "new AppDbContext()".

                // 1. Garante que existe um caixa aberto
                var caixaAberto = _context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == usuarioId);
                if (caixaAberto == null)
                {
                    this.mensagemErro = "Não é possível registrar a venda. Nenhum caixa está aberto.";
                    return;
                }

                var novaVenda = new Venda
                {
                    UsuarioId = usuarioId,
                    MetodoPagamento = metodoPagamento,
                    DataVenda = DateTime.Now,
                };

                decimal valorTotalVenda = 0;
                decimal descontoTotalVenda = 0;

                // 2. Processa cada item do "carrinho"
                foreach (var itemDto in itensDto)
                {
                    var produto = _context.Produtos
                        .Include(p => p.PromocaoProdutos).ThenInclude(pp => pp.Promocao)
                        .FirstOrDefault(p => p.ProdutoId == itemDto.ProdutoId);

                    if (produto == null || produto.EstoqueAtual < itemDto.Quantidade)
                    {
                        this.mensagemErro = $"Produto '{produto?.NomeCompleto ?? "ID " + itemDto.ProdutoId}' indisponível ou com estoque insuficiente.";
                        return;
                    }

                    // Lógica de promoção (simplificada)
                    var promocaoAtiva = produto.PromocaoProdutos
                        .Select(pp => pp.Promocao)
                        .FirstOrDefault(promo => promo.Ativo && DateTime.Now >= promo.DataInicio && DateTime.Now <= promo.DataFim);

                    decimal precoVenda = produto.PrecoVenda; // Preço normal
                    decimal descontoItem = 0;

                    if (promocaoAtiva != null)
                    {
                        if (promocaoAtiva.TipoDesconto == "Porcentagem")
                        {
                            descontoItem = precoVenda * (promocaoAtiva.ValorDesconto / 100);
                            precoVenda -= descontoItem;
                        }
                        else // Fixo
                        {
                            descontoItem = promocaoAtiva.ValorDesconto;
                            precoVenda -= descontoItem;
                        }
                    }

                    // 3. Cria o VendaItem e dá baixa no estoque
                    var novoItemVenda = new VendaItem
                    {
                        ProdutoId = produto.ProdutoId,
                        Quantidade = itemDto.Quantidade,
                        PrecoUnitarioMomento = precoVenda,
                        DescontoItem = descontoItem * itemDto.Quantidade
                    };
                    novaVenda.VendaItems.Add(novoItemVenda);

                    produto.EstoqueAtual -= itemDto.Quantidade; // Baixa no estoque

                    valorTotalVenda += novoItemVenda.PrecoUnitarioMomento * novoItemVenda.Quantidade;
                    descontoTotalVenda += novoItemVenda.DescontoItem;
                }

                novaVenda.ValorTotal = valorTotalVenda;
                novaVenda.DescontoTotal = descontoTotalVenda;
                _context.Vendas.Add(novaVenda);

                // 4. Cria a movimentação de entrada no caixa
                var movimentacao = new MovimentacaoCaixa
                {
                    CaixaId = caixaAberto.CaixaId,
                    Venda = novaVenda, // EF associa o ID
                    DataHora = DateTime.Now,
                    Valor = novaVenda.ValorTotal,
                    TipoMovimentacao = "Venda",
                    MetodoDePagamento = metodoPagamento
                };
                _context.MovimentacaoCaixas.Add(movimentacao);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao registrar a venda." + ex;
            }
        }

        public async Task<List<Venda>> ObterTodaAsVendasAsync()
        {
            // A lógica também usa o "_context" aqui.
            List<Venda> todasAsVendas = await _context.Vendas
                .Include(v => v.VendaItems)
                .Include(v => v.Usuario)
                .ToListAsync();

            return todasAsVendas;
        }
    }
}


