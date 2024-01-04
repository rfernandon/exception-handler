using System;
using System.Windows.Forms;
using System.Collections;
using CentraisCDX.Class.Model;
using System.Collections.Generic;
using System.Media;
using CentraisCDX.Class.View;

namespace CentraisCDX.Class.Comunicacao
{
    class Enviar : Transferencia, IExecutavel
    {
        private Central central;
        private ComunicacaoV01 transfer;
        ArrayList programacao = new ArrayList();

        public Enviar(Central c, string porta)
        {
            this.central = c;
            this.transfer = new ComunicacaoV01(porta);
        }

        public void executar()
        {
            Processo.countComandoEnviado = 0;

            // Define a variável que irá guardar a rsposta do envio
            Resposta result = Resposta.NACK;

            // Inicializa a lista de programação
            this.montarListaProgramacao();

            // Inicializa o log
            Log.log = "ENVIANDO PROGRAMAÇÃO:\r\n";

            // Abre a porta COM
            transfer.abrirPorta();

            // Envia todos os comandos
            foreach (String cmd in programacao)
            {
                Processo.countComandoEnviado++;

                // Envia o comando e recebe a resposta
                result = this.enviarComando(cmd, transfer);

                //MessageBox.Show(cmd + "--><--" + Transferencia.dadosRecebidos);
                
                // Escreve o log
                if (result != Resposta.INVALIDO)
                    Log.log += cmd + " --><-- " + result.ToString() + "\r\n";
                else Log.log += cmd + " --><-- " + result.ToString() + " [" + Transferencia.dadosRecebidos + "]\r\n";

                // Sai do loop caso ocorra algum erro no envio
                if (result != Resposta.ACK) break;
            }

            // Fecha a porta COM
            transfer.fecharPorta();

            // Grava o arquivo de log
            Log.gravarLog();

            // Define o estado
            switch (result)
            {
                case (Resposta.ACK):
                    Processo.mensagem = "A programação foi enviada para a central com sucesso.";
                    Processo.estado = estados.CONCLUIDO;
                    break;

                case (Resposta.NACK):
                    Processo.mensagem = "O envio da programação falhou. A central não reconheceu o comando enviado.";
                    Processo.estado = estados.INCORRETO;
                    break;

                case (Resposta.TIMEOUT):
                    Processo.mensagem = "O envio da programação falhou. Erro de TIMEOUT";
                    Processo.estado = estados.TIMEOUT;
                    break;

                case (Resposta.INVALIDO):
                    Processo.mensagem = "O envio da programação falhou. A central retornou um comando inválido.";
                    Processo.estado = estados.INVALIDO;
                    break;
            }
        }

        private void montarListaProgramacao()
        {
            // Inicializa lista de programação
            programacao = new ArrayList();

            // Define o comando para inicalizar o envio = STX | NUMBER | 00 | 01 | DLE | ETX | CHK
            addComando("00" + "01");

            // Prepara a programação inicial
            this.colherProgramacaoInicial();

            // Prepara a programação dos ramais
            this.colherProgramacaoRamais();

            // Prepara a programação dos troncos e dos números liberados/bloqueados
            this.colherProgramacaoTroncos();

            // Prepara a programação da mesa de operadora
            this.colherProgramacaoMesaOperadora();

            // Prepara a programação do alarme
            this.colherProgramacaoAlarme();

            // Prepara a programação dos Vídeos
            this.colherProgramacaoVideos();

            // Define o comando para finalizar o envio = STX | NUMBER | 05 | DLE | ETX | CHK
            addComando("05");
        }

