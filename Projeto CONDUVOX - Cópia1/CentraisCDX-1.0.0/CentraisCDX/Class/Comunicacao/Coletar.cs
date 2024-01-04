using System;
using System.Windows.Forms;
using System.Collections;
using CentraisCDX.Class.Model;
using System.Collections.Generic;
using System.Media;
using CentraisCDX.Class.View;

namespace CentraisCDX.Class.Comunicacao
{
    class Coletar : TransferenciaEntrada, IExecutavel
    {
        public Coletar(Central c, string porta)
        {
            this.central = c;
            this.comunicacao = new Comunicacao(porta);

            this.programacao = new ArrayList();
            this.programacaoInvalida = new ArrayList();
            this.inicial = new ArrayList();
            this.ramal = new ArrayList();
            this.tronco = new ArrayList();
            this.mesa = new ArrayList();
            this.alarme = new ArrayList();
            this.video = new ArrayList();
        }

        public void executar(Processo processo)
        {
            try
            {
                // Inicializa o log
                Log.init();
                Log.addText("ENVIANDO A PROGRAMACAO:");

                processo.OnUpdateProgressBar(1);

                // Abre a porta COM
                comunicacao.abrirPorta();

                // Envia o comando e recebe a resposta
                // Possíveis respostas: 0 = ACK | 1 = NACK | 2 = TIMEOUT | 3 = INVALIDO
                respostaTrans = recuperarProgramacaoCentral(processo);

                if (respostaTrans != EnumRespostaTrans.FIM_TRANSMISSAO)
                {
                    // Separando as programações
                    foreach (string s in programacao)
                    {
                        switch (s.Substring(0, 2))
                        {
                            // 04 00FFFFFFF0
                            case ("04"):
                                inicial.Add(s);
                                break;
                            case ("01"):
                                ramal.Add(s);
                                break;
                            case ("02"):
                                tronco.Add(s);
                                break;
                            case ("03"):
                                mesa.Add(s);
                                break;
                            case ("07"):
                                alarme.Add(s);
                                break;
                            case ("08"):
                                video.Add(s);
                                break;
                            default:
                                programacaoInvalida.Add(s);
                                break;
                        }
                    }

                    if (programacaoInvalida.Count == 0)
                    {
                        central.programacaoInicial = getProgramacaoInicial();
                        central.listRamal          = getRamal();
                        central.linhaExterna       = getLinhaExterna();
                        central.listMesaOperadora  = getMesaOperadora();
                        central.alarme             = getAlarme();
                        central.listVideo          = getListaVideo();
                    }
                    else
                    {
                        respostaTrans = EnumRespostaTrans.INVALIDO;
                    }
                }

                // Define a resposta da coleta
                switch (respostaTrans)
                {
                    case (EnumRespostaTrans.NACK):
                        processo.mensagem = "O programa não conseguiu reconhecer o comando enviado pela central!";
                        processo.estado = EnumEstado.INCORRETO;
                        break;
                    case (EnumRespostaTrans.TIMEOUT):
                        processo.mensagem = "Ocorreu um erro de TIMEOUT ao tentar coletar a programação!";
                        processo.estado = EnumEstado.TIMEOUT;
                        break;
                    case (EnumRespostaTrans.FIM_TRANSMISSAO):
                        processo.mensagem = "A programação foi coletada da central com sucesso!";
                        processo.estado = EnumEstado.CONCLUIDO;
                        break;
                    case (EnumRespostaTrans.INVALIDO):
                        processo.mensagem = "Foram recebidos dados inválidos da central!";
                        processo.estado = EnumEstado.INVALIDO;
                        break;
                }

                processo.OnUpdateProgressBar(100);
            }
            catch (Exception Ex)
            {
                processo.mensagem = "Ocorreu uma falha ao tentar coletar a programação!\n\nErro: " + Ex.Message;
                processo.estado = EnumEstado.EXCEPTION;
                Log.addText(Ex.Message);
            }
            finally
            {
                // Fecha a porta COM
                comunicacao.fecharPorta();

                // Grava o arquivo de log
                Log.gravarLog();

                // Informa o fim do processo
                processo.OnCompleted();
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Criar o objeto Ramal com base na programação coletada.           */
        /* --------------------------------------------------------------------------------- */
        private ProgramacaoInicial getProgramacaoInicial()
        {
            // Comando = STX Number 04 SBC nnnnnnnn DLE ETX CHK
            // Exemplo = 0201 0400FFFFFFF0 7F030A
            ProgramacaoInicial pi = new ProgramacaoInicial();
            foreach (string cmd in this.inicial)
            {
                switch (cmd.Substring(2, 2))
                {
                    case ("01"):
                        // 0x 01 = Quantidade de blocos (0 - 999)
                        // nn nn nn nn = FF FF F9 99
                        pi.qtdeDeBlocos = Int32.Parse(cmd.Substring(4, 8));
                        break;
                    case ("02"):
                        // 0x 02 = Numero do primeiro bloco (0 – 999)
                        // nn nn nn nn = FF FF F9 99
                        pi.nroPrimeiroBloco = Int32.Parse(cmd.Substring(4, 8));
                        break;
                    case ("03"):
                        // 0x 03 = Numero do bloco 
                        // Antes  = nn nn nn nn = 00 00 00 01
                        // Depois = nn nn nn nn = 00 00 00 00
                        pi.posicaoDoNumeroBloco = Int32.Parse(cmd.Substring(4, 8)) == 1 ? posicaoNroBloco.ANTES : posicaoNroBloco.DEPOIS; 
                        break;
                    case ("04"):
                        // 0x 04 = Quantidade de apto por andar (0 - 999)
                        // nn nn nn nn = FF FF F9 99
                        pi.qtdeAptoAndar = Int32.Parse(cmd.Substring(4, 8));
                        break;
                    case ("05"):
                        // 0x 05 = Quantidade de andares (0 - 999)
                        // nn nn nn nn = FF FF F9 99
                        pi.qtdeDeAndares = Int32.Parse(cmd.Substring(4, 8));
                        break;
                    case ("06"):
                        // 0x 06 = Numero do primeiro apto (0 - 99999)
                        // nn nn nn nn = FF F9 99 99
                        pi.nroPrimeiroApto = cmd.Substring(4, 8);
                        break;
                    case ("07"):
                        // 0x 07 = Possui Ramal de bloco 
                        // Sim = nn nn nn nn = 00 00 00 01
                        // Não = nn nn nn nn = 00 00 00 00
                        pi.ramalBloco = Int32.Parse(cmd.Substring(4, 8)) == 1 ? true : false;
                        break;
                    case ("08"):
                        // 0x 08 = Ramais restritos
                        // Sim = nn nn nn nn = 00 00 00 01
                        // Não = nn nn nn nn = 00 00 00 00
                        pi.ramalRestrito = Int32.Parse(cmd.Substring(4, 8)) == 1 ? true : false;
                        break;
                    case ("09"):
                        // 0x 09 = Acrescer no numero por andar
                        // Unidade = nn nn nn nn = 00 00 00 00
                        // Dezena  = nn nn nn nn = 00 00 00 01
                        // Centena = nn nn nn nn = 00 00 00 02
                        // Milhar  = nn nn nn nn = 00 00 00 03
                        if (Int32.Parse(cmd.Substring(4, 8)) == 0)
                        {
                            pi.multiploPorAndar = 0;
                        }
                        else if (Int32.Parse(cmd.Substring(4, 8)) == 1)
                        {
                            pi.multiploPorAndar = 10;
                        }
                        else if (Int32.Parse(cmd.Substring(4, 8)) == 2)
                        {
                            pi.multiploPorAndar = 100;
                        }
                        else if (Int32.Parse(cmd.Substring(4, 8)) == 3)
                        {
                            pi.multiploPorAndar = 1000;
                        }
                        break;
                    case ("0A"):
                        // 0x 0A = Numeração
                        // Normal  = nn nn nn nn = 00 00 00 00
                        // Prumada = nn nn nn nn = 00 00 00 01
                        pi.modo = Int32.Parse(cmd.Substring(4, 8)) == 0 ? modoNumeracao.NORMAL : modoNumeracao.PRUMADA;
                        break;
                    case ("0B"):
                        // 0x 0B = Numero inicial da central 2
                        // nn nn nn nn = FF F9 99 99
                        pi.iniciarCentral2 = cmd.Substring(4, 8).Replace("F", "");
                        break;
                    case ("0C"):
                        // 0x 0C = Numero inicial da central 3
                        // nn nn nn nn = FF F9 99 99
                        pi.iniciarCentral3 = cmd.Substring(4, 8).Replace("F", "");
                        break;
                }
            }

            return pi;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Criar o objeto Ramal com base na programação coletada.           */
        /* --------------------------------------------------------------------------------- */
        private List<Ramal> getRamal()
        {
            // Comando = STX Number 01 NN NN nn nn nn nn BPN BL RST LT xx xx xx xx DLE ETX CHK
            // Exemplo = 020E 01FFF1FFFFFFF100000000FFFFFFFB 7F030B
            List<Ramal> listaRamal = new List<Ramal>();
            int i = 1;
            foreach (string cmd in this.alarme)
            {
                Ramal r = new Ramal();
                r.id_ramal = "R"+i;
                r.ramal = cmd.Substring(2, 4).Replace("F", "");
                r.apartamento = cmd.Substring(6, 8).Replace("F", "").Replace("B", "*").Replace("C", "#");
                r.ramalHOT = cmd.Substring(16, 2) == "01" ? true : false; ;
                r.placaPortPhone = cmd.Substring(16, 2) == "02" ? true : false; ;
                r.ramalBLOCO = cmd.Substring(16, 2) == "01" ? true : false;
                r.ramalRESTRITO = cmd.Substring(18, 2) == "01" ? true : false;
                r.acessoLinhaExterna = cmd.Substring(20, 2) == "01" ? true : false;
                r.atendedor = cmd.Substring(22, 8).Replace("F", "").Replace("B", "*").Replace("C", "#");

                listaRamal.Add(r);

                i++;
            }

            return listaRamal;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Criar o objeto LinhaExterna/Tronco com base na progr. coletada.  */
        /* --------------------------------------------------------------------------------- */
        private LinhaExterna getLinhaExterna()
        {
            // Comando = STX Number 02 00 NUM-TRONCO TR AC nn nn nn nn DLE ETX CHK
            // Exemplo = 0210 0200010000FFFFFFFB 7F0317
            List<Tronco> listaTronco = new List<Tronco>(2);
            for (int i = 0; i < 2; i++)
            {
                string cmd = (string)this.tronco[i];

                Tronco t = new Tronco();
                t.estado = cmd.Substring(6, 2) == "01" ? true : false;
                t.estadoChamadaCobrar = cmd.Substring(8, 2) == "01" ? true : false;
                t.atendedor = cmd.Substring(10, 8).Replace("F", "").Replace("B", "*").Replace("C", "#");

                listaTronco.Add(t);
            }

            // Comando = STX Number 02 SBC XX xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx DLE ETX CHK
            // Exemplo = 0212 020201FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF 7F0313
            ArrayList listaNumeroLiberado = new ArrayList(20);
            ArrayList listaNumeroBloqueado = new ArrayList(20);
            for (int i = 2; i < this.tronco.Count; i++)
            {
                string cmd = (string)this.tronco[i];

                if (cmd.Substring(4, 2) == "01")
                {
                    listaNumeroBloqueado.Add(cmd.Substring(6, 40).Replace("F", "").Replace("B", "*").Replace("C", "#"));
                }
                else
                {
                    listaNumeroLiberado.Add(cmd.Substring(6, 40).Replace("F", "").Replace("B", "*").Replace("C", "#"));
                }
            }

            LinhaExterna linha = new LinhaExterna();
            linha.listNumeroLiberado = listaNumeroLiberado;
            linha.listNumeroBloqueado = listaNumeroBloqueado;
            linha.listTronco = listaTronco;

            return linha;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Criar o objeto MesaOperadora com base na programação coletada.   */
        /* --------------------------------------------------------------------------------- */
        private List<MesaOperadora> getMesaOperadora()
        {
            return new List<MesaOperadora>();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Criar o objeto Alarme com base na programação coletada.          */
        /* --------------------------------------------------------------------------------- */
        private Alarme getAlarme()
        {
            // Comando = STX Number 07 nn nn nn nn POS-ATENDEDOR xx xx xx xx tt DLE ETX CHK
            // Exemplo = 028A 07FFFFFFFF01FFFFFFFF10 7F039C
            Alarme alarme = new Alarme();
            ArrayList atendedores = new ArrayList(10);
            foreach (string cmd in this.alarme)
            {
                alarme.numero = cmd.Substring(2, 8).Replace("F", "").Replace("B", "*").Replace("C", "#");
                alarme.tempo = cmd.Substring(20, 2);

                atendedores.Add(cmd.Substring(12, 8).Replace("F", "").Replace("B", "*").Replace("C", "#"));
            }
            alarme.listAtendedores = atendedores;

            return alarme;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Criar o objeto Video com base na programação coletada.           */
        /* --------------------------------------------------------------------------------- */
        private List<Video> getListaVideo()
        {
            // Comando = Comando = STX Number 08 VV A/D nn nn nn nn DLE ETX CHK
            // Exemplo = 0294 080100FFFFFFFF 7F039D
            List<Video> listaVideo = new List<Video>();
            foreach (string cmd in this.video)
            {
                Video v = new Video();
                v.estado = cmd.Substring(4, 2) == "01" ? true : false;
                v.numero = cmd.Substring(6, 8).Replace("F", "").Replace("B", "*").Replace("C", "#");

                listaVideo.Add(v);
            }

            return listaVideo;
        }
    }
}
