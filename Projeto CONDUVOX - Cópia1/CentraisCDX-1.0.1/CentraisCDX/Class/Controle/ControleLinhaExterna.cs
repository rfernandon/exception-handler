/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Controla o estado da linha externa e seus objetos.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CentraisCDX.Class.Modelo;

namespace CentraisCDX.Class.Controle
{
    class ControleLinhaExterna
    {
        // INSTÂNCIA DO MODELO
        LinhaExterna linha = new LinhaExterna();

        // CONSTRUTOR
        public ControleLinhaExterna()
        {
            // INICIALIZA O ESTADO DO OBJETO (MODELO)
            for (int i = 1; i <= 2; i++)
            {
                Tronco t = new Tronco();
                t.estado = false;
                t.estadoChamadaCobrar = false;
                t.atendedor = "M1";
                this.linha.listaDeTroncos.Add(i, t);
            }

            for (int i = 1; i <= 20; i++)
            {
                this.linha.listaDeNumerosLiberado.Add(i, "");
                this.linha.listaDeNumerosBloqueado.Add(i, "");
            }
        }

        // MÉTODOS DE BUSCA
    }
}
