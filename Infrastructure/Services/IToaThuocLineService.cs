using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IToaThuocLineService: IBaseService<ToaThuocLine>
    {
        void ComputeName(IEnumerable<ToaThuocLine> self);
    }
}
