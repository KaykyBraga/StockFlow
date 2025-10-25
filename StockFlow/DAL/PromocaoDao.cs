using Microsoft.EntityFrameworkCore;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.DAL
{
    public class PromocaoDao
    {
        public string mensagemErro = "";
        private readonly AppDbContext _context;

        // O construtor agora recebe a instância do AppDbContext
        public PromocaoDao(AppDbContext context)
        {
            _context = context;
        }

        public void CadastrarPromocao(Promocao novaPromocao)
        {
            this.mensagemErro = ""; // Limpa a mensagem no início

            if (novaPromocao.DataFim <= novaPromocao.DataInicio)
            {
                this.mensagemErro = "A data de fim deve ser posterior à data de início.";
                return;
            }
            if (novaPromocao.ValorDesconto <= 0)
            {
                this.mensagemErro = "O valor do desconto deve ser maior que zero.";
                return;
            }
            try
            {
                _context.Promocoes.Add(novaPromocao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Erro ao cadastrar a promoção. Causa: " + ex.InnerException?.Message ?? ex.Message;
            }
        }



        //desativa promoções 
        public void DesativarPromocao(int promocaoId) // Alterado para receber ID para ser mais testável
        {
            this.mensagemErro = "";
            try
            {
                var promocao = _context.Promocoes.Find(promocaoId);
                if (promocao == null)
                {
                    this.mensagemErro = "Promocao não encontrado para desativação.";
                    return;
                }
                promocao.Ativo = false;
                _context.Promocoes.Update(promocao);
                _context.SaveChanges();
                this.mensagemErro = "Promocao desativado com sucesso!";
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Erro ao desativar o produto! " + ex.Message;
            }
        }


        public void ReativarPromocao(int promocaoId) 
        {
            this.mensagemErro = "";
            try
            {
                var promocao = _context.Promocoes.Find(promocaoId);
                if (promocao == null)
                {
                    this.mensagemErro = "Promocao não encontrado para desativação.";
                    return;
                }
                promocao.Ativo = true;
                _context.Promocoes.Update(promocao);
                _context.SaveChanges();
                this.mensagemErro = "Promocao desativado com sucesso!";
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Erro ao desativar o produto! " + ex.Message;
            }
        }

        public void DesativarPromocoesExpiradas()
        {
            this.mensagemErro = "";
            try
            {
                var dataHoraAtual = DateTime.Now;

                
                var promocoesParaDesativar = _context.Promocoes
                    .Where(p => p.Ativo && p.DataFim < dataHoraAtual)
                    .ToList();

                // Altera o estado de cada uma.
                promocoesParaDesativar.ForEach(p => p.Ativo = false);

                // Salva todas as alterações de uma vez.
                if (promocoesParaDesativar.Any())
                {
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao atualizar o status das promoções. Causa: " + ex.Message;
            }
        }

        public void RemoverPromocao(int promocaoId)
        {
            this.mensagemErro = "";
            try
            {
                var promocao = _context.Promocoes.Find(promocaoId);
                if (promocao == null)
                {
                    this.mensagemErro = "Promoção não encontrada para remoção.";
                    return;
                }
                _context.Promocoes.Remove(promocao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Erro ao remover a promoção. Causa: " + ex.InnerException?.Message ?? ex.Message;
            }
        }

        public async Task<List<Promocao>> ObterTodosAsPromocoesAsync()
        {
            return await _context.Promocoes.Include(p => p.PromocaoProdutos).ToListAsync();
        }

        public async Task<List<Promocao>> ObterPromocoesAtivasAsync()
        {
            return await _context.Promocoes.Where(p => p.Ativo).ToListAsync();
        }
    }
}
