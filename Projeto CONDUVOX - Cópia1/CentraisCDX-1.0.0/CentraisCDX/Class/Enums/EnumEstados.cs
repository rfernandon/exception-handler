using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentraisCDX.Class
{
    public enum EnumEstado
    {
        PARADO, 
        EXECUTANDO, 
        CONCLUIDO,
        EXCEPTION, 
        INVALIDO, 
        TIMEOUT, 
        INCORRETO,
        RECEBIDO,
        ABORTADO
    };
}
