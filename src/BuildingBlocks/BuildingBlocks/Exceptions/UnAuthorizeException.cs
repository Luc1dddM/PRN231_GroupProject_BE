using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Exceptions
{
    public class UnAuthorizeException : Exception
    {
        public UnAuthorizeException(string message) : base(message)
        {
        }

    }
}
