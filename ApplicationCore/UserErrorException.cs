using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class UserErrorException: Exception
    {
        public UserErrorException(string msg): base(msg)
        {
        }
    }
}
