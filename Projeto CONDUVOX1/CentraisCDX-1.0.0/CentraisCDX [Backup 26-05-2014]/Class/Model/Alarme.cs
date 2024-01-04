/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado do alarme.
 * - Versão do arquivo    : 01
 * - Data criação         : 01/03/2014
 * - Data alteração       : 01/03/2014
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
using System.Collections;
using System.Collections.Generic;

namespace CentraisCDX.Class.Model
{
    class Alarme
    {
        // ESTADO DO OBJETO
        private string _numero;
        private string _tempo;
        private ArrayList _listAtendedores = new ArrayList();

        // MÉTODOS GETTER E SETTER
        public string numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        public string tempo
        {
            get { return _tempo; }
            set { _tempo = value; }
        }

        public ArrayList listAtendedores
        {
            get { return _listAtendedores; }
            set { _listAtendedores = value; }
        }
    }
}