/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado da linha externa e seus objetos.
 * - Versão do arquivo    : 01
 * - Data criação         : 03/03/2014
 * - Data alteração       : 03/03/2014
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
using System.Collections;

namespace CentraisCDX.Class.Modelo
{
    class LinhaExterna
    {
        // ESTADO DO OBJETO
        private Dictionary<int, Tronco> _listTronco = new Dictionary<int, Tronco>();
        private Dictionary<int, string> _listNumeroLiberado = new Dictionary<int, string>();
        private Dictionary<int, string> _listNumeroBloqueado = new Dictionary<int, string>();

        // MÉTODOS GETTER E SETTER
        public Dictionary<int, Tronco> listaDeTroncos
        {
            get { return _listTronco; }
            set { _listTronco = value; }
        }

        public Dictionary<int, string> listaDeNumerosLiberado
        {
            get { return _listNumeroLiberado; }
            set { _listNumeroLiberado = value; }
        }

        public Dictionary<int, string> listaDeNumerosBloqueado
        {
            get { return _listNumeroBloqueado; }
            set { _listNumeroBloqueado = value; }
        }
    }
}
