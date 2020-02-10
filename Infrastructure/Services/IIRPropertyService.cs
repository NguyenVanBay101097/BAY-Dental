using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IIRPropertyService: IBaseService<IRProperty>
    {
        void set_multi(string name, string model, IDictionary<string, object> values, object default_value = null,
         Guid? force_company = null);
        object get(string name, string model, string res_id = null, Guid? force_company = null);
    }
}
