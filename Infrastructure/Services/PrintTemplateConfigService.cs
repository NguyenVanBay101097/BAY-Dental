using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class PrintTemplateConfigService : BaseService<PrintTemplateConfig> , IPrintTemplateConfigService
    {
        private readonly IMapper _mapper;
        public PrintTemplateConfigService(IAsyncRepository<PrintTemplateConfig> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
    }
}
