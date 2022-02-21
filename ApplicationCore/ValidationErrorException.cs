using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class ValidationErrorException: Exception
    {
        public ValidationErrorException(string msg): base(msg)
        {
        }
    }
}
