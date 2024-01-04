using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Comunicacao
{
    class TransferenciaSaida : Transferencia
    {
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia um comando pela porta COM configurada.                     */
        /*                  Resposta: 0 = ACK | 1 = NACK | 2 = TIMEOUT | 3 = INVALIDO        */
        /* --------------------------------------------------------------------------------- */
        public EnumRespostaTrans enviarComando(string cmd)
        {
            string number = cmd.Substring(2, 2);

            // Realiza 6 tentativas de envio do comando
            for (int t = 1; t <= 6; t++)
            {
                // Inicializa a variável que irá receber a resposta
                Transferencia.dadosRecebidos = "";

                // Envia o comando pela porta COM e aguarda o retorno dos dados
                comunicacao.sendComando(cmd);
                System.Threading.Thread.Sleep(60);

                // Guarda o retorno
                Transferencia.dadosRecebidos = comunicacao.dadosRecebidos.Replace(" ", "");

                //return EnumRespostaTrans.ACK;

                // Verifica os dados recebidos
                // Sai do laço se o camando for aceito pela central
                if (Transferencia.dadosRecebidos == (STX + number + DLE + ACK))
                {
                    return EnumRespostaTrans.ACK;
                }
            }

            // Define o tipo de retorno (caso ocorra erro no envio)
            if (Transferencia.dadosRecebidos == (STX + number + DLE + NACK))
            {
                return EnumRespostaTrans.NACK;
            }
            else if (Transferencia.dadosRecebidos == "")
            {
                return EnumRespostaTrans.TIMEOUT;
            }
            else
            {
                return EnumRespostaTrans.INVALIDO;
            }
        }
    }
}