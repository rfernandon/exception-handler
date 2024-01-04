using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Model
{
    public enum posicaoNroBloco { ANTES, DEPOIS };
    public enum modoNumeracao { NORMAL, PRUMADA };

    class ProgramacaoInicial
    {
        // ESTADO DO OBJETO
        private int _posicao;

        private int _qtdeDeBlocos;
        private int _nroPrimeiroBloco;
        private posicaoNroBloco _posicaoDoNumeroBloco;

        private int _qtdeDeAndares;
        private int _multiploPorAndar;
        private int _qtdeAptoAndar;
        private string _nroPrimeiroApto;

        private bool _ramalHot;
        private bool _ramalRestrito;
        private bool _ramalBloco;

        private string _iniciarCentral2;
        private string _iniciarCentral3;

        private modoNumeracao _modo;

        // MÉTODOS GETTER E SETTER
        public int posicao
        {
            get { return _posicao; }
            set { _posicao = value; }
        }

        public int qtdeDeBlocos
        {
            get { return _qtdeDeBlocos; }
            set { _qtdeDeBlocos = value; }
        }

        public int nroPrimeiroBloco
        {
            get { return _nroPrimeiroBloco; }
            set { _nroPrimeiroBloco = value; }
        }

        public posicaoNroBloco posicaoDoNumeroBloco
        {
            get { return _posicaoDoNumeroBloco; }
            set { _posicaoDoNumeroBloco = value; }
        }

        public int qtdeDeAndares
        {
            get { return _qtdeDeAndares; }
            set { _qtdeDeAndares = value; }
        }

        public int multiploPorAndar
        {
            get { return _multiploPorAndar; }
            set { _multiploPorAndar = value; }
        }

        public int qtdeAptoAndar
        {
            get { return _qtdeAptoAndar; }
            set { _qtdeAptoAndar = value; }
        }

        public string nroPrimeiroApto
        {
            get { return _nroPrimeiroApto; }
            set { _nroPrimeiroApto = value; }
        }

        public bool ramalHot
        {
            get { return _ramalHot; }
            set { _ramalHot = value; }
        }

        public bool ramalRestrito
        {
            get { return _ramalRestrito; }
            set { _ramalRestrito = value; }
        }

        public bool ramalBloco
        {
            get { return _ramalBloco; }
            set { _ramalBloco = value; }
        }

        public string iniciarCentral2
        {
            get { return _iniciarCentral2; }
            set { _iniciarCentral2 = value; }
        }

        public string iniciarCentral3
        {
            get { return _iniciarCentral3; }
            set { _iniciarCentral3 = value; }
        }

        public modoNumeracao modo
        {
            get { return _modo; }
            set { _modo = value; }
        }
    }
}