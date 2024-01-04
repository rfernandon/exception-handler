using System;
using System.Windows.Forms;

namespace CentraisCDX.Class.Comunicacao
{
    class Testar : TransferenciaSaida, IExecutavel
    {
        public Testar(string porta)
        {
            this.comunicacao = new Comunicacao(porta);
        }

        public void executar(Processo processo)
        {
            try
            {
                // Inicializa o camando
                string cmd = montarComandoDeTeste();

                // Inicializa o log
                Log.init();
                Log.addText("VERIFICANDO CONEXÃO:");

                comunicacao.abrirPorta();

                // Envia o comando e recebe a resposta
                // Possíveis respostas: 0 = ACK | 1 = NACK | 2 = TIMEOUT | 3 = INVALIDO
                respostaTrans = enviarComando(cmd);

                // Escreve o log
                Log.addText(cmd + " --><-- " + respostaTrans.ToString() + " [" + Transferencia.dadosRecebidos + "]");
                
                switch (respostaTrans)
                {
                    case (EnumRespostaTrans.ACK):
                        processo.mensagem = "Teste de conexão realizado com sucesso!";
                        processo.estado = EnumEstado.CONCLUIDO;
                        break;
                    case (EnumRespostaTrans.NACK):
                        processo.mensagem = "O teste de conexão falhou. A central não conseguiu reconhecer o comando enviado!";
                        processo.estado = EnumEstado.INCORRETO;
                        break;
                    case (EnumRespostaTrans.TIMEOUT):
                        processo.mensagem = "Ocorreu um erro de TIMEOUT ao tentar testar a conexão!";
                        processo.estado = EnumEstado.TIMEOUT;
                        break;
                    case (EnumRespostaTrans.INVALIDO):
                        processo.mensagem = "O aplicativo não conseguiu reconhecer o retorno enviado pela central ao testar a conexão!";
                        processo.estado = EnumEstado.INVALIDO;
                        break;
                }
            }
            catch (Exception Ex)
            {
                processo.mensagem = "Ocorreu uma falha ao tentar testar a porta COM!\n\nErro: " + Ex.Message;
                processo.estado = EnumEstado.EXCEPTION;
                Log.addText(Ex.Message);
            }
            finally
            {
                // Fecha a porta COM
                comunicacao.fecharPorta();

                // Grava o arquivo de log
                Log.gravarLog();

                // Informa o fim do processo
                processo.OnCompleted();
            }
        }
    
        // Define o camando para testar a comunicação
        private string montarComandoDeTeste()
        {
            // Define o valor do NUMBER
            int _number = this.pegarProxNumber();

            // Define o valor do CHK
            int _chk = _number ^ int.Parse(CMD_START) ^ int.Parse(CMD_SUB_CHECK);

            // Converte o NUMBER e o CHK em HEXA
            string number = Convert.ToString(_number, 16).PadLeft(2, '0').ToUpper();
            string chk = Convert.ToString(_chk, 16).PadLeft(2, '0').ToUpper();

            // Define o comando
            return STX + number + CMD_START + CMD_SUB_CHECK + DLE + ETX + chk;
        }
    }
}