/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla o estado do alarme.
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

namespace CentraisCDX.Class.Controle
{
    class ControleAlarme
    {
        // INSTÂNCIA DO MODELO
        Alarme alarme = new Alarme();

        // CONSTRUTOR
        public ControleAlarme()
        {
            // INICIALIZA O ESTADO DO OBJETO (MODELO)
            for (int i = 1; i <= 10; i++)
                this.alarme.listaDeAtendedores.Add(i, "");

            this.alarme.numero = "";
            this.alarme.tempo = "10";
        }

        // MÉTODOS DE BUSCA







        /*
        public string buscarAtendedor(int key)
        {
            return this._listAtendedores[key];
        }

        public void guardarAtendedor(int key, string value)
        {
            this._listAtendedores.Add(key, value);
        }

        
        public string buscarNumeroAlarme()
        {
            return buscarPorChaveNumero(central.alarme.numero);
        }

        public string buscarTempoAlarme()
        {
            return this.alarme.tempo;
        }

        public string buscarPorIdAtendedorAlarme(int key)
        {
            return buscarPorChaveNumero((string)central.alarme.listAtendedores[posicao - 1]);
        }

        // MÉTODOS DE INSERSÃO


        public string buscarAtendedor(int key)
        {
            return this._listAtendedores[key];
        }

        public void guardarAtendedor(int key, string value)
        {
            this._listAtendedores.Add(key, value);
        }

        // MÉTODOS GETTER E SETTER
        public string numero
        {
            get { return this.alarme.numero; }
            set { this.alarme.numero = value; }
        }

        public string tempo
        {
            get { return _tempo; }
            set { _tempo = value; }
        }
























        
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
                string atendedor = atendedores[i - 1].ToString();
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

        */
    }
}