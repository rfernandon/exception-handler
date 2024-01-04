/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Define a exception que será lançada sempre que o usuário
 *                          abrir um arquivo inválido.
 * - Versão do arquivo    : 01
 * - Data criação         : 27/05/2014
 * - Data alteração       : 27/05/2014
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

namespace CentraisCDX.Class.Util.Exceptions
{
    class ArquivoInvalidoException : Exception
    {
        private string _mensagem;
        private string _mensagemDefault = "O alvo não é um arquivo de programação válido. Escolha um outro arquivo.";

        public ArquivoInvalidoException() { }

        public ArquivoInvalidoException(string mensagem)
        {
            _mensagem = mensagem;
        }

        /* Métodos reescritos da classe Exception */
        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_mensagem))
                    _mensagem = this._mensagemDefault;
                return _mensagem;
            }
        }
    }
}