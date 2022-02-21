using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class AccessErrorException: Exception
    {
        public AccessErrorException(string msg): base(msg)
        {
        }
    }
}