        // Prepara a programação inicial
        // Comando = STX Number 04 SBC nnnnnnnn DLE ETX CHK
        private void colherProgramacaoInicial()
        {
            string data;

            // Recupera o objeto
            ProgramacaoInicial pi = central.programacaoInicial;

            // 0x00 = Quantidade de ramais 
            // nn nn nn nn = FF FF 12 34
            int total = pi.qtdeAptoAndar * pi.qtdeDeAndares;
            if (pi.qtdeDeBlocos > 2) total = total * pi.qtdeDeBlocos;
            data = "04" + "00" + Convert.ToString(total).PadLeft(8, 'F');
            addComando(data);

            // 0x 01 = Quantidade de blocos (0 - 999)
            // nn nn nn nn = FF FF F9 99
            // pi.qtdeDeBlocos = Convert.ToInt32(this.txtQtdeBlocos.Text);
            data = "04" + "01" + Convert.ToString(pi.qtdeDeBlocos).PadLeft(8, 'F');
            addComando(data);

            // 0x 02 = Numero do primeiro bloco (0 – 999)
            // nn nn nn nn = FF FF F9 99
            // pi.nroPrimeiroBloco = Convert.ToInt32(this.txtQtdeBlocos.Text) == 0 ? 0 : Convert.ToInt32(this.txtNro1oBloco.Text);
            data = "04" + "02" + Convert.ToString(pi.qtdeDeBlocos).PadLeft(8, 'F');
            addComando(data);

            // 0x 03 = Numero do bloco 
            // Antes  = nn nn nn nn = 00 00 00 01
            // Depois = nn nn nn nn = 00 00 00 00
            // pi.posicaoDoNumeroBloco = this.cboPosicaoNroBloco.SelectedIndex == 0 ? posicaoNroBloco.ANTES : posicaoNroBloco.DEPOIS;
            int posicao = pi.posicaoDoNumeroBloco == 0 ? 1 : 0;
            data = "04" + "03" + Convert.ToString(posicao).PadLeft(8, '0');
            addComando(data);

            // 0x 04 = Quantidade de apto por andar (0 - 999)
            // nn nn nn nn = FF FF F9 99
            // pi.qtdeAptoAndar = Convert.ToInt32(this.txtQtdeAptoAndar.Text);
            data = "04" + "04" + Convert.ToString(pi.qtdeAptoAndar).PadLeft(8, 'F');
            addComando(data);

            // 0x 05 = Quantidade de andares (0 - 999)
            // nn nn nn nn = FF FF F9 99
            // pi.qtdeDeAndares = Convert.ToInt32(this.txtQtdeAndares.Text);
            data = "04" + "05" + Convert.ToString(pi.qtdeDeAndares).PadLeft(8, 'F');
            addComando(data);

            // 0x 06 = Numero do primeiro apto (0 - 99999)
            // nn nn nn nn = FF F9 99 99
            // pi.nroPrimeiroApto = this.txtNro1oApto.Text;
            data = "04" + "06" + pi.nroPrimeiroApto.PadLeft(8, 'F');
            addComando(data);

            // 0x 07 = Possui Ramal de bloco 
            // Sim = nn nn nn nn = 00 00 00 01
            // Não = nn nn nn nn = 00 00 00 00
            // pi.ramalBloco = this.cboDefinirRamalBloco.SelectedIndex == 0 ? false : true;
            int bloco = pi.ramalBloco == true ? 1 : 0;
            data = "04" + "07" + Convert.ToString(bloco).PadLeft(8, '0');
            addComando(data);

            // 0x 08 = Ramais restritos
            // Sim = nn nn nn nn = 00 00 00 01
            // Não = nn nn nn nn = 00 00 00 00
            // pi.ramalRestrito = this.cboDefinirRamalRestrito.SelectedIndex == 0 ? false : true;
            int restrito = pi.ramalRestrito == true ? 1 : 0;
            data = "04" + "08" + Convert.ToString(restrito).PadLeft(8, '0');
            addComando(data);

            // 0x 09 = Acrescer no numero por andar
            // Unidade = nn nn nn nn = 00 00 00 00
            // Dezena  = nn nn nn nn = 00 00 00 01
            // Centena = nn nn nn nn = 00 00 00 02
            // Milhar  = nn nn nn nn = 00 00 00 03
            int multiplo = 0;
            if (pi.multiploPorAndar == 0)
                multiplo = 0;
            else if (pi.multiploPorAndar == 10)
                multiplo = 1;
            else if (pi.multiploPorAndar == 100)
                multiplo = 2;
            else if (pi.multiploPorAndar == 1000)
                multiplo = 3;
            data = "04" + "09" + Convert.ToString(multiplo).PadLeft(8, '0');
            addComando(data);

            // 0x 0A = Numeração
            // Normal  = nn nn nn nn = 00 00 00 00
            // Prumada = nn nn nn nn = 00 00 00 01
            // pi.modo = this.cboSequenciaRamal.SelectedIndex == 0 ? modoNumeracao.NORMAL : modoNumeracao.PRUMADA;
            if (pi.modo == modoNumeracao.NORMAL)
                data = "04" + "0A" + "00000000";
            else if (pi.modo == modoNumeracao.PRUMADA)
                data = "04" + "0A" + "00000001";
            addComando(data);

            // 0x 0B = Numero inicial da central 2
            // nn nn nn nn = FF F9 99 99
            // pi.iniciarCentral2 = this.txtNroInicialCentral2.Text;
            data = "04" + "0B" + pi.iniciarCentral2.PadLeft(8, 'F');
            addComando(data);

            // 0x 0C = Numero inicial da central 3
            // nn nn nn nn = FF F9 99 99
            // pi.iniciarCentral2 = this.txtNroInicialCentral2.Text;
            data = "04" + "0C" + pi.iniciarCentral3.PadLeft(8, 'F');
            addComando(data);
        }

        // Prepara a programação dos ramais
        private void colherProgramacaoRamais()
        { 
            string data;
            List<Ramal> ramais = central.listRamal;

            // Prepara oo alarme
            for (int i = 0; i < ramais.Count; i++)
            {
                /*
                 * Comando = STX Number 01 NN NN nn nn nn nn BPN BL RST LT xx xx xx xx DLE ETX CHK
                 * 
                 * NN NN = POSIÇÃO DO RAMAL EM HEXADECIMAL
                 * Ex. Posição 1234 = 12 34
                 * 
                 * nn nn nn nn = NUMERO PROGRAMADO DO RAMAL
                 * Ex. Ramal 1234 = FF FF 12 34
                 * 
                 * BPN (TIPO DO RAMAL)
                 * - 0x00 = RAMAL NORMAL
                 * - 0x01 = RAMAL HOT
                 * - 0x02 = RAMAL Port-CDX
                 * 
                 * BL (RAMAL DE BLOCO)
                 * - 0x00 = RAMAL NORMAL
                 * - 0x01 = RAMAL DE BLOCO
                 * 
                 * RST (RAMAL RESTRITO)
                 * - 0x00 = RAMAL NORMAL 
                 * - 0x01 = RAMAL RESTRITO
                 * 
                 * LT (ACESSO LINHA EXTERNA)
                 * - 0x00 = NÃO TEM ACESSO A LINHA
                 * - 0x01 = TEM ACESSO A LINHA
                 * 
                 * xx xx xx xx = NUMERO DO RAMAL PROGRAMADO COMO ATENDEDOR
                 * Ex. Ramal 1234 - xx xx xx xx= FF FF 12 34
                 */
                data = "01";
                data += ramais[i].ramal.PadLeft(4, 'F');
                data += ramais[i].apartamento.PadLeft(8, 'F');

                // Define o tipo do RAMAL
                if(ramais[i].ramalHOT)
                    data += "01";
                else if(ramais[i].placaPortPhone)
                    data += "02";
                else 
                    data += "00";

                // Define se o ramal é de BLOCO
                if (ramais[i].ramalBLOCO)
                    data += "01";
                else
                    data += "00";

                // Define se o ramal é RESTRITO
                if (ramais[i].ramalRESTRITO)
                    data += "01";
                else
                    data += "00";

                // Define se o ramal é RESTRITO
                if (ramais[i].acessoLinhaExterna)
                    data += "01";
                else
                    data += "00";

                data += buscarPorChaveNumero(ramais[i].atendedor).PadLeft(8, 'F');

                addComando(data);
            }
        }

        // Prepara a programação dos troncos e os números liberados/bloqueados
        private void colherProgramacaoTroncos()
        {
            string data;

            // Prepara os troncos
            List<Tronco> ts = central.linhaExterna.listTronco;
            for (int i = 0; i < ts.Count; i++)
            {
                /*
                 * Comando = STX Number 02 00 NUM-TRONCO TR AC nn nn nn nn DLE ETX CHK
                 * 
                 * NUM-TRONCO = identificador do tronco
                 * 0x01 = tronco 1 / 0x02 = tronco 2
                 * 
                 * TR = Estado do tronco
                 * 0x00 = TRONCO NÃO CONECTADO  / 0x01 = TRONCO CONECTADO
                 * 
                 * AC = Chamadas a cobrar
                 * 0x00 = TRONCO NÃO ACEITA A COBRAR / 0x01 = ACEITA A COBRAR
                 * 
                 * NUMERO DO ATENDEDOR TRONCO 
                 * Ex. nn nn nn nn = FF FF 12 34
                 */
                data = "02" + "00";
                data += Convert.ToString(i+1).PadLeft(2, '0');
                data += ts[i].estado == true ? "01" : "00";
                data += ts[i].estadoChamadaCobrar == true ? "01" : "00";
                data += buscarPorChaveNumero(ts[i].atendedor).PadLeft(8, 'F');
                addComando(data);
            }

            // Prepara dos números liberados
            ArrayList nl = central.linhaExterna.listNumeroLiberado;
            for (int i = 0; i < nl.Count; i++)
            {
                /*
                 * Comando = STX Number 02 SBC XX xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx DLE ETX CHK
                 * 
                 * SBC = Opção de bloqueio/liberação
                 * 0x01 - Numero bloqueado
                 * 0x02 - Numero liberado
                 * 
                 * XX = Posição do numero bloqueado / liberado
                 * Ex. Posição 9 - XX = 09
                 * 
                 * xx xx ... xx = numero bloqueado / liberado
                 */
                data = "02" + "02";
                data += Convert.ToString(i+1).PadLeft(2, '0');
                data += nl[i].ToString().PadLeft(40, 'F');
                addComando(data);
            }

            // Prepara dos números bloqueados
            ArrayList nb = central.linhaExterna.listNumeroBloqueado;
            for (int i = 0; i < nb.Count; i++)
            {
                /*
                 * Comando = STX Number 02 SBC XX xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx DLE ETX CHK
                 * 
                 * SBC = Opção de bloqueio/liberação
                 * 0x01 - Numero bloqueado
                 * 0x02 - Numero liberado
                 * 
                 * XX = Posição do numero bloqueado / liberado
                 * Ex. Posição 9 - XX = 09
                 * 
                 * xx xx ... xx = numero bloqueado / liberado
                 */
                data = "02" + "01";
                data += Convert.ToString(i+1).PadLeft(2, '0');
                data += nb[i].ToString().PadLeft(40, 'F');
                addComando(data);
            }
        }

        // Prepara a programação da mesa operadora
        private void colherProgramacaoMesaOperadora()
        {
            /* FALTA IMPLEMENTAR */
            string data;
            List<MesaOperadora> m = central.listMesaOperadora;

            // ############### DEFINE OS NÚMEROS DAS MESAS
            for (int i = 0; i <= 7; i++)
            {
                /* 
                 * Comando = STX Number 03 00 NN nnnnnnnn DLE ETX CHK
                 * 
                 * NN = POSIÇÃO DA MESA 0 à 7
                 * Ex. Posição 1 = 01
                 * 
                 * nn nn nn nn = NUMERO PROGRAMADO DA MESA
                 * Ex. Número da mesa 1234 = FF FF 12 34
                 */
                data = "0300";
                data += Convert.ToString(i).PadLeft(2, '0');
                data += m[i].numero.PadLeft(8, 'F');
                addComando(data);
            }

            // ############### DEFINE A SITUAÇÃO DAS TECLAS
            for (int i = 0; i <= 7; i++)
            {
                /*
                 * Comando = STX Number 03 01 NN XX AA nn nn nn nn nn nn nn nn nn nn nn nn nn nn nn nn DLE ETX CHK
                 *           02  DC     03 01 00 02 00                                                 7F  03  DC
                 * 
                 * NN = POSIÇÃO DA MESA 0 à 7
                 * 
                 * XX = Identifica a tecla
                 * - 0x00 = Tecla FECH1
                 * - 0x01 = Tecla FECH2
                 * - 0x02 = Tecla Zelador
                 * - 0x03 = Tecla Sindico
                 * - 0x04 = Tecla A1
                 * - 0x05 = Tecla A2
                 * - 0x06 = Tecla A3
                 * - 0x07 = Tecla Porteiro
                 * - 0x08 = Tecla TEL
                 * 
                 * AA = Identifica a programação da tecla XX
                 * - 0x00 = SEM PROGRAMAÇÃO
                 * - 0x01 = FECHADURA 1
                 * - 0x02 = FECHADURA 2
                 * - 0x03 = ACESSO A RAMAL
                 * - 0x04 = NUMERO TELEFONICO
                 * 
                 * nn nn  ... nn = Ramal ou numero externo
                 * Exemplo: 41(11)32817287 = FF FF FF FF FF FF FF FF FF FF 41 11 32 81 72 87
                 */

                // Recupera a tecla da mesa corrente
                for (int x = 0; x < 9; x++)
                {
                    Tecla t = m[i].buscarTeclaPorId(x);

                    data = "0301";
                    data += Convert.ToString(i).PadLeft(2, '0');

                    // Define a tecla
                    if(t.nome == nome.ZELADOR) {
                        data += "02";
                    }
                    else if(t.nome == nome.SINDICO) {
                        data += "03";
                    }
                    else if(t.nome == nome.A1) {
                        data += "04";
                    }
                    else if(t.nome == nome.A2) {
                        data += "05";
                    }
                    else if(t.nome == nome.A3) {
                        data += "06";
                    }
                    else if(t.nome == nome.PORTEIRO) {
                        data += "07";
                    }
                    else if(t.nome == nome.TELEFONE) {
                        data += "08";
                    }
                    else if(t.nome == nome.FECH1) {
                        data += "00";
                    }
                    else if(t.nome == nome.FECH2) {
                        data += "01";
                    }

                    // Define a programação da tecla
                    if(t.estado == estado.DESATIVADA) {
                        data += "00";
                    }
                    else if(t.estado == estado.FECHADURA) {
                        data += "01";
                    }
                    else if(t.estado == estado.RAMAL) {
                        data += "03";
                    }
                    else if(t.estado == estado.TELEFONE) {
                        data += "04";
                    }

                    data += t.atendedor.PadLeft(32, 'F');
                    
                    addComando(data);
                }
            }
        }
        
        // Prepara a programação do alarme
        private void colherProgramacaoAlarme() 
        {
            string data;
            Alarme al = central.alarme;

            // Prepara oo alarme
            for (int i = 0; i < al.listAtendedores.Count; i++)
            {
                /*
                 * Comando = STX Number 07 nn nn nn nn POS-ATENDEDOR xx xx xx xx DLE ETX CHK
                 * 
                 * nn nn nn nn = Número do ramal programado como ALARME
                 * POS-ATENDEDOR = posição do ramal ATENDEDOR (Posição de 1 a 10)
                 * Número do ramal programado como ATENDEDOR
                 */
                data = "07";
                data += al.numero.PadLeft(8, 'F');
                data += Convert.ToString(i+1).PadLeft(2, '0');
                data += al.listAtendedores[i].ToString().PadLeft(8, 'F');
                data += al.tempo.PadLeft(2, 'F');
                addComando(data);
            }
        }

        // Prepara a programação dos Vídeos
        private void colherProgramacaoVideos()
        {
            string data;
            List<Video> vids = central.listVideo;

            // Prepara oo alarme
            for (int i = 0; i < vids.Count; i++)
            {
                /*
                 * Comando = STX Number 08 VV A/D nn nn nn nn DLE ETX CHK
                 * 
                 * VV = Número do vídeo (1-8)
                 * A/D = ATIVADO (1) / DESATIVADO (0)
                 * nn nn nn nn = Número do ramal ligado ao vídeo
                 */
                data = "08";
                data += Convert.ToString(i+1).PadLeft(2, '0');
                data += vids[i].estado == true ? "01" : "00";
                data += vids[i].numero.PadLeft(8, 'F');
                addComando(data);
            }
        }

        // Adiciona o comando na lista
        private void addComando(string cmd)
        {
            // Define o valor do NUMBER
            int _number = this.pegarProxNumber();
            int _chk = _number;
            cmd = cmd.Replace('*', 'B').Replace('#', 'C');

            // Define o valor do CHK
            for (int i = 0; i < cmd.Length; i = i + 2)
            {
                _chk = _chk ^ Convert.ToInt32(cmd.Substring(i, 2), 16);
            }

            // Converte o NUMBER e o CHK em HEXA
            string number = Convert.ToString(_number, 16).PadLeft(2, '0').ToUpper();
            string chk = Convert.ToString(_chk, 16).PadLeft(2, '0').ToUpper();

            //MessageBox.Show(STX + number + cmd + DLE + ETX + chk);

            // Adiciona o comando na lista
            programacao.Add(STX + number + cmd + DLE + ETX + chk);
        }

        // Define o CHK com base nos valores existentes na lista
        private string definirCHK(string data)
        {
            // Converte o primeiro bit em inteiro (esse bit é o Number)
            int result = Convert.ToInt32(data.Substring(0, 2));

            for (int i = 2; i < data.Length; i = i + 2)
            {
                // Extrai o bit eliminando o "F"
                string x = data.Substring(i, 2).Replace("F", "");

                // Verifica se sobrou algum valor para ser convertido em inteiro
                if (x.Length > 0)
                {
                    // Converte o primeiro bit em inteiro (esse bit é o Number)
                    int n = Convert.ToInt32(x);

                    // Efetua o cálculo XOR
                    result = result ^ n;
                }
            }
            return Convert.ToString(result, 16).PadLeft(2, '0').ToUpper();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Verifica se a chave está asociada a um ramal ou uma mesa. Depois */
        /*                  chama o método responsável em retornar o número (mesa ou ramal). */
        /* --------------------------------------------------------------------------------- */
        private string buscarPorChaveNumero(string chave)
        {
            if (chave != "")
                if (chave[0] == 'R')
                    return buscarPorIdNumeroRamal(chave);
                else if (chave[0] == 'M')
                    return buscarPorIdNumeroMesa(chave);
                else return "";
            else return "";
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o número do ramal (apto) do id passado por parâmetro.    */
        /*                  Retorna vazio se não encontrar o id.                             */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdNumeroRamal(string chave)
        {
            string result = "";
            foreach (Ramal r in this.central.listRamal)
            {
                if (r.id_ramal.Equals(chave))
                {
                    result = r.apartamento;
                    break;
                }
            }
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o número da mesa do id passado por parâmetro.            */
        /*                  Retorna vazio se não encontrar o id.                             */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdNumeroMesa(string chave)
        {
            string result = "";
            foreach (MesaOperadora m in this.central.listMesaOperadora)
            {
                if (m.id_mesa.Equals(chave))
                {
                    result = m.numero;
                    break;
                }
            }
            return result;
        }
    }
}