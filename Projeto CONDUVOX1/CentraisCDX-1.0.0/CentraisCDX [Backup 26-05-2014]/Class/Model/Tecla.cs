/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado das teclas da mesa operadora.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class.Model
{
    class Tecla
    {
        // ESTADO DO OBJETO
        private string _atendedor;
        private nome _nome;
        private estado _estado;

        // MÉTODOS GETTER E SETTER
        public Tecla(nome n, estado e)
        {
            this._nome = n;
            this._estado = e;
        }

        public string atendedor
        {
            get { return _atendedor; }
            set { _atendedor = value; }
        }

        public nome nome
        {
            get { return _nome; }
            //set { _nome = value; }
        }

        public estado estado
        {
            get { return _estado; }
            set { _estado = value; }
        }
    }
}
