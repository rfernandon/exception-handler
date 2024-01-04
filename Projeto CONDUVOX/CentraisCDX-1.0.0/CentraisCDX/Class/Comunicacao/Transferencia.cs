using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CentraisCDX.Class.Model;

namespace CentraisCDX.Class.Comunicacao
{
    abstract class Transferencia
    {
        protected Comunicacao comunicacao;

        protected static int sequencia = 0;

        protected static string dadosRecebidos;

        protected const string FIM = "05";
        protected const string STX = "02";
        protected const string DLE = "7F";
        protected const string ACK = "06";
        protected const string NACK = "15";
        protected const string ETX = "03";
        protected const string CMD_START = "00";
        protected const string CMD_SUB_CHECK = "00";
        protected const string CMD_SUB_OUT = "01";
        protected const string CMD_SUB_IN = "02";
        protected const string CMD_FIM = "05";
        protected const string CMD_CANCELAR = "06";

        protected Central central;
        protected ArrayList programacao;
        protected EnumRespostaTrans respostaTrans;

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

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna cmd para informar a central que o envio foi cancelado.   */
        /*                  COMANDO: STX | NUMBER | 06 | DLE | ETX | CHK                     */
        /* --------------------------------------------------------------------------------- */
        public string retornarComandoDeCancelamento()
        {
            // Define o valor do NUMBER
            int _number = this.pegarProxNumber();

            // Define o valor do CHK
            int _chk = _number ^ int.Parse(CMD_CANCELAR);

            // Converte o NUMBER e o CHK em HEXA
            string number = Convert.ToString(_number, 16).PadLeft(2, '0').ToUpper();
            string chk = Convert.ToString(_chk, 16).PadLeft(2, '0').ToUpper();

            // Define o comando
            return STX + number + CMD_CANCELAR + DLE + ETX + chk;
        }
    }
}