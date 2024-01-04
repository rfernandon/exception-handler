/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado da mesa operadora, suas teclas e seus
 *                          objetos.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Modelo
{
    class MesaOperadora
    {
        // ESTADO DO OBJETO
        private string _key;
        private string _numero;
        private Dictionary<Tecla.Nome, Tecla> _listaDeTeclas = new Dictionary<Tecla.Nome, Tecla>();

        // MÉTODOS GETTER E SETTER
        public string key
        {
            get { return _key; }
            set { _key = value; }
        }
        
        public string numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        public Dictionary<Tecla.Nome, Tecla> listaDeTeclas
        {
            get { return _listaDeTeclas; }
            set { _listaDeTeclas = value; }
        }
    }
}
