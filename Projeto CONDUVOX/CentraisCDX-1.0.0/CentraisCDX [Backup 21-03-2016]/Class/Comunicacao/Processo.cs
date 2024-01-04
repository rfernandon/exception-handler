using System.Threading;
using CentraisCDX.Class.Model;

namespace CentraisCDX.Class.Comunicacao
{
    public enum estados { PARADO, EXECUTANDO, CONCLUIDO, ABORTADO, INCORRETO, TIMEOUT, INVALIDO, EXCEPTION };

    public class Processo
    {
        private Thread thread;
        private IExecutavel executavel;
        public static string portaCOM;
        public static estados estado = estados.PARADO;
        public static string mensagem = "";

        public static int countComandoEnviado = 0;

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Inicia um novo processo em background (outra Thread).            */
        /* --------------------------------------------------------------------------------- */
        public void iniciarProcesso(IExecutavel ic)
        {
            Processo.estado = estados.EXECUTANDO;
            this.executavel = ic;
            this.thread = new Thread(processo);
            this.thread.Start();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Aborta o processo que está rodando em background.                */
        /* --------------------------------------------------------------------------------- */
        public void terminarProcesso()
        {
            Processo.estado = estados.PARADO;
            if (this.thread != null && this.thread.IsAlive)
                this.thread.Abort();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Método que será executado em background (outra Thread).          */
        /* --------------------------------------------------------------------------------- */
        private void processo()
        {
            this.executavel.executar();
        }
    }
}