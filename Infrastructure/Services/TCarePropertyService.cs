using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class TCarePropertyService : BaseService<TCareProperty>, ITCarePropertyService
    {
        public TCarePropertyService(IAsyncRepository<TCareProperty> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public object GetValue(TCareProperty self)
        {
            if (self.Type == "Text")
            {
                var prop = typeof(TCareProperty).GetProperty($"Value{self.Type}");
                return Convert.ToString(prop.GetValue(self));
            }
            else if (self.Type == "Integer")
            {
                var prop = typeof(TCareProperty).GetProperty($"Value{self.Type}");
                return Convert.ToInt32(prop.GetValue(self));
            }
            else if (self.Type == "Decimal")
            {
                var prop = typeof(TCareProperty).GetProperty($"Value{self.Type}");
                return Convert.ToDecimal(prop.GetValue(self));
            }
            else if (self.Type == "Double")
            {
                var prop = typeof(TCareProperty).GetProperty($"Value{self.Type}");
                return Convert.ToDouble(prop.GetValue(self));
            }
            else if (self.Type == "DateTime")
            {
                var prop = typeof(TCareProperty).GetProperty($"Value{self.Type}");
                return Convert.ToDateTime(prop.GetValue(self));
            }
            else if (self.Type == "Many2One")
            {
                return null;
            }
            else
                throw new Exception("Not Support");
        }
    }
}
