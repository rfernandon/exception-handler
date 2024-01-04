/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guarda o estado da programação inicial.
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
using CentraisCDX.Class.Modelo;

namespace CentraisCDX.Class.Controle
{
    class ControleProgramacaoInicial
    {
        // INSTÂNCIA DO MODELO
        ProgramacaoInicial programacao = new ProgramacaoInicial();

        ControleRamal cRamal;

        // CONSTRUTOR
        public ControleProgramacaoInicial()
        {
            // INICIALIZA O ESTADO DO OBJETO (MODELO)
            this.programacao.iniciarCentral2 = "";
            this.programacao.iniciarCentral3 = "";
            this.programacao.modo = ProgramacaoInicial.ModoNumeracao.NORMAL;
            this.programacao.posicao = 1;
            this.programacao.qtdeDeBlocos = 0;
            this.programacao.nroPrimeiroBloco = 0;
            this.programacao.posicaoDoNumeroBloco = ProgramacaoInicial.PosicaoNroBloco.ANTES;
            this.programacao.qtdeAptoAndar = 0;
            this.programacao.qtdeDeAndares = 0;
            this.programacao.nroPrimeiroApto = "0";
            this.programacao.ramalBloco = false;
            this.programacao.ramalRestrito = false;
            this.programacao.ramalHot = false;
            this.programacao.multiploPorAndar = ProgramacaoInicial.Multiplo.CENTENA;
        }

        // MÉTODOS DE BUSCA
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o status do vídeo de um determinado item da lista.      */
        /* --------------------------------------------------------------------------------- */
        public ProgramacaoInicial buscarProgramacao()
        {
            return this.programacao;
        }

        // MÉTODOS DE INSERSÃO 
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Cria um conjunto de numeração com base nos dados formecidos pelo */
        /*                  usuário (modo andar).                                            */
        /* --------------------------------------------------------------------------------- */
        public void calcularNumeracaoAptos(ProgramacaoInicial pi, ControleRamal cRamal)
        {
            // Atualiza o objeto
            this.programacao = pi;
            this.cRamal = cRamal;

            // Define as variáveis utilizadas no método 
            int posicao = pi.posicao;
            string nroDoBloco = "";
            int bloco_fim = 0;
            int nroPrimeiroBloco = pi.nroPrimeiroBloco;

            // Limpa os primeiros ramais (caso exista)
            for (int p = 1; p < posicao; p++)
                this.definirRamal(p, "", false, false);

            if (pi.qtdeDeBlocos == 0)
                bloco_fim = 1;
            else bloco_fim = pi.qtdeDeBlocos;

            // Percorre todos os blocos
            for (int bloco_inicio = 1; bloco_inicio <= bloco_fim; bloco_inicio++)
            {
                // Verifica se existe blocos
                if (pi.qtdeDeBlocos != 0)
                {
                    // Define o número e a mascara do bloco (9 BLOCOS = # / 99 BLOCOS = ## / 999 BLOCOS = ###)
                    if (pi.qtdeDeBlocos >= 10 && pi.qtdeDeBlocos < 100)
                        nroDoBloco = nroPrimeiroBloco++.ToString("00");
                    else if (pi.qtdeDeBlocos >= 100 && pi.qtdeDeBlocos < 1000)
                        nroDoBloco = nroPrimeiroBloco++.ToString("000");
                    else nroDoBloco = nroPrimeiroBloco++.ToString("0");

                    // Verifica se existe ramal de bloco
                    if (pi.ramalBloco)
                    {
                        /*
                        string key = "R" + posicao;
                        Ramal r = cRamal.buscarRamalPorKey(key);
                        r.apartamento = nroDoBloco;
                        r.ramalHOT = false;
                        r.ramalBLOCO = pi.ramalBloco;
                        r.ramalRESTRITO = false;
                        r.acessoLinhaExterna = false;
                        r.placaPortPhone = false;
                        r.atendedor = "M1";
                        cRamal.inserirRamalPorKey(key, r);
                        posicao++;
                         */
                    }
                }
                else nroDoBloco = "";
                
                // Percorre todos os andares do bloco
                for (int andar = 1; andar <= pi.qtdeDeAndares; andar++)
                {
                    // Percorre todos os apartamentos do andar
                    for (int apto = 1; apto <= pi.qtdeAptoAndar; apto++)
                    {
                        // Define o número do primeiro apartamento do andar corrente
                        string apartamento = retornarNumeroRamal(pi.modo, apto, andar, (int)pi.multiploPorAndar, pi.nroPrimeiroApto, nroDoBloco, pi.posicaoDoNumeroBloco);

                        // Define em qual central será colocado o número do apartamento
                        if (apartamento == pi.iniciarCentral2 && posicao <= 1025)
                        {
                            this.limparNumeroRamal(posicao, 1024);
                            posicao = 1025;
                        }
                        else if (apartamento == pi.iniciarCentral3 && posicao <= 2049)
                        {
                            this.limparNumeroRamal(posicao, 2048); ;
                            posicao = 2049;
                        }

                        // Define a programação do ramal referente a uma determinada posição
                        this.definirRamal(posicao, apartamento, pi.ramalHot, pi.ramalRestrito);

                        // Incrementa a posição do ramal
                        posicao++;
                    }
                }
            }
            this.limparNumeroRamal(posicao, 3072);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o número do ramal (apto) com base nos dados formecidos   */
        /*                  pelo usuário.                                                    */
        /* --------------------------------------------------------------------------------- */
        private string retornarNumeroRamal(
            ProgramacaoInicial.ModoNumeracao modo,
            int apto,
            int andar,
            int multAndar,
            string nro1oApto, 
            string nroBloco,
            ProgramacaoInicial.PosicaoNroBloco posicao)
        {
            string result = "";
            string apartamento = "";

            // Separa os 0 (zeros) a esquerda do número
            string zeroEsquerda = "";
            int nroAptoSemZeroEsquerda = 0;
            for (int i = 0; i < nro1oApto.Length; i++)
            {
                if (nro1oApto[i] == '0')
                    zeroEsquerda = zeroEsquerda + nro1oApto[i];
                else
                {
                    string temp = nro1oApto.Substring(i, (nro1oApto.Length - zeroEsquerda.Length));
                    nroAptoSemZeroEsquerda = Convert.ToInt32(temp);
                    break;
                }
            }

            // Define o número do apartamento conforme o modo (ANDAR / PRUMADA)
            if (modo == ProgramacaoInicial.ModoNumeracao.PRUMADA)
                apartamento = zeroEsquerda + ((nroAptoSemZeroEsquerda + ((multAndar * apto) - multAndar)) + (andar - 1));
            else apartamento = zeroEsquerda + ((multAndar * (andar - 1)) + (nroAptoSemZeroEsquerda + (apto - 1))).ToString();

            // Verifica se existe blocos
            if (nroBloco != "")
            {
                // Verifica a posição do número do bloco no ramal (E = ANTES / D = DEPOIS)
                if (posicao == ProgramacaoInicial.PosicaoNroBloco.ANTES)
                    result = string.Concat(nroBloco, apartamento);
                else if (posicao == ProgramacaoInicial.PosicaoNroBloco.DEPOIS) result = string.Concat(apartamento, nroBloco);
            }
            else result = apartamento;

            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o ramal com base nos dados formecidos pelo usuário.       */
        /*                  Verifica se não ultrapassou a capacidade da central.             */
        /* --------------------------------------------------------------------------------- */
        private void definirRamal(int posicao, string apartamento, bool ramalHot, bool ramalRestrito)
        {
            if (posicao <= 3072)
            {
                /*
                string key = "R" + posicao;
                Ramal r = cRamal.buscarRamalPorKey(key);
                r.apartamento = apartamento;
                r.ramalHOT = ramalHot;
                r.ramalBLOCO = false;
                r.ramalRESTRITO = ramalRestrito;
                r.acessoLinhaExterna = false;
                r.placaPortPhone = false;
                r.atendedor = "M1";
                cRamal.inserirRamalPorKey(key, r);
                 */
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define os números dos ramais (de um determinado range) como "".  */
        /*                  As demais programação ficam default.                             */
        /* --------------------------------------------------------------------------------- */
        private void limparNumeroRamal(int inicio, int fim)
        {
            /*
            for (int x = inicio; x <= fim; x++)
            {
                string key = "R" + x;
                Ramal r = cRamal.buscarRamalPorKey(key);
                r.apartamento = "";
                r.ramalHOT = false;
                r.ramalBLOCO = false;
                r.ramalRESTRITO = false;
                r.acessoLinhaExterna = false;
                r.atendedor = "M1";
                cRamal.inserirRamalPorKey(key, r);
            }
             */
        }
    }
}