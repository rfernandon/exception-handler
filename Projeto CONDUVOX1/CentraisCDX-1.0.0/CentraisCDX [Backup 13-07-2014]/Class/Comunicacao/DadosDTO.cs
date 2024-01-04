using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Comunicacao
{
    class DadosDTO
    {
        // ESTADO DO OBJETO
        private string _verifica;
        private string _iniciarEnvio;
        private string _iniciarRecebimento;

        // MÉTODOS GETTER E SETTER
        public string verifica
        {
            get { return _verifica; }
            set { _verifica = value; }
        }

        public string iniciarEnvio
        {
            get { return _iniciarEnvio; }
            set { _iniciarEnvio = value; }
        }

        public string iniciarRecebimento
        {
            get { return _iniciarRecebimento; }
            set { _iniciarRecebimento = value; }
        }
    }
}
