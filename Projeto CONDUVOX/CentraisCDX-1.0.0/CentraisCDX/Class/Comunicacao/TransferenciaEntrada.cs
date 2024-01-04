using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Comunicacao
{
    class TransferenciaEntrada : Transferencia
    {
        protected ArrayList programacaoInvalida = new ArrayList();
        protected ArrayList inicial = new ArrayList();
        protected ArrayList ramal = new ArrayList();
        protected ArrayList tronco = new ArrayList();
        protected ArrayList mesa = new ArrayList();
        protected ArrayList alarme = new ArrayList();
        protected ArrayList video = new ArrayList();

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia um comando pela porta COM configurada.                     */
        /*                  Resposta: 0 = ACK | 1 = NACK | 2 = TIMEOUT | 3 = INVALIDO        */
        /* --------------------------------------------------------------------------------- */
        public EnumRespostaTrans recuperarProgramacaoCentral(Processo processo)
        {
            string cmd       = montarComandoDeColeta();
            string resposta  = "";
            string number    = "";
            int count        = 1;
            int countTimeout = 1;
            int porcentagem  = 1;
            int countComandoEnviado = 1;

            // Envia o comando pela porta COM e aguarda o retorno dos dados
            comunicacao.sendComando(cmd);
            System.Threading.Thread.Sleep(60);
            resposta = comunicacao.dadosRecebidos.Replace(" ", "");

            Log.addText(cmd + " --><--" + resposta);

            if (resposta == "") return EnumRespostaTrans.TIMEOUT;

            number = resposta.Substring(2, 2);

            if (resposta == (STX + number + DLE + ACK))
            {
                // Inicia a coleta dos dados de programação
                do
                {
                    // Atualiza o progressbar
                    countComandoEnviado++;
                    if (countComandoEnviado == 32)
                    {
                        porcentagem++;
                        processo.OnUpdateProgressBar(porcentagem < 100 ? porcentagem : 98);
                        countComandoEnviado = 0;
                    }

                    // Define o number
                    number = Convert.ToString(pegarProxNumber(), 16).PadLeft(2, '0').ToUpper();

                    // Define o comando
                    cmd = STX + number + DLE + ACK;

                    // Envia o comando e aguarda a resposta
                    comunicacao.sendComando(cmd);
                    System.Threading.Thread.Sleep(60);
                    resposta = comunicacao.dadosRecebidos.Replace(" ", "");

                    Log.addText(cmd + " --><--" + resposta);

                    if (resposta == "")
                    {
                        // Caso não exista nenhum valor na resposta
                        if (countTimeout == 6) return EnumRespostaTrans.TIMEOUT;

                        countTimeout++;
                        continue;
                    }
                    countTimeout = 1;

                    number   = resposta.Substring(2, 2);

                    // Caso ultrapasse o limite de envio
                    if (count > 3500) return EnumRespostaTrans.TIMEOUT;

                    // Verifica a resposta
                    if (validateCmd(resposta))
                    {
                        programacao.Add(resposta.Trim().Substring(4, resposta.Length - 8));
                    }
                    else
                    {
                        return EnumRespostaTrans.INVALIDO;
                    }

                    count++;

                } while (resposta != (STX + number + CMD_FIM + DLE + ETX));

                return EnumRespostaTrans.FIM_TRANSMISSAO;

            }
            else if (resposta == (STX + number + DLE + NACK))
            {
                return EnumRespostaTrans.NACK;
            }
            else
            {
                return EnumRespostaTrans.INVALIDO;
            }
        }

        // Define o camando para solicitar a programação
        // STX | NUMBER | 00 | 02 | DLE | ETX | CHK
        private string montarComandoDeColeta()
        {
            // Define o valor do NUMBER
            int _number = this.pegarProxNumber();

            // Define o valor do CHK
            int _chk = _number ^ int.Parse(CMD_START) ^ int.Parse(CMD_SUB_IN);

            // Converte o NUMBER e o CHK em HEXA
            string number = Convert.ToString(_number, 16).PadLeft(2, '0').ToUpper();
            string chk = Convert.ToString(_chk, 16).PadLeft(2, '0').ToUpper();

            // Define o comando
            return STX + number + CMD_START + CMD_SUB_IN + DLE + ETX + chk;
        }

        private bool validateCmd(string cmd)
        {
            int _chk = Convert.ToInt32(cmd.Substring(cmd.Length - 2), 16);
            int _data_chk = 0;
            string _data = cmd.Trim().Substring(2, cmd.Length - 8);

            for (int i = 0; i < _data.Length; i = i + 2)
            {
                _data_chk = _data_chk ^ Convert.ToInt32(_data.Substring(i, 2), 16);
            }

            return (_data_chk == _chk ? true : false);
        }
    }
}