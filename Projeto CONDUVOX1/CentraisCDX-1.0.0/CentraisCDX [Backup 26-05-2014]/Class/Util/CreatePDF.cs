using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using CentraisCDX.Class.Model;
using System.IO;

namespace CentraisCDX.Class.Util
{
    class CreatePDF
    {
        private Central central;

        public void criarPDFProgramacao(Central c, string caminho)
        {
            central = c;

            // Define as fontes do relatório
            Font fonteSlogan = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font fonteTitulo = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font fonteTituloProg = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font fonteValorProg = new Font(Font.FontFamily.COURIER, 8, (int)System.Drawing.FontStyle.Regular);

            // Define a imagem do titulo do relatório
            // Define a imagem para o cabeçalho
            System.Drawing.Image imagem;
            imagem = new System.Drawing.Bitmap(global::CentraisCDX.Properties.Resources.logo_conduvox);
            iTextSharp.text.Image imagemPDF = iTextSharp.text.Image.GetInstance(imagem, System.Drawing.Imaging.ImageFormat.Png);
            imagemPDF.Alignment = iTextSharp.text.Image.ALIGN_CENTER | iTextSharp.text.Image.TOP_BORDER;
            imagemPDF.ScalePercent(10);

            // Define o paragrafo do titulo do relatório
            Paragraph slogan = new Paragraph("TECNOLOGIA EM COMUNICAÇÃO", fonteSlogan);
            Paragraph titulo = new Paragraph("Relatório de Programação das Centrais CDX", fonteTitulo);
            slogan.Alignment = 1;
            titulo.Alignment = 1;
            Paragraph line = new Paragraph(new Chunk(new LineSeparator(0.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

            try
            {
                // cria uma instância de iTextSharp.text.Document
                Document documento = new Document(PageSize.A4, 30, 30, 15, 35);

                // cria um Writer para o documento
                PdfWriter writer = PdfWriter.GetInstance(documento, new FileStream(caminho, FileMode.Create));

                // abre o documento
                documento.Open();

                // Adiciona o topo do relatório
                documento.Add(imagemPDF);
                documento.Add(slogan);
                documento.Add(titulo);
                documento.Add(line);

                // #################### ESCREVE A PROGRAMAÇÃO DAS LINHAS EXTERNAS
                documento.Add(new Paragraph("\nDefinições da Programação das Linhas Externas", fonteTituloProg));
                documento.Add(new Paragraph(this.getRelatorioLExterna(), fonteValorProg));

                // #################### ESCREVE A PROGRAMAÇÃO DO ALARME
                documento.Add(new Paragraph("\n\nDefinições da Programação do Alarme", fonteTituloProg));
                documento.Add(new Paragraph(this.getRelatorioAlarme(), fonteValorProg));

                // #################### ESCREVE A PROGRAMAÇÃO DO VÍDEO
                documento.Add(new Paragraph("\n\nDefinições da Programação do Vídeo", fonteTituloProg));
                documento.Add(new Paragraph(this.getRelatorioVideo(), fonteValorProg));

                // QUEBRA DE PÁGINA
                documento.NewPage();

                // #################### ESCREVE A PROGRAMAÇÃO INICIAL
                documento.Add(new Paragraph("Definições da Programação Inicial", fonteTituloProg));
                documento.Add(new Paragraph(this.getRelatorioPInicial(), fonteValorProg));

                // #################### ESCREVE A PROGRAMAÇÃO DOS RAMAIS
                documento.Add(new Paragraph("\n\nDefinições da Programação dos Ramais\n\n", fonteTituloProg));
                foreach (Ramal r in central.listRamal)
                    documento.Add(new Paragraph(this.getRelatorioRamais(r), fonteValorProg));

                // QUEBRA DE PÁGINA
                documento.NewPage();

                // #################### ESCREVE A PROGRAMAÇÃO INICIAL
                documento.Add(new Paragraph("Definições da Programação da Mesa Operadora", fonteTituloProg));
                documento.Add(new Paragraph(this.getRelatorioMOperadora(), fonteValorProg));

                // fecha o documento
                documento.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o relatório referente a programação da linha externa.    */
        /* --------------------------------------------------------------------------------- */
        private string getRelatorioLExterna()
        {
            string result = "\n";
            int x = 1;
            foreach (Tronco t in central.linhaExterna.listTronco)
            {
                string estado = t.estado == false ? "OFF" : "ON ";
                string estadoChamadaCobrar = t.estadoChamadaCobrar == false ? "OFF" : "ON ";
                string atendedor = buscarPorChaveNumero(t.atendedor);
                result += "Tronco: " + x + " | ";
                result += "Estado: " + estado + " | ";
                result += "Chamada a Cobrar: " + estadoChamadaCobrar + " | ";
                result += "Atendedor: " + atendedor.PadLeft(8, ' ');
                result += "\n";
                x++;
            }
            result += "\n";
            for (int i = 0; i < 20; i++)
            {
                string lib = central.linhaExterna.listNumeroLiberado[i].ToString();
                string blo = central.linhaExterna.listNumeroBloqueado[i].ToString();

                if (lib == "") lib = "VAZIO";
                if (blo == "") blo = "VAZIO";

                result += "Número Liberado " + (i + 1).ToString("00") + ": " + lib.PadLeft(20, ' ') + " | ";
                result += "Número Bloqueado " + (i + 1).ToString("00") + ": " + blo.PadLeft(20, ' ');
                result += "\n";
            }
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o relatório referente a programação do alarme.           */
        /* --------------------------------------------------------------------------------- */
        private string getRelatorioAlarme()
        {
            string result = "\n";
            string al = buscarPorChaveNumero(central.alarme.numero);

            if (al == "") al = "VAZIO";

            result += "Número do Alarme: " + al.PadLeft(8, ' ');
            result += "\nTempo           : " + central.alarme.tempo.PadLeft(8, ' ');
            result += "\n";
            result += "\n";

            for (int i = 1; i <= 10; i++)
            {
                string at = buscarPorChaveNumero(central.alarme.listAtendedores[i - 1].ToString());

                if (at == "") at = "VAZIO";

                result += "Atendedor       : " + at.PadLeft(8, ' ');
                result += "\n";
            }
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o relatório referente a programação do vídeo.            */
        /* --------------------------------------------------------------------------------- */
        private string getRelatorioVideo()
        {
            string result = "\n";

            for (int i = 1; i <= 8; i++)
            {
                string status = "";
                if (central.listVideo[i - 1].estado)
                    status = " ON";
                else status = "OFF";
                string nro = buscarPorChaveNumero(central.listVideo[i - 1].numero);

                if (nro == "") nro = "VAZIO";

                result += "Vídeo: " + i + " | Estado: " + status + " | Número: " + nro.PadLeft(8, ' ');
                result += "\n";
            }
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o relatório referente a programação inicial.             */
        /* --------------------------------------------------------------------------------- */
        private string getRelatorioPInicial()
        {
            string result = "";
            ProgramacaoInicial pi = central.programacaoInicial;
            string result_ramalHot = pi.ramalHot == false ? "OFF" : " ON";
            string result_ramalRestrito = pi.ramalRestrito == false ? "OFF" : " ON";
            string result_ramalBloco = pi.ramalBloco == false ? "OFF" : " ON";
            string result_iniciarCentral2 = pi.iniciarCentral2 == "" ? "VAZIO" : pi.iniciarCentral2;
            string result_iniciarCentral3 = pi.iniciarCentral3 == "" ? "VAZIO" : pi.iniciarCentral3;
            result += "\nPosição                   : " + pi.posicao;
            result += "\nQuantidade de Blocos      : " + pi.qtdeDeBlocos;
            result += "\nNúmero Primeiro Blocos    : " + pi.nroPrimeiroBloco;
            result += "\nPosição Número Blocos     : " + pi.posicaoDoNumeroBloco;
            result += "\nQuantidade de Andares     : " + pi.qtdeDeAndares;
            result += "\nMultiplicador do Andar    : " + pi.multiploPorAndar;
            result += "\nQuantidade APTO por Andar : " + pi.qtdeAptoAndar;
            result += "\nNúmero do Primeiro APTO   : " + pi.nroPrimeiroApto;
            result += "\nStatus Ramal HOT          : " + result_ramalHot;
            result += "\nStatus Ramal Restrito     : " + result_ramalRestrito;
            result += "\nStatus Ramal Bloco        : " + result_ramalBloco;
            result += "\nIniciar na Centrao 2      : " + result_iniciarCentral2;
            result += "\nIniciar na Centrao 3      : " + result_iniciarCentral3;
            result += "\nModo de Numeração         : " + pi.modo;
            result += "\n";
            return result;
        }
        
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o relatório referente a programação dos ramais.          */
        /* --------------------------------------------------------------------------------- */
        private string getRelatorioRamais(Ramal r)
        {
            string result = "";
            string ramal = Convert.ToInt32(r.ramal).ToString("0000");
            string apartamento = r.apartamento.PadLeft(8, ' ');
            string ramalHOT = r.ramalHOT == false ? "OFF" : " ON";
            string ramalBLOCO = r.ramalBLOCO == false ? "OFF" : " ON";
            string ramalRESTRITO = r.ramalRESTRITO == false ? "OFF" : " ON";
            string placaPortPhone = r.placaPortPhone == false ? "OFF" : " ON";
            string acessoLinhaExterna = r.acessoLinhaExterna == false ? "OFF" : " ON";
            string atendedor = buscarPorChaveNumero(r.atendedor).PadLeft(8, ' ');

            result += "R: " + ramal + " | ";
            result += "Apto: " + apartamento + " | ";
            result += "R.HOT: " + ramalHOT + " | ";
            result += "R.BL: " + ramalBLOCO + " | ";
            result += "R.Restr: " + ramalRESTRITO + " | ";
            result += "P.PHONE: " + placaPortPhone + " | ";
            result += "L.Ext: " + acessoLinhaExterna + " | ";
            result += "Atend: " + atendedor;
            result += "\n";
            return result;
        }

        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Retorna o relatório referente a programação da mesa operadora.   */
        /* --------------------------------------------------------------------------------- */
        private string getRelatorioMOperadora()
        {
            string result = "";
            List<nome> teclas = new List<nome> { nome.ZELADOR, nome.SINDICO, nome.A1, nome.A2, nome.A3, nome.PORTEIRO, nome.TELEFONE, nome.FECH1, nome.FECH2 };
            for (int m = 0; m < 8; m++)
            {
                result += "\nMesa           : " + (m + 1);
                result += "\nNúmero da Mesa : " + central.listMesaOperadora[m].numero.PadLeft(10, ' ');
                for (int i = 0; i < 9; i++)
                {
                    nome tecla = teclas[i];
                    string t = tecla.ToString().PadLeft(8, ' ');
                    string e = central.listMesaOperadora[m].buscarPorNomeEstadoTecla(tecla).ToString().PadLeft(10, ' ');
                    string a = central.listMesaOperadora[m].buscarPorNomeAtendedorTecla(tecla);

                    if (a == "") a = "VAZIO";

                    result += "\nTecla " + t + " : " + e + " | ";
                    result += "Atendedor Tecla : " + a.PadLeft(20, ' ');
                }
                result += "\n";
                if (m == 4) result += "\n\n\n";
            }
            return result;
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
        private string buscarPorIdNumeroRamal(string chave)
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
        private string buscarPorIdNumeroMesa(string chave)
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