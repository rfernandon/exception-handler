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
        /* Funcionalidade : Inicializa o conteúdo do log (evita gravar lixo no log).         */
        /* --------------------------------------------------------------------------------- */
        public static void init()
        {
            Log.log = "";
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Adiciona um texto no arquivo de log.                             */
        /* --------------------------------------------------------------------------------- */
        public static void addText(string value)
        {
            Console.WriteLine(value);
            Log.log += value + "\r\n";
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Grava um arquivo com o log.                                      */
        /* --------------------------------------------------------------------------------- */
        public static void gravarLog()
        {
            

            string dia  = Convert.ToString(System.DateTime.Now.Day).PadLeft(2, '0');
            string mes  = Convert.ToString(System.DateTime.Now.Month).PadLeft(2, '0');
            string ano  = System.DateTime.Now.Year.ToString();
            string hora = Convert.ToString(System.DateTime.Now.Hour).PadLeft(2, '0');
            string min  = Convert.ToString(System.DateTime.Now.Minute).PadLeft(2, '0');
            string seg  = Convert.ToString(System.DateTime.Now.Second).PadLeft(2, '0');

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