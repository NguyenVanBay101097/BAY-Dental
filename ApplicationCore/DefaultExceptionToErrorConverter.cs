using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ApplicationCore
{
    public class DefaultExceptionToErrorConverter : IExceptionToErrorConverter
    {
        public RemoteServiceErrorResponse Convert(Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // Cần dựa vào exception để xác định status code 
            var error = new RemoteServiceErrorResponse
            {
                Code = (int)code,
                Message = "TDental Server Error",
                Data = SerializeException(exception)
            };

            return error;
        }

        private RemoteServiceErrorInfo SerializeException(Exception ex)
        {
            var errorInfo = new RemoteServiceErrorInfo
            {
                Message = ex.Message,
                Name = ex.GetType().Name
            };

            return errorInfo;
        }
    }
}
