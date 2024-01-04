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
using CentraisCDX.Class.Modelo;

namespace CentraisCDX.Class.Controle
{
    class ControleCentral
    {
        // ESTADO DO OBJETO
        private ControleProgramacaoInicial cInicial = new ControleProgramacaoInicial();
        private ControleAlarme cAlarme = new ControleAlarme();
        private ControleLinhaExterna cLinha = new ControleLinhaExterna();
        private ControleVideo cVideo = new ControleVideo();

        // CONSTRUTOR
        public ControleCentral()
        {
            ControleRamal.inicializar();
            ControleMO.inicializar();
        }

        // OPERAÇÕES DIVERSAS
        public string buscarAtendedorPorKey(string key)
        {
            string atendedor = ControleRamal.buscarApartamentoPorKey(key);
            if (atendedor != null)
            {
                return atendedor;
            }
            else return ControleMO.buscarNumeroPorKey(key);
        }

        // OPERAÇÕES DA FUNCIONALIDADE "RAMAL"
        public List<Ramal> buscarListaDeRamais()
        {
            return ControleRamal.buscarListaDeRamais();
        }

        public void incluirNovoRamal(Ramal ramal, int posicao)
        {
            ControleRamal.incluirNovoRamal(ramal, posicao);
        }


        // ############################################################################################################
        // OPERAÇÕES DA FUNCIONALIDADE "PROGRAMAÇÃO INICIAL"
        public ProgramacaoInicial buscarProgramacaoInicial()
        {
            return cInicial.buscarProgramacao();
        }

        public void calcularNumeracaoAptos(ProgramacaoInicial pi)
        {
            //cInicial.calcularNumeracaoAptos(pi, ControleRamal);
        }

        

        






        /*
        public Ramal buscarRamalPorKey(string key)
        {
            return this.cRamal.buscarRamalPorKey(key);
        }

        public void removerRamais(ArrayList listaDeRamais)
        {
            this.cRamal.removerRamais(listaDeRamais);
        }
        */
        // OPERAÇÕES DA FUNCIONALIDADE "MESA_OPERADORA"
        



    }
}