using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class ValidacaoEstoque
    {
        public string mensagem = "";
        public bool TentarConverterParaDecimal(string valor, out decimal resultado)
        {
            this.mensagem = ""; // Supondo que 'mensagem' é uma propriedade da sua classe

            // Define os estilos numéricos permitidos (sinal de negativo, ponto/vírgula decimal)
            var styles = NumberStyles.Number;

            // 1. Tenta converter usando a Cultura Invariante (que usa PONTO "." como separador)
            //    Isso é útil para dados vindos de APIs, arquivos ou bancos de dados.
            if (decimal.TryParse(valor, styles, CultureInfo.InvariantCulture, out resultado))
            {
                return true; // Sucesso!
            }

            // 2. Se falhou, tenta converter usando a Cultura atual do sistema (que no Brasil usa VÍRGULA ",")
            //    Isso é útil para a entrada direta do usuário em um sistema em português.
            if (decimal.TryParse(valor, styles, CultureInfo.CurrentCulture, out resultado))
            {
                return true; // Sucesso!
            }

            // 3. Se ambos falharam, a string é inválida.
            this.mensagem = $"O valor '{valor}' não é um número decimal válido.";
            resultado = 0; // Garante que o 'out' tenha um valor padrão.
            return false;
        }
        public int CoverterParaInt(string valor)
        {
            this.mensagem = "";
            int Valor = 0;
            try
            {
               Valor = Convert.ToInt32(valor);
            }
            catch (Exception)
            {
                this.mensagem = "Erro ao converter o valor para int.";
            }
            return Valor;
        }
    }
}
