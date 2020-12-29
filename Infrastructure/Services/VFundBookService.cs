﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class VFundBookService : IVFundBookService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VFundBookService(CatalogDbContext context, IMapper mapper,
           IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }


        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        public async Task<PagedResult2<VFundBookDisplay>> GetMoney(VFundBookSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.VFundBooks.Where(x => company_ids.Contains(x.CompanyId));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type.Equals(val.Type));
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.ResultSelection) && val.ResultSelection != "cash_bank")
                query = query.Where(x => x.Journal.Type.Equals(val.ResultSelection));
                var totalItems = await query.CountAsync();
            query = query.Take(val.Limit).Skip(val.Offset);
            var items = await query.Include(x => x.Journal).ToListAsync();

            return new PagedResult2<VFundBookDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<VFundBookDisplay>>(items)
            };
        }

        public string CompareType2(string val)
        {
            switch (val)
            {
                case "inbound-customer":
                    return "Khách hàng thanh toán";

                case "outbound-customer":
                    return "Hoàn tiền khách hàng";

                case "inbound-supplier":
                    return "NCC hoàn tiền";

                case "outbound-supplier":
                    return "Thanh toán NCC";

                case "salary":
                    return "Chi lương";

                case "advance":
                    return "Chi lương tạm ứng";

                default:
                    break;
            }
            return val;
        }
    }
}
