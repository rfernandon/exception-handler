/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado das teclas da mesa operadora.
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
    class Tecla
    {
        // ENUMS ESTÁTICOS
        public enum Nome { ZELADOR, SINDICO, A1, A2, A3, PORTEIRO, TELEFONE, FECH1, FECH2 };
        public enum Estado { DESATIVADA, RAMAL, TELEFONE, FECHADURA };

        // ESTADO DO OBJETO
        private string _atendedor;
        private Nome _nome;
        private Estado _estado;

        // CONSTRUTOR DO OBJETO
        public Tecla(Nome nome, Estado estado, string atendedor)
        {
            this._nome = nome;
            this._estado = estado;
            this._atendedor = atendedor;
        }

        // MÉTODOS GETTER E SETTER
        public string atendedor
        {
            get { return _atendedor; }
            set { _atendedor = value; }
        }
        
        public Nome nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public Estado estado
        {
            get { return _estado; }
            set { _estado = value; }
        }
    }
}
