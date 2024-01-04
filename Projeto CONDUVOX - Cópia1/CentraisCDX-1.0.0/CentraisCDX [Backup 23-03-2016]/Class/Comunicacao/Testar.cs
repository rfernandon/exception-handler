using System;
using System.Windows.Forms;

namespace CentraisCDX.Class.Comunicacao
{
    class Testar : Transferencia, IExecutavel
    {
        private string portaCOM;

        public Testar(string porta)
        {
            this.portaCOM = porta;
        }

        public void executar()
        {
            Resposta result;

            // Inicializa o log
            Log.log = "VERIFICANDO CONEXÃO:\r\n";

            // Define o valor do NUMBER
            int _number = this.pegarProxNumber();

            // Define o valor do CHK
            int _chk = _number ^ int.Parse(CMD_START) ^ int.Parse(CMD_SUB_CHECK);

            // Converte o NUMBER e o CHK em HEXA
            string number = Convert.ToString(_number, 16).PadLeft(2, '0').ToUpper();
            string chk    = Convert.ToString(_chk, 16).PadLeft(2, '0').ToUpper();

            // Define o comando
            string comando = STX + number + CMD_START + CMD_SUB_CHECK + DLE + ETX + chk;

            // Envia o comando e recebe a resposta
            Comunicacao transfer = new Comunicacao(portaCOM);

            try
            {
                transfer.abrirPorta();
                result = this.enviarComando(comando, transfer);
                transfer.fecharPorta();

                // Escreve o log
                if(result != Resposta.INVALIDO)
                    Log.log += comando + " --><-- " + result.ToString() + "\r\n";
                else Log.log += comando + " --><-- " + result.ToString() + " [" +Transferencia.dadosRecebidos+ "]\r\n";
            }
            catch (Exception Ex)
            {
                result = Resposta.EXCEPTION;
                Processo.mensagem = Ex.Message + "\nVerifique se ela não está sendo usada por outro processo.";
                Log.log += Ex.Message;
            }

            // Grava o arquivo de log
            Log.gravarLog();

            // Define o estado
            switch (result)
            {
                case (Resposta.ACK):
                    Processo.mensagem = "Teste de conexão realizado com sucesso.\n" + comando + "--><--" + Transferencia.dadosRecebidos;
                    Processo.estado = estados.CONCLUIDO;
                    break;

                case (Resposta.NACK):
                    Processo.mensagem = "O teste de conexão falhou. A central não reconheceu o comando enviado.\n" + comando + "--><--" + Transferencia.dadosRecebidos;
                    Processo.estado = estados.INCORRETO;
                    break;

                case (Resposta.TIMEOUT):
                    Processo.mensagem = "O teste de conexão falhou. Erro de TIMEOUT.\n" + comando + "--><--" + Transferencia.dadosRecebidos;
                    Processo.estado = estados.TIMEOUT;
                    break;

                case (Resposta.INVALIDO):
                    Processo.mensagem = "O teste de conexão falhou. A central retornou um comando inválido.\n" + comando + "--><--" + Transferencia.dadosRecebidos;
                    Processo.estado = estados.INVALIDO;
                    break;
                case (Resposta.EXCEPTION):
                    Processo.estado = estados.EXCEPTION;
                    break;
            }
        }
    }
}