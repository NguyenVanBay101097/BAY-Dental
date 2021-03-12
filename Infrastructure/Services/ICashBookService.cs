﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICashBookService
    {
        Task<CashBookReport> GetSumary(CashBookSearch val);

        Task<PagedResult2<CashBookReportDetail>> GetDetails(CashBookDetailFilter val);
        Task<decimal> GetTotal(CashBookSearch val);
    }
}
