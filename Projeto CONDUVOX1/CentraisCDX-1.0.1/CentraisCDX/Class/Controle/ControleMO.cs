/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado da mesa operadora, suas teclas e seus
 *                          objetos.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CentraisCDX.Class.Modelo;

namespace CentraisCDX.Class.Controle
{
    class ControleMO
    {
        // INSTÂNCIA DO MODELO
        private static Dictionary<string, MesaOperadora> mesas = new Dictionary<string, MesaOperadora>();

        // CONSTRUTOR DA CLASSE
        public static void inicializar()
        {
            // INICIALIZA O ESTADO DO OBJETO (MODELO)
            for (int i = 1; i <= 8; i++)
            {
                string key = "M" + i;

                MesaOperadora m = new MesaOperadora();
                m.key = key;
                m.numero = i == 1 ? "*" : "*"+(i-1);
                m.listaDeTeclas.Add(Tecla.Nome.ZELADOR, new Tecla(Tecla.Nome.ZELADOR, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.SINDICO, new Tecla(Tecla.Nome.SINDICO, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.A1, new Tecla(Tecla.Nome.A1, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.A2, new Tecla(Tecla.Nome.A2, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.A3, new Tecla(Tecla.Nome.A3, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.PORTEIRO, new Tecla(Tecla.Nome.PORTEIRO, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.TELEFONE, new Tecla(Tecla.Nome.TELEFONE, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.FECH1, new Tecla(Tecla.Nome.FECH1, Tecla.Estado.DESATIVADA, ""));
                m.listaDeTeclas.Add(Tecla.Nome.FECH2, new Tecla(Tecla.Nome.FECH2, Tecla.Estado.DESATIVADA, ""));
                ControleMO.mesas.Add(key, m);
            }
        }

        // OPERAÇÕES
        public static string buscarNumeroPorKey(string key)
        {
            return ControleMO.mesas[key].numero;
        }

        public static bool verificarExistenciaNumero(string numero)
        {
            bool result = false;
            foreach (KeyValuePair<string, MesaOperadora> kvp in ControleMO.mesas)
                if (kvp.Value.numero.Equals(numero))
                {
                    result = true;
                    break;
                }
            return result;
        }



        // ############################################################################################################











        /*
        public Tecla buscarTeclaPorId(int id)
        {
            return this._listTecla[id];
        }
        
        
        public estado buscarPorNomeEstadoTecla(nome n)
        {
            return this._listTecla[(int)n].estado;
        }

        
        public string buscarPorNomeAtendedorTecla(nome n)
        {
            return this._listTecla[(int)n].atendedor;
        }

        
        public void definirEstadoTecla(nome n, estado e)
        {
            this._listTecla[(int)n].estado = e;
        }

        
        public void definirAtendedorTecla(nome n, string s)
        {
            this._listTecla[(int)n].atendedor = s;
        }
         */
    }
}
