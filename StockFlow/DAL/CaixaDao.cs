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
        public void AbrirCaixa(Caixa novoCaixa) 
        { 
            try
            {
                using (var context = new AppDbContext())
                {
                    // Verifica se já existe um caixa aberto no sistema
                    bool existeCaixaAberto = context.Caixas.Any(c => c.Status == "Aberto" && c.UsuarioAberturaId == novoCaixa.UsuarioAberturaId);
                    if (existeCaixaAberto)
                    {
                        this.mensagemErro = "Já existe um caixa aberto. Feche o caixa atual antes de abrir um novo.";
                        return;
                    }

                    context.Caixas.Add(novoCaixa);                  
                    context.SaveChanges();

                }
            }
            catch (Exception ex)
            {

                Exception innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                // A mensagem de erro agora mostrará a causa raiz do problema no banco de dados.
                this.mensagemErro = "Erro ao abrir o caixa. Causa: " + innerEx.Message;
                return;
            }          
        }


        public Caixa FecharCaixa(int usuarioFechamentoId, decimal valorInformado)
        {
            this.mensagemErro = "";
            try
            {
                using (var context = new AppDbContext())
                {
                    var caixaAberto = context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == usuarioFechamentoId);
                    if (caixaAberto == null)
                    {
                        this.mensagemErro = "Nenhum caixa aberto encontrado para fechar.";
                        return null;
                    }

                    // Calcula o valor total das movimentações
                    decimal totalMovimentacoes = context.MovimentacaoCaixas
                        .Where(m => m.CaixaId == caixaAberto.CaixaId)
                        .Sum(m => m.TipoMovimentacao == "Entrada" || m.TipoMovimentacao == "Abertura" || m.TipoMovimentacao == "Venda" || m.TipoMovimentacao == "Reforco" ? m.Valor : -m.Valor);

                    decimal valorDoCaixa = context.MovimentacaoCaixas
                        .Where(m => m.CaixaId == caixaAberto.CaixaId)
                        .Sum(m => m.TipoMovimentacao == "Entrada" || m.TipoMovimentacao == "Abertura" || (m.TipoMovimentacao == "Venda" && m.MetodoDePagamento == "Dinheiro") || m.TipoMovimentacao == "Reforco" ? m.Valor : -m.Valor);

                    // Atualiza o caixa
                    caixaAberto.UsuarioFechamentoId = usuarioFechamentoId;
                    caixaAberto.DataHoraFechamento = DateTime.Now;
                    caixaAberto.ValorFechamentoCalculado = totalMovimentacoes;
                    caixaAberto.ValorFechamentoInformado = valorInformado;
                    caixaAberto.ValorFechamentoCaixa = valorDoCaixa;
                    caixaAberto.Diferenca = valorDoCaixa - valorInformado ;
                    caixaAberto.Status = "Fechado";

                    context.SaveChanges();
                    return caixaAberto;
                }
            }
            catch (Exception)
            {
                this.mensagemErro = "Erro ao fechar o caixa.";
                return null;
            }
             
        }

        public void FazerSangria(decimal valor, string motivo)
        {
            this.mensagemErro = "";

            // 1. Validações iniciais dos dados de entrada
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
                using (var context = new AppDbContext())
                {
                    // 2. Encontrar o caixa que está aberto
                    var caixaAberto = context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == SessaoUsuario.UsuarioId);
                    if (caixaAberto == null)
                    {
                        this.mensagemErro = "Não é possível fazer a sangria. Nenhum caixa está aberto.";
                        return;
                    }

                    // 3. Calcular o saldo atual do caixa
                    // (Soma todas as entradas e subtrai todas as saídas)
                    var saldoAtual = context.MovimentacaoCaixas
                        .Where(m => m.CaixaId == caixaAberto.CaixaId)
                        .Sum(m =>
                            (m.TipoMovimentacao == "Abertura" || m.TipoMovimentacao == "Venda" || m.TipoMovimentacao == "Reforco")
                            ? m.Valor  // Soma se for entrada
                            : -m.Valor // Subtrai se for saída (ex: outra sangria)
                        );

                    // 4. Validar se há saldo suficiente para a sangria
                    if (valor > saldoAtual)
                    {
                        this.mensagemErro = $"Não há saldo suficiente para uma sangria de {valor:C}. Saldo atual: {saldoAtual:C}.";
                        return;
                    }

                    // 5. Criar a nova movimentação de sangria
                    var novaMovimentacao = new MovimentacaoCaixa
                    {
                        CaixaId = caixaAberto.CaixaId,
                        DataHora = DateTime.Now,
                        Valor = valor, // O valor é sempre positivo, o TIPO define se é entrada ou saída
                        TipoMovimentacao = "Sangria",
                        Descricao = motivo
                    };

                    context.MovimentacaoCaixas.Add(novaMovimentacao);

                    // 6. Salvar a alteração no banco de dados
                    context.SaveChangesAsync();

                    return;
                }
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao registrar a sangria. Causa: " + ex.InnerException?.Message ?? ex.Message;
                return;
            }
        }

        public void AdicionarReforco(decimal valor, string motivo)
        {
            this.mensagemErro = "";

            // 1. Validações iniciais dos dados de entrada
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
                using (var context = new AppDbContext())
                {
                    // 2. Encontrar o caixa que está aberto
                    var caixaAberto = context.Caixas.FirstOrDefault(c => c.Status == "Aberto" && c.UsuarioAberturaId == SessaoUsuario.UsuarioId);
                    if (caixaAberto == null)
                    {
                        this.mensagemErro = "Não é possível adicionar o reforço. Nenhum caixa está aberto.";
                        return;
                    }

                    // 3. Criar a nova movimentação de reforço
                    // Não precisamos calcular o saldo, pois estamos apenas adicionando dinheiro.
                    var novaMovimentacao = new MovimentacaoCaixa
                    {
                        CaixaId = caixaAberto.CaixaId,
                        DataHora = DateTime.Now,
                        Valor = valor,
                        TipoMovimentacao = "Reforco", // Tipo específico para entrada de troco
                        Descricao = motivo
                    };

                    context.MovimentacaoCaixas.Add(novaMovimentacao);

                    // 4. Salvar a alteração no banco de dados
                    context.SaveChanges();

                    return;
                }
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao registrar o reforço de caixa. Causa: " + ex.InnerException?.Message ?? ex.Message;
                return;
            }
        }

        public async Task<List<Caixa>> ObterTodosOsCaixasAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<Caixa> todosOsCaixas = await context.Caixas.Include(c => c.UsuarioAbertura).ToListAsync();

                return todosOsCaixas;
            }
        }
    }

}

