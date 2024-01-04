/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla a entrada e saída (Input/Output) da programação
 *                          pela porta serial (COM).
 * - Versão do arquivo    : 01
 * - Data criação         : 07/05/2014
 * - Data alteração       : 07/05/2014
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
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace CentraisCDX.Class.Comunicacao
{
    class ComunicacaoV01
    {
        private string portName = "";

        private string _dadosRecebidos = "";

        string lixo;

        // The main control for communicating through the RS-232 port
        private SerialPort comport = new SerialPort();

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Construtor da classe.                                            */
        /*                  No momento da instância da classe deve-se passar a porta COM.    */
        /* --------------------------------------------------------------------------------- */
        public ComunicacaoV01(string porta)
        {
            this.portName = porta;

            // When data is recieved through the port, call this method
            comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Abre uma determinada porta COM.                                  */
        /* --------------------------------------------------------------------------------- */
        public void abrirPorta()
        {
            if (!comport.IsOpen)
            {
                // Set the port's settings
                comport.BaudRate = 9600;
                comport.DataBits = 8;
                comport.StopBits = StopBits.One;
                comport.Parity = Parity.None;
                comport.PortName = portName;

                // Open the port
                comport.Open();
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Fecha a porta COM.                                               */
        /* --------------------------------------------------------------------------------- */
        public void fecharPorta()
        {
            if (comport.IsOpen)
            {
                comport.Close();
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia o comando para a central pela porta COM.                   */
        /* --------------------------------------------------------------------------------- */
        public void sendComando(string comando)
        {
            // Inicializa o atributo que irá receber os bits
            this._dadosRecebidos = "";

            // Converte a string para um array de byte
            byte[] data = HexStringToByteArray(comando);

            // Escreve os dados binários na porta COM
            comport.Write(data, 0, data.Length);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Monitora o recebimento de dados da porta COM aberta.             */
        /* --------------------------------------------------------------------------------- */
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Obtem o número de bytes do buffer da porta COM
            int bytes = comport.BytesToRead;

            // Cria uma matriz de bytes para armazenar os dados de entrada
            byte[] buffer = new byte[bytes];

            // Ler os dados e armazena no buffer
            comport.Read(buffer, 0, bytes);

            // Atribui o que foi recebido pela porta COM (lê bit por bit que está sendo recebido)
            this._dadosRecebidos += this.ByteArrayToHexString(buffer);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Converter uma string para um array de byte.                      */
        /* --------------------------------------------------------------------------------- */
        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Converter um array de byte para string.                          */
        /* --------------------------------------------------------------------------------- */
        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Metodos getter e setter.                                         */
        /* --------------------------------------------------------------------------------- */
        public string dadosRecebidos
        {
            get { return _dadosRecebidos; }
            set { _dadosRecebidos = value; }
        }
    }
}