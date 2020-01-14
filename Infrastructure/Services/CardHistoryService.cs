﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class CardHistoryService : BaseService<CardHistory>, ICardHistoryService
    {
        public CardHistoryService(IAsyncRepository<CardHistory> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}