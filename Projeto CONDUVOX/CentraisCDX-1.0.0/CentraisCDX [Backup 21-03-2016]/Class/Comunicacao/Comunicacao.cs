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

namespace CentraisCDX.Class.Comunicacao
{
    class Comunicacao
    {
        private SerialPort serial = new SerialPort();

        private string _dadosRecebidos = "";

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Construtor da classe.                                            */
        /*                  No momento da instância da classe deve-se passar a porta COM.    */
        /* --------------------------------------------------------------------------------- */
        public Comunicacao(string porta)
        {
            this.serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);

            this.serial.PortName = porta;
            this.serial.BaudRate = 9600;
            this.serial.DataBits = 8;
            this.serial.StopBits = StopBits.One;
            this.serial.Parity = Parity.None;
            this.serial.ReadBufferSize = 12288;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Abre uma determinada porta COM.                                  */
        /* --------------------------------------------------------------------------------- */
        public void abrirPorta()
        {
            if (!this.serial.IsOpen)
                this.serial.Open();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Fecha a porta COM.                                               */
        /* --------------------------------------------------------------------------------- */
        public void fecharPorta()
        {
            if (this.serial.IsOpen)
                this.serial.Close();
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Envia o comando para a central pela porta COM.                   */
        /* --------------------------------------------------------------------------------- */
        public void sendComando(string comando)
        {
            // Converte a string para um array de byte
            byte[] data = this.HexStringToByteArray(comando);

            // Escreve os dados binários na porta COM
            this.serial.Write(data, 0, data.Length);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Monitora o recebimento de dados da porta COM aberta.             */
        /* --------------------------------------------------------------------------------- */
        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Obtem o número de bytes do buffer da porta COM
            int intQtdeBytes = serial.BytesToRead;

            // Cria uma matriz de bytes para armazenar os dados de entrada
            byte[] buffer = new byte[intQtdeBytes];

            // Ler os dados e armazena no buffer
            serial.Read(buffer, 0, intQtdeBytes);

            // Atribui o que foi recebido pela porta COM
            this._dadosRecebidos = this.ByteArrayToHexString(buffer);
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Converter uma string para um array de byte.                      */
        /* --------------------------------------------------------------------------------- */
        private byte[] HexStringToByteArray(string dataString)
        {
            byte[] buffer = new byte[dataString.Length / 2];
            for (int i = 0; i < dataString.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(dataString.Substring(i, 2), 16);
            return buffer;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Converter um array de byte para string.                          */
        /* --------------------------------------------------------------------------------- */
        private string ByteArrayToHexString(byte[] dataByte)
        {
            StringBuilder sb = new StringBuilder(dataByte.Length * 3);
            foreach (byte b in dataByte)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
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