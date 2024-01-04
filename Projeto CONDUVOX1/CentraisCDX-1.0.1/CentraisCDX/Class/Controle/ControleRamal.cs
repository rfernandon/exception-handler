/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla o estado do ramal.
 * - Versão do arquivo    : 01
 * - Data criação         : 02/10/2014
 * - Data alteração       : 02/10/2014
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
using System.Collections;
using System.Collections.Generic;
using CentraisCDX.Class.Modelo;
using CentraisCDX.Class.Utils;
using System.Text.RegularExpressions;

namespace CentraisCDX.Class.Controle
{
    class ControleRamal
    {
        // INSTÂNCIA DO MODELO
        private static List<Ramal> ramais = new List<Ramal>();
        
        // CONSTRUTOR
        public static void inicializar()
        {
            // INICIALIZA O ESTADO DO OBJETO (MODELO)
            for (int i = 1; i <= 3072; i++)
            {
                string key = "R" + i;

                Ramal r = new Ramal();
                r.key = key;
                r.ramal = i.ToString();
                r.apartamento = i.ToString();
                r.ramalHOT = false;
                r.ramalBLOCO = false;
                r.ramalRESTRITO = false;
                r.placaPortPhone = false;
                r.acessoLinhaExterna = false;
                r.atendedor = "M1";
                ControleRamal.ramais.Add(r);
            }

            // GUARDA O PRÓXIMO KEY
            Ramal.proximoNroKEY = 3073;
        }

        // OPERAÇÕES
        public static List<Ramal> buscarListaDeRamais()
        {
            return ControleRamal.ramais;
        }

        public static string buscarApartamentoPorKey(string key)
        {
            string result = null;
            foreach (Ramal r in ControleRamal.ramais)
            {
                if (r.key.Equals(key))
                {
                    result = r.apartamento;
                    break;
                }
            }
            return result;
        }

        public static bool verificarExistenciaApto(string apto)
        {
            bool result = false;
            foreach (Ramal r in ControleRamal.ramais)
            {
                if (r.apartamento.Equals(apto))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static void incluirNovoRamal(Ramal ramal, int posicao)
        {
            String result = ControleRamal.validarRamal(ramal);

            // Valida os dados do ramal (lança uma Exception se algo estiver errado)
            if (result.Equals("SUCESS"))
            {
                // Define uma chave para o ramal e guarda a próxima chava
                ramal.key = "R" + Ramal.proximoNroKEY++;

                // Incluir a key do atendedor (RAMAL ou MESA)
                // -------------------------------------------------------------------------------> FALTA IMPLEMENTAR

                // Inclui o novo Ramal na posição especificada
                ControleRamal.ramais.Insert(posicao, ramal);

                // Remove o último Ramal da lista
                ControleRamal.ramais.RemoveAt(ControleRamal.ramais.Count - 1);

                // Verifica se existe alguma funcionalidade programada com o último ramal
                // -------------------------------------------------------------------------------> FALTA IMPLEMENTAR

                // Redefine a numeração dos ramais
                for (int i = 0; i < ControleRamal.ramais.Count; i++)
                    ControleRamal.ramais[i].ramal = (i + 1).ToString();
            }
            else throw new Exception(result);
        }

        private static string validarRamal(Ramal ramal)
        {
            // Verifica se o número do apartamento é válido
            if (ramal.apartamento != "")
            {
                if (!ControleRamal.validarNumeroRamal(ramal.apartamento))
                {
                    return "O formato do número '" + ramal.apartamento + "' do novo ramal está inválido.\n\nAtenção:\nO número só pode conter os caracteres * e # e números de 0 a 9.\n- Também não pode ser iniciado com # e nem ultrapassar 8 digitos.";
                }
                else if (ControleRamal.verificarExistenciaApto(ramal.apartamento))
                {
                    return "Já existe um RAMAL programado com o número " + ramal.apartamento + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.";
                }
                else if (ControleMO.verificarExistenciaNumero(ramal.apartamento))
                {
                    return "Já existe uma MESA programada com o número " + ramal.apartamento + ".\n\nAtenção:\n- Utilize um número que não exista na lista de ramal ou mesa.";
                }
            }
            else return "O número do ramal não pode ficar vazio.";

            // Verifica se o atendedor é válido
            if (ramal.atendedor != "")
            {
                if (!ControleMO.verificarExistenciaNumero(ramal.atendedor) && !ControleRamal.verificarExistenciaApto(ramal.atendedor))
                {
                    return "Não foi possível definir o número " + ramal.atendedor + " como atendedor do ramal.\n\nAtenção:\n- Utilize um número de ramal ou mesa válido;\n- Não utilize o número do último ramal da lista de ramais.";
                }
            }
            else return "O atendedor do ramal não pode ficar vazio.";

            // Verifica se a funcionalodade de R.HOT e P.PHONE estão ativadas ao mesmo tempo
            if(ramal.ramalHOT && ramal.placaPortPhone) 
            {
                return "A funcionalidade de R.HOT não pode ficar ativada junto com a funcionalidade de P.PHONE.";
            }

            // Envia SUCESS se o ramal for válido
            return "SUCESS";
        }

        private static bool validarNumeroRamal(string value)
        {
            if (value == "" || Regex.IsMatch(value, @"^[0-9*][0-9*#]{0,7}$"))
            {
                return true;
            }
            else return false;
        }

        





        // ############################################################################################################

        // MÉTODOS DE BUSCA
        

        // MÉTODOS DE INSERSÃO
        

        // MÉTODOS UTILITÁRIOS
        

        

        








        /*
        public Ramal buscarRamalPorKey(string key)
        {
            try
            {
                return ControleRamal.ramais[key];
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }
        */
        

        // MÉTODOS DE INSERSÃO
        public void inserirRamalPorKey(string key, Ramal ramal)
        {
            //ControleRamal.ramais[key] = ramal;
        }

        // MÉTODOS DE FUNCIONALIDADES
        public void removerRamais(ArrayList listaDeRamais)
        {
            // Define lista tamporária de ramais (ramais que não serão excluidos)
            Dictionary<string, Ramal> temp = new Dictionary<string, Ramal>();

            // Exclui os ramais da lista
            for (int i = 0; i < listaDeRamais.Count; i++)
            {
                string key = "R" + listaDeRamais[i];
                //ControleRamal.ramais.Remove(key);
            }
            /*
            // Guarda na lista temporária os ramais restantes (redefine a "key" e o "ramal")
            int ramal = 1;
            foreach (KeyValuePair<string, Ramal> kvp in ramais)
            {
                string key = "R" + ramal;
                Ramal r = kvp.Value;
                r.key = key;
                r.ramal = ramal.ToString();
                temp.Add(key, r);
                ramal++;
            }

            // Completa a lista temporária com novos ramais até chegar nos 3072 registros
            int count = temp.Count + 1;
            for (int i = count; i <= 3072; i++)
            {
                string key = "R" + i;

                Ramal r = new Ramal();
                r.key = key;
                r.ramal = i.ToString();
                r.apartamento = "";
                r.atendedor = "M1";
                temp.Add(key, r);
            }

            ControleRamal.ramais = temp;

            /*
            // Voltar a programação default no números que fazem referências a um ramal excluído
            for (int i = 0; i < listaDeRamais.Count; i++)
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
                    if (central.listVideo[v].numero == (string)listaDeRamais[i])
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
                if (central.alarme.numero == listaDeRamais[i].ToString())
                    central.alarme.numero = "";
                for (int a = 0; a < central.alarme.listAtendedores.Count; a++)
                    if (central.alarme.listAtendedores[a] == listaDeRamais[i])
                        central.alarme.listAtendedores[a] = "";
            }
            */
        }



















        

        

        
        /*
        public string buscarKeyPorApartamento(string apto)
        {
            string result = "";
            foreach (KeyValuePair<string, Ramal> kvp in ramais)
                if (kvp.Value.apartamento.Equals(apto))
                {
                    result = kvp.Key;
                    break;
                }
            return result;
        }
        */
        

        // MÉTODOS DE INSERSÃO
    }
}