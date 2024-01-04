/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado do alarme.
 * - Versão do arquivo    : 01
 * - Data criação         : 02/10/2014
 * - Data alteração       : 02/10/2014
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

namespace CentraisCDX.Class.Modelo
{
    class Alarme
    {
        // ESTADO DO OBJETO
        private string _numero;
        private string _tempo;
        private Dictionary<int, string> _listaDeAtendedores = new Dictionary<int,string>();

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

        public Dictionary<int, string> listaDeAtendedores
        {
            get { return _listaDeAtendedores; }
            set { _listaDeAtendedores = value; }
        }
    }
}