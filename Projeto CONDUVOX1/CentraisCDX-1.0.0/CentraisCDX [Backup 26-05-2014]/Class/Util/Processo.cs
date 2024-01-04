using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CentraisCDX.Class.Util
{
    abstract class Processo
    {
        Thread thread;

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Inicia um novo processo em background (outra Thread).            */
        /* --------------------------------------------------------------------------------- */
        public void iniciarProcesso()
        {
            thread = new Thread(processo);
            thread.Start();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Aborta o processo que está rodando em background.                */
        /* --------------------------------------------------------------------------------- */
        public void terminarProcesso()
        {
            if (thread.IsAlive)
                thread.Abort();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Método que será executado em background (outra Thread).          */
        /*                  Deve ser implementado na classe concreta.                        */
        /* --------------------------------------------------------------------------------- */
        public abstract void processo();
    }
}
