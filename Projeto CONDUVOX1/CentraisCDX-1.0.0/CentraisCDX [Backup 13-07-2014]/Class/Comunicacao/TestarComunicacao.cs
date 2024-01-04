using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CentraisCDX.Class.Comunicacao
{
    class TestarComunicacao
    {
        Thread thread;

        public void iniciarTeste()
        {
            thread = new Thread(processo);
            thread.Start();
        }

        public void terminarTeste()
        {
            if (thread.IsAlive)
                thread.Abort();
        }

        private void processo()
        {
            try
            {
                int x = 1;
                for (; ; )
                {

                    if (x == 10) throw new CentraisCDX.Class.Util.Exceptions.ComandoTimeOutException();
                    System.Threading.Thread.Sleep(1000);
                    Console.Beep();
                    x++;
                }
            }
            catch (Exception Ex)
            {
                System.Windows.Forms.MessageBox.Show(Ex.Message);
            }

        }
    }
}
