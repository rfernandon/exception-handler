/* =================================================================================
 * INFORMAÇÕES DO ARQUIVO
 * ---------------------------------------------------------------------------------
 * - Utilidade do arquivo : Define a exception que será lançada sempre que o usuário
 *                          digitar um número que não exista na lista de ramais.
 * - Versão do arquivo    : 01
 * - Data criação         : 28/02/2014
 * - Data alteração       : 28/02/2014
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
    class NumeroNaoEncontradoException : Exception
    {
        private string _mensagem;
        private string _mensagemDefault = "Não foi possível localizar o número na lista de ramais. Entre com um número válido";

        public NumeroNaoEncontradoException() { }

        public NumeroNaoEncontradoException(string mensagem)
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