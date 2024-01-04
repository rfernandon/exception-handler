/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado da linha externa.
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

namespace CentraisCDX.Class.Model
{
    class LinhaExterna
    {
        // ESTADO DO OBJETO
        private List<Tronco> _listTronco = new List<Tronco>(2);

        private ArrayList _listNumeroLiberado = new ArrayList(20);
        private ArrayList _listNumeroBloqueado = new ArrayList(20);

        // CONSTRUTOR DA CLASSE
        public LinhaExterna()
        {
            this._listTronco.Add(new Tronco());
            this._listTronco.Add(new Tronco());

            for (int i = 1; i <= 20; i++)
                _listNumeroLiberado.Add("");

            for (int i = 1; i <= 20; i++)
                _listNumeroBloqueado.Add("");
        }

        // MÉTODOS GETTER E SETTER
        public List<Tronco> listTronco
        {
            get { return _listTronco; }
            set { _listTronco = value; }
        }

        public ArrayList listNumeroLiberado
        {
            get { return _listNumeroLiberado; }
            set { _listNumeroLiberado = value; }
        }

        public ArrayList listNumeroBloqueado
        {
            get { return _listNumeroBloqueado; }
            set { _listNumeroBloqueado = value; }
        }
    }
}
