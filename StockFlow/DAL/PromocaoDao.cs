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

        public void CadastrarPromocao(Promocao novaPromocao)
        {
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
                using (var context = new AppDbContext())
                {
                    // Lógica para adicionar a nova promoção ao banco de dados
                     context.Promocoes.Add(novaPromocao);
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
                this.mensagemErro = "Erro ao cadastrar a promoção. Causa: " + innerEx.Message;
                return;
            }
        }

        public void DesativarPromocoesExpiradas()
        {
            this.mensagemErro = "";
            try
            {
                using (var context = new AppDbContext())
                {
                    // Pega a data e hora atual UMA VEZ para garantir consistência na consulta
                    var dataHoraAtual = DateTime.Now;

                    // Monta a consulta e executa a atualização diretamente no banco
                    int promocoesAtualizadas = context.Promocoes
                        .Where(p => p.Ativo && p.DataFim < dataHoraAtual) // Filtra as promoções ativas e expiradas
                        .ExecuteUpdate(updates =>
                            updates.SetProperty(p => p.Ativo, false)); // Define o novo valor para a coluna 'Ativo'

                    
                }
            }
            catch (Exception ex)
            {
                this.mensagemErro = "Ocorreu um erro ao atualizar o status das promoções. Causa: " + ex.Message;
                
            }
        }

        public async Task<List<Promocao>> ObterTodosAsPromocoesAsync()
        {
            await using (var context = new AppDbContext())
            {

                List<Promocao> todosAsPromocoes = await context.Promocoes.ToListAsync();

                return todosAsPromocoes;
            }
        }

        public async Task<List<Promocao>> ObterPromocoesAtivasAsync()
        {

            await using (var context = new AppDbContext())
            {

                List<Promocao> PromocoesAtivas = await context.Promocoes.Where(u => u.Ativo == true).ToListAsync();

                return PromocoesAtivas;
            }
        }
    }
}
