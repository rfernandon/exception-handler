using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CentraisCDX.Class.Util.Exceptions;

namespace CentraisCDX.Class.Comunicacao
{
    class DadosController
    {
        // ESTADO DO OBJETO
        private string STX = "02";
        private string DLE = "7F";
        private string ETX = "03";
        private string ACK = "06";
        private string NACK = "15";
        private string CMD_START = "00";
        private string CMD_SUB_CHECK = "00";
        private string CMD_SUB_OUT = "01";
        
        private int sequencia = 0;

        public DadosDTO getDadosEnvio()
        {
            // Atribui os comando no objeto
            DadosDTO dados = new DadosDTO();
            
            return dados;
        }


        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o comando que verifica se a central está respondendo.    */
        /* --------------------------------------------------------------------------------- */
        public string getCMDVerificarConexao()
        {
            // Define o valor do "number"
            string number = this.pegarHexProxNumber();

            // Define os valores que serão usados para criar o CHK
            int _number = this.converterHexToBinary(number);
            int _cmd_start = this.converterHexToBinary(CMD_START);
            int _cmd_check = this.converterHexToBinary(CMD_SUB_CHECK);

            // Define o CHK do comando
            string chk = (_number ^ (_cmd_start ^ _cmd_check)).ToString();

            // Completa o valor do CHK (caso necessário)
            if ((chk.Length % 2) != 0) chk = "0" + chk;

            // Define o comando
            return STX + number + CMD_START + CMD_SUB_CHECK + DLE + ETX + chk;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o comando que informará a central que será enviado os    */
        /*                  comando de programação.                                          */
        /* --------------------------------------------------------------------------------- */
        public string getCMDEnviarProgramacao()
        {
            // Define o valor do "number"
            string number = this.pegarHexProxNumber();

            // Define os valores que serão usados para criar o CHK
            int _number = this.converterHexToBinary(number);
            int _cmd_start = this.converterHexToBinary(CMD_START);
            int _cmd_out = this.converterHexToBinary(CMD_SUB_OUT);

            // Define o CHK do comando
            string chk = (_number ^ (_cmd_start ^ _cmd_out)).ToString();

            // Completa o valor do CHK (caso necessário)
            if ((chk.Length % 2) != 0) chk = "0" + chk;

            return STX + number + CMD_START + CMD_SUB_CHECK + DLE + ETX + chk;
        }


        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o valor em hexa do próximo número da sequencia.          */
        /*                  O método pode retornar 0 até FF (quando chegar em FF volta p/ 0) */
        /* --------------------------------------------------------------------------------- */
        private string pegarHexProxNumber()
        {
            if (sequencia > 255)
                sequencia = 0;
            string number = Convert.ToString(sequencia, 16);
            if (number.Length == 1)
                number = "0" + number;
            this.sequencia++;
            return number.ToUpper();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Converte Hexa para Binário.                                      */
        /* --------------------------------------------------------------------------------- */
        private int converterHexToBinary(string hex)
        {
            string binary = "";
            binary = Convert.ToString(Convert.ToInt32(hex, 16), 2);
            return Convert.ToInt32(binary);
        }
    }
}
