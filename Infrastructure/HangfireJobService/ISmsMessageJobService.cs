using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HangfireJobService
{
    public interface ISmsMessageJobService
    {
       Task RunJobFindSmsMessage(string db);
    }
}
