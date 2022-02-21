using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public interface IExceptionToErrorConverter
    {
        RemoteServiceErrorResponse Convert(Exception exception);
    }
}
