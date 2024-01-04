using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Comunicacao
{
    enum Resposta { ACK, NACK, TIMEOUT, INVALIDO, EXCEPTION }

    class Transferencia
    {
        private static int sequencia = 0;

        public static string dadosRecebidos;

        public const string STX = "02";
        public const string DLE = "7F";
        public const string ACK = "06";
        public const string NACK = "15";
        public const string ETX = "03";
        public const string CMD_START = "00";
        public const string CMD_SUB_CHECK = "00";
        public const string CMD_SUB_OUT = "01";
        public const string CMD_FIM = "05";

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia um comando pela porta COM configurada.                     */
        /*                  Resposta: 0 = ACK | 1 = NACK | 2 = TIMEOUT | 3 = INVALIDO        */
        /* --------------------------------------------------------------------------------- */
        public Resposta enviarComando(string cmd, ComunicacaoV01 transfer)
        {
            string number = cmd.Substring(2, 2);
            Resposta result = Resposta.INVALIDO;

            // Realiza 3 tentativas de envio do comando
            for (int t = 1; t <= 6; t++)
            {
                // Inicializa a variável que irá receber a resposta
                Transferencia.dadosRecebidos = "";

                // Envia o comando pela porta COM e aguarda o retorno dos dados
                transfer.sendComando(cmd);
                System.Threading.Thread.Sleep(60);

                // Guarda o retorno
                Transferencia.dadosRecebidos = transfer.dadosRecebidos.Replace(" ", "");

                // Verifica os dados recebidos
                if (Transferencia.dadosRecebidos == (STX + number + DLE + ACK))
                {
                    // Sai do laço se o camando for aceito pela central
                    result = Resposta.ACK;
                    break;
                }
            }

            // Define o tipo de retorno (caso ocorra erro no envio)
            if (result.ToString() != Resposta.ACK.ToString())
            {
                if (Transferencia.dadosRecebidos == (STX + number + DLE + NACK))
                {
                    result = Resposta.NACK;
                }
                else if (Transferencia.dadosRecebidos == "")
                {
                    result = Resposta.TIMEOUT;
                }
                else
                {
                    result = Resposta.INVALIDO;
                }
            }
            return result;
            /*
            
            // Verifica os dados recebidos
            if (Transferencia.dadosRecebidos == (STX + number + DLE + ACK))
            {
                return Resposta.ACK;
            }
            else
            {
                if (Transferencia.dadosRecebidos == (STX + number + DLE + NACK))
                {
                    return Resposta.NACK;
                }
                else if (Transferencia.dadosRecebidos == "")
                {
                    return Resposta.TIMEOUT;
                }
                else
                {
                    return Resposta.INVALIDO;
                }
            }
            */
        }

        /* VER SE AINDA USA ---------------------------------------------------------------- */
        /* Funcionalidade : Converte Hexa para Binário.                                      */
        /* --------------------------------------------------------------------------------- */
        public int converterHexToBinary(string hex)
        {
            string binary = "";
            binary = Convert.ToString(Convert.ToInt32(hex, 16), 2);
            return Convert.ToInt32(binary);
        }

        /* VER SE AINDA USA ---------------------------------------------------------------- */
        /* Funcionalidade : Retorna o valor em hexa do próximo número da sequencia.          */
        /*                  O método pode retornar 0 até FF (quando chegar em FF volta p/ 0) */
        /* --------------------------------------------------------------------------------- */
        public string pegarHexProxNumber()
        {
            if (Transferencia.sequencia > 255)
                Transferencia.sequencia = 0;
            string number = Convert.ToString(Transferencia.sequencia, 16);
            if (number.Length == 1)
                number = "0" + number;
            Transferencia.sequencia++;
            return number.ToUpper();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o próximo número da sequencia.                           */
        /* --------------------------------------------------------------------------------- */
        public int pegarProxNumber()
        {
            if (Transferencia.sequencia > 255)
                Transferencia.sequencia = 0;
            int number = Transferencia.sequencia;
            Transferencia.sequencia++;
            return number;
        }
    }
}