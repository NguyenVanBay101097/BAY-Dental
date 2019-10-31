using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class LaboOrderService : BaseService<LaboOrder>, ILaboOrderService
    {
        private readonly IMapper _mapper;
        public LaboOrderService(IAsyncRepository<LaboOrder> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LaboOrderBasic>> GetPagedResultAsync(LaboOrderPaged val)
        {
            ISpecification<LaboOrder> spec = new InitialSpecification<LaboOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new LaboOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                State = x.State,
                CustomerName = x.Customer.Name
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LaboOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<LaboOrderDisplay> GetLaboDisplay(Guid id)
        {
            //var res = await SearchQuery(x => x.Id == id).Select(x => new LaboOrderDisplay
            //{
            //    Id = x.Id,
            //    AmountTotal = x.AmountTotal,
            //    DateOrder = x.DateOrder,
            //    DatePlanned = x.DatePlanned,
            //    DotKhamId = x.DotKhamId,
            //    Name = x.Name,
            //    PartnerId = x.PartnerId,
            //    PartnerRef = x.PartnerRef,
            //    State = x.State
            //}).FirstOrDefaultAsync();

            //var partnerObj = GetService<IPartnerService>();
            //res.Partner = await partnerObj.SearchQuery(x => x.Id == res.PartnerId).Select(x => new PartnerSimple
            //{
            //    Id = x.Id,
            //    Name = x.Name
            //}).FirstOrDefaultAsync();
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.OrderLines)
                .Include("OrderLines.Product")
                .Include("OrderLines.ToothCategory")
                .Include("OrderLines.LaboOrderLineToothRels")
                .Include("OrderLines.LaboOrderLineToothRels.Tooth").FirstOrDefaultAsync();
            var res = _mapper.Map<LaboOrderDisplay>(labo);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return res;
        }

        public async Task<LaboOrder> CreateLabo(LaboOrderDisplay val)
        {
            var labo = _mapper.Map<LaboOrder>(val);
            labo.CompanyId = CompanyId;
            SaveOrderLines(val, labo);

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeAmount(labo.OrderLines);

            _AmountAll(new List<LaboOrder>() { labo });
            await CreateAsync(labo);
            return labo;
        }

        public void _AmountAll(IEnumerable<LaboOrder> orders)
        {
            foreach (var order in orders)
            {
                var totalAmountUntaxed = 0M;

                foreach (var orderLine in order.OrderLines)
                {
                    totalAmountUntaxed += orderLine.PriceSubtotal;
                }

                order.AmountTotal = totalAmountUntaxed;
            }
        }

        public async Task UpdateLabo(Guid id, LaboOrderDisplay val)
        {
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
                .Include("OrderLines.LaboOrderLineToothRels")
                .FirstOrDefaultAsync();
            labo = _mapper.Map(val, labo);
            SaveOrderLines(val, labo);

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeAmount(labo.OrderLines);

            await UpdateAsync(labo);
        }

        public LaboOrderDisplay DefaultGet(LaboOrderDefaultGet val)
        {
            var res = new LaboOrderDisplay();
            return res;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft", "cancel" };
            foreach (var order in self)
            {
                if (!states.Contains(order.State))
                    throw new Exception("Chỉ có thể xóa phiếu labo ở trạng thái nháp hoặc hủy bỏ.");
            }

            await DeleteAsync(self);
        }

        private void SaveOrderLines(LaboOrderDisplay val, LaboOrder order)
        {
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<LaboOrderLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.OrderLines)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.OrderLines.Remove(line);
            }

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                line.Sequence = sequence++;
            }

            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var lbLine = _mapper.Map<LaboOrderLine>(line);
                    lbLine.CompanyId = order.CompanyId;
                    lbLine.PartnerId = order.PartnerId;
                    lbLine.CustomerId = order.CustomerId;
                    foreach (var tooth in line.Teeth)
                    {
                        lbLine.LaboOrderLineToothRels.Add(new LaboOrderLineToothRel
                        {
                            ToothId = tooth.Id
                        });
                    }
                    order.OrderLines.Add(lbLine);
                }
                else
                {
                    var lbLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (lbLine != null)
                    {
                        _mapper.Map(line, lbLine);
                        lbLine.LaboOrderLineToothRels.Clear();
                        foreach (var tooth in line.Teeth)
                        {
                            lbLine.LaboOrderLineToothRels.Add(new LaboOrderLineToothRel
                            {
                                ToothId = tooth.Id
                            });
                        }
                    }
                }
            }
        }

        public override async Task<LaboOrder> CreateAsync(LaboOrder labo)
        {
            if (string.IsNullOrEmpty(labo.Name))
            {
                var sequenceObj = GetService<IIRSequenceService>();
                labo.Name = await sequenceObj.NextByCode("labo.order");
            }
            return await base.CreateAsync(labo);
        }
    }
}
