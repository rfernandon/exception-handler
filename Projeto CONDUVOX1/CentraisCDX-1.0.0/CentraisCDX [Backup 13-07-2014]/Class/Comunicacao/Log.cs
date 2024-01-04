/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla os logs dos dados trafegados pela porta COM.
 * - Versão do arquivo    : 01
 * - Data criação         : 08/07/2014
 * - Data alteração       : 08/07/2014
 * - Desenvolvido por     : Ricardo Fernando
 * - Alterado por         : Ricardo Fernando
 * =================================================================================
 * HISTÓRICO DA VERSÃO
 * ---------------------------------------------------------------------------------
 * VERSÃO 01
 * - Desenvolvendo
 * =================================================================================
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CentraisCDX.Class.Comunicacao
{
    class Log
    {
        // ESTADO DO OBJETO
        public static String log = "";

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Grava um arquivo com o log.                                      */
        /* --------------------------------------------------------------------------------- */
        public void gravarLog()
        {
            string dia = System.DateTime.Now.Day.ToString();
            string mes = System.DateTime.Now.Month.ToString();
            string ano = System.DateTime.Now.Year.ToString();
            string hora = System.DateTime.Now.Hour.ToString();
            string min = System.DateTime.Now.Minute.ToString();
            string seg = System.DateTime.Now.Second.ToString();

            string nome_arquivo = "LOG_" + ano + "-" + mes + "-" + dia + "_" + hora + "-" + min + "-" + seg + ".txt";
            string caminho = Application.StartupPath + "\\log\\" + nome_arquivo;
            System.IO.TextWriter arquivo = System.IO.File.AppendText(caminho);
            string referecia = "LOG CRIADO EM: " + dia + "/" + mes + "/" + ano + " " + hora + ":" + min + ":" + seg + "\r\n";
            log = referecia + log;
            arquivo.WriteLine(log);
            arquivo.Close();
        }
    }
}