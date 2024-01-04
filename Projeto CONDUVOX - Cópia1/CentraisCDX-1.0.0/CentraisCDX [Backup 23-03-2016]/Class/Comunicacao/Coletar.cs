using System;
using System.Windows.Forms;
using System.Collections;
using CentraisCDX.Class.Model;
using System.Collections.Generic;
using System.Media;
using CentraisCDX.Class.View;

namespace CentraisCDX.Class.Comunicacao
{
    class Coletar : Transferencia, IExecutavel
    {
        private Central central;
        private Comunicacao transfer;
        ArrayList programacao = new ArrayList();

        public Coletar(Central c, string porta)
        {
            this.central = c;
            this.transfer = new Comunicacao(porta);
        }

        public void executar()
        {
            // Define a variável que irá guardar a resposta da coleta
            Resposta result = Resposta.NACK;

            try
            {
                Processo.countComandoEnviado = 1;
                
                // Inicializa o log
                Log.log = "COLETANDO A PROGRAMAÇÃO:\r\n";

                // Abre a porta COM
                transfer.abrirPorta();

                // Envia o comando solicitando a programação
                // //STX | NUMBER | 00 | 02 | DLE | ETX | CHK
                result = this.enviarComando(getComandoFull("0002"), transfer);

                


                // Envia todos os comandos
                foreach (String cmd in programacao)
                {
                    Processo.countComandoEnviado++;

                    

                    // Escreve o log
                    if (result != Resposta.INVALIDO)
                        Log.log += cmd + " --><-- " + result.ToString() + "\r\n";
                    else Log.log += cmd + " --><-- " + result.ToString() + " [" + Transferencia.dadosRecebidos + "]\r\n";

                    // Sai do loop caso ocorra algum erro no envio
                    if (result != Resposta.ACK) break;
                }

                // Fecha a porta COM
                transfer.fecharPorta();

                // Grava o arquivo de log
                Log.gravarLog();
            }
            catch (Exception Ex)
            {
                result = Resposta.EXCEPTION;
            }





            // Define o estado
            switch (result)
            {
                case (Resposta.FIM_TRANSMISSAO):
                    Processo.mensagem = "A programação foi recebida com sucesso.";
                    Processo.estado = estados.CONCLUIDO;
                    break;

                case (Resposta.NACK):
                    Processo.mensagem = "A coleta da programação falhou. O programa não reconheceu o comando enviado.";
                    Processo.estado = estados.INCORRETO;
                    break;

                case (Resposta.TIMEOUT):
                    Processo.mensagem = "O envio da programação falhou. Erro de TIMEOUT";
                    Processo.estado = estados.TIMEOUT;
                    break;

                case (Resposta.INVALIDO):
                    Processo.mensagem = "O envio da programação falhou. A central retornou um comando inválido.";
                    Processo.estado = estados.INVALIDO;
                    break;

                case (Resposta.EXCEPTION):
                    Processo.mensagem = "Ocorreu uma falha inesperada, favor reportar o erro ao suporte.";
                    Processo.estado = estados.EXCEPTION;
                    break;
            }
        }

        private string getComandoFull(string cmd)
        {
            // Define o valor do NUMBER
            int _number = this.pegarProxNumber();
            int _chk = _number;

            // Define o valor do CHK
            for (int i = 0; i < cmd.Length; i = i + 2)
            {
                _chk = _chk ^ Convert.ToInt32(cmd.Substring(i, 2), 16);
            }

            // Converte o NUMBER e o CHK em HEXA
            string number = Convert.ToString(_number, 16).PadLeft(2, '0').ToUpper();
            string chk = Convert.ToString(_chk, 16).PadLeft(2, '0').ToUpper();

            return STX + number + cmd + DLE + ETX + chk;
        }
    }
}
