/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado do tronco.
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
    class Tronco
    {
        // ESTADO DO OBJETO
        private bool _estado;
        private bool _estadoChamadaCobrar;
        private string _atendedor;

        // MÉTODOS GETTER E SETTER
        public bool estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        public bool estadoChamadaCobrar
        {
            get { return _estadoChamadaCobrar; }
            set { _estadoChamadaCobrar = value; }
        }

        public string atendedor
        {
            get { return _atendedor; }
            set { _atendedor = value; }
        }
    }
}