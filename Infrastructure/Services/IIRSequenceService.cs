using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IIRSequenceService : IBaseService<IRSequence>
    {
        Task<string> NextByCode(string v);
        Task<string> NextById(Guid id);
        Task<IQueryable<IRSequenceViewModel>> GetViewModelsAsync();
    }
}
