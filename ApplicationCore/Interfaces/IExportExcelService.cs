using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IExportExcelService
    {
        Task<object> createExcel<T>(IEnumerable<T> list,string titleSheet);
    }
}
