/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Guardar o estado do vídeoa.
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
using CentraisCDX.Class.Modelo;

namespace CentraisCDX.Class.Controle
{
    class ControleVideo
    {
        // INSTÂNCIA DO MAP COM O MODELO
        Dictionary<int, Video> videos = new Dictionary<int, Video>();

        // CONSTRUTOR DA CLASSE
        public ControleVideo()
        {
            // INICIALIZA O ESTADO DO OBJETO (MODELO)
            for (int i = 1; i <= 8; i++)
            {
                Video v = new Video();
                v.estado = false;
                v.numero = "";
                videos.Add(i, v);
            }
        }

        // MÉTODOS DE BUSCA
    }
}