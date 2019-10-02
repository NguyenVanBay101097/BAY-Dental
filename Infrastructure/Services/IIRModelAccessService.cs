using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IIRModelAccessService: IBaseService<IRModelAccess>
    {
        bool Check(string model, string mode = "Read", bool raiseException = true);
    }
}
