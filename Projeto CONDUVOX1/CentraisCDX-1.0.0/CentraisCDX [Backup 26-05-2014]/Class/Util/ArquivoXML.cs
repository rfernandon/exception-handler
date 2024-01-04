/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla a entrada e saída (Input/Output) da programação
 *                          nos arquivos XML.
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
using System.Xml;
using System.Collections;
using CentraisCDX.Class.Model;
using System.Collections.Generic;

namespace CentraisCDX.Class.Util
{
    class ArquivoXML
    {
        #region OPERAÇÃO ABRIR
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Atualiza os objetos conforme o arquivo XML escolhido.            */
        /* --------------------------------------------------------------------------------- */
        public Central carregarDadosArquivoXML(Object objeto)
        {
            // Instância da classe utilizada no método
            Central c = new Central();
            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList xmlNodeList;

            // Verifica se foi passado um objeto do tipo String ou Stream
            if (objeto.GetType() == typeof(String))
            {
                xmlDocument.Load((objeto as String));
            }
            else xmlDocument.Load((objeto as Stream));

            xmlNodeList = xmlDocument.GetElementsByTagName("central");
            foreach (XmlNode xn in xmlNodeList)
            {
                string id = xn..Attributes["Id"].ToString();
            }
            /*
            // Verifica se é um arquivo de programação
            escritor.WriteStartElement("central");
            escritor.WriteAttributeString("Id", "CDX-1.0.0");
            escritor.WriteAttributeString("Empresa", "Conduvox");
            escritor.WriteAttributeString("Software", "Centrais telefônicas CDX");

            
             foreach(XElement x in xml.Elements())
    {
        Pessoa p = new Pessoa()
        {
            codigo = int.Parse(x.Attribute("codigo").Value),
            nome = x.Attribute("nome").Value,
            telefone = x.Attribute("telefone").Value
        };
        pessoas.Add(p);
    }

             * 
             */


            // #################### RECUPERA OS ELEMENTOS DA TAG "PROGRAMACAOINICIAL"
            xmlNodeList = xmlDocument.GetElementsByTagName("programacao_inicial");
            ProgramacaoInicial pi = new ProgramacaoInicial();
            foreach (XmlNode xn in xmlNodeList)
            {
                pi.posicao = Convert.ToInt32(xn["posicao"].InnerText);
                pi.qtdeDeBlocos = Convert.ToInt32(xn["qtde_blocos"].InnerText);
                pi.nroPrimeiroBloco = Convert.ToInt32(xn["nro_primeiro_bloco"].InnerText);
                pi.posicaoDoNumeroBloco = (posicaoNroBloco)Enum.Parse(typeof(posicaoNroBloco), xn["posicao_numero_bloco"].InnerText);
                pi.qtdeDeAndares = Convert.ToInt32(xn["qtde_andares"].InnerText);
                pi.multiploPorAndar = Convert.ToInt32(xn["multiplo_andar"].InnerText);
                pi.qtdeAptoAndar = Convert.ToInt32(xn["qtde_apto_andar"].InnerText);
                pi.nroPrimeiroApto = xn["nro_primeiro_apto"].InnerText;
                pi.ramalHot = Convert.ToBoolean(xn["ramal_hot"].InnerText);
                pi.ramalRestrito = Convert.ToBoolean(xn["ramal_restrito"].InnerText);
                pi.ramalBloco = Convert.ToBoolean(xn["ramal_bloco"].InnerText);
                pi.iniciarCentral2 = xn["iniciar_central_2"].InnerText;
                pi.iniciarCentral3 = xn["iniciar_central_3"].InnerText;
                pi.modo = (modoNumeracao)Enum.Parse(typeof(modoNumeracao), xn["modo_sequencia"].InnerText);
            }
            c.programacaoInicial = pi;
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "RAMAL"
            xmlNodeList = xmlDocument.GetElementsByTagName("ramal");
            List<Ramal> ramais = new List<Ramal>();
            foreach (XmlNode xn in xmlNodeList)
            {
                Ramal r = new Ramal();
                r.id_ramal = xn["id_ramal"].InnerText;
                r.ramal = xn["nro_ramal"].InnerText;
                r.apartamento = xn["numero"].InnerText;
                r.ramalHOT = Convert.ToBoolean(xn["ramal_hot"].InnerText);
                r.ramalBLOCO = Convert.ToBoolean(xn["ramal_bloco"].InnerText);
                r.ramalRESTRITO = Convert.ToBoolean(xn["ramal_restrito"].InnerText);
                r.placaPortPhone = Convert.ToBoolean(xn["port_phone"].InnerText);
                r.acessoLinhaExterna = Convert.ToBoolean(xn["acesso_linha"].InnerText);
                r.atendedor = xn["atendedor"].InnerText;
                ramais.Add(r);
            }
            c.listRamal = ramais;
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "TRONCO"
            xmlNodeList = xmlDocument.GetElementsByTagName("tronco");
            int id_tronco = 0;
            foreach (XmlNode xn in xmlNodeList)
            {
                Tronco t = new Tronco();
                t.estado = Convert.ToBoolean(xn["habilitado"].InnerText);
                t.estadoChamadaCobrar = Convert.ToBoolean(xn["chamada_cobrar"].InnerText);
                t.atendedor = xn["atendedor"].InnerText;
                c.linhaExterna.listTronco[id_tronco] = t;
                id_tronco++;
            }
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "NUMERO_LIBERADO"
            xmlNodeList = xmlDocument.GetElementsByTagName("numero_liberado");
            int id_liberado = 0;
            foreach (XmlNode xn in xmlNodeList)
            {
                c.linhaExterna.listNumeroLiberado[id_liberado] = xn.InnerText;
                id_liberado++;
            }
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "NUMERO_LIBERADO"
            xmlNodeList = xmlDocument.GetElementsByTagName("numero_bloqueado");
            int id_bloqueado = 0;
            foreach (XmlNode xn in xmlNodeList)
            {
                c.linhaExterna.listNumeroBloqueado[id_bloqueado] = xn.InnerText;
                id_bloqueado++;
            }
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "VIDEO"
            xmlNodeList = xmlDocument.GetElementsByTagName("video");
            List<Video> video = new List<Video>();
            foreach (XmlNode xn in xmlNodeList)
            {
                Video v = new Video();
                v.estado = Convert.ToBoolean(xn["estado"].InnerText);
                v.numero = xn["numero"].InnerText;
                video.Add(v);
            }
            c.listVideo = video;
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "MESA_OPERADORA"
            xmlNodeList = xmlDocument.GetElementsByTagName("mesa");
            List<MesaOperadora> mesa = new List<MesaOperadora>();
            foreach (XmlNode xn in xmlNodeList)
            {
                MesaOperadora m = new MesaOperadora();
                m.id_mesa = xn["id_mesa"].InnerText;
                m.numero = xn["numero_mesa"].InnerText;

                m.definirEstadoTecla(nome.ZELADOR, (estado)Enum.Parse(typeof(estado), xn["tecla_zelador"].InnerText));
                m.definirEstadoTecla(nome.SINDICO, (estado)Enum.Parse(typeof(estado), xn["tecla_sindico"].InnerText));
                m.definirEstadoTecla(nome.A1, (estado)Enum.Parse(typeof(estado), xn["tecla_a1"].InnerText));
                m.definirEstadoTecla(nome.A2, (estado)Enum.Parse(typeof(estado), xn["tecla_a2"].InnerText));
                m.definirEstadoTecla(nome.A3, (estado)Enum.Parse(typeof(estado), xn["tecla_a3"].InnerText));
                m.definirEstadoTecla(nome.PORTEIRO, (estado)Enum.Parse(typeof(estado), xn["tecla_porteiro"].InnerText));
                m.definirEstadoTecla(nome.TELEFONE, (estado)Enum.Parse(typeof(estado), xn["tecla_telefone"].InnerText));
                m.definirEstadoTecla(nome.FECH1, (estado)Enum.Parse(typeof(estado), xn["tecla_fech1"].InnerText));
                m.definirEstadoTecla(nome.FECH2, (estado)Enum.Parse(typeof(estado), xn["tecla_fech2"].InnerText));

                m.definirAtendedorTecla(nome.ZELADOR, xn["numero_zelador"].InnerText);
                m.definirAtendedorTecla(nome.SINDICO, xn["numero_sindico"].InnerText);
                m.definirAtendedorTecla(nome.A1, xn["numero_a1"].InnerText);
                m.definirAtendedorTecla(nome.A2, xn["numero_a2"].InnerText);
                m.definirAtendedorTecla(nome.A3, xn["numero_a3"].InnerText);
                m.definirAtendedorTecla(nome.PORTEIRO, xn["numero_porteiro"].InnerText);
                m.definirAtendedorTecla(nome.TELEFONE, xn["numero_telefone"].InnerText);
                m.definirAtendedorTecla(nome.FECH1, xn["numero_fech1"].InnerText);
                m.definirAtendedorTecla(nome.FECH2, xn["numero_fech2"].InnerText);

                mesa.Add(m);
            }
            c.listMesaOperadora = mesa;
            // #################### FIM

            // #################### RECUPERA OS ELEMENTOS DA TAG "ALARME"
            xmlNodeList = xmlDocument.GetElementsByTagName("alarme");
            foreach (XmlNode xn in xmlNodeList)
            {
                c.alarme.numero = xn["numero"].InnerText;
                c.alarme.tempo = xn["tempo"].InnerText;
                ArrayList l = new ArrayList();
                for (int i = 1; i <= 10; i++)
                    l.Add(xn["atendedor" + i].InnerText);

                c.alarme.listAtendedores = l;
            }
            // #################### FIM
            return c;
        }
        #endregion

        #region OPERAÇÃO SALVAR
        /* --------------------------------------------------------------------------------- */
        /* Funcionalidade : Grava o estado dos objetos no arquivo XML escolhido.             */
        /* --------------------------------------------------------------------------------- */
        public void salvarDadosArquivoXML(Central central, String caminho)
        {
            // Abre (ou criando) o documento XML para escrita 
            XmlTextWriter escritor = new XmlTextWriter(caminho, System.Text.Encoding.UTF8);

            // Define a formatação do documento (indentação) 
            escritor.Formatting = Formatting.Indented;

            // inicia a escrita do documento XML
            escritor.WriteStartDocument();

            // Define o elemento inicial
            escritor.WriteStartElement("central");
            escritor.WriteAttributeString("Id", "CDX-1.0.0");
            escritor.WriteAttributeString("Empresa", "Conduvox");
            escritor.WriteAttributeString("Software", "Centrais telefônicas CDX");

            // #################### DEFINE O ELEMENTO DO OBJETO "PROGRAMACAOINICIAL"
            escritor.WriteStartElement("programacao_inicial");
            escritor.WriteElementString("posicao", central.programacaoInicial.posicao.ToString());
            escritor.WriteElementString("qtde_blocos", central.programacaoInicial.qtdeDeBlocos.ToString());
            escritor.WriteElementString("nro_primeiro_bloco", central.programacaoInicial.nroPrimeiroBloco.ToString());
            escritor.WriteElementString("posicao_numero_bloco", central.programacaoInicial.posicaoDoNumeroBloco.ToString());
            escritor.WriteElementString("qtde_andares", central.programacaoInicial.qtdeDeAndares.ToString());
            escritor.WriteElementString("multiplo_andar", central.programacaoInicial.multiploPorAndar.ToString());
            escritor.WriteElementString("qtde_apto_andar", central.programacaoInicial.qtdeAptoAndar.ToString());
            escritor.WriteElementString("nro_primeiro_apto", central.programacaoInicial.nroPrimeiroApto);
            escritor.WriteElementString("ramal_hot", central.programacaoInicial.ramalHot.ToString());
            escritor.WriteElementString("ramal_restrito", central.programacaoInicial.ramalRestrito.ToString());
            escritor.WriteElementString("ramal_bloco", central.programacaoInicial.ramalBloco.ToString());
            escritor.WriteElementString("iniciar_central_2", central.programacaoInicial.iniciarCentral2);
            escritor.WriteElementString("iniciar_central_3", central.programacaoInicial.iniciarCentral3);
            escritor.WriteElementString("modo_sequencia", central.programacaoInicial.modo.ToString());
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "RAMAL"
            escritor.WriteStartElement("ramais");
            for (int i = 1; i <= 3072; i++)
            {
                escritor.WriteStartElement("ramal");
                escritor.WriteElementString("id_ramal", central.listRamal[i - 1].id_ramal);
                escritor.WriteElementString("nro_ramal", central.listRamal[i - 1].ramal);
                escritor.WriteElementString("numero", central.listRamal[i - 1].apartamento);
                escritor.WriteElementString("ramal_hot", central.listRamal[i - 1].ramalHOT.ToString());
                escritor.WriteElementString("ramal_bloco", central.listRamal[i - 1].ramalBLOCO.ToString());
                escritor.WriteElementString("ramal_restrito", central.listRamal[i - 1].ramalRESTRITO.ToString());
                escritor.WriteElementString("port_phone", central.listRamal[i - 1].placaPortPhone.ToString());
                escritor.WriteElementString("acesso_linha", central.listRamal[i - 1].acessoLinhaExterna.ToString());
                escritor.WriteElementString("atendedor", central.listRamal[i - 1].atendedor);
                escritor.WriteEndElement();
            }
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "TRONCO"
            escritor.WriteStartElement("troncos");
            for (int i = 1; i <= 2; i++)
            {
                escritor.WriteStartElement("tronco");
                escritor.WriteElementString("habilitado", central.linhaExterna.listTronco[i - 1].estado.ToString());
                escritor.WriteElementString("chamada_cobrar", central.linhaExterna.listTronco[i - 1].estadoChamadaCobrar.ToString());
                escritor.WriteElementString("atendedor", central.linhaExterna.listTronco[i - 1].atendedor);
                escritor.WriteEndElement();
            }
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "NUMERO_LIBERADO"
            escritor.WriteStartElement("numeros_liberados");
            for (int i = 1; i <= 20; i++)
                escritor.WriteElementString("numero_liberado", central.linhaExterna.listNumeroLiberado[i-1].ToString());
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "NUMERO_BLOQUEADOS"
            escritor.WriteStartElement("numeros_bloqueados");
            for (int i = 1; i <= 20; i++)
                escritor.WriteElementString("numero_bloqueado", central.linhaExterna.listNumeroBloqueado[i-1].ToString());
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "VIDEO"
            escritor.WriteStartElement("videos");
            for (int i = 1; i <= 8; i++)
            {
                escritor.WriteStartElement("video");
                escritor.WriteElementString("estado", central.listVideo[i - 1].estado.ToString());
                escritor.WriteElementString("numero", central.listVideo[i - 1].numero);
                escritor.WriteEndElement();
            }
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "MESA_OPERADORA"
            escritor.WriteStartElement("mesa_operadora");
            for (int i = 1; i <= 8; i++)
            {
                escritor.WriteStartElement("mesa");
                escritor.WriteElementString("id_mesa", central.listMesaOperadora[i - 1].id_mesa);
                escritor.WriteElementString("numero_mesa", central.listMesaOperadora[i - 1].numero);

                escritor.WriteElementString("tecla_zelador", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.ZELADOR).ToString());
                escritor.WriteElementString("tecla_sindico", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.SINDICO).ToString());
                escritor.WriteElementString("tecla_a1", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.A1).ToString());
                escritor.WriteElementString("tecla_a2", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.A2).ToString());
                escritor.WriteElementString("tecla_a3", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.A3).ToString());
                escritor.WriteElementString("tecla_porteiro", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.PORTEIRO).ToString());
                escritor.WriteElementString("tecla_telefone", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.TELEFONE).ToString());
                escritor.WriteElementString("tecla_fech1", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.FECH1).ToString());
                escritor.WriteElementString("tecla_fech2", central.listMesaOperadora[i - 1].buscarPorNomeEstadoTecla(nome.FECH2).ToString());

                escritor.WriteElementString("numero_zelador", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.ZELADOR));
                escritor.WriteElementString("numero_sindico", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.SINDICO));
                escritor.WriteElementString("numero_a1", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.A1));
                escritor.WriteElementString("numero_a2", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.A2));
                escritor.WriteElementString("numero_a3", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.A3));
                escritor.WriteElementString("numero_porteiro", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.PORTEIRO));
                escritor.WriteElementString("numero_telefone", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.TELEFONE));
                escritor.WriteElementString("numero_fech1", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.FECH1));
                escritor.WriteElementString("numero_fech2", central.listMesaOperadora[i - 1].buscarPorNomeAtendedorTecla(nome.FECH2));
                escritor.WriteEndElement();
            }
            escritor.WriteEndElement();
            // #################### FIM

            // #################### DEFINE O ELEMENTO DO OBJETO "ALARME"
            escritor.WriteStartElement("alarme");
            escritor.WriteElementString("numero", central.alarme.numero);
            escritor.WriteElementString("tempo", central.alarme.tempo);
            for (int i = 1; i <= 10; i++)
            {
                escritor.WriteElementString("atendedor" + i, central.alarme.listAtendedores[i - 1].ToString());
            }
            escritor.WriteEndElement();
            // #################### FIM

            // Fim do elemento inicial
            escritor.WriteEndElement();

            // Fecha o documento XML
            escritor.Close();
        }
        #endregion
    }
}