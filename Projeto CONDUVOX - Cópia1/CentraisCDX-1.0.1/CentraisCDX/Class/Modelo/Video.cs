/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado do vídeoa.
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
    class Video
    {
        // ESTADO DO OBJETO
        private bool _estado = false;
        private string _numero = "";

        // MÉTODOS GETTER E SETTER
        public bool estado
        {
            get { return _estado; }
            set { _estado = value; }
        }
        
        public string numero
        {
            get { return _numero; }
            set { _numero = value; }
        }
    }
}