/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla os serviços dos objetos (interface entre a UI e
 *                          os objetos da aplicação).
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
using System.IO;
using System.Collections;
using CentraisCDX.Class.Util;
using CentraisCDX.Class.Model;
using CentraisCDX.Class.Util.Exceptions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using CentraisCDX.Class.Comunicacao;

namespace CentraisCDX.Class.Controller
{
    delegate void ProcessoConcluidoEventHandler(object source, EventArgs e);
    delegate void ProcessoAbortadoEventHandler(object source, EventArgs e);

    class CentralControl
    {
        public static event ProcessoConcluidoEventHandler ProcessoConcluido;
        public static event ProcessoAbortadoEventHandler ProcessoAbortado;

        // ESTADO DO OBJETO
        TransferDados transfer;
        private ArquivoXML axml = new ArquivoXML();
        private CreatePDF createPDF = new CreatePDF();
        private Central central = new Central();
        private string _arquivo_nome = "";
        private string _arquivo_caminho = "";

        #region OPERAÇÕES GERAIS UTILIZADAS PELA APLICAÇÃO
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Verifica se um número de telefone é válido.                      */
        /* --------------------------------------------------------------------------------- */
        private bool validarNumeroRamal(string value)
        {
            if (value == "" || Regex.IsMatch(value, @"^[0-9*][0-9*#]{0,7}$"))
            {
                return true;
            }
            else return false;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Verifica se um número de telefone é válido.                      */
        /* --------------------------------------------------------------------------------- */
        private bool validarNumeroTelefone(string value)
        {
            if (Regex.IsMatch(value, @"^[0-9]{0,20}$"))
            {
                return true;
            }
            else return false;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define a central com a programação de fábrica.                   */
        /* --------------------------------------------------------------------------------- */
        public bool carregarCentralDefault()
        {
            // Carrega a programação padrão da central (arquivo XML no resource da aplicação)
            // Mudou a opção "Build Action" do arquivo XML para: Embedded Resource
            Stream stream = this.GetType().Assembly.GetManifestResourceStream("CentraisCDX.Resources.programacaoInicial.xml");

            this.arquivo_caminho = "";
            this.central = null;
            this.central = axml.carregarDadosArquivoXML(stream);
            stream.Close();

            if (this.central != null)
                return true;
            else return false;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Abre uma programação (arquivo XML).                              */
        /* --------------------------------------------------------------------------------- */
        public bool abrirArquivoXML(string caminho)
        {
            this.arquivo_caminho = caminho;
            this.arquivo_nome = Path.GetFileName(caminho);
            this.central = null;
            this.central = axml.carregarDadosArquivoXML(caminho);

            if (this.central != null)
                return true;
            else return false;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Salva a programação do arquivo aberto.                           */
        /* --------------------------------------------------------------------------------- */
        public void salvarArquivoXML()
        {
            this.axml.salvarDadosArquivoXML(this.central, this._arquivo_caminho);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Salva a programação em um novo caminho.                          */
        /* --------------------------------------------------------------------------------- */
        public void salvarArquivoXMLComo(string caminho)
        {
            this._arquivo_caminho = caminho;
            this._arquivo_nome = Path.GetFileName(caminho);
            this.axml.salvarDadosArquivoXML(this.central, caminho);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Redefine os números dos ramais da lista.                         */
        /*                  Garante a sequencia: 1, 2, 3, 4 ... 3070, 3071 e 3072            */
        /* --------------------------------------------------------------------------------- */
        private void redefinirNumeroRamal()
        {
            for (int i = 0; i < this.central.listRamal.Count; i++)
                this.central.listRamal[i].ramal = (i + 1).ToString();
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

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o objeto ramal do apartamento passado por parâmetro.    */
        /*                  Retorna null se não encontrar o apartamento.                     */
        /* --------------------------------------------------------------------------------- */
        public Ramal buscarPorNumeroRamal(string apartamento)
        {
            Ramal result = null;
            foreach (Ramal r in this.central.listRamal)
            {
                if (r.apartamento == apartamento)
                {
                    result = r;
                    break;
                }
            }
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o objeto mesa do numero passado por parâmetro.          */
        /*                  Retorna null se não encontrar o numero.                          */
        /* --------------------------------------------------------------------------------- */
        public MesaOperadora buscarPorNumeroMesa(string numero)
        {
            MesaOperadora result = null;
            foreach (MesaOperadora m in this.central.listMesaOperadora)
            {
                if (m.numero == numero)
                {
                    result = m;
                    break;
                }
            }
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define uma nova chave para o ramal.                              */
        /*                  Retorna a próxima chave da lista (última + 1).                   */
        /* --------------------------------------------------------------------------------- */
        private string criarNovaChaveRamal()
        {
            int chave = 0;
            for (int i = 0; i < central.listRamal.Count; i++)
            {
                int n = Convert.ToInt32(central.listRamal[i].id_ramal.Replace("R", ""));
                if (n > chave) chave = n;
            }
            return "R" + (chave + 1).ToString();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Metodos getter e setter.                                         */
        /* --------------------------------------------------------------------------------- */
        public string arquivo_caminho
        {
            get { return _arquivo_caminho; }
            set { _arquivo_caminho = value; }
        }

        public string arquivo_nome
        {
            get { return _arquivo_nome; }
            set { _arquivo_nome = value; }
        }
        #endregion

        #region OPERAÇÕES DA TELA "PROGRAMAÇÃO INICIAL"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o status do vídeo de um determinado item da lista.      */
        /* --------------------------------------------------------------------------------- */
        public ProgramacaoInicial buscaProgramacaoInicial()
        {
            return central.programacaoInicial;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Cria um conjunto de numeração com base nos dados formecidos pelo */
        /*                  usuário (modo andar).                                            */
        /* --------------------------------------------------------------------------------- */
        public void calcularNumeracaoAptos(ProgramacaoInicial pi)
        {
            // Atualiza o objeto
            this.central.programacaoInicial = pi;

            // Define as variáveis utilizadas no método 
            int posicao = pi.posicao - 1;
            string nroDoBloco = "";
            int bloco_fim = 0;
            int nroPrimeiroBloco = pi.nroPrimeiroBloco;

            // Limpa os primeiros ramais (caso exista)
            for (int p = 0; p < posicao; p++)
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
                        central.listRamal[posicao].apartamento = nroDoBloco;
                        central.listRamal[posicao].ramalHOT = false;
                        central.listRamal[posicao].ramalBLOCO = pi.ramalBloco;
                        central.listRamal[posicao].ramalRESTRITO = false;
                        central.listRamal[posicao].acessoLinhaExterna = false;
                        central.listRamal[posicao].placaPortPhone = false;
                        central.listRamal[posicao].atendedor = "M1";
                        posicao++;
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
                        string apartamento = retornarNumeroRamal(pi.modo, apto, andar, pi.multiploPorAndar, pi.nroPrimeiroApto, nroDoBloco, pi.posicaoDoNumeroBloco);

                        // Define em qual central será colocado o número do apartamento
                        if (apartamento == pi.iniciarCentral2 && posicao <= 1024)
                        {
                            this.limparNumeroRamal(posicao, 1023);
                            posicao = 1024;
                        }
                        else if (apartamento == pi.iniciarCentral3 && posicao <= 2048)
                        {
                            this.limparNumeroRamal(posicao, 2047); ;
                            posicao = 2048;
                        }

                        // Define a programação do ramal referente a uma determinada posição
                        this.definirRamal(posicao, apartamento, pi.ramalHot, pi.ramalRestrito);

                        // Incrementa a posição do ramal
                        posicao++;
                    }
                }
            }
            this.limparNumeroRamal(posicao, 3071);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o número do ramal (apto) com base nos dados formecidos   */
        /*                  pelo usuário.                                                    */
        /* --------------------------------------------------------------------------------- */
        private string retornarNumeroRamal(
            modoNumeracao modo, int apto, int andar, int multAndar, string nro1oApto, string nroBloco, posicaoNroBloco posicao)
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
            if(modo == modoNumeracao.PRUMADA)
                apartamento = zeroEsquerda + ((nroAptoSemZeroEsquerda + ((multAndar * apto) - multAndar)) + (andar - 1));
            else apartamento = zeroEsquerda + ((multAndar * (andar - 1)) + (nroAptoSemZeroEsquerda + (apto - 1))).ToString();

            // Verifica se existe blocos
            if (nroBloco != "")
            {
                // Verifica a posição do número do bloco no ramal (E = ANTES / D = DEPOIS)
                if (posicao == posicaoNroBloco.ANTES)
                    result = string.Concat(nroBloco, apartamento);
                else if (posicao == posicaoNroBloco.DEPOIS) result = string.Concat(apartamento, nroBloco);
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
            if (posicao < 3072)
            {
                central.listRamal[posicao].apartamento = apartamento;
                central.listRamal[posicao].ramalHOT = ramalHot;
                central.listRamal[posicao].ramalBLOCO = false;
                central.listRamal[posicao].ramalRESTRITO = ramalRestrito;
                central.listRamal[posicao].acessoLinhaExterna = false;
                central.listRamal[posicao].placaPortPhone = false;
                central.listRamal[posicao].atendedor = "M1";
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define os números dos ramais (de um determinado range) como "".  */
        /*                  As demais programação ficam default.                             */
        /* --------------------------------------------------------------------------------- */
        private void limparNumeroRamal(int inicio, int fim)
        {
            for (int x = inicio; x <= fim; x++)
            {
                central.listRamal[x].apartamento = "";
                central.listRamal[x].ramalHOT = false;
                central.listRamal[x].ramalBLOCO = false;
                central.listRamal[x].ramalRESTRITO = false;
                central.listRamal[x].acessoLinhaExterna = false;
                central.listRamal[x].atendedor = "M1";
            }
        }
        #endregion

        #region OPERAÇÕES DA TELA "INCLUIR RAMAL"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Inclui um ramal em uma posição especifica da lista de ramais.    */
        /* --------------------------------------------------------------------------------- */
        public void incluirNovoRamal(Ramal ramal, int posicao)
        {
            // Verifica se o número do ramal é válido
            if (ramal.apartamento != "")
            {
                if (!this.validarNumeroRamal(ramal.apartamento))
                    throw new NumeroInvalidoException("O formato do número '" + ramal.apartamento + "' para o novo ramal está inválido.\n\nAtenção:\nO número só pode conter os caracteres * e # e números de 0 a 9.\n- Também não pode ser iniciado com # e nem ultrapassar 8 digitos.");
                else if (buscarPorNumeroRamal(ramal.apartamento) != null)
                    throw new NumeroDuplicadoException("Já existe um RAMAL programado com o número " + ramal.apartamento + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.");
                else if (buscarPorNumeroMesa(ramal.apartamento) != null)
                    throw new NumeroDuplicadoException("Já existe uma MESA programada com o número " + ramal.apartamento + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.");
            }
            else throw new NumeroRamalVazioException("O número do ramal não pode ficar vazio.");

            // Verifica se o atendedor é válido
            if (ramal.atendedor != "")
            {
                Ramal r = buscarPorNumeroRamal(ramal.atendedor);
                if (r != null && r.ramal != "3072")
                    ramal.atendedor = buscarPorNumeroRamal(ramal.atendedor).id_ramal;
                else if (buscarPorNumeroMesa(ramal.atendedor) != null)
                    ramal.atendedor = buscarPorNumeroMesa(ramal.atendedor).id_mesa;
                else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + ramal.atendedor + " como atendedor do ramal.\n\nAtenção:\n- Utilize um número de ramal ou mesa válido;\n- Não utilize o número do último ramal da lista de ramais.");
            }
            else throw new NumeroRamalVazioException("O atendedor do ramal não pode ficar vazio.");

            // Define uma chave para o ramal
            ramal.id_ramal = criarNovaChaveRamal();

            // Inclui o novo Ramal na posição especificada
            this.central.listRamal.Insert(posicao, ramal);

            // Remove o último Ramal da lista
            this.central.listRamal.RemoveAt(this.central.listRamal.Count - 1);

            // Redefine a numeração dos ramais
            this.redefinirNumeroRamal();
        }
        #endregion

        #region OPERAÇÕES DA TELA "LISTA RAMAL"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o objeto do ramal de uma determinada posição da lisra.    */
        /* --------------------------------------------------------------------------------- */
        public void modificarListRamal(Ramal ramal, int posicao)
        {
            // Verifica se o número do ramal é válido
            Ramal r = buscarPorNumeroRamal(ramal.apartamento);
            if (!this.validarNumeroRamal(ramal.apartamento))
                throw new NumeroInvalidoException("O formato do número '" + ramal.apartamento + "' para o novo ramal está inválido.\n\nAtenção:\n- O número só pode conter os caracteres * e # e números de 0 a 9.\n- Também não pode ser iniciado com # e nem ultrapassar 8 digitos.");
            else if (r != null && r.ramal != ramal.ramal && r.apartamento != "")
                throw new NumeroDuplicadoException("Já existe um RAMAL programado com o número " + ramal.apartamento + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.");
            else if (buscarPorNumeroMesa(ramal.apartamento) != null)
                throw new NumeroDuplicadoException("Já existe uma MESA programada com o número " + ramal.apartamento + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.");

            // Verifica se o atendedor é válido
            if (ramal.atendedor != "")
            {
                if (buscarPorNumeroRamal(ramal.atendedor) != null)
                    ramal.atendedor = buscarPorNumeroRamal(ramal.atendedor).id_ramal;
                else if (buscarPorNumeroMesa(ramal.atendedor) != null)
                    ramal.atendedor = buscarPorNumeroMesa(ramal.atendedor).id_mesa;
                else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + ramal.atendedor + " como atendedor do ramal.\n\nAtenção:\n- Utilize um número de ramal ou mesa válido;\n- Não utilize o número do último ramal da lista de ramais.");
            }
            else throw new NumeroRamalVazioException("O atendedor do ramal não pode ficar vazio.");

            // Atualiza o objeto se nenhuma exception foi lançada
            central.listRamal[posicao].apartamento = ramal.apartamento;
            central.listRamal[posicao].ramalHOT = ramal.ramalHOT;
            central.listRamal[posicao].ramalBLOCO = ramal.ramalBLOCO;
            central.listRamal[posicao].ramalRESTRITO = ramal.ramalRESTRITO;
            central.listRamal[posicao].placaPortPhone = ramal.placaPortPhone;
            central.listRamal[posicao].acessoLinhaExterna = ramal.acessoLinhaExterna;
            central.listRamal[posicao].atendedor = ramal.atendedor;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera um determidado ramal pelo posição da lista.             */
        /* --------------------------------------------------------------------------------- */
        public Ramal buscarPorIdRamal(int id)
        {
            Ramal ramal = new Ramal();

            ramal.ramal = central.listRamal[id - 1].ramal;
            ramal.apartamento = central.listRamal[id - 1].apartamento;
            ramal.ramalHOT = central.listRamal[id - 1].ramalHOT;
            ramal.ramalBLOCO = central.listRamal[id - 1].ramalBLOCO;
            ramal.ramalRESTRITO = central.listRamal[id - 1].ramalRESTRITO;
            ramal.placaPortPhone = central.listRamal[id - 1].placaPortPhone;
            ramal.acessoLinhaExterna = central.listRamal[id - 1].acessoLinhaExterna;
            ramal.atendedor = buscarPorChaveNumero(central.listRamal[id - 1].atendedor);

            return ramal;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Remove os ramais existentes do ArrayList.                        */
        /* --------------------------------------------------------------------------------- */
        public void removerRamais(ArrayList excluidos)
        {
            // Cria novos ramais no fim da lista (a mesma quantidade que será excluida)
            // Se excluir primeiro, o método criarNovaChaveRamal() pega as chaves dos ramais excluidos
            for (int i = 0; i < excluidos.Count; i++)
            {
                Ramal r = new Ramal();
                r.id_ramal = criarNovaChaveRamal();
                r.apartamento = "";
                r.atendedor = "M1";
                this.central.listRamal.Add(r);
            }

            // Percorre a lista excluindo os ramais
            int tamanho = this.central.listRamal.Count;
            for (int i = 0; i < tamanho; i++)

                // Verifica se o ramal corrente está na lista de excluidos
                if (excluidos.Contains(this.central.listRamal[i].ramal))
                {
                    this.central.listRamal.RemoveAt(i);
                    tamanho = this.central.listRamal.Count;
                    i--;
                }

            // Chama o método responsável em redefinir os números dos ramais
            redefinirNumeroRamal();

            // Voltar a programação default no números que fazem referências a um ramal excluído
            for (int i = 0; i < excluidos.Count; i++)
            {
                // Verifica os ramais
                for (int r = 0; r < central.listRamal.Count; r++)
                {
                    string chave = central.listRamal[r].atendedor;
                    string n = buscarPorChaveNumero(chave);
                    if (n == "")
                        central.listRamal[r].atendedor = "M1";
                }

                // Verifica os troncos
                for (int t = 0; t < central.linhaExterna.listTronco.Count; t++)
                {
                    string chave = central.linhaExterna.listTronco[t].atendedor;
                    string n = buscarPorChaveNumero(chave);
                    if (n == "")
                        central.linhaExterna.listTronco[t].atendedor = "M1";
                }

                // Verifica os vídeos
                for (int v = 0; v < central.listVideo.Count; v++)
                    if (central.listVideo[v].numero == (string)excluidos[i])
                        central.listVideo[v].numero = "";

                // Verifica as mesas
                List<nome> teclas = new List<nome>();
                teclas.Add(nome.ZELADOR);
                teclas.Add(nome.SINDICO);
                teclas.Add(nome.A1);
                teclas.Add(nome.A2);
                teclas.Add(nome.A3);
                teclas.Add(nome.PORTEIRO);
                teclas.Add(nome.TELEFONE);
                teclas.Add(nome.FECH1);
                teclas.Add(nome.FECH2);
                for (int m = 0; m < central.listMesaOperadora.Count; m++)
                {
                    MesaOperadora mo = central.listMesaOperadora[m];
                    foreach (nome tecla in teclas)
                    {
                        string ch = mo.buscarPorNomeAtendedorTecla(tecla);
                        string at = buscarPorChaveNumero(ch);
                        if (at == "" && mo.buscarPorNomeEstadoTecla(tecla) == estado.RAMAL)
                        {
                            central.listMesaOperadora[m].definirEstadoTecla(tecla, estado.DESATIVADA);
                            central.listMesaOperadora[m].definirAtendedorTecla(tecla, "");
                        }
                    }
                }

                // Verifica os alarmes
                if (central.alarme.numero == excluidos[i].ToString())
                    central.alarme.numero = "";
                for(int a = 0; a < central.alarme.listAtendedores.Count; a++)
                    if (central.alarme.listAtendedores[a] == excluidos[i])
                        central.alarme.listAtendedores[a] = "";
            }
        }
        #endregion

        #region OPERAÇÕES DA TELA "TRONCO"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o tronco. Pode lançar 3 exception.                        */
        /* --------------------------------------------------------------------------------- */
        public void definirLinhaExterna(List<Tronco> troncos, ArrayList liberados, ArrayList bloqueados)
        {
            // Verifica se o atendedor do tronco 1 existe na lista de RAMAIS ou MESA
            for (int i = 1; i <= 2; i++)
            {
                string numero = troncos[i - 1].atendedor;
                if (numero != "")
                {
                    if (buscarPorNumeroRamal(numero) != null)
                        troncos[i - 1].atendedor = buscarPorNumeroRamal(numero).id_ramal;
                    else if (buscarPorNumeroMesa(numero) != null)
                        troncos[i - 1].atendedor = buscarPorNumeroMesa(numero).id_mesa;
                    else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + numero + " para o atendedor do tronco " + i + ".\n\nAtenção:\n- Utilize um número de ramal ou mesa válido.");
                }
                else throw new NumeroRamalVazioException("O atendedor do tronco " + i + " não pode ficar vazio.");
            }

            // Verifica se o número liberado é valido
            for (int i = 1; i <= 20; i++)
            {
                string telefone = (string)liberados[i-1];
                if(!validarNumeroTelefone(telefone))
                    throw new NumeroTelefoneInvalidoException("O número de telefone liberado '" + telefone + "' da posição " + i + " está inválido.\n\nAtenção:\n- Utilize números de 0 a 9.\n- O número não pode passar de 20 digitos.");
            }

            // Verifica se o número bloqueados é valido
            for (int i = 1; i <= 20; i++)
            {
                string telefone = (string)bloqueados[i - 1];
                if (!validarNumeroTelefone(telefone))
                    throw new NumeroTelefoneInvalidoException("O número de telefone bloqueado '" + telefone + "' da posição " + i + " está inválido.\n\nAtenção:\n- Utilize números de 0 a 9.\n- O número não pode passar de 20 digitos.");
            }

            // Atualiza o objeto se nenhuma exception foi lançada
            central.linhaExterna.listTronco[0] = troncos[0];
            central.linhaExterna.listTronco[1] = troncos[1];
            central.linhaExterna.listNumeroLiberado = liberados;
            central.linhaExterna.listNumeroBloqueado = bloqueados;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o estado do tronco de um determinado item da lista.     */
        /* --------------------------------------------------------------------------------- */
        public bool buscaPorIdEstadoTronco(int posicao)
        {
            return central.linhaExterna.listTronco[posicao - 1].estado;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o estado da chamada a cobrar de um determinado item da  */
        /*                  lista.                                                           */
        /* --------------------------------------------------------------------------------- */
        public bool buscaPorIdEstadoChCobrar(int posicao)
        {
            return central.linhaExterna.listTronco[posicao - 1].estadoChamadaCobrar;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número do atendedor de um determinado item da lista.  */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdAtendedorTronco(int posicao)
        {
            return buscarPorChaveNumero((string)central.linhaExterna.listTronco[posicao - 1].atendedor);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número do telefone liberado de um determinado item da */
        /*                  lista.                                                           */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdTelLiberadoTonco(int posicao)
        {
            return (string)central.linhaExterna.listNumeroLiberado[posicao - 1];
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número do telefone liberado de um determinado item da */
        /*                  lista.                                                           */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdTelBloqueadoTonco(int posicao)
        {
            return (string)central.linhaExterna.listNumeroBloqueado[posicao - 1];
        }
        #endregion

        #region OPERAÇÕES DA TELA "VÍDEO"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o objeto vídeo. Pode lançar 1 exception.                  */
        /* --------------------------------------------------------------------------------- */
        public void definirVideo(List<Video> videos)
        {
            // Cria o objeto que irá recebe as novas definições
            List<Video> listVideo = new List<Video>();

            // Verifica se os números dos vídeos existem na lista de RAMAIS ou MESA
            for (int i = 1; i <= 8; i++)
            {
                Video v = new Video();
                v.estado = videos[i - 1].estado;
                string numero = videos[i - 1].numero;
                if (numero != "")
                {
                    if (buscarPorNumeroRamal(numero) != null)
                        v.numero = buscarPorNumeroRamal(numero).id_ramal;
                    else if (buscarPorNumeroMesa(numero) != null)
                        v.numero = buscarPorNumeroMesa(numero).id_mesa;
                    else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + numero + " para o vídeo " + i + ".\n\nAtenção:\n- Utilize um número de ramal ou mesa válido.");
                }
                else v.numero = "";
                listVideo.Add(v);
            }

            // Atualiza o objeto se nenhuma exception foi lançada
            central.listVideo = listVideo;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o status do vídeo de um determinado item da lista.      */
        /* --------------------------------------------------------------------------------- */
        public bool buscaPorIdStatusVideo(int posicao)
        {
            return central.listVideo[posicao - 1].estado;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número do vídeo de um determinado item da lista.      */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdNumeroVideo(int posicao)
        {
            return buscarPorChaveNumero(central.listVideo[posicao - 1].numero);
        }
        #endregion

        #region OPERAÇÕES DA TELA "MESA OPERADORA"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define a mesa operadora.                                         */
        /* --------------------------------------------------------------------------------- */
        public void definirMesa(MesaOperadora mesa, int id)
        {
            // Define a chave da mesa
            mesa.id_mesa = "M" + id.ToString();

            // Verifica se o número do alarme está na lista de RAMAIS ou MESA
            if (mesa.numero != "")
            {
                MesaOperadora m = buscarPorNumeroMesa(mesa.numero);
                if (!this.validarNumeroRamal(mesa.numero))
                    throw new NumeroInvalidoException("O formato do número '" + mesa.numero + "' para a mesa está inválido.\n\nAtenção:\n- O número só pode conter os caracteres * e # e números de 0 a 9.\n- Também não pode ser iniciado com # e nem ultrapassar 8 digitos.");
                else if (buscarPorNumeroRamal(mesa.numero) != null)
                    throw new NumeroDuplicadoException("Já existe um RAMAL programado com o número " + mesa.numero + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.");
                else if (m != null && m.id_mesa != mesa.id_mesa)
                    throw new NumeroDuplicadoException("Já existe uma MESA programada com o número " + mesa.numero + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.");
            }
            else throw new NumeroRamalVazioException("O número da mesa não pode ficar vazio.");

            // Verifica se o usuário digitou um atendedor válido para as teclas
            // Percorre todas as teclas
            for (int i = 0; i < 9; i++)
            {
                // Retorna uma string com nome da tecla com base no indice do enum (variável i do laço for) 
                string nomeTecla = Enum.GetName(typeof(nome), i);

                // Retorna o enum referente a string
                nome tecla = (nome)Enum.Parse(typeof(nome), nomeTecla);

                if (mesa.buscarPorNomeEstadoTecla(tecla) != estado.DESATIVADA)
                {
                    string nro_atendedor = mesa.buscarPorNomeAtendedorTecla(tecla);

                    if (nro_atendedor == "")
                        throw new NumeroRamalVazioException("O atendedor da tecla " + tecla.ToString() + " não pode ficar vazio.");
                    else if (mesa.buscarPorNomeEstadoTecla(tecla) == estado.TELEFONE && !validarNumeroTelefone(nro_atendedor))
                        throw new NumeroTelefoneInvalidoException("Não foi possível definir o número " + nro_atendedor + " como atendedor para a tecla " + tecla.ToString() + ".\n\nAtenção:\n- Utilize um número de telefone válido.");
                    /*
                    if (nro_atendedor == "")
                        throw new NumeroRamalVazioException("O atendedor da tecla " + tecla.ToString() + " não pode ficar vazio.");
                    else if (mesa.buscarPorNomeEstadoTecla(tecla) == estado.RAMAL)
                    {
                        if (buscarPorNumeroRamal(nro_atendedor) != null)
                            mesa.definirAtendedorTecla(tecla, buscarPorNumeroRamal(nro_atendedor).id_ramal);
                        else if (buscarPorNumeroMesa(nro_atendedor) != null)
                            mesa.definirAtendedorTecla(tecla, buscarPorNumeroMesa(nro_atendedor).id_mesa);
                        else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + nro_atendedor + " como atendedor para a tecla " + tecla.ToString() + ".\n\nAtenção:\n- Utilize um número de ramal ou mesa válido.");
                    }
                    else if (!validarNumeroTelefone(nro_atendedor))
                        throw new NumeroTelefoneInvalidoException("Não foi possível definir o número " + nro_atendedor + " como atendedor para a tecla " + tecla.ToString() + ".\n\nAtenção:\n- Utilize um número de telefone válido.");
                    */
                }
            }

            // Atualiza o objeto na lista
            central.listMesaOperadora[id - 1] = mesa;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o estado de uma determidada tecla/mesa.                 */
        /* --------------------------------------------------------------------------------- */
        public estado buscarPorNomeEstadoTecla(int id, nome n)
        {
            return central.listMesaOperadora[id-1].buscarPorNomeEstadoTecla(n);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número de uma determinada mesa.                       */
        /* --------------------------------------------------------------------------------- */
        public string buscarNumeroMesa(int id)
        {
            return central.listMesaOperadora[id-1].numero;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número de uma determinada tecla/mesa.                 */
        /* --------------------------------------------------------------------------------- */
        public string buscarNumeroAtendedorTecla(int id, nome n)
        {
            // Recupera a chave do número (M=Mesa / R=Ramal)
            string result = central.listMesaOperadora[id-1].buscarPorNomeAtendedorTecla(n);

            if (result != "" && (result[0] == 'R' || result[0] == 'M'))
                return buscarPorChaveNumero(result);
            else return result;
        }
        #endregion

        #region OPERAÇÕES DA TELA "ALARME"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o objeto alarme. Pode lançar 1 exception.                 */
        /* --------------------------------------------------------------------------------- */
        public void definirAlarme(string nro_alarme, string tempo, ArrayList atendedores)
        {
            // Cria o objeto que irá recebe as novas definições
            Alarme alarme = new Alarme();

            // Verifica se o número do alarme está na lista de RAMAIS ou MESA
            if (nro_alarme != "")
            {
                if (buscarPorNumeroRamal(nro_alarme) != null)
                    alarme.numero = buscarPorNumeroRamal(nro_alarme).id_ramal;
                else if (buscarPorNumeroMesa(nro_alarme) != null)
                    alarme.numero = buscarPorNumeroMesa(nro_alarme).id_mesa;
                else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + nro_alarme + " como Alarme.\n\nAtenção:\n- Utilize um número de ramal ou mesa válido.");
            }
            else alarme.numero = "";

            // Atualiza o tempo do alarme
            alarme.tempo = tempo;

            // Verifica se os atendedores do alarme existem na lista de RAMAIS ou MESA
            for (int i = 1; i <= 10; i++)
            {
                string atendedor = atendedores[i-1].ToString();
                if (atendedor != "")
                {
                    if (buscarPorNumeroRamal(atendedor) != null)
                        alarme.listAtendedores.Add(buscarPorNumeroRamal(atendedor).id_ramal);
                    else if (buscarPorNumeroMesa(atendedor) != null)
                        alarme.listAtendedores.Add(buscarPorNumeroMesa(atendedor).id_mesa);
                    else throw new NumeroNaoEncontradoException("Não foi possível definir o número " + atendedor + " como atendedor " + i + " do Alarme.\n\nAtenção:\n- Utilize um número de ramal ou mesa válido.");
                }
                else alarme.listAtendedores.Add("");
            }

            // Atualiza o objeto se nenhuma exception foi lançada
            central.alarme = alarme;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número do alarme.                                     */
        /* --------------------------------------------------------------------------------- */
        public string buscarNumeroAlarme()
        {
            return buscarPorChaveNumero(central.alarme.numero);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o tempo do alarme.                                      */
        /* --------------------------------------------------------------------------------- */
        public string buscarTempoAlarme()
        {
            return central.alarme.tempo;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Recupera o número do atendedor de um determinado item da lista.  */
        /* --------------------------------------------------------------------------------- */
        public string buscarPorIdAtendedorAlarme(int posicao)
        {
            return buscarPorChaveNumero((string)central.alarme.listAtendedores[posicao - 1]);
        }
        #endregion

        #region OPERAÇÕES DA CRIAÇÃO DO ARQUIVO PDF
        public void criarRelatorioPDF()
        {
            string pasta = Application.StartupPath + "\\Relatorio\\";
            string caminho = "";

            // Define o nome do arquivo
            if (this.arquivo_nome == "")
                caminho = pasta + "relatorio_programacao" + ".pdf";
            else
                caminho = pasta + this.arquivo_nome + ".pdf";

            // Cria o arquivo PDF
            this.createPDF.criarPDFProgramacao(central, caminho);

            // Abre o arquivo PDF
            Process.Start(caminho);
        }
        #endregion

        #region OPERAÇÕES DA PORTA COM

        enum Resposta { ACK, NACK, TIMEOUT, INVALIDO }

        TransferDados t = new TransferDados("COM1");

        private string portaCOM = "";

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o nome da porta COM. Pode lançar 1 exception.             */
        /* --------------------------------------------------------------------------------- */
        public void definirPortaCOM(string porta)
        {
            if (porta != null && porta.Contains("COM"))
                this.portaCOM = porta;
            else throw new PortaCOMInvalidaException("A porta COM especificada não é uma porta válida.");
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o nome da porta COM configurada.                         */
        /* --------------------------------------------------------------------------------- */
        public string buscarPortaCOM()
        {
            return this.portaCOM;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Verifica se a central está respondendo na porta COM configurada. */
        /* --------------------------------------------------------------------------------- */
        public bool verificarConexao()
        {
            bool result = false;

            // Inicializa o log
            log = "VERIFICANDO CONEXÃO:\r\n";

            // Define o valor do "number"
            string number = this.pegarHexProxNumber();

            // Define os valores que serão usados para criar o CHK
            int _number = this.converterHexToBinary(number);
            int _cmd_start = this.converterHexToBinary(CMD_START);
            int _cmd_check = this.converterHexToBinary(CMD_SUB_CHECK);

            // Define o CHK do comando
            string chk = (_number ^ (_cmd_start ^ _cmd_check)).ToString();

            // Completa o valor do CHK (caso necessário)
            if ((chk.Length % 2) != 0) chk = "0" + chk;

            // Define o comando
            string comando = STX + number + CMD_START + CMD_SUB_CHECK + DLE + ETX + chk;

            // Envia o comando
            result = this.enviarComando(comando, porta);

            // Cria o arquivo de log
            this.gravarLog(log);

            return result;
        }




        /*
         // Lança a exception se a porta COM for inválida
            if (porta != null || porta.Contains("COM"))
            {
                
            }
            else throw new PortaCOMInvalidaException("A porta COM especificada não é uma porta válida.");
         */
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia um comando pela porta COM configurada.                     */
        /*                  Resposta: 0 = ACK | 1 = NACK | 2 = TIMEOUT | 3 = INVALIDO        */
        /* --------------------------------------------------------------------------------- */
        private Resposta enviarComando(string cmd)
        {
            Resposta result = Resposta.NACK;
            string STX = "02";
            string DLE = "7F";
            string ACK = "06";
            string NACK = "15";

            string number = cmd.Substring(2, 2);

            // Envia o comando pela porta COM e aguarda o retorno dos dados
            t.abrirPorta();
            t.sendComando(cmd);
            System.Threading.Thread.Sleep(200);
            string dadosRecebidos = t.dadosRecebidos;
            t.fecharPorta();

            // Verifica os dados recebidos e atualiza o log
            Log.log += cmd + " -->";
            if (dadosRecebidos == (STX + number + DLE + ACK))
            {
                Log.log += "<-- ACK\r\n";
                result = Resposta.ACK;
            }
            else
            {
                if (dadosRecebidos == (STX + number + DLE + NACK))
                {
                    Log.log += "<-- NACK\r\n";
                    result = Resposta.NACK;
                }
                else if (dadosRecebidos == "")
                {
                    Log.log += "<-- TIME OUT\r\n";
                    result = Resposta.TIMEOUT;
                }
                else
                {
                    Log.log += "<-- RETORNO INVÁLIDO [" + dadosRecebidos + "]\r\n";
                    result = Resposta.INVALIDO;
                }
            }
            return result;
        }






        

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia a programação para a central pela COM configurada.         */
        /* --------------------------------------------------------------------------------- */
        public void enviarProgramacaoCentral()
        {
            // Inicializa o log
            log = "ENVIANDO PROGRAMAÇÃO PARA A CENTRAL:\r\n";

            // Define o valor do "number"
            string number = this.pegarHexProxNumber();

            // Define os valores que serão usados para criar o CHK
            int _number = this.converterHexToBinary(number);
            int _cmd_start = this.converterHexToBinary(CMD_START);
            int _cmd_out = this.converterHexToBinary(CMD_SUB_OUT);

            // Define o CHK do comando
            string chk = (_number ^ (_cmd_start ^ _cmd_out)).ToString();

            // Completa o valor do CHK (caso necessário)
            if ((chk.Length % 2) != 0) chk = "0" + chk;

            // Define o comando
            string comando = STX + number + CMD_START + CMD_SUB_CHECK + DLE + ETX + chk;

            // Envia o comando
            //result = this.enviarComando(comando, porta);

            // Cria o arquivo de log
            this.gravarLog(log);





            if (this.portaCOM.Contains("COM"))
            {
                try
                {
                    transfer = new TransferDados("COM1");
                    this.transfer.abrirPorta();

                    /*
                    // Inicia o envio
                    foreach (string cmd in listComandos)
                    {
                        string comando = cmd;
                        string number = comando.Substring(2, 2);
                        transfer.sendComando(comando);
                        System.Threading.Thread.Sleep(200);
                        string dadosRecebidos = transfer.dadosRecebidos;

                        // Verifica os dados recebidos e atualiza o log
                        log += comando + " -->";
                        if (dadosRecebidos == (STX + number + DLE + ACK))
                            log += "<-- ACK\r\n";
                        else
                            if (dadosRecebidos == (STX + number + DLE + NACK))
                            {
                                log += "<-- NACK\r\n";
                                this.gravarLog(log);
                                this.transfer.fecharPorta();
                                throw new ComandoNaoReconhecidoException();
                            }
                            else if (dadosRecebidos == "")
                            {
                                log += "<-- TIME OUT\r\n";
                                this.gravarLog(log);
                                this.transfer.fecharPorta();
                                throw new ComandoTimeOutException();
                            }
                            else
                            {
                                log += "<-- RETORNO INVÁLIDO [" + dadosRecebidos + "]\r\n";
                                this.gravarLog(log);
                                this.transfer.fecharPorta();
                                throw new ComandoRespostaInvalidaException();
                            }
                    }
                     */
                    this.transfer.fecharPorta();
                    this.gravarLog(log);
                    this.onProcessoConcluido();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
            else throw new PortaCOMInvalidaException("A porta COM especificada não é uma porta válida.\n\nAtenção:\n- Entre na tela Configuração e escolha uma porta COM válida.");
        }

        

        

        














        



        

        

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Dispara o evento "ProcessoConcluidoEventHandler".                */
        /* --------------------------------------------------------------------------------- */
        private void onProcessoConcluido()
        {
            if (ProcessoConcluido != null)
                ProcessoConcluido(this, new EventArgs());
        }
        #endregion






        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o comando que envia a programação para a central.        */
        /* --------------------------------------------------------------------------------- */
        private string getComandoEnviar()
        {
            

            // Define o número da sequencia
            string number = this.pegarHexProxNumber();

            // Define o CHK
            int _number = this.converterHexToBinary(number);
            int _start = this.converterHexToBinary(CMD_START);
            int _out = this.converterHexToBinary(CMD_SUB_OUT);
            int _chk = _number ^ (_start ^ _out);
            string chk = _chk.ToString();

            if ((chk.Length % 2) != 0)
                chk = "0" + chk;

            // Retorna o comando
            return STX + number + CMD_START + CMD_SUB_OUT + DLE + ETX + chk;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o comando que coleta a programação da central.           */
        /* --------------------------------------------------------------------------------- */
        private string getComandoColetar()
        {
            string CMD_START = "00";
            string CMD_SUB_IN = "02";

            // Define o número da sequencia
            string number = this.pegarHexProxNumber();

            // Define o CHK
            int _number = this.converterHexToBinary(number);
            int _start = this.converterHexToBinary(CMD_START);
            int _in = this.converterHexToBinary(CMD_SUB_IN);
            int _chk = _number ^ (_start ^ _in);
            string chk = _chk.ToString();

            if ((chk.Length % 2) != 0)
                chk = "0" + chk;

            // Retorna o comando
            return STX + number + CMD_START + CMD_SUB_IN + DLE + ETX + chk;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o comando que finaliza o envio da programação.           */
        /* --------------------------------------------------------------------------------- */
        private string getComandoFinalizar()
        {
            string CMD_FIM = "05";

            // Define o número da sequencia
            string number = this.pegarHexProxNumber();

            // Define o CHK
            int _number = this.converterHexToBinary(number);
            int _fim = this.converterHexToBinary(CMD_FIM);
            int _chk = _number ^ _fim;
            string chk = _chk.ToString();

            if ((chk.Length % 2) != 0)
                chk = "0" + chk;

            // Retorna o comando
            return STX + number + CMD_FIM + DLE + ETX + chk;
        }

        

        

        

        

        

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Dispara o evento "ProcessoAbortadoEventHandler".                 */
        /* --------------------------------------------------------------------------------- */
        private void onProcessoAbortado()
        {
            if (ProcessoAbortado != null)
                ProcessoAbortado(this, new EventArgs());
        }
    }
}