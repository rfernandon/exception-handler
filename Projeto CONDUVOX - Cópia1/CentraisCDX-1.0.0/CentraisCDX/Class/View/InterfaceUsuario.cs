/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Prover as funcionalidades da aplicação para os usuários.
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
using System.Windows.Forms;
using CentraisCDX.Class.Controller;
using System.Collections.Generic;
using CentraisCDX.Class.Model;
using System.Drawing;
using System.Text.RegularExpressions;
using iTextSharp.text;
using CentraisCDX.Class.Comunicacao;
using System.Threading;
using CentraisCDX.Class.Util.Exceptions;
using System.Runtime.InteropServices;

namespace CentraisCDX.Class.View
{
    public partial class InterfaceUsuario : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private const String VERSION = "1.0.0Beta04-2016";

        private enum tela { APRESENTACAO, PROG_INICIAL, LISTA_RAMAL, TRONCO, MESA, ALARME, VIDEO, CONFIGURACAO, NOVO_RAMAL };

        // ESTADO DO OBJETO
        private tela telaAtiva = tela.APRESENTACAO;

        private String titulo_aplicacao = "Centrais telefônicas Conduvox - Versão " + InterfaceUsuario.VERSION;
        private String fileDialog_Filter = "Centrais conduvox CDX (*.cdx)|*cdx";
        private String fileDialog_DefaultExt = "cdx";
        private String fileDialog_FileName = "nova_programacao";
        private String fileDialog_InitialDirectory = Application.StartupPath + "\\Arquivos";

        private string[] listSequenciaRamal = new string[2] { "Normal", "Prumada" };
        private string[] listPosicaoNroBloco = new string[2] { "Antes do apto", "Depois do apto" };
        private string[] listMultiploAndar = new string[4] { "Unidade", "Dezena", "Centena", "Milhar" };
        private string[] listNaoSim = new string[2] { "Não", "Sim" };
        private string[] listCentrais = new string[3] { "Central 1", "Central 2", "Central 3" };
        private string[] listMesaOperadora = new string[8] { "Mesa *", "Mesa *1", "Mesa *2", "Mesa *3", "Mesa *4", "Mesa *5", "Mesa *6", "Mesa *7" };

        private CentralControl cc = new CentralControl();
        private Processo processo = new Processo();

        private List<RadioButton> listTeclaZelador = new List<RadioButton>();
        private List<RadioButton> listTeclaSindico = new List<RadioButton>();
        private List<RadioButton> listTeclaA1 = new List<RadioButton>();
        private List<RadioButton> listTeclaA2 = new List<RadioButton>();
        private List<RadioButton> listTeclaA3 = new List<RadioButton>();
        private List<RadioButton> listTeclaPorteiro = new List<RadioButton>();
        private List<RadioButton> listTeclaTelefone = new List<RadioButton>();
        private List<RadioButton> listTeclaFech1 = new List<RadioButton>();
        private List<RadioButton> listTeclaFech2 = new List<RadioButton>();

        #region CONSTRUTOR DA CLASSE
        public InterfaceUsuario()
        {
            // Inicializa os componentes
            InitializeComponent();

            // Inicializa a tela: pnlTelaConfiguracao
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
                this.cboPortaCOM.Items.Add(s);

            // Inicializa a tela: pnlTelaListaRamais
            this.definirComboBox(this.cboListaCentrais, this.listCentrais);
            this.grdListaRamais.Rows.Add(1024);
            this.grdListaRamais.Columns["Column2"].DefaultCellStyle.BackColor = Color.GhostWhite;
            this.txtPesquisarRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress); // Define o metodo para o evento "KeyPress" dos "TextBox"

            // Inicializa a tela: pnlTelaVideo
            this.txtVideo1Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo2Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo3Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo4Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo5Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo6Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo7Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtVideo8Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);

            // Inicializa a tela: pnlTelaTronco
            this.txtAtendedorTronco1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAtendedorTronco2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            for (int i = 1; i <= 20; i++)
            {
                TextBox t = (TextBox)this.Controls.Find("txtTelefoneLiberado" + i, true)[0];
                t.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            }
            for (int i = 1; i <= 20; i++)
            {
                TextBox t = (TextBox)this.Controls.Find("txtTelefoneBloqueado" + i, true)[0];
                t.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            }

            // Inicializa a tela: pnlTelaMesaOperadora
            this.definirComboBox(this.cboListMesaOperadora, this.listMesaOperadora);
            this.txtMesaNumero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            
            this.txtMesaSindico_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaA1_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaA2_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaA2_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaPorteiro_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaTelefone_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaFech1_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtMesaFech2_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            // ZELADOR
            this.listTeclaZelador.Add(rdbZelador_Desativada);
            this.listTeclaZelador.Add(rdbZelador_Ramal);
            this.listTeclaZelador.Add(rdbZelador_Telefone);
            // SINDICO
            this.listTeclaSindico.Add(rdbSindico_Desativada);
            this.listTeclaSindico.Add(rdbSindico_Ramal);
            this.listTeclaSindico.Add(rdbSindico_Telefone);
            // A1
            this.listTeclaA1.Add(rdbA1_Desativada);
            this.listTeclaA1.Add(rdbA1_Ramal);
            this.listTeclaA1.Add(rdbA1_Telefone);
            // A2
            this.listTeclaA2.Add(rdbA2_Desativada);
            this.listTeclaA2.Add(rdbA2_Ramal);
            this.listTeclaA2.Add(rdbA2_Telefone);
            // A3
            this.listTeclaA3.Add(rdbA3_Desativada);
            this.listTeclaA3.Add(rdbA3_Ramal);
            this.listTeclaA3.Add(rdbA3_Telefone);
            // PORTEIRO
            this.listTeclaPorteiro.Add(rdbPorteiro_Desativada);
            this.listTeclaPorteiro.Add(rdbPorteiro_Ramal);
            this.listTeclaPorteiro.Add(rdbPorteiro_Telefone);
            // TELEFONE
            this.listTeclaTelefone.Add(rdbTelefone_Desativada);
            this.listTeclaTelefone.Add(rdbTelefone_Ramal);
            this.listTeclaTelefone.Add(rdbTelefone_Telefone);
            // FECH1
            this.listTeclaFech1.Add(rdbFech1_Desativada);
            this.listTeclaFech1.Add(rdbFech1_Ramal);
            this.listTeclaFech1.Add(rdbFech1_Telefone);
            this.listTeclaFech1.Add(rdbFech1_Fechadura);
            // FECH2
            this.listTeclaFech2.Add(rdbFech2_Desativada);
            this.listTeclaFech2.Add(rdbFech2_Ramal);
            this.listTeclaFech2.Add(rdbFech2_Telefone);
            this.listTeclaFech2.Add(rdbFech2_Fechadura);

            // Inicializa a tela: pnlTelaAlarme
            this.txtAlarmeNumero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.nroAlarmeTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor7.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor8.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor9.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtAlarmeAtendedor10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);

            // Inicializa a tela: pnlTelaIncluirRamal
            this.definirComboBox(this.cboRamalHot, listNaoSim);
            this.definirComboBox(this.cboRamalBloco, listNaoSim);
            this.definirComboBox(this.cboRamalRestrito, listNaoSim);
            this.definirComboBox(this.cboPortPhone, listNaoSim);
            this.definirComboBox(this.cboAcessoLinhaExterna, listNaoSim);
            this.txtPosicaoNovoRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtNumeroRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);
            this.txtNumeroAtendedor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlApartamento_KeyPress);

            // Inicializa a tela: pnlTelaProgramacaoInicial
            this.definirComboBox(this.cboSequenciaRamal, listSequenciaRamal);
            this.definirComboBox(this.cboPosicaoNroBloco, listPosicaoNroBloco);
            this.definirComboBox(this.cboDefinirRamalBloco, listNaoSim);
            this.definirComboBox(this.cboDefinirRamalRestrito, listNaoSim);
            this.definirComboBox(this.cboDefinirRamalHot, listNaoSim);
            this.definirComboBox(this.cboMultiploAndar, listMultiploAndar);
            this.txtIniciarNoRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtNroInicialCentral2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtNroInicialCentral3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtQtdeBlocos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtNro1oBloco.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtQtdeAptoAndar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtQtdeAndares.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
            this.txtNro1oApto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlRamalAndTelefone_KeyPress);
        }
        #endregion

        #region EVENTO LOAD DO FORMULÁRIO
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Evento que ocorre antes que um formulário seja exibido pela      */
        /*                  primeira vez.                                                    */
        /* --------------------------------------------------------------------------------- */
        private void InterfaceUsuario_Load(object sender, EventArgs e)
        {
            /*if(Program.debug)*/ AllocConsole();

            // Define o estado do processo
            processo.estado = EnumEstado.PARADO;

            // Define o titulo do form com o nome da aplicação
            this.Text = titulo_aplicacao;

            // Carrega os objetos com a programação padrão de fábrica
            this.cc.carregarCentralDefault();

            // Exibe a tela referente a APRESENTAÇÃO
            this.exibirTela(tela.APRESENTACAO);
        }
        
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Evento que ocorre quando o formulário está sendo fechado.        */
        /* --------------------------------------------------------------------------------- */
        private void InterfaceUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processo.estado != EnumEstado.PARADO)
            {
                MessageBox.Show("Existe um processo em andamento. Aguarde a sua finalização ou clique no botão cancelar!", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }
        #endregion

        #region EVENTO CLICK DOs COMPONENTES "TOOLSTRIPBUTTON" (TOOLBAR)
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Prepara uma nova programação (default).                          */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonNovo_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se realmente o usuário deseja iniciar uma nova programação
                if (DialogResult.Yes == MessageBox.Show("Deseja iniciar uma nova programação?\n\nATENÇÃO:\nAs modificações anteriores serão descartadas.", "CentralCDX", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Define o titulo do form com o nome da aplicação
                    this.Text = titulo_aplicacao;

                    // Carrega os objetos com a programação padrão de fábrica
                    cc.carregarCentralDefault();

                    // Exibe a tela referente a APRESENTAÇÃO
                    this.exibirTela(tela.APRESENTACAO);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Carrega uma programação gravada em disco.                        */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                // Cria o objeto para definir o nome e o caminho do arquivo do arquivo XML
                OpenFileDialog openFileDialog = new OpenFileDialog();

                // Define as propriedades do OpenFileDialog
                openFileDialog.Filter = this.fileDialog_Filter;
                openFileDialog.DefaultExt = this.fileDialog_DefaultExt;
                openFileDialog.InitialDirectory = this.fileDialog_InitialDirectory;

                // Verifica se o usuário cancelou a janela de dialogo
                if (DialogResult.OK == openFileDialog.ShowDialog())
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Abre o arquivo XML se nenhuma exception for capturada
                    cc.abrirArquivoXML(openFileDialog.FileName);

                    // Exibe a tela referente a APRESENTAÇÃO
                    this.exibirTela(tela.APRESENTACAO);

                    // Define o titulo do form com o nome da aplicação + nome do arquivo XML
                    this.Text = titulo_aplicacao + " - " + Path.GetFileName(openFileDialog.FileName);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Grava a programação atual no disco.                              */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se existe arquivo XML aberto
                if (cc.arquivo_caminho != "")
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Salva o arquivo XML aberto se nenhuma exception for capturada
                    cc.salvarArquivoXML();

                    // Exibe a mensagem de sucesso se nenhuma exception for capturada
                    MessageBox.Show("Programação gravada com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else this.toolButtonSalvarComo.PerformClick();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Grava a programação atual com um novo nome.                      */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonSalvarComo_Click(object sender, EventArgs e)
        {
            try
            {
                // Cria o objeto para definir o nome e o caminho do arquivo do arquivo XML
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                // Define as propriedades saveFileDialog
                saveFileDialog.Filter = this.fileDialog_Filter;
                saveFileDialog.FileName = this.fileDialog_FileName;
                saveFileDialog.DefaultExt = this.fileDialog_DefaultExt;
                saveFileDialog.InitialDirectory = this.fileDialog_InitialDirectory;

                // Verifica se o usuário cancelou a janela de dialogo
                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Salva o novo arquivo XML se nenhuma exception for capturada
                    cc.salvarArquivoXMLComo(saveFileDialog.FileName);

                    // Define o titulo do form com o nome da aplicação + nome do novo arquivo XML
                    this.Text = titulo_aplicacao + " - " + Path.GetFileName(saveFileDialog.FileName);

                    // Exibe a mensagem de sucesso se nenhuma exception for capturada
                    MessageBox.Show("Programação gravada com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.StackTrace + Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia a programação feita na aplicação para a central.           */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.cc.buscarPortaCOM().Contains("COM"))
                {
                    // Cria o objeto do processo
                    processo = new Processo();
                    processo.JobCompleted += exibirResultadoProcesso;
                    processo.ProgressBarUpdated += atualizarBarraProgresso;

                    // Exibe a tela referente a APRESENTAÇÃO
                    this.exibirTela(tela.APRESENTACAO);

                    this.pnlTransmissao.Visible = true;
                    this.menuTool.Enabled = false;
                    this.btnVisualizarInicial.Enabled = false;
                    this.btnVisualizarRamais.Enabled = false;
                    this.btnVisualizarTroncos.Enabled = false;
                    this.btnVisualizarVideo.Enabled = false;
                    this.btnVisualizarMesaOperadora.Enabled = false;
                    this.btnVisualizarAlarme.Enabled = false;

                    // Chama o metodo responsável em enviar a programação para a central
                    cc.enviarProgramacaoCentral(processo, cc.buscarPortaCOM());
                }
                else throw new PortaCOMInvalidaException("A porta COM especificada não é uma porta válida.");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Coleta a programação da central para a aplicação.                */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonColetar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.cc.buscarPortaCOM().Contains("COM"))
                {
                    // Cria o objeto do processo
                    processo = new Processo();
                    processo.JobCompleted += exibirResultadoProcesso;
                    processo.ProgressBarUpdated += atualizarBarraProgresso;

                    // Exibe a tela referente a APRESENTAÇÃO
                    this.exibirTela(tela.APRESENTACAO);

                    this.pnlTransmissao.Visible = true;
                    this.menuTool.Enabled = false;
                    this.btnVisualizarInicial.Enabled = false;
                    this.btnVisualizarRamais.Enabled = false;
                    this.btnVisualizarTroncos.Enabled = false;
                    this.btnVisualizarVideo.Enabled = false;
                    this.btnVisualizarMesaOperadora.Enabled = false;
                    this.btnVisualizarAlarme.Enabled = false;

                    // Chama o metodo responsável em coletar a programação da central
                    cc.coletarProgramacaoCentral(processo, cc.buscarPortaCOM());
                }
                else throw new PortaCOMInvalidaException("A porta COM especificada não é uma porta válida.");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a tela referente as informações da porta COM.              */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonConfiguracao_Click(object sender, EventArgs e)
        {
            // Verifica qual é á tela que está ativa no momento
            if(this.pnlTelaNovoRamal.Visible)
                telaAtiva = tela.NOVO_RAMAL;
            else if (this.pnlTelaProgramacaoInicial.Visible)
                telaAtiva = tela.PROG_INICIAL;
            else if (this.pnlTelaListaRamais.Visible)
                telaAtiva = tela.LISTA_RAMAL;
            else if (this.pnlTelaTronco.Visible)
                telaAtiva = tela.TRONCO;
            else if (this.pnTelaMesaOperadora.Visible)
                telaAtiva = tela.MESA;
            else if (this.pnlTelaAlarme.Visible)
                telaAtiva = tela.ALARME;
            else if (this.pnlTelaVideo.Visible)
                telaAtiva = tela.VIDEO;
            else telaAtiva = tela.APRESENTACAO;

            try
            {
                // Verifica se a tela não está visível
                if (!this.pnlTelaConfiguracao.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Atualiza o componente com o valor do atributo
                    this.cboPortaCOM.SelectedItem = cc.buscarPortaCOM();

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.CONFIGURACAO);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Cria um relatório PDF com a programação da central.              */
        /* --------------------------------------------------------------------------------- */
        private void toolButtonExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cria o relatório PDF
                cc.criarRelatorioPDF();

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Relatório criado com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException Ex)
            {
                MessageBox.Show("O arquivo já está sendo usado por outro processo. Feche o arquivo e tente novamente.\n\nErro: " + Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region OPERAÇÕES E EVENTOS PARA O QUADRO "PROGRAMAÇÃO INICIAL"
        /* ---------------------------------------------------------------------------------OK */
        /* Funcionalidade : Exibe a tela referente a criação de um novo ramal.               */
        /* --------------------------------------------------------------------------------- */
        private void btnVisualizarInicial_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!this.pnlTelaProgramacaoInicial.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Cria o objeto que irá receber as novas definições
                    ProgramacaoInicial pi = cc.buscaProgramacaoInicial();
                    
                    // Atualiza os componentes com base nos dados do objeto
                    this.txtQtdeRamaisAEnviar.Text = pi.qtdeDeRamais.ToString();
                    this.txtIniciarNoRamal.Text = pi.posicao.ToString();
                    this.txtQtdeBlocos.Text = pi.qtdeDeBlocos.ToString();

                    this.txtNro1oBloco.Text = pi.nroPrimeiroBloco.ToString();
                    this.cboPosicaoNroBloco.SelectedIndex = (int)pi.posicaoDoNumeroBloco;

                    this.txtQtdeAndares.Text = pi.qtdeDeAndares.ToString();
                    this.txtQtdeAptoAndar.Text = pi.qtdeAptoAndar.ToString();

                    this.txtNro1oApto.Text = pi.nroPrimeiroApto;
                    this.cboDefinirRamalHot.SelectedIndex = pi.ramalHot == false ? 0 : 1;
                    this.cboDefinirRamalRestrito.SelectedIndex = pi.ramalRestrito == false ? 0 : 1;
                    this.cboDefinirRamalBloco.SelectedIndex = pi.ramalBloco == false ? 0 : 1;
                    this.txtNroInicialCentral2.Text = pi.iniciarCentral2;
                    this.txtNroInicialCentral3.Text = pi.iniciarCentral3;

                    if (pi.multiploPorAndar == 0)
                        this.cboMultiploAndar.SelectedIndex = 0;
                    else if (pi.multiploPorAndar == 1)
                        this.cboMultiploAndar.SelectedIndex = 10;
                    else if (pi.multiploPorAndar == 2)
                        this.cboMultiploAndar.SelectedIndex = 100;
                    else if (pi.multiploPorAndar == 3)
                        this.cboMultiploAndar.SelectedIndex = 1000;

                    this.cboSequenciaRamal.SelectedIndex = (int)pi.modo;

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.PROG_INICIAL);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Grava os dados que foram digitados.                              */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirProgInicial_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cria o objeto que irá receber as novas definições
                ProgramacaoInicial pi = new ProgramacaoInicial();

                // Atualiza o objeto com os dados dos componentes
                pi.qtdeDeRamais = Convert.ToInt32(this.txtQtdeRamaisAEnviar.Text);
                pi.posicao = Convert.ToInt32(this.txtIniciarNoRamal.Text);
                pi.qtdeDeBlocos = Convert.ToInt32(this.txtQtdeBlocos.Text);

                pi.nroPrimeiroBloco = Convert.ToInt32(this.txtQtdeBlocos.Text) == 0 ? 0 : Convert.ToInt32(this.txtNro1oBloco.Text);
                pi.posicaoDoNumeroBloco = this.cboPosicaoNroBloco.SelectedIndex == 0 ? posicaoNroBloco.ANTES : posicaoNroBloco.DEPOIS;

                pi.qtdeDeAndares = Convert.ToInt32(this.txtQtdeAndares.Text);
                pi.qtdeAptoAndar = Convert.ToInt32(this.txtQtdeAptoAndar.Text);

                pi.nroPrimeiroApto = this.txtNro1oApto.Text;
                pi.ramalHot = this.cboDefinirRamalHot.SelectedIndex == 0 ? false : true;
                pi.ramalRestrito = this.cboDefinirRamalRestrito.SelectedIndex == 0 ? false : true;
                pi.ramalBloco = this.cboDefinirRamalBloco.SelectedIndex == 0 ? false : true;
                pi.iniciarCentral2 = this.txtNroInicialCentral2.Text;
                pi.iniciarCentral3 = this.txtNroInicialCentral3.Text;

                if (this.cboMultiploAndar.SelectedIndex == 0)
                    pi.multiploPorAndar = 0;
                else if (this.cboMultiploAndar.SelectedIndex == 1)
                    pi.multiploPorAndar = 10;
                else if (this.cboMultiploAndar.SelectedIndex == 2)
                    pi.multiploPorAndar = 100;
                else if (this.cboMultiploAndar.SelectedIndex == 3)
                    pi.multiploPorAndar = 1000;

                pi.modo = this.cboSequenciaRamal.SelectedIndex == 0 ? modoNumeracao.NORMAL : modoNumeracao.PRUMADA;

                // Atualiza o objeto
                this.cc.gravarProgramacaoInicial(pi);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Os dados da programação inicial foi gravada com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Prepara a lista de ramais confrorme as informações digitas.      */
        /* --------------------------------------------------------------------------------- */
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (this.txtIniciarNoRamal.Text != "" && (Convert.ToInt32(this.txtIniciarNoRamal.Text) >= 1 && Convert.ToInt32(this.txtIniciarNoRamal.Text) <= 3072))
                {
                    if (this.txtQtdeBlocos.Text == "")
                        MessageBox.Show("É necessário especificar a quantidade de blocos.\n\nObservação:\nDefina essa opção como 0 (zero) para desativar as opções de bloco.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (this.txtNro1oBloco.Text == "" && this.txtQtdeBlocos.Text != "0")
                        MessageBox.Show("É necessário especificar o número do primeiro blocos.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (this.txtQtdeAptoAndar.Text == "")
                        MessageBox.Show("É necessário especificar a quantidade de apartamentos por andar.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (this.txtQtdeAndares.Text == "")
                        MessageBox.Show("É necessário especificar a quantidade de andares.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (this.txtNro1oApto.Text == "")
                        MessageBox.Show("É necessário especificar o número do primeiro apartamento.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (!(Convert.ToInt32(this.txtQtdeRamaisAEnviar.Text) >= 1 && Convert.ToInt32(this.txtQtdeRamaisAEnviar.Text) <= 3072))
                        MessageBox.Show("É necessário especificar a quantidade de ramais que serão enviados para a central.\nEscolha um valor entre 1 e 3072.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        // Cria o objeto que irá receber as novas definições
                        ProgramacaoInicial pi = new ProgramacaoInicial();

                        // Atualiza o objeto com os dados dos componentes
                        pi.qtdeDeRamais = Convert.ToInt32(this.txtQtdeRamaisAEnviar.Text);
                        pi.posicao = Convert.ToInt32(this.txtIniciarNoRamal.Text);
                        pi.qtdeDeBlocos = Convert.ToInt32(this.txtQtdeBlocos.Text);

                        pi.nroPrimeiroBloco = Convert.ToInt32(this.txtQtdeBlocos.Text) == 0 ? 0 : Convert.ToInt32(this.txtNro1oBloco.Text);
                        pi.posicaoDoNumeroBloco = this.cboPosicaoNroBloco.SelectedIndex == 0 ? posicaoNroBloco.ANTES : posicaoNroBloco.DEPOIS;

                        pi.qtdeDeAndares = Convert.ToInt32(this.txtQtdeAndares.Text);
                        pi.qtdeAptoAndar = Convert.ToInt32(this.txtQtdeAptoAndar.Text);

                        pi.nroPrimeiroApto = this.txtNro1oApto.Text;
                        pi.ramalHot = this.cboDefinirRamalHot.SelectedIndex == 0 ? false : true;
                        pi.ramalRestrito = this.cboDefinirRamalRestrito.SelectedIndex == 0 ? false : true;
                        pi.ramalBloco = this.cboDefinirRamalBloco.SelectedIndex == 0 ? false : true;
                        pi.iniciarCentral2 = this.txtNroInicialCentral2.Text;
                        pi.iniciarCentral3 = this.txtNroInicialCentral3.Text;

                        if (this.cboMultiploAndar.SelectedIndex == 0)
                            pi.multiploPorAndar = 0;
                        else if (this.cboMultiploAndar.SelectedIndex == 1)
                            pi.multiploPorAndar = 10;
                        else if (this.cboMultiploAndar.SelectedIndex == 2)
                            pi.multiploPorAndar = 100;
                        else if (this.cboMultiploAndar.SelectedIndex == 3)
                            pi.multiploPorAndar = 1000;

                        pi.modo = this.cboSequenciaRamal.SelectedIndex == 0 ? modoNumeracao.NORMAL : modoNumeracao.PRUMADA;

                        // Chama o método responsável em atualiza os objetos
                        cc.calcularNumeracaoAptos(pi);

                        // Posiciona o comboBox na CENTRAL 1
                        this.cboListaCentrais.SelectedIndex = 0;

                        // Atualizar o gridDataView com os ramais da CENTRAL 1
                        this.carregarListaRamais(0);

                        // Exibe o quadro referente a lista de ramais
                        this.exibirTela(tela.LISTA_RAMAL);

                        // Exibe a mensagem de sucesso se nenhuma exception for capturada
                        MessageBox.Show("O calculo da programação inicial foi realizado com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else MessageBox.Show("É necessário definir a posição de inicio para o calculo.\n\nAtenção:\n- Escolha um número entre 1 e 3072.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region OPERAÇÕES E EVENTOS PARA O QUADRO "NOVO RAMAL"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a tela referente a criação de um novo ramal.               */
        /* --------------------------------------------------------------------------------- */
        private void btnAdicionarRamal_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!pnlTelaNovoRamal.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Define o item inicial dos ComboBox
                    this.cboRamalHot.SelectedIndex = 0;
                    this.cboRamalBloco.SelectedIndex = 0;
                    this.cboRamalRestrito.SelectedIndex = 0;
                    this.cboPortPhone.SelectedIndex = 0;
                    this.cboAcessoLinhaExterna.SelectedIndex = 0;

                    // Define o valor inicial para os TextBox
                    this.txtPosicaoNovoRamal.Text = "";
                    this.txtNumeroRamal.Text = "";
                    this.txtNumeroAtendedor.Text = "*";

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.NOVO_RAMAL);       
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Adiciona na lista de ramais o novo ramal.                        */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirNovoRamal_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPosicaoNovoRamal.Text != "" && (Convert.ToInt32(txtPosicaoNovoRamal.Text) >= 1 && Convert.ToInt32(txtPosicaoNovoRamal.Text) <= 3072))
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Cria o objeto que irá receber as definições do novo ramal
                    Ramal novo = new Ramal();

                    // Atualiza o objeto com os dados dos componentes
                    novo.apartamento = this.txtNumeroRamal.Text;
                    novo.atendedor = this.txtNumeroAtendedor.Text;
                    novo.ramalHOT = (this.cboRamalHot.SelectedIndex == 0) ? false : true;
                    novo.ramalBLOCO = (this.cboRamalBloco.SelectedIndex == 0) ? false : true;
                    novo.ramalRESTRITO = (this.cboRamalRestrito.SelectedIndex == 0) ? false : true;
                    novo.placaPortPhone = (this.cboPortPhone.SelectedIndex == 0) ? false : true;
                    novo.acessoLinhaExterna = (this.cboAcessoLinhaExterna.SelectedIndex == 0) ? false : true;

                    // Envia o ramal para o método responsável em adicionar os ramais
                    cc.incluirNovoRamal(novo, Convert.ToInt32(this.txtPosicaoNovoRamal.Text));

                    // Atualizar o gridDataView com os ramais da CENTRAL 1
                    this.carregarListaRamais(0);

                    // Posiciona o comboBox na CENTRAL 1
                    this.cboListaCentrais.SelectedIndex = 0;

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.LISTA_RAMAL);

                    // Exibe a mensagem de sucesso se nenhuma exception for capturada
                    MessageBox.Show("O Ramal foi adicionado com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("É necessário definir a posição do novo ramal.\n\nAtenção:\n- Escolha um número entre 1 e 3072;\n- O ramal será incluido após o ramal escolhido.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Volta para tela da lista de ramais.                              */
        /* --------------------------------------------------------------------------------- */
        private void btnVoltarListaRamais_Click(object sender, EventArgs e)
        {
            try
            {
                // Exibe a tela referente ao objeto
                this.exibirTela(tela.LISTA_RAMAL);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define a ação que deverá ser tomada qndo o TextBox do atendedor  */
        /*                  perder o foco.                                                   */
        /* --------------------------------------------------------------------------------- */
        private void txtNumeroAtendedor_Leave(object sender, EventArgs e)
        {
            if (this.txtNumeroAtendedor.Text == "")
                this.txtNumeroAtendedor.Text = this.cc.buscarPorIdNumeroMesa("M1");
        }
        #endregion

        #region OPERAÇÕES E EVENTOS DA TELA "LISTA RAMAIS"
        /* -------------------------------------------------------------------------------OK */
        /* Funcionalidade : Exibe a tela referente as informações dos ramais.                */
        /* --------------------------------------------------------------------------------- */
        private void btnVisualizarRamais_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!pnlTelaListaRamais.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Seleciona o primeiro item do ComboBox (0 = CENTRAL 1)
                    // O evento SelectedIndexChanged do ComboBox chama o método responsável em buscar e exibir os dados dos RAMAIS
                    cboListaCentrais.SelectedIndex = 0;

                    // Carrega os componentes com os dados dos objetos (0 = RAMAIS DA CENTRAL 1)
                    this.carregarListaRamais(0);

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.LISTA_RAMAL);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* -------------------------------------------------------------------------------ok */
        /* Funcionalidade : Atualiza os objetos dos ramais com os novos valores.             */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirRamais_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Identifica o primeiro ramal da central selecionada
                int index = ((cboListaCentrais.SelectedIndex + 1) * 1024) - 1024;

                // Percorre todas as linhas do dataGridView
                for (int i = 0; i <= 1023; i++)
                {
                    // Cria o objeto ramal (recebe as novas definições)
                    Ramal r = new Ramal();

                    // Atualiza o objeto com os dados dos componentes
                    r.ramal = this.grdListaRamais["Column2", i].Value.ToString();
                    r.apartamento = this.grdListaRamais["Column3", i].Value.ToString();
                    r.ramalHOT = Convert.ToBoolean(this.grdListaRamais["Column4", i].Value);
                    r.ramalBLOCO = Convert.ToBoolean(this.grdListaRamais["Column5", i].Value);
                    r.ramalRESTRITO = Convert.ToBoolean(this.grdListaRamais["Column6", i].Value);
                    r.placaPortPhone = Convert.ToBoolean(this.grdListaRamais["Column7", i].Value);
                    r.acessoLinhaExterna = Convert.ToBoolean(this.grdListaRamais["Column8", i].Value);
                    r.atendedor = this.grdListaRamais["Column9", i].Value.ToString();

                    // Atualiza o objeto
                    cc.modificarListRamal(r, index);
                    index++;
                }

                // Carrega os componentes com os dados dos objetos
                this.carregarListaRamais(cboListaCentrais.SelectedIndex);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Atualizações realizadas com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* -------------------------------------------------------------------------------OK */
        /* Funcionalidade : Busca os dados dos RAMAIS sempre que mudar o item do ComboBox.   */
        /* --------------------------------------------------------------------------------- */
        private void cboCentral_SelectedIndexChanged(object sender, EventArgs e)
        {
            // O Panel não pode estar visível quando o ComboBox for populado
            if (this.pnlTelaListaRamais.Visible == true)
            {
                carregarListaRamais(cboListaCentrais.SelectedIndex);
                grdListaRamais.Focus();
            }
        }

        /* -------------------------------------------------------------------------------OK */
        /* Funcionalidade : Carrega os dados nos componentes da tela lista de ramais.        */
        /* --------------------------------------------------------------------------------- */
        private void carregarListaRamais(int id)
        {
            // Define a central que será utilizada para recuperar os RAMAIS
            int id_ramal = (((id + 1) * 1024) - 1024) + 1;

            for (int i = 0; i <= 1023; i++)
            {
                Ramal r = cc.buscarPorIdRamal(id_ramal);
                //this.grdListaRamais.Rows.Add(1);
                this.grdListaRamais["Column1", i].Value = false;
                this.grdListaRamais["Column2", i].Value = r.ramal;
                this.grdListaRamais["Column3", i].Value = r.apartamento;
                this.grdListaRamais["Column4", i].Value = r.ramalHOT;
                this.grdListaRamais["Column5", i].Value = r.ramalBLOCO;
                this.grdListaRamais["Column6", i].Value = r.ramalRESTRITO;
                this.grdListaRamais["Column7", i].Value = r.placaPortPhone;
                this.grdListaRamais["Column8", i].Value = r.acessoLinhaExterna;
                this.grdListaRamais["Column9", i].Value = r.atendedor;
                id_ramal++;
            }

            // Seleciona a linha do dataGridView e define qual linha deverá aparecer primeiro no dataGridView
            this.grdListaRamais.Rows[0].Selected = true;
            this.grdListaRamais.FirstDisplayedScrollingRowIndex = 0;
        }

        /* -------------------------------------------------------------------------------OK */
        /* Funcionalidade : Verifica se pressionado a tecla enter do TextBox (faz a busca).  */
        /* --------------------------------------------------------------------------------- */
        private void txtPesquisarRamal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                picPesquisarRamal_Click(sender, e);
        }

        /* -------------------------------------------------------------------------------OK */
        /* Funcionalidade : Encontra o ramal pesquisado e seleciona ele na lista.            */
        /* --------------------------------------------------------------------------------- */
        private void picPesquisarRamal_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se o número do ramal está dentro do range de ramais
                if (txtPesquisarRamal.Text != "" && (Convert.ToInt32(txtPesquisarRamal.Text) >= 1 && Convert.ToInt32(txtPesquisarRamal.Text) <= 3072))
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Verifica em qual central o ramal está localizado e carrega as informações dos ramais
                    int pesquisa = Convert.ToInt32(txtPesquisarRamal.Text);
                    if (pesquisa >= 1 && pesquisa <= 1024)
                    {
                        cboListaCentrais.SelectedIndex = 0;
                        pesquisa = pesquisa - 1;
                    }
                    else if (pesquisa >= 1025 && pesquisa <= 2048)
                    {
                        cboListaCentrais.SelectedIndex = 1;
                        pesquisa = (pesquisa - 1024) - 1;
                    }
                    else if (pesquisa >= 2049 && pesquisa <= 3072)
                    {
                        cboListaCentrais.SelectedIndex = 2;
                        pesquisa = (pesquisa - 2048) - 1;
                    }

                    // Seleciona a linha do dataGridView e define qual linha deverá aparecer primeiro no dataGridView
                    this.grdListaRamais.Rows[pesquisa].Selected = true;
                    this.grdListaRamais.FirstDisplayedScrollingRowIndex = pesquisa;

                    // Limpa a caixa de texto da pesquisa
                    this.txtPesquisarRamal.Text = "";
                }
                else MessageBox.Show("O número do ramal para a pesquisa não é válido.\nEscolha um número entre 1 e 3072.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* -------------------------------------------------------------------------------OK */
        /* Funcionalidade : Descarta as modificações realizadas (Reload no dataGridView).    */
        /* --------------------------------------------------------------------------------- */
        private void btnAtualizarGrid_Click(object sender, EventArgs e)
        {
            this.carregarListaRamais(cboListaCentrais.SelectedIndex);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exclui os ramais colocados em um ArrayList.                      */
        /* --------------------------------------------------------------------------------- */
        private void btnExcluirRamal_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se deseja excluir os ramais selecionados
                if (DialogResult.Yes == MessageBox.Show("Deseja excluir os ramais selecionados?", "CentralCDX", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Cria o ArrayList que vai receber os ramais que deverão ser excluidos
                    ArrayList excluidos = new ArrayList();

                    // Percorre todas as linhas do dataGridView
                    for (int i = 0; i <= 1023; i++)
                    {
                        // Verifica qual linha do dataGridView está selecionada (exclusão)
                        if (Convert.ToBoolean(this.grdListaRamais["Column1", i].Value))
                        {
                            // Guarda o número do ramal que deverá ser excluido
                            excluidos.Add(this.grdListaRamais["Column2", i].Value);
                        }
                    }

                    // verifica se o ArrayList está vázio
                    if (excluidos.Count != 0)
                    {
                        // Remove os ramais do ArrayList se nenhuma exception for capturada
                        this.cc.removerRamais(excluidos);

                        // Atualiza os componentes com os dados dos objetos da central corrente
                        this.carregarListaRamais(cboListaCentrais.SelectedIndex);

                        // Exibe a mensagem de sucesso se nenhuma exception for capturada
                        MessageBox.Show("Os ramais foram excluidos com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("Não existem ramais para serem excluidos.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* -------------------------------------------------------------------------------ok */
        /* Funcionalidade : Define as teclas que não podem ser usadas nos TextBox do grid.   */
        /*                  O evento verifica se o dataGridView sofreu modificação.          */
        /* --------------------------------------------------------------------------------- */
        private void grdListaRamais_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Verifica se as modificações são referente as coluna Número ou Atendedor
            if (grdListaRamais.CurrentCell.ColumnIndex == 2 || grdListaRamais.CurrentCell.ColumnIndex == 8)
            {
                // Define o método para o evento KeyPress dos TextBox do DataGridView
                DataGridViewTextBoxEditingControl textBox = e.Control as DataGridViewTextBoxEditingControl;
                if (textBox != null)
                {
                    textBox.KeyPress -= new KeyPressEventHandler(ControlApartamento_KeyPress);
                    textBox.KeyPress += new KeyPressEventHandler(ControlApartamento_KeyPress);
                }
            }
        }
        #endregion

        #region OPERAÇÕES E EVENTOS DA TELA "TRONCO"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a tela referente as informações do tronco.                 */
        /* --------------------------------------------------------------------------------- */
        private void btnVisualizarTronco_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!pnlTelaTronco.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Carrega os componentes com os dados dos objetos
                    this.chkHabilitaTronco1.Checked = this.cc.buscaPorIdEstadoTronco(1);
                    this.chkChamadaCobrarTronco1.Checked = this.cc.buscaPorIdEstadoChCobrar(1);
                    this.txtAtendedorTronco1.Text = this.cc.buscarPorIdAtendedorTronco(1);

                    this.chkHabilitaTronco2.Checked = this.cc.buscaPorIdEstadoTronco(2);
                    this.chkChamadaCobrarTronco2.Checked = this.cc.buscaPorIdEstadoChCobrar(2);
                    this.txtAtendedorTronco2.Text = this.cc.buscarPorIdAtendedorTronco(2);

                    for (int i = 1; i <= 20; i++)
                    {
                        TextBox t = (TextBox)this.Controls.Find("txtTelefoneLiberado" + i, true)[0];
                        t.Text = this.cc.buscarPorIdTelLiberadoTonco(i);
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        TextBox t = (TextBox)this.Controls.Find("txtTelefoneBloqueado" + i, true)[0];
                        t.Text = this.cc.buscarPorIdTelBloqueadoTonco(i);
                    }

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.TRONCO);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Atualiza o objeto do tronco com os novos valores.                */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirTronco_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cria os objetos que irão receber as novas definições
                List<Tronco> troncos = new List<Tronco>();
                ArrayList liberados = new ArrayList();
                ArrayList bloqueados = new ArrayList();
                
                // Atualiza os objetos com os dados dos componentes
                Tronco t1 = new Tronco();
                t1.estado = this.chkHabilitaTronco1.Checked;
                t1.estadoChamadaCobrar = this.chkChamadaCobrarTronco1.Checked;
                t1.atendedor = this.txtAtendedorTronco1.Text;

                Tronco t2 = new Tronco();
                t2.estado = this.chkHabilitaTronco2.Checked;
                t2.estadoChamadaCobrar = this.chkChamadaCobrarTronco2.Checked;
                t2.atendedor = this.txtAtendedorTronco2.Text;

                troncos.Add(t1);
                troncos.Add(t2);

                for (int i = 1; i <= 20; i++)
                {
                    TextBox t = (TextBox)this.Controls.Find("txtTelefoneLiberado" + i, true)[0];
                    liberados.Add(t.Text);
                }

                for (int i = 1; i <= 20; i++)
                {
                    TextBox t = (TextBox)this.Controls.Find("txtTelefoneBloqueado" + i, true)[0];
                    bloqueados.Add(t.Text);
                }

                // Atualiza o objeto
                cc.definirLinhaExterna(troncos, liberados, bloqueados);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Atualizações realizadas com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define a ação que deverá ser tomada quando o TextBox do tronco 1 */
        /*                  perder o foco.                                                   */
        /* --------------------------------------------------------------------------------- */
        private void txtAtendedorTronco1_Leave(object sender, EventArgs e)
        {
            if (txtAtendedorTronco1.Text == "")
                txtAtendedorTronco1.Text = this.cc.buscarPorIdNumeroMesa("M1");
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define a ação que deverá ser tomada quando o TextBox do tronco 2 */
        /*                  perder o foco.                                                   */
        /* --------------------------------------------------------------------------------- */
        private void txtAtendedorTronco2_Leave(object sender, EventArgs e)
        {
            if (txtAtendedorTronco2.Text == "")
                txtAtendedorTronco2.Text = this.cc.buscarPorIdNumeroMesa("M1");
        }
        #endregion

        #region OPERAÇÕES E EVENTOS DA TELA "VÍDEO"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a tela referente as informações do vídeo.                  */
        /* --------------------------------------------------------------------------------- */
        private void btnVisualizarVideo_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!this.pnlTelaVideo.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Carrega os componentes com os dados do objeto
                    for (int i = 1; i <= 8; i++)
                    {
                        CheckBox chk = (CheckBox)this.Controls.Find("chkVideo" + i + "Habilita", true)[0];
                        chk.Checked = this.cc.buscaPorIdStatusVideo(i);
                        TextBox txt = (TextBox)this.Controls.Find("txtVideo" + i + "Numero", true)[0];
                        txt.Text = this.cc.buscarPorIdNumeroVideo(i);
                    }

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.VIDEO);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Atualiza o objeto do alarme com os novos valores.                */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirVideo_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cria a lista de objeto que irão receber as novas definições
                List<Video> videos = new List<Video>();

                // Prepara a lista de vídeos
                for (int i = 1; i <= 8; i++)
                {
                    Video v = new Video();
                    CheckBox chk = (CheckBox)this.Controls.Find("chkVideo" + i + "Habilita", true)[0];
                    v.estado = chk.Checked;
                    TextBox txt = (TextBox)this.Controls.Find("txtVideo" + i + "Numero", true)[0];
                    v.numero = txt.Text;
                    videos.Add(v);
                }

                // Atualiza o objeto
                this.cc.definirVideo(videos);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Atualizações realizadas com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region OPERAÇÕES E EVENTOS DA TELA "MESA OPERADORA"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a tela referente as informações da mesa operadora.         */
        /* --------------------------------------------------------------------------------- */
        private void btnVisualizarMesaOperadora_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!this.pnTelaMesaOperadora.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Seleciona o primeiro item do ComboBox (referente a MESA 1)
                    cboListMesaOperadora.SelectedIndex = 0;

                    // Chama o método responsável em carrega os dados do objeto
                    this.carregarDadosMesaOperadora(1);

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.MESA);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Atualiza o objeto da mesa operadora com os novos valores.        */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirMesa_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cria o objeto que irá receber as novas definições
                MesaOperadora mesa = new MesaOperadora();

                // Atualiza o objeto
                mesa.numero = this.txtMesaNumero.Text;
                mesa.definirEstadoTecla(nome.ZELADOR, this.buscarEstadoAtivoListRadioBottun(this.listTeclaZelador));
                mesa.definirEstadoTecla(nome.SINDICO, this.buscarEstadoAtivoListRadioBottun(this.listTeclaSindico));
                mesa.definirEstadoTecla(nome.A1, this.buscarEstadoAtivoListRadioBottun(this.listTeclaA1));
                mesa.definirEstadoTecla(nome.A2, this.buscarEstadoAtivoListRadioBottun(this.listTeclaA2));
                mesa.definirEstadoTecla(nome.A3, this.buscarEstadoAtivoListRadioBottun(this.listTeclaA3));
                mesa.definirEstadoTecla(nome.PORTEIRO, this.buscarEstadoAtivoListRadioBottun(this.listTeclaPorteiro));
                mesa.definirEstadoTecla(nome.TELEFONE, this.buscarEstadoAtivoListRadioBottun(this.listTeclaTelefone));
                mesa.definirEstadoTecla(nome.FECH1, this.buscarEstadoAtivoListRadioBottun(this.listTeclaFech1));
                mesa.definirEstadoTecla(nome.FECH2, this.buscarEstadoAtivoListRadioBottun(this.listTeclaFech2));
                mesa.definirAtendedorTecla(nome.ZELADOR, this.txtMesaZelador_Numero.Text);
                mesa.definirAtendedorTecla(nome.SINDICO, this.txtMesaSindico_Numero.Text);
                mesa.definirAtendedorTecla(nome.A1, this.txtMesaA1_Numero.Text);
                mesa.definirAtendedorTecla(nome.A2, this.txtMesaA2_Numero.Text);
                mesa.definirAtendedorTecla(nome.A3, this.txtMesaA3_Numero.Text);
                mesa.definirAtendedorTecla(nome.PORTEIRO, this.txtMesaPorteiro_Numero.Text);
                mesa.definirAtendedorTecla(nome.TELEFONE, this.txtMesaTelefone_Numero.Text);
                mesa.definirAtendedorTecla(nome.FECH1, this.txtMesaFech1_Numero.Text);
                mesa.definirAtendedorTecla(nome.FECH2, this.txtMesaFech2_Numero.Text);

                // Atualiza o objeto
                this.cc.definirMesa(mesa, cboListMesaOperadora.SelectedIndex + 1);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Atualizações realizadas com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Busca os dados da MESA sempre que mudar o item do ComboBox.      */
        /*                  O evento chama o método responsável em buscar e exibir os dados  */
        /*                  da MESA OPERADORA quando o Panel estiver visivel.                */
        /* --------------------------------------------------------------------------------- */
        private void cboListMesaOperadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            // O Panel não pode estar visível quando o ComboBox for populado
            if (this.pnTelaMesaOperadora.Visible == true)
                carregarDadosMesaOperadora(cboListMesaOperadora.SelectedIndex + 1);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Carrega os componentes com os dados do objeto da mesa operadora. */
        /* --------------------------------------------------------------------------------- */
        private void carregarDadosMesaOperadora(int id)
        {
            // Atualiza os componentes
            this.txtMesaNumero.Text = cc.buscarNumeroMesa(id);

            this.listTeclaZelador[(int)cc.buscarPorNomeEstadoTecla(id, nome.ZELADOR)].Checked = true;
            this.listTeclaSindico[(int)cc.buscarPorNomeEstadoTecla(id, nome.SINDICO)].Checked = true;
            this.listTeclaA1[(int)cc.buscarPorNomeEstadoTecla(id, nome.A1)].Checked = true;
            this.listTeclaA2[(int)cc.buscarPorNomeEstadoTecla(id, nome.A2)].Checked = true;
            this.listTeclaA3[(int)cc.buscarPorNomeEstadoTecla(id, nome.A3)].Checked = true;
            this.listTeclaPorteiro[(int)cc.buscarPorNomeEstadoTecla(id, nome.PORTEIRO)].Checked = true;
            this.listTeclaTelefone[(int)cc.buscarPorNomeEstadoTecla(id, nome.TELEFONE)].Checked = true;
            this.listTeclaFech1[(int)cc.buscarPorNomeEstadoTecla(id, nome.FECH1)].Checked = true;
            this.listTeclaFech2[(int)cc.buscarPorNomeEstadoTecla(id, nome.FECH2)].Checked = true;

            this.txtMesaZelador_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.ZELADOR);
            this.txtMesaSindico_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.SINDICO);
            this.txtMesaA1_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.A1);
            this.txtMesaA2_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.A2);
            this.txtMesaA3_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.A3);
            this.txtMesaPorteiro_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.PORTEIRO);
            this.txtMesaTelefone_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.TELEFONE);
            this.txtMesaFech1_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.FECH1);
            this.txtMesaFech2_Numero.Text = cc.buscarNumeroAtendedorTecla(id, nome.FECH2);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Verifica qual RadioBox (na lista de RadioBox's) está selecionado */
        /*                  e retorna o estado da tecla.                                     */
        /* --------------------------------------------------------------------------------- */
        private estado buscarEstadoAtivoListRadioBottun(List<RadioButton> list)
        {
            estado result = estado.DESATIVADA;
            foreach (RadioButton r in list)
            {
                if (r.Checked && r.Name.Contains("_Desativada"))
                    result = estado.DESATIVADA;
                else if (r.Checked && r.Name.Contains("_Ramal"))
                    result = estado.RAMAL;
                else if (r.Checked && r.Name.Contains("_Telefone"))
                    result = estado.TELEFONE;
                else if (r.Checked && r.Name.Contains("_Fechadura"))
                    result = estado.FECHADURA;
            }
            return result;
        }

        #region Evento CheckedChanged dos RadioBox da MESA OPERADORA
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Chama o método responsável em define o TextBox.                  */
        /* --------------------------------------------------------------------------------- */
        #region RadioBox Zelador
        private void rdbZelador_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaZelador_Numero, estado.DESATIVADA);
        }

        private void rdbZelador_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaZelador_Numero, estado.RAMAL);
        }

        private void rdbZelador_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaZelador_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox Sindico
        private void rdbSindico_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaSindico_Numero, estado.DESATIVADA);
        }
        private void rdbSindico_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaSindico_Numero, estado.RAMAL);
        }
        private void rdbSindico_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaSindico_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox A1
        private void rdbA1_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA1_Numero, estado.DESATIVADA);
        }
        private void rdbA1_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA1_Numero, estado.RAMAL);
        }
        private void rdbA1_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA1_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox A2
        private void rdbA2_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA2_Numero, estado.DESATIVADA);
        }
        private void rdbA2_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA2_Numero, estado.RAMAL);
        }
        private void rdbA2_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA2_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox A3
        private void rdbA3_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA3_Numero, estado.DESATIVADA);
        }
        private void rdbA3_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA3_Numero, estado.RAMAL);
        }
        private void rdbA3_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaA3_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox Porteiro
        private void rdbPorteiro_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaPorteiro_Numero, estado.DESATIVADA);
        }
        private void rdbPorteiro_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaPorteiro_Numero, estado.RAMAL);
        }
        private void rdbPorteiro_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaPorteiro_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox Telefone
        private void rdbTelefone_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaTelefone_Numero, estado.DESATIVADA);
        }
        private void rdbTelefone_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaTelefone_Numero, estado.RAMAL);
        }
        private void rdbTelefone_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaTelefone_Numero, estado.TELEFONE);
        }
        #endregion

        #region RadioBox Fech1
        private void rdbFech1_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech1_Numero, estado.DESATIVADA);
        }
        private void rdbFech1_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech1_Numero, estado.RAMAL);
        }
        private void rdbFech1_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech1_Numero, estado.TELEFONE);
        }
        private void rdbFech1_Fechadura_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech1_Numero, estado.FECHADURA);
        }
        #endregion

        #region RadioBox Fech2
        private void rdbFech2_Desativada_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech2_Numero, estado.DESATIVADA);
        }
        private void rdbFech2_Ramal_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech2_Numero, estado.RAMAL);
        }
        private void rdbFech2_Telefone_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech2_Numero, estado.TELEFONE);
        }
        private void rdbFech2_Fechadura_CheckedChanged(object sender, EventArgs e)
        {
            defineTextBoxMesa(txtMesaFech2_Numero, estado.FECHADURA);
        }
        #endregion

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Define o estado e o método para o evento keypress do TextBox.    */
        /* --------------------------------------------------------------------------------- */
        private void defineTextBoxMesa(TextBox t, estado e)
        {
            if (e == estado.TELEFONE)
            {
                t.Enabled = true;
                t.Text = "";
                t.MaxLength = 20;
                t.KeyPress -= new KeyPressEventHandler(ControlRamalAndTelefone_KeyPress);
                t.KeyPress += new KeyPressEventHandler(ControlRamalAndTelefone_KeyPress);
            }
            else if (e == estado.RAMAL)
            {
                t.Enabled = true;
                t.Text = "";
                t.MaxLength = 8;
                t.KeyPress -= new KeyPressEventHandler(ControlApartamento_KeyPress);
                t.KeyPress += new KeyPressEventHandler(ControlApartamento_KeyPress);
            }
            else if (e == estado.DESATIVADA || e == estado.FECHADURA)
            {
                t.Enabled = false;
                t.Text = "";
            }
        }
        #endregion
        #endregion

        #region OPERAÇÕES E EVENTOS DA TELA "ALARME"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a tela referente as informações do alarme.                 */
        /* --------------------------------------------------------------------------------- */
        private void btnVisualizarAlarme_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a tela não está visível
                if (!this.pnlTelaAlarme.Visible)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Carrega os componentes com os dados do objeto
                    this.txtAlarmeNumero.Text = this.cc.buscarNumeroAlarme();
                    this.nroAlarmeTempo.Text = this.cc.buscarTempoAlarme();
                    for (int i = 1; i <= 10; i++)
                    {
                        TextBox t = (TextBox)this.Controls.Find("txtAlarmeAtendedor" + i, true)[0];
                        t.Text = cc.buscarPorIdAtendedorAlarme(i);
                    }

                    // Exibe a tela referente ao objeto
                    this.exibirTela(tela.ALARME);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Atualiza o objeto do alarme com os novos valores.                */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirAlarme_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cria a lista de objeto que irão receber as novas definições
                ArrayList atendedores = new ArrayList();

                // Prepara a lista de atendedores
                for (int i = 1; i <= 10; i++)
                {
                    TextBox t = (TextBox)this.Controls.Find("txtAlarmeAtendedor" + i, true)[0];
                    atendedores.Add(t.Text);
                }

                // Atualiza o objeto
                this.cc.definirAlarme(this.txtAlarmeNumero.Text, this.nroAlarmeTempo.Text, atendedores);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Atualizações realizadas com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region OPERAÇÕES E EVENTOS DA TELA "CONFIGURAÇÕES"
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Atualiza o objeto da porta serial com o novo valor.              */
        /* --------------------------------------------------------------------------------- */
        private void btnConcluirPortaCOM_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Atualiza o objeto com o valor do componente
                cc.definirPortaCOM((string)this.cboPortaCOM.SelectedItem);

                // Exibe a mensagem de sucesso se nenhuma exception for capturada
                MessageBox.Show("Atualizações realizadas com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Verifica se a central está respondendo pela porta COM.           */
        /* --------------------------------------------------------------------------------- */
        private void btnTestarConexao_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.cboPortaCOM.Items.Count != 0 && this.cboPortaCOM.SelectedItem.ToString().Contains("COM"))
                {
                    // Cria o objeto do processo
                    processo = new Processo();
                    processo.JobCompleted += exibirResultadoProcesso;

                    // Verifica se a central está respondendo na porta COM configurada
                    this.cc.verificarConexao(processo, (string)this.cboPortaCOM.SelectedItem);
                }
                else throw new PortaCOMInvalidaException("A porta COM especificada não é uma porta válida.");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibe a última tela ativa.                                       */
        /* --------------------------------------------------------------------------------- */
        private void bntVoltar_Click(object sender, EventArgs e)
        {
            this.exibirTela(telaAtiva);
        }
        #endregion

        #region FUNCIONALIDADES ÚTEIS DA CLASSE
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Adiciona os itens de um array em um determinado ComboBox.        */
        /* --------------------------------------------------------------------------------- */
        private void definirComboBox(object objComboBox, object objValor)
        {
            // Limpa os itens do ComboBox
            (objComboBox as ComboBox).Items.Clear();

            // Adciona os itens da variável no ComboBox
            for (int i = 0; i < (objValor as String[]).Length; i++)
                (objComboBox as ComboBox).Items.Add((objValor as String[])[i]);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Definir qual tela deverá ficar visível.                          */
        /* --------------------------------------------------------------------------------- */
        private void exibirTela(tela id)
        {
            // Define a visibilidade de todos os Panel's como FALSE
            this.pnlTelaApresentacao.Visible = false;
            this.pnlTelaProgramacaoInicial.Visible = false;
            this.pnlTelaListaRamais.Visible = false;
            this.pnlTelaTronco.Visible = false;
            this.pnTelaMesaOperadora.Visible = false;
            this.pnlTelaAlarme.Visible = false;
            this.pnlTelaVideo.Visible = false;
            this.pnlTelaConfiguracao.Visible = false;
            this.pnlTelaNovoRamal.Visible = false;

            // Seleciona a tela
            switch ((int)id)
            {
                case 0:
                    this.pnlTelaApresentacao.Visible = true;

                    // Define o item inicial dos ComboBox
                    this.cboSequenciaRamal.SelectedIndex = 0;
                    this.cboPosicaoNroBloco.SelectedIndex = 0;
                    this.cboDefinirRamalBloco.SelectedIndex = 0;
                    this.cboDefinirRamalRestrito.SelectedIndex = 0;
                    this.cboDefinirRamalHot.SelectedIndex = 0;
                    this.cboMultiploAndar.SelectedIndex = 2;

                    // Limpa os TextBox
                    this.txtNroInicialCentral2.Text = "";
                    this.txtNroInicialCentral3.Text = "";
                    this.txtQtdeBlocos.Text = "";
                    this.txtNro1oBloco.Text = "";
                    this.txtQtdeAptoAndar.Text = "";
                    this.txtQtdeAndares.Text = "";
                    this.txtNro1oApto.Text = "";
                    break;
                case 1:
                    this.pnlTelaProgramacaoInicial.Visible = true;
                    this.btnCalcular.Focus();
                    break;
                case 2:
                    this.pnlTelaListaRamais.Visible = true;
                    this.txtPesquisarRamal.Focus();
                    break;
                case 3:
                    this.pnlTelaTronco.Visible = true;
                    this.btnConcluirTronco.Focus();
                    break;
                case 4:
                    this.pnTelaMesaOperadora.Visible = true;
                    this.btnConcluirMesa.Focus();
                    break;
                case 5:
                    this.pnlTelaAlarme.Visible = true;
                    this.btnConcluirAlarme.Focus();
                    break;
                case 6:
                    this.pnlTelaVideo.Visible = true;
                    this.btnConcluirVideo.Focus();
                    break;
                case 7:
                    this.pnlTelaConfiguracao.Visible = true;
                    break;
                case 8:
                    this.pnlTelaNovoRamal.Visible = true;
                    this.btnConcluirNovoRamal.Focus();
                    break;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Definir os caracteres que poderão entrar no TextBox referente    */
        /*                  ao número do APARTAMENTO.                                        */
        /* --------------------------------------------------------------------------------- */
        private void ControlApartamento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Regex reg = new Regex("^[0-9*#]$");
            if (reg.IsMatch(Convert.ToString(e.KeyChar)) || (e.KeyChar == (char)Keys.Back))
                e.Handled = false;
            else e.Handled = true;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Definir os caracteres que poderão entrar no TextBox referente    */
        /*                  ao número do RAMAL e TELEFONE.                                   */
        /* --------------------------------------------------------------------------------- */
        private void ControlRamalAndTelefone_KeyPress(object sender, KeyPressEventArgs e)
        {
            Regex reg = new Regex("^[0-9]$");
            if (reg.IsMatch(Convert.ToString(e.KeyChar)) || (e.KeyChar == (char)Keys.Back))
                e.Handled = false;
            else e.Handled = true;
        }
        #endregion

        #region FUNCIONALIDADES DE MONITORAÇÃO DE PROCESSOS

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Cancelar o envio da programação para a central.                  */
        /* --------------------------------------------------------------------------------- */
        private void btnCancelarProcesso_Click(object sender, EventArgs e)
        {
            this.processo.cancelar = true;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Exibir o resultado do processo.                                  */
        /* --------------------------------------------------------------------------------- */
        delegate void CompletedCallback(object sender);

        public void exibirResultadoProcesso(object sender)
        {
            HabilitarComponentesCallback d = new HabilitarComponentesCallback(habilitarComponentes);
            this.Invoke(d, new object[] { });

            switch (processo.estado)
            {
                case (EnumEstado.CONCLUIDO):
                case (EnumEstado.RECEBIDO):
                    MessageBox.Show(processo.mensagem, "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case (EnumEstado.INCORRETO):
                case (EnumEstado.INVALIDO):
                case (EnumEstado.TIMEOUT):
                case (EnumEstado.ABORTADO):
                case (EnumEstado.EXCEPTION):
                    MessageBox.Show(processo.mensagem, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            processo.estado = EnumEstado.PARADO;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Habilitar os componentes depois do envio da programação.         */
        /* --------------------------------------------------------------------------------- */
        delegate void HabilitarComponentesCallback();

        public void habilitarComponentes()
        {
            this.pnlTransmissao.Visible = false;
            this.menuTool.Enabled = true;
            this.btnVisualizarInicial.Enabled = true;
            this.btnVisualizarRamais.Enabled = true;
            this.btnVisualizarTroncos.Enabled = true;
            this.btnVisualizarVideo.Enabled = true;
            this.btnVisualizarMesaOperadora.Enabled = true;
            this.btnVisualizarAlarme.Enabled = true;

            this.lblPorcent.Text = "0 %";
            this.progressBar.Value = 0;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Informa o andamento do processo atualizando a barra de status.   */
        /* --------------------------------------------------------------------------------- */
        delegate void ChangedCallback(object sender, int value);

        void atualizarBarraProgresso(object sender, int value)
        {
            if (this.progressBar.InvokeRequired)
            {
                ChangedCallback d = new ChangedCallback(atualizarBarraProgresso);
                this.Invoke(d, new object[] { sender, value });
            }
            else
            {
                this.lblPorcent.Text = value + " %";
                this.progressBar.Value = value;
            }
        }
        #endregion
    }
}