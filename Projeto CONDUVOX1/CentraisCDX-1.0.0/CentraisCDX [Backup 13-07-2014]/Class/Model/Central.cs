/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado da central e seus objetos.
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
using System.Collections;
using System.Collections.Generic;
using System;

namespace CentraisCDX.Class.Model
{
    class Central
    {
        // ESTADO DO OBJETO
        private ProgramacaoInicial _programacaoInicial = new ProgramacaoInicial();
        private Alarme _alarme = new Alarme();
        private LinhaExterna _linhaExterna = new LinhaExterna();

        private List<Ramal> _listRamal = new List<Ramal>();
        private List<Video> _listVideo = new List<Video>();
        private List<MesaOperadora> _listMesaOperadora = new List<MesaOperadora>();

        // MÉTODOS GETTER E SETTER
        public ProgramacaoInicial programacaoInicial
        {
            get { return _programacaoInicial; }
            set { _programacaoInicial = value; }
        }

        public Alarme alarme
        {
            get { return _alarme; }
            set { _alarme = value; }
        }

        public LinhaExterna linhaExterna
        {
            get { return _linhaExterna; }
            set { _linhaExterna = value; }
        }

        public List<Ramal> listRamal
        {
            get { return _listRamal; }
            set { _listRamal = value; }
        }

        public List<Video> listVideo
        {
            get { return _listVideo; }
            set { _listVideo = value; }
        }

        public List<MesaOperadora> listMesaOperadora
        {
            get { return _listMesaOperadora; }
            set { _listMesaOperadora = value; }
        }
    }
}