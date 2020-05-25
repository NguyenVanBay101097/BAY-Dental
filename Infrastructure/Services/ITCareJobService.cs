using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface ITCareJobService
    {
        void Run(string db);
    }
}
