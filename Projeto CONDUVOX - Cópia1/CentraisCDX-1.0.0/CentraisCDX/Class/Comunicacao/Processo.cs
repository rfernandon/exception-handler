using System;
using System.Threading;
using CentraisCDX.Class.Model;

namespace CentraisCDX.Class.Comunicacao
{
    public class Processo
    {
        // A delegate type for hooking up change notifications.
        public delegate void JobCompletedEventHandler(object sender);
        public delegate void ProgressBarUpdatedEventHandler(object sender, int value);

        // Eventos
        public event JobCompletedEventHandler JobCompleted;
        public event ProgressBarUpdatedEventHandler ProgressBarUpdated;

        private Thread thread;
        private IExecutavel executavel;

        public bool cancelar = false;
        public string mensagem = "";
        public EnumEstado estado = EnumEstado.PARADO;

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Inicia um novo processo em background (outra Thread).            */
        /* --------------------------------------------------------------------------------- */
        public void iniciarProcesso(IExecutavel ic)
        {
            this.executavel = ic;
            this.thread = new Thread(run);
            this.thread.Start();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Método que será executado em background (outra Thread).          */
        /*                  A classe "IExecutavel" ficará atualizando o status do processo.  */
        /* --------------------------------------------------------------------------------- */
        private void run()
        {
            this.mensagem = "";
            this.estado = EnumEstado.EXECUTANDO;
            this.executavel.executar(this);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Dispara o evento informando a conclusão do processo.             */
        /* --------------------------------------------------------------------------------- */
        public virtual void OnCompleted()
        {
            JobCompleted(this);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Dispara o evento informando que o progress bar foi atualizado.   */
        /* --------------------------------------------------------------------------------- */
        public virtual void OnUpdateProgressBar(int value)
        {
            ProgressBarUpdated(this, value);
        }
    }
}