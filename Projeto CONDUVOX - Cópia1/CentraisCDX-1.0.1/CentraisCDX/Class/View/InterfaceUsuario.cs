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
using CentraisCDX.Class.Controle;
using System.Collections.Generic;
using CentraisCDX.Class.Modelo;
using System.Drawing;
using System.Text.RegularExpressions;
using iTextSharp.text;
using System.Threading;

namespace CentraisCDX.Class.View
{
    public partial class InterfaceUsuario : Form
    {
        private enum Tela { APRESENTACAO, PROG_INICIAL, LISTA_RAMAL, TRONCO, MESA, ALARME, VIDEO, CONFIGURACAO, NOVO_RAMAL };

        // ESTADO DO OBJETO
        private Tela telaAtiva = Tela.APRESENTACAO;

        private string titulo_aplicacao = "Centrais telefônicas Conduvox";
        private string fileDialog_Filter = "Centrais conduvox CDX (*.cdx)|*cdx";
        private string fileDialog_DefaultExt = "cdx";
        private string fileDialog_FileName = "nova_programacao";
        private string fileDialog_InitialDirectory = Application.StartupPath + "\\Arquivos";

        private string[] listSequenciaRamal = new string[2] { "Normal", "Prumada" };
        private string[] listPosicaoNroBloco = new string[2] { "Antes do apto", "Depois do apto" };
        private string[] listMultiploAndar = new string[4] { "Unidade", "Dezena", "Centena", "Milhar" };
        private string[] listNaoSim = new string[2] { "Não", "Sim" };
        private string[] listCentrais = new string[3] { "Central 1", "Central 2", "Central 3" };
        private string[] listMesaOperadora = new string[8] { "Mesa *", "Mesa *1", "Mesa *2", "Mesa *3", "Mesa *4", "Mesa *5", "Mesa *6", "Mesa *7" };

        private ControleCentral cc = new ControleCentral();

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
            this.txtPesquisarRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress); // Define o metodo para o evento "KeyPress" dos "TextBox"

            // Inicializa a tela: pnlTelaVideo
            this.txtVideo1Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo2Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo3Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo4Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo5Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo6Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo7Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtVideo8Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);

            // Inicializa a tela: pnlTelaTronco
            this.txtAtendedorTronco1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAtendedorTronco2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            for (int i = 1; i <= 20; i++)
            {
                TextBox t = (TextBox)this.Controls.Find("txtTelefoneLiberado" + i, true)[0];
                t.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            }
            for (int i = 1; i <= 20; i++)
            {
                TextBox t = (TextBox)this.Controls.Find("txtTelefoneBloqueado" + i, true)[0];
                t.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            }

            // Inicializa a tela: pnlTelaMesaOperadora
            this.definirComboBox(this.cboListMesaOperadora, this.listMesaOperadora);
            this.txtMesaNumero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            
            this.txtMesaSindico_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaA1_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaA2_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaA2_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaPorteiro_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaTelefone_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaFech1_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtMesaFech2_Numero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
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
            // FECH2
            this.listTeclaFech2.Add(rdbFech2_Desativada);
            this.listTeclaFech2.Add(rdbFech2_Ramal);
            this.listTeclaFech2.Add(rdbFech2_Telefone);

            // Inicializa a tela: pnlTelaAlarme
            this.txtAlarmeNumero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.nroAlarmeTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor7.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor8.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor9.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtAlarmeAtendedor10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);

            // Inicializa a tela: pnlTelaIncluirRamal
            this.definirComboBox(this.cboRamalHot, listNaoSim);
            this.definirComboBox(this.cboRamalBloco, listNaoSim);
            this.definirComboBox(this.cboRamalRestrito, listNaoSim);
            this.definirComboBox(this.cboPortPhone, listNaoSim);
            this.definirComboBox(this.cboAcessoLinhaExterna, listNaoSim);
            this.txtPosicaoNovoRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtNumeroRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);
            this.txtNumeroAtendedor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlApartamento_KeyPress);

            // Inicializa a tela: pnlTelaProgramacaoInicial
            this.definirComboBox(this.cboSequenciaRamal, listSequenciaRamal);
            this.definirComboBox(this.cboPosicaoNroBloco, listPosicaoNroBloco);
            this.definirComboBox(this.cboDefinirRamalBloco, listNaoSim);
            this.definirComboBox(this.cboDefinirRamalRestrito, listNaoSim);
            this.definirComboBox(this.cboDefinirRamalHot, listNaoSim);
            this.definirComboBox(this.cboMultiploAndar, listMultiploAndar);
            this.txtIniciarNoRamal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtNroInicialCentral2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtNroInicialCentral3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtQtdeBlocos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtNro1oBloco.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtQtdeAptoAndar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtQtdeAndares.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
            this.txtNro1oApto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlRamalAndTelefone_KeyPress);
        }
        #endregion

        #region EVENTO LOAD DO FORMULÁRIO
        /* ---------------------------------------------------------------------------------VERIFICAR */
        /* Funcionalidade : Evento que ocorre antes que um formulário seja exibido pela      */
        /*                  primeira vez.                                                    */
        /* --------------------------------------------------------------------------------- */
        private void InterfaceUsuario_Load(object sender, EventArgs e)
        {
            // Define o estado do processo
            //Processo.estado = estados.PARADO;

            // Define o método que ficará ficará monitorando o estado do processo de transferência de dados
            //this.thread = new Thread(monitor);
            //this.thread.Start();

            // Define o titulo do form com o nome da aplicação
            this.Text = titulo_aplicacao;

            // Carrega os objetos com a programação padrão de fábrica
            //this.cc.carregarCentralDefault();

            // Exibe a tela referente a APRESENTAÇÃO
            this.exibirTela(Tela.APRESENTACAO);
        }

        /* ---------------------------------------------------------------------------------VERIFICAR */
        /* Funcionalidade : Evento que ocorre quando o formulário está sendo fechado.        */
        /* --------------------------------------------------------------------------------- */
        private void InterfaceUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Mata os processos que estão rodando em background (escondido)
            //this.processo.terminarProcesso();
            //this.thread.Abort();
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
        private void exibirTela(Tela id)
        {
            // Define a visibilidade de todos os Panel's como FALSE
            this.picTelaApresentacao.Visible = false;
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
                    this.picTelaApresentacao.Visible = true;
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
        private void controlApartamento_KeyPress(object sender, KeyPressEventArgs e)
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
        private void controlRamalAndTelefone_KeyPress(object sender, KeyPressEventArgs e)
        {
            Regex reg = new Regex("^[0-9]$");
            if (reg.IsMatch(Convert.ToString(e.KeyChar)) || (e.KeyChar == (char)Keys.Back))
                e.Handled = false;
            else e.Handled = true;
        }
        #endregion

        #region OPERAÇÕES E EVENTOS PARA O QUADRO "PROGRAMAÇÃO INICIAL"
        /* --------------------------------------------------------------------------------- */
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
                    ProgramacaoInicial pi = cc.buscarProgramacaoInicial();

                    // Atualiza os componentes com base nos dados do objeto
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

                    if (pi.multiploPorAndar == ProgramacaoInicial.Multiplo.UNIDADE)
                        this.cboMultiploAndar.SelectedIndex = 0;
                    else if (pi.multiploPorAndar == ProgramacaoInicial.Multiplo.DEZENA)
                        this.cboMultiploAndar.SelectedIndex = 1;
                    else if (pi.multiploPorAndar == ProgramacaoInicial.Multiplo.CENTENA)
                        this.cboMultiploAndar.SelectedIndex = 2;
                    else if (pi.multiploPorAndar == ProgramacaoInicial.Multiplo.MILHAR)
                        this.cboMultiploAndar.SelectedIndex = 3;

                    this.cboSequenciaRamal.SelectedIndex = (int)pi.modo;

                    // Exibe a tela referente ao objeto
                    this.exibirTela(Tela.PROG_INICIAL);
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
                    else
                    {
                        // Cria o objeto que irá receber as novas definições
                        ProgramacaoInicial pi = new ProgramacaoInicial();

                        // Atualiza o objeto com os dados dos componentes
                        pi.posicao = Convert.ToInt32(this.txtIniciarNoRamal.Text);
                        pi.qtdeDeBlocos = Convert.ToInt32(this.txtQtdeBlocos.Text);
                        pi.nroPrimeiroBloco = Convert.ToInt32(this.txtQtdeBlocos.Text) == 0 ? 0 : Convert.ToInt32(this.txtNro1oBloco.Text);
                        pi.posicaoDoNumeroBloco = this.cboPosicaoNroBloco.SelectedIndex == 0 ? ProgramacaoInicial.PosicaoNroBloco.ANTES : ProgramacaoInicial.PosicaoNroBloco.DEPOIS;
                        pi.qtdeDeAndares = Convert.ToInt32(this.txtQtdeAndares.Text);
                        pi.qtdeAptoAndar = Convert.ToInt32(this.txtQtdeAptoAndar.Text);
                        pi.nroPrimeiroApto = this.txtNro1oApto.Text;
                        pi.ramalHot = this.cboDefinirRamalHot.SelectedIndex == 0 ? false : true;
                        pi.ramalRestrito = this.cboDefinirRamalRestrito.SelectedIndex == 0 ? false : true;
                        pi.ramalBloco = this.cboDefinirRamalBloco.SelectedIndex == 0 ? false : true;
                        pi.iniciarCentral2 = this.txtNroInicialCentral2.Text;
                        pi.iniciarCentral3 = this.txtNroInicialCentral3.Text;

                        if (this.cboMultiploAndar.SelectedIndex == 0)
                            pi.multiploPorAndar = ProgramacaoInicial.Multiplo.UNIDADE;
                        else if (this.cboMultiploAndar.SelectedIndex == 1)
                            pi.multiploPorAndar = ProgramacaoInicial.Multiplo.DEZENA;
                        else if (this.cboMultiploAndar.SelectedIndex == 2)
                            pi.multiploPorAndar = ProgramacaoInicial.Multiplo.CENTENA;
                        else if (this.cboMultiploAndar.SelectedIndex == 3)
                            pi.multiploPorAndar = ProgramacaoInicial.Multiplo.MILHAR;

                        pi.modo = this.cboSequenciaRamal.SelectedIndex == 0 ? ProgramacaoInicial.ModoNumeracao.NORMAL : ProgramacaoInicial.ModoNumeracao.PRUMADA;

                        // Chama o método responsável em atualiza os objetos
                        cc.calcularNumeracaoAptos(pi);

                        // Posiciona o comboBox na CENTRAL 1
                        this.cboListaCentrais.SelectedIndex = 0;

                        // Atualizar o gridDataView com os ramais da CENTRAL 1
                        this.carregarListaRamais(0);

                        // Exibe o quadro referente a lista de ramais
                        this.exibirTela(Tela.LISTA_RAMAL);

                        // Exibe a mensagem de sucesso se nenhuma exception for capturada
                        MessageBox.Show("A programação inicial foi concluida com sucesso.", "CentralCDX", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region OPERAÇÕES E EVENTOS DA TELA "LISTA RAMAIS"
        /* ------------------------------------------------------------------------------- MÉTODO OK */
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
                    this.cboListaCentrais.SelectedIndex = 0;

                    // Carrega os componentes com os dados dos objetos (0 = RAMAIS DA CENTRAL 1)
                    this.carregarListaRamais(0);

                    // Exibe a tela referente ao objeto
                    this.exibirTela(Tela.LISTA_RAMAL);
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

        /* ------------------------------------------------------------------------------- MÉTODO OK */
        /* Funcionalidade : Carrega os ramais no DataGridView de uma determinada central.    */
        /*                  0 = CENTRAL 1 / 1 = CENTRAL 2 / 2 = CENTRAL 3                    */
        /* --------------------------------------------------------------------------------- */
        private void carregarListaRamais(int central)
        {
            this.Cursor = Cursors.WaitCursor;

            // Recupera a lista de ramais
            List<Ramal> l = this.cc.buscarListaDeRamais();

            // Verifica se retornou todos os ramais
            if (l.Count == 3072)
            {
                // Encontra o primeiro ramal da central passada por paramentro
                int ramal = (((central + 1) * 1024) - 1024);

                for (int i = 0; i <= 1023; i++)
                {   
                    this.grdListaRamais["Column0", i].Value = l[ramal].key;
                    this.grdListaRamais["Column1", i].Value = false;
                    this.grdListaRamais["Column2", i].Value = l[ramal].ramal;
                    this.grdListaRamais["Column3", i].Value = l[ramal].apartamento;
                    this.grdListaRamais["Column4", i].Value = l[ramal].ramalHOT;
                    this.grdListaRamais["Column5", i].Value = l[ramal].ramalBLOCO;
                    this.grdListaRamais["Column6", i].Value = l[ramal].ramalRESTRITO;
                    this.grdListaRamais["Column7", i].Value = l[ramal].placaPortPhone;
                    this.grdListaRamais["Column8", i].Value = l[ramal].acessoLinhaExterna;
                    this.grdListaRamais["Column9", i].Value = this.cc.buscarAtendedorPorKey(l[ramal].atendedor);
                    ramal++;
                }

                // Seleciona a linha do dataGridView e define qual linha deverá aparecer primeiro no dataGridView
                this.grdListaRamais.Rows[0].Selected = true;
                this.grdListaRamais.FirstDisplayedScrollingRowIndex = 0;
            }
            else MessageBox.Show("Foi encontrada uma inconsistência na lista de ramais.", "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            this.Cursor = Cursors.Default;
        }

        /* -------------------------------------------------------------------------------MÉTODO OK */
        /* Funcionalidade : Busca os dados dos RAMAIS sempre que mudar o item do ComboBox.   */
        /* --------------------------------------------------------------------------------- */
        private void cboListaCentrais_SelectedIndexChanged(object sender, EventArgs e)
        {
            // O Panel não pode estar visível quando o ComboBox for populado
            if (this.pnlTelaListaRamais.Visible == true)
            {
                this.carregarListaRamais(this.cboListaCentrais.SelectedIndex);
                this.grdListaRamais.Focus();
            }
        }

        /* -------------------------------------------------------------------------------MÉTODO OK */
        /* Funcionalidade : Verifica se pressionado a tecla enter do TextBox (faz a busca).  */
        /* --------------------------------------------------------------------------------- */
        private void txtPesquisarRamal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.picPesquisarRamal_Click(sender, e);
        }

        /* -------------------------------------------------------------------------------MÉTODO OK */
        /* Funcionalidade : Encontra o ramal pesquisado e seleciona ele na lista.            */
        /* --------------------------------------------------------------------------------- */
        private void picPesquisarRamal_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se o número do ramal está dentro do range de ramais
                if (this.txtPesquisarRamal.Text != "" && (Convert.ToInt32(this.txtPesquisarRamal.Text) >= 1 && Convert.ToInt32(this.txtPesquisarRamal.Text) <= 3072))
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Verifica em qual central o ramal está localizado e carrega as informações dos ramais
                    int pesquisa = Convert.ToInt32(this.txtPesquisarRamal.Text);
                    if (pesquisa >= 1 && pesquisa <= 1024)
                    {
                        this.cboListaCentrais.SelectedIndex = 0;
                        pesquisa = pesquisa - 1;
                    }
                    else if (pesquisa >= 1025 && pesquisa <= 2048)
                    {
                        this.cboListaCentrais.SelectedIndex = 1;
                        pesquisa = (pesquisa - 1024) - 1;
                    }
                    else if (pesquisa >= 2049 && pesquisa <= 3072)
                    {
                        this.cboListaCentrais.SelectedIndex = 2;
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

        /* -------------------------------------------------------------------------------MÉTODO OK */
        /* Funcionalidade : Define as teclas que não podem ser usadas nos TextBox do grid.   */
        /*                  O evento verifica se o dataGridView sofreu modificação.          */
        /* --------------------------------------------------------------------------------- */
        private void grdListaRamais_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Verifica se as modificações são referente as coluna Número ou Atendedor
            if (this.grdListaRamais.CurrentCell.ColumnIndex == 2 || this.grdListaRamais.CurrentCell.ColumnIndex == 8)
            {
                // Define o método para o evento KeyPress dos TextBox do DataGridView
                DataGridViewTextBoxEditingControl textBox = e.Control as DataGridViewTextBoxEditingControl;
                if (textBox != null)
                {
                    textBox.KeyPress -= new KeyPressEventHandler(this.controlApartamento_KeyPress);
                    textBox.KeyPress += new KeyPressEventHandler(this.controlApartamento_KeyPress);
                }
            }
        }

        /* -------------------------------------------------------------------------------MÉTODO OK */
        /* Funcionalidade : Descarta as modificações realizadas (Reload no dataGridView).    */
        /* --------------------------------------------------------------------------------- */
        private void btnAtualizarGrid_Click(object sender, EventArgs e)
        {
            this.carregarListaRamais(this.cboListaCentrais.SelectedIndex);
        }

        /* -------------------------------------------------------------------------------MÉTODO OK */
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
                    this.txtNumeroAtendedor.Text = "";
                    
                    // Exibe a tela referente ao objeto
                    this.exibirTela(Tela.NOVO_RAMAL);
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
                for (int i = 1; i <= 1024; i++)
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
                    //cc.modificarListRamal(r, index);
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

        /* ---------------------------------------------------------------------------------VISTO */
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
                        //this.cc.removerRamais(excluidos);

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

        
        #endregion


        #region OPERAÇÕES E EVENTOS DA TELA "NOVO RAMAL"
        /* ---------------------------------------------------------------------------------MÉTODO OK */
        /* Funcionalidade : Volta para tela da lista de ramais.                              */
        /* --------------------------------------------------------------------------------- */
        private void btnVoltarListaRamais_Click(object sender, EventArgs e)
        {
            try
            {
                // Exibe a tela referente ao objeto
                this.exibirTela(Tela.LISTA_RAMAL);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Ops! Ocorreu um erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* ---------------------------------------------------------------------------------MÉTODO OK */
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
                    this.cc.incluirNovoRamal(novo, Convert.ToInt32(this.txtPosicaoNovoRamal.Text));

                    // Atualizar o gridDataView com os ramais da CENTRAL 1
                    this.carregarListaRamais(0);

                    // Posiciona o comboBox na CENTRAL 1
                    this.cboListaCentrais.SelectedIndex = 0;

                    // Exibe a tela referente ao objeto
                    this.exibirTela(Tela.LISTA_RAMAL);

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
        #endregion












    }
}