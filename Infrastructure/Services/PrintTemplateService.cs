using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PrintTemplateService : BaseService<PrintTemplate>, IPrintTemplateService
    {
        private readonly IMapper _mapper;
        public PrintTemplateService(IAsyncRepository<PrintTemplate> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<string> GetPrintTemplate(PrintTemplateDefault val)
        {
            var template = await SearchQuery(x => x.Type == val.Type).FirstOrDefaultAsync();
            if (template == null)
                throw new Exception("Loại mẫu in hiện chưa có mẫu sẵn");

            return template.Content;
        }
    }
}
