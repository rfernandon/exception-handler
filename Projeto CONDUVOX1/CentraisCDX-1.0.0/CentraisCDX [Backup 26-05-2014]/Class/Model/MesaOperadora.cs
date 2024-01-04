/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado da mesa operadora e suas teclas.
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
    public enum estado { DESATIVADA, RAMAL, TELEFONE, FECHADURA };
    public enum nome { ZELADOR, SINDICO, A1, A2, A3, PORTEIRO, TELEFONE, FECH1, FECH2 };

    class MesaOperadora
    {
        // ESTADO DO OBJETO
        private string _id_mesa;
        private string _numero;
        private List<Tecla> _listTecla = new List<Tecla>();

        // CONSTRUTOR DA CLASSE
        public MesaOperadora()
        {
            this._listTecla.Add(new Tecla(nome.ZELADOR, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.SINDICO, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.A1, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.A2, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.A3, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.PORTEIRO, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.TELEFONE, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.FECH1, estado.DESATIVADA));
            this._listTecla.Add(new Tecla(nome.FECH2, estado.DESATIVADA));
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o estado de uma determidada tecla.                      */
        /* --------------------------------------------------------------------------------- */
        public estado buscarPorNomeEstadoTecla(nome n)
        {
            return this._listTecla[(int)n].estado;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o atendedor de uma determidada tecla.                   */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorNomeAtendedorTecla(nome n)
        {
            return this._listTecla[(int)n].atendedor;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o estado de uma determidada tecla.                        */
        /* --------------------------------------------------------------------------------- */
        public void definirEstadoTecla(nome n, estado e)
        {
            this._listTecla[(int)n].estado = e;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o número do Atendedor de uma determidada tecla.           */
        /* --------------------------------------------------------------------------------- */
        public void definirAtendedorTecla(nome n, string s)
        {
            this._listTecla[(int)n].atendedor = s;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Método getter e setter.                                          */
        /* --------------------------------------------------------------------------------- */
        public string id_mesa
        {
            get { return _id_mesa; }
            set { _id_mesa = value; }
        }
        
        public string numero
        {
            get { return _numero; }
            set { _numero = value; }
        }
    }
}
