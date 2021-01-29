using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermods.Novo
{
    public class HermodsNovoUnauthorizedException : ApplicationException
    {
        public HermodsNovoUnauthorizedException(string message) : base(message) { }

        public HermodsNovoUnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
