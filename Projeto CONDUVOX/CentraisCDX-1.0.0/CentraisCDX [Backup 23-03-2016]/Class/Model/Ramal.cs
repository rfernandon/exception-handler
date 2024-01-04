/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado do ramal.
 * - Versão do arquivo    : 01
 * - Data criação         : 09/03/2014
 * - Data alteração       : 09/03/2014
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

namespace CentraisCDX.Class.Model
{
    class Ramal
    {
        // ESTADO DO OBJETO
        private string _id_ramal = "";
        private string _ramal = "";
        private string _apartamento = "";
        private bool _ramalHOT = false;
        private bool _ramalBLOCO = false;
        private bool _ramalRESTRITO = false;
        private bool _placaPortPhone = false;
        private bool _acessoLinhaExterna = false;
        private string _atendedor = "";

        // MÉTODOS GETTER E SETTER
        public string id_ramal
        {
            get { return _id_ramal; }
            set { _id_ramal = value; }
        }

        public string ramal
        {
            get { return _ramal; }
            set { _ramal = value; }
        }

        public string apartamento
        {
            get { return _apartamento; }
            set { _apartamento = value; }
        }

        public bool ramalHOT
        {
            get { return _ramalHOT; }
            set { _ramalHOT = value; }
        }

        public bool ramalBLOCO
        {
            get { return _ramalBLOCO; }
            set { _ramalBLOCO = value; }
        }

        public bool ramalRESTRITO
        {
            get { return _ramalRESTRITO; }
            set { _ramalRESTRITO = value; }
        }

        public bool placaPortPhone
        {
            get { return _placaPortPhone; }
            set { _placaPortPhone = value; }
        }

        public bool acessoLinhaExterna
        {
            get { return _acessoLinhaExterna; }
            set { _acessoLinhaExterna = value; }
        }

        public string atendedor
        {
            get { return _atendedor; }
            set { _atendedor = value; }
        }
    }
}
