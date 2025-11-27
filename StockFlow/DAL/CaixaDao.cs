using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class CaixaDao
    {
        public string mensagemErro = "";
        private readonly AppDbContext _context;

        // 1. O construtor agora recebe a instância do AppDbContext
        public CaixaDao(AppDbContext context)
        {
            _context = context;
        }

        public void AbrirCaixa(Caixa novoCaixa)
        {
            try
            {
                // Usa o _context em vez de criar um novo
                bool existeCaixaAberto = _context.Caixas.Any(c => c.Status == "Aberto" && c.UsuarioAberturaId == novoCaixa.UsuarioAberturaId);
                if (existeCaixaAberto)
                {
                    this.mensagemErro = "Já existe um caixa aberto. Feche o caixa atual antes de abrir um novo.";
                    return;
                }

                _context.Caixas.Add(novoCaixa);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Erro ao abrir o caixa. Causa: " + ex.InnerException?.Message ?? ex.Message;
            }
        }

        public Caixa FecharCaixa(int usuarioFechamentoId, decimal valorInformado)
        {
            this.mensagemErro = "";
            try
            {
                // Usa o _context
                var caixaAberto = _context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == usuarioFechamentoId);
                if (caixaAberto == null)
                {
                    this.mensagemErro = "Nenhum caixa aberto encontrado para fechar.";
                    return null;
                }

                var movimentacoesDoCaixa = _context.MovimentacaoCaixas
                    .Where(m => m.CaixaId == caixaAberto.CaixaId);

                decimal entradasEmDinheiro = movimentacoesDoCaixa
            .Where(m =>
                m.TipoMovimentacao == "Abertura" ||
                m.TipoMovimentacao == "Reforco" ||
                (m.TipoMovimentacao == "Venda" && m.MetodoDePagamento == "Dinheiro"))
            .Sum(m => m.Valor);

                decimal saidasEmDinheiro = movimentacoesDoCaixa
                    .Where(m => m.TipoMovimentacao == "Sangria") // Sangria é saída
                    .Sum(m => m.Valor);

                // O valor que o sistema acha que tem na gaveta
                decimal valorEsperadoNoCaixa = entradasEmDinheiro - saidasEmDinheiro;

                decimal totalMovimentacoes = movimentacoesDoCaixa
                    .Sum(m => m.TipoMovimentacao == "Sangria" ? -m.Valor : m.Valor);

                decimal valorDoCaixa = movimentacoesDoCaixa
                    .Where(m => m.MetodoDePagamento == "Dinheiro" || m.MetodoDePagamento == null)
                    .Sum(m => m.TipoMovimentacao == "Sangria" ? -m.Valor : m.Valor);

                caixaAberto.UsuarioFechamentoId = usuarioFechamentoId;
                caixaAberto.DataHoraFechamento = DateTime.Now;
                caixaAberto.ValorFechamentoCalculado = totalMovimentacoes;
                caixaAberto.ValorFechamentoInformado = valorInformado;
                caixaAberto.ValorFechamentoCaixa = valorEsperadoNoCaixa;
                caixaAberto.Diferenca = valorInformado - valorEsperadoNoCaixa;
                caixaAberto.Status = "Fechado";

                _context.SaveChanges();
                return caixaAberto;
            }
            catch (Exception)
            {
                this.mensagemErro = "Erro ao fechar o caixa.";
                return null;
            }
        }

        // 2. O método agora recebe o usuarioId em vez de usar a sessão global
        public void FazerSangria(int usuarioId, decimal valor, string motivo)
        {
            this.mensagemErro = "";

            if (valor <= 0)
            {
                this.mensagemErro = "O valor da sangria deve ser maior que zero.";
                return;
            }
            if (string.IsNullOrWhiteSpace(motivo))
            {
                this.mensagemErro = "É obrigatório informar um motivo para a sangria.";
                return;
            }
            try
            {
                // Usa o usuarioId recebido por parâmetro
                var caixaAberto = _context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == usuarioId);
                if (caixaAberto == null)
                {
                    this.mensagemErro = "Não é possível fazer a sangria. Nenhum caixa está aberto.";
                    return;
                }

                var saldoAtual = _context.MovimentacaoCaixas
                    .Where(m => m.CaixaId == caixaAberto.CaixaId)
                    .Sum(m => m.TipoMovimentacao == "Sangria" ? -m.Valor : m.Valor);

                if (valor > saldoAtual)
                {
                    this.mensagemErro = $"Não há saldo suficiente para uma sangria de {valor:C}. Saldo atual: {saldoAtual:C}.";
                    return;
                }

                var novaMovimentacao = new MovimentacaoCaixa
                {
                    CaixaId = caixaAberto.CaixaId,
                    DataHora = DateTime.Now,
                    Valor = valor,
                    TipoMovimentacao = "Sangria",
                    Descricao = motivo
                };
                _context.MovimentacaoCaixas.Add(novaMovimentacao);
                _context.SaveChanges(); // Corrigido de SaveChangesAsync para SaveChanges
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao registrar a sangria. Causa: " + ex.InnerException?.Message ?? ex.Message;
            }
        }

        // 3. O método agora recebe o usuarioId em vez de usar a sessão global
        public void AdicionarReforco(int usuarioId, decimal valor, string motivo)
        {
            this.mensagemErro = "";
            if (valor <= 0)
            {
                this.mensagemErro = "O valor do reforço deve ser maior que zero.";
                return;
            }
            if (string.IsNullOrWhiteSpace(motivo))
            {
                this.mensagemErro = "É obrigatório informar um motivo para o reforço de caixa.";
                return;
            }
            try
            {
                // Usa o usuarioId recebido por parâmetro
                var caixaAberto = _context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == usuarioId);
                if (caixaAberto == null)
                {
                    this.mensagemErro = "Não é possível adicionar o reforço. Nenhum caixa está aberto.";
                    return;
                }

                var novaMovimentacao = new MovimentacaoCaixa
                {
                    CaixaId = caixaAberto.CaixaId,
                    DataHora = DateTime.Now,
                    Valor = valor,
                    TipoMovimentacao = "Reforco",
                    Descricao = motivo
                };
                _context.MovimentacaoCaixas.Add(novaMovimentacao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao registrar o reforço de caixa. Causa: " + ex.InnerException?.Message ?? ex.Message;
            }
        }

        public async Task<List<Caixa>> ObterTodosOsCaixasAsync()
        {
            return await _context.Caixas.Include(c => c.UsuarioAbertura).ToListAsync();
        }
       
    }

}

