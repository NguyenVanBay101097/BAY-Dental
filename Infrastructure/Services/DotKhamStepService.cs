using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
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
    public class DotKhamStepService : BaseService<DotKhamStep>, IDotKhamStepService
    {
        readonly public IMapper _mapper;
        public DotKhamStepService(IMapper mapper, IAsyncRepository<DotKhamStep> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<DotKhamStep>> GetVisibleSteps(Guid dotKhamId, string show = "dotkham")
        {
            var dotKhamService = GetService<IDotKhamService>();
            var dotKham = await dotKhamService.GetByIdAsync(dotKhamId);
            if (show == "all")
            {
                return await SearchQuery(x => x.SaleOrderId.HasValue && x.SaleOrderId == dotKham.SaleOrderId)
                    .Include(x => x.Product).Include(x => x.DotKham).ToListAsync();
            }
            else
            {
                return await SearchQuery(x => x.SaleOrderId.HasValue && x.SaleOrderId == dotKham.SaleOrderId &&
                (!x.DotKhamId.HasValue || x.DotKhamId == dotKham.Id), orderBy: x => x.OrderBy(s => s.SaleLine.Sequence).ThenBy(s => s.Order))
                    .Include(x => x.Product).Include(x => x.DotKham).ToListAsync();
            }
        }

        public async Task<DotKhamStep> CloneInsert(DotKhamStepCloneInsert val)
        {
            var step = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            var steps = await SearchQuery(x => x.SaleLineId == step.SaleLineId, x => x.OrderBy(s => s.Order)).ToListAsync();
            var clone = new DotKhamStep(step);
            await CreateAsync(clone);

            if (val.CloneInsert == "down")
            {
                var index = steps.IndexOf(step);
                steps.Insert(index + 1, clone);
            }
            else
            {
                var index = steps.IndexOf(step);
                steps.Insert(index, clone);
            }

            //re sequence
            var sequence = 0;
            foreach (var st in steps)
                st.Order = sequence++;
            await UpdateAsync(steps);

            return clone;
        }

        public async Task<DotKhamStepDisplay> GetDisplay(Guid id)
        {
            var step = await SearchQuery(x => x.Id == id).Include(x => x.Product).FirstOrDefaultAsync();
            return _mapper.Map<DotKhamStepDisplay>(step);
        }

        public async Task<IEnumerable<IEnumerable<DotKhamStep>>> GetVisibleSteps2(Guid dotKhamId, string show = "dotkham")
        {
            var dotKhamService = GetService<IDotKhamService>();
            var dotKham = await dotKhamService.GetByIdAsync(dotKhamId);
            var res = new List<List<DotKhamStep>>();
            if (show == "all")
            {
                res = await SearchQuery(x => x.InvoicesId == dotKham.InvoiceId)
                    .Include(x => x.Product).Include(x => x.DotKham).OrderBy(x => x.Order).GroupBy(x => x.ProductId).Select(x => x.ToList()).ToListAsync();
            }
            else
            {
                res = await SearchQuery(x => x.InvoicesId == dotKham.InvoiceId &&
                (!x.DotKhamId.HasValue || x.DotKhamId == dotKham.Id))
                    .Include(x => x.Product).Include(x => x.DotKham).OrderBy(x => x.Order).GroupBy(x => x.ProductId).Select(x => x.ToList()).ToListAsync();
            }
            var invLinesObj = GetService<IAccountInvoiceLineService>();
            foreach (var lines in res)
            {
                foreach (var step in lines)
                {
                    //Đổi tên dịch vụ Product.name là têm của dịch vụ InvoiceLine
                    var invLine = invLinesObj.SearchQuery(x => x.InvoiceId == step.InvoicesId && x.ProductId == step.ProductId).ToList().FirstOrDefault();
                    step.Product.Name = invLine.Name;
                }
            }
            return res;
        }

        public async Task AssignDotKham(DotKhamStepAssignDotKhamVM val)
        {
            var steps = await SearchQuery(x => val.Ids.Contains(x.Id)).ToListAsync();
            foreach (var step in steps)
            {
                if (step.IsDone)
                    continue;

                if (!step.DotKhamId.HasValue)
                {
                    step.DotKhamId = val.DotKhamId;
                }
                else
                {
                    step.DotKhamId = null;
                }
            }
            await UpdateAsync(steps);
        }

        public async Task ToggleIsDone(DotKhamStepSetDone val)
        {
            var steps = await SearchQuery(x => val.Ids.Contains(x.Id)).ToListAsync();
            foreach (var step in steps)
            {
                //if (step.SaleOrder != null && (step.SaleOrder.State == "done" || step.SaleOrder.State == "cancel"))
                //    continue;
                step.IsDone = val.IsDone;
                if (val.IsDone)
                    step.DotKhamId = val.DotKhamId;
                else
                    step.DotKhamId = null;
            }

            await UpdateAsync(steps);
        }

        public async Task Unlink(IEnumerable<DotKhamStep> self)
        {
            if (self.Any(x => x.IsDone))
                throw new Exception("Không thể xóa chi tiết khám đã hoàn thành");
            await DeleteAsync(self);
        }

        public async Task<PagedResult2<DotKhamStepReport>> DotKhamStepReport(DotKhamStepPaged val)
        {
            var query = SearchQuery(x => x.IsDone == true, orderBy: x => x.OrderByDescending(s => s.DateCreated));
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
                query = query.Where(x => x.DotKham.Date >= val.DateFrom && x.DotKham.Date <= val.DateTo);
            if (!string.IsNullOrEmpty(val.UserId))
                query = query.Where(x => x.DotKham.UserId == val.UserId);
            if (val.DoctorId.HasValue)
                query = query.Where(x => x.DotKham.DoctorId == val.DoctorId);
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.DotKham.PartnerId == val.PartnerId.Value);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                x.DotKham.User.Name.Contains(val.Search) ||
                x.DotKham.AssistantUser.Name.Contains(val.Search) ||
                x.DotKham.Partner.Name.Contains(val.Search) ||
                x.Product.Name.Contains(val.Search));
            var items = await query.Select(x => new DotKhamStepReport
            {
                Date = x.DotKham.Date,
                AssistantName = x.DotKham.Assistant != null ? x.DotKham.Assistant.Name : "",
                DoctorName = x.DotKham.Doctor != null ? x.DotKham.Doctor.Name : "",
                PartnerName = x.DotKham.Partner != null ? x.DotKham.Partner.Name : "",
                ServiceName = x.Product != null ? x.Product.Name : "",
                StepName = x.Name
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<DotKhamStepReport>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
