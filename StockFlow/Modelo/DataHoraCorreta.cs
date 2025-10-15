using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Modelo
{
    public class DataHoraCorreta
    {
        private async Task<DateTime?> GetNtpUtcTimeAsync(string ntpServer = "pool.ntp.org")
        {
            try
            {
                // (Código nativo para buscar a hora NTP, como na mensagem anterior)
                // ...
                var ntpData = new byte[48];
                ntpData[0] = 0x1B;
                var addresses = await Dns.GetHostAddressesAsync(ntpServer);
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    await socket.ConnectAsync(ipEndPoint);
                    socket.ReceiveTimeout = 3000;
                    socket.Send(ntpData);
                    socket.Receive(ntpData);
                    socket.Close();
                }
                const byte serverReplyTime = 40;
                ulong intPart = (ulong)ntpData[serverReplyTime] << 24 | (ulong)ntpData[serverReplyTime + 1] << 16 | (ulong)ntpData[serverReplyTime + 2] << 8 | (ulong)ntpData[serverReplyTime + 3];
                ulong fractPart = (ulong)ntpData[serverReplyTime + 4] << 24 | (ulong)ntpData[serverReplyTime + 5] << 16 | (ulong)ntpData[serverReplyTime + 6] << 8 | (ulong)ntpData[serverReplyTime + 7];
                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
                return networkDateTime;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private DateTime ConvertUtcToBrasiliaTime(DateTime utcDateTime)
        {
            try
            {
                string brasiliaTimeZoneId = "America/Sao_Paulo"; // Padrão IANA, mais compatível
                try
                {
                    TimeZoneInfo.FindSystemTimeZoneById(brasiliaTimeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                    brasiliaTimeZoneId = "E. South America Standard Time"; // Fallback para Windows
                }

                TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(brasiliaTimeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, brasiliaTimeZone);
            }
            catch (Exception)
            {
                Console.WriteLine("ERRO: O fuso horário de Brasília não foi encontrado no sistema.");
                return utcDateTime;
            }
        }

        public void ObterHoraCorretaComCallback(Action<DateTime?> callback)
        {
            var _ = Task.Run(async () =>
            {
                // 1. Busca o resultado em segundo plano
                DateTime? resultado = await this.GetNtpUtcTimeAsync(); // Usando a instância de AccurateTimeService

                // 2. AGORA, simplesmente chamamos o callback com o resultado.
                // A responsabilidade de usar o Dispatcher é de QUEM CHAMA (o Frontend).
                if (resultado.HasValue)
                    resultado = this.ConvertUtcToBrasiliaTime(resultado.Value);
                callback(resultado);
            });
        }
    }
}
