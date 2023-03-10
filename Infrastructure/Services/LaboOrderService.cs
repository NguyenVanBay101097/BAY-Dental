using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class LaboOrderService : BaseService<LaboOrder>, ILaboOrderService
    {
        private readonly IMapper _mapper;
        public LaboOrderService(IAsyncRepository<LaboOrder> repository, IHttpContextAccessor httpContextAccessor, IUploadService uploadService,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LaboOrderBasic>> GetPagedResultAsync(LaboOrderPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Ref.Contains(val.Search) ||
                x.SaleOrderLine.Order.Name.Contains(val.Search) ||
                x.WarrantyCode.Contains(val.Search));

            if (val.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == val.CustomerId);
            }

            if (val.SaleOrderLineId.HasValue)
            {
                query = query.Where(x => x.SaleOrderLineId == val.SaleOrderLineId);
            }

            if (val.DateExportFrom.HasValue)
                query = query.Where(x => x.DateExport >= val.DateExportFrom);

            if (val.DateExportTo.HasValue)
            {
                var dateOrderTo = val.DateExportTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateExport <= dateOrderTo);
            }

            if (!string.IsNullOrEmpty(val.State))
            {
                query = query.Where(x => x.State == val.State);
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.Partner)
                .Include(x => x.SaleOrderLine.Order)
                .Include(x => x.LaboOrderToothRel).ThenInclude(x => x.Tooth);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<LaboOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<LaboOrderBasic>>(items)
            };

            return paged;

        }

        public async Task<PagedResult2<LaboOrderBasic>> GetFromSaleOrder_OrderLine(LaboOrderPaged val)
        {
            ISpecification<LaboOrder> spec = new InitialSpecification<LaboOrder>(x => true);
            var listSaleOrderLineIds = new List<Guid>();
            if (val.SaleOrderId.HasValue)
            {
                listSaleOrderLineIds = GetService<ISaleOrderLineService>().SearchQuery(x => x.OrderId == val.SaleOrderId.Value).Select(x => x.Id).ToList();
            }
            if (val.SaleOrderLineId.HasValue)
            {
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.OrderLines.Any(y => y.SaleOrderLineId == val.SaleOrderLineId)));
            }
            if (listSaleOrderLineIds.Count() > 0)
            {
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.OrderLines.Any(y => listSaleOrderLineIds.Contains(y.SaleOrderLineId.Value))));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<LaboOrderBasic>(query).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LaboOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<LaboOrderReceiptBasic>> GetPagedOrderLaboAsync(OrderLaboPaged val)
        {
            var query = SearchQuery(x => x.State == "confirmed" && !x.DateReceipt.HasValue);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Ref.Contains(val.Search) ||
                x.SaleOrderLine.Order.Name.Contains(val.Search));

            var now = DateTime.Now;

            if (!string.IsNullOrEmpty(val.State))
            {
                if (val.State == "trehan")
                {
                    query = query.Where(x => x.DatePlanned.HasValue && now.Date > x.DatePlanned.Value);
                }
                else if (val.State == "chonhan")
                {
                    query = query.Where(x => (x.DatePlanned.HasValue && now.Date < x.DatePlanned.Value) || !x.DatePlanned.HasValue);
                }
                else if (val.State == "toihan")
                {
                    query = query.Where(x => x.DatePlanned.HasValue && now.Date == x.DatePlanned.Value);
                }
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.Partner)
                .Include(x => x.SaleOrderLine.Order);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<LaboOrderReceiptBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<LaboOrderReceiptBasic>>(items)
            };

            return paged;
        }

        public async Task<PagedResult2<LaboOrderBasic>> GetPagedExportLaboAsync(ExportLaboPaged val)
        {
            ISpecification<LaboOrder> spec = new InitialSpecification<LaboOrder>(x => x.State == "confirmed" && x.DateReceipt.HasValue);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) || x.Partner.DisplayName.Contains(val.Search) ||
                x.Partner.Ref.Contains(val.Search) || x.Customer.Name.Contains(val.Search) || x.Customer.DisplayName.Contains(val.Search) ||
                x.SaleOrderLine.Order.Name.Contains(val.Search) ||
                x.WarrantyCode.Contains(val.Search)));

            if (!string.IsNullOrEmpty(val.State))
            {
                if (val.State == "daxuat")
                {
                    spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateExport.HasValue));
                }
                else if (val.State == "chuaxuat")
                {
                    spec = spec.And(new InitialSpecification<LaboOrder>(x => !x.DateExport.HasValue));
                }
            }

            if (val.PartnerId.HasValue)
            {
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.PartnerId == val.PartnerId));
            }

            if (val.DateExportFrom.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateExport >= val.DateExportFrom));

            if (val.DateExportTo.HasValue)
            {
                var dateOrderTo = val.DateExportTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateExport <= dateOrderTo));
            }

            if (val.DateReceiptFrom.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateReceipt >= val.DateReceiptFrom));

            if (val.DateReceiptTo.HasValue)
            {
                var dateReceipt = val.DateReceiptTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateReceipt <= dateReceipt));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await _mapper.ProjectTo<LaboOrderBasic>(query).ToListAsync();

            return new PagedResult2<LaboOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<LaboOrderStatisticsBasic>> GetStatisticsPaged(LaboOrderStatisticsPaged val)
        {
            ISpecification<LaboOrder> spec = new InitialSpecification<LaboOrder>(x => true);

            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.PartnerId == val.PartnerId));

            if (val.ProductId.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.ProductId == val.ProductId));

            if (val.DateOrderFrom.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateOrder >= val.DateOrderFrom));

            if (val.DateOrderTo.HasValue)
            {
                var dateOrderTo = val.DateOrderTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateOrder <= dateOrderTo));
            }

            if (val.DatePlannedFrom.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DatePlanned >= val.DatePlannedFrom));

            if (val.DatePlannedTo.HasValue)
            {
                var datePlannedTo = val.DatePlannedTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DatePlanned <= datePlannedTo));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new LaboOrderStatisticsBasic
            {
                Id = x.Id,
                PartnerDisplayName = x.Partner.DisplayName,
                CustomerDisplayName = x.Customer.DisplayName,
                ProductName = x.Product.Name,
                //OrderName = x.Order.Name,
                ProductQty = x.Quantity,
                PriceTotal = x.AmountTotal,
                //OrderDateOrder = x.Order.DateOrder,
                //OrderDatePlanned = x.Order.DatePlanned,
                WarrantyCode = x.WarrantyCode,
                WarrantyPeriod = x.WarrantyPeriod,
                //State = x.State,
                SaleOrderName = x.SaleOrderLine.Order.Name,
                SaleOrderId = x.SaleOrderLine.Order.Id,
                //IsReceived = x.IsReceived,
                //ReceivedDate = x.ReceivedDate
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LaboOrderStatisticsBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override ISpecification<LaboOrder> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "labo.labo_order_comp_rule":
                    return new InitialSpecification<LaboOrder>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<LaboOrderBasic>> GetAllForDotKham(Guid dotKhamId)
        {
            var res = await SearchQuery().Select(x => new LaboOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                //PartnerName = x.Partner.Name,
            }).ToListAsync();
            return res;
        }

        public async Task<LaboOrderDisplay> GetLaboDisplay(Guid id)
        {
            var attachmentObj = GetService<IIrAttachmentService>();
            var toothObj = GetService<IToothService>();
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.LaboBridge)
                .Include(x => x.LaboBiteJoint)
                .Include(x => x.LaboFinishLine)
                .Include(x => x.Product)
                .Include(x => x.Customer)
                .Include(x => x.SaleOrderLine).ThenInclude(x => x.Employee)
                .Include(x => x.SaleOrderLine).ThenInclude(x => x.Order)
                .Include(x => x.LaboOrderProductRel).ThenInclude(x => x.Product)
                .Include(x => x.LaboOrderToothRel).ThenInclude(x => x.Tooth)
                .Include("SaleOrderLine.Product").FirstOrDefaultAsync();

            var res = _mapper.Map<LaboOrderDisplay>(labo);
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var type = res.SaleOrderLine.ToothType;
            if (type == "manual")
            {
                var teeth = await saleOrderLineObj.SearchQuery(x => x.Id == res.SaleOrderLineId).SelectMany(x => x.SaleOrderLineToothRels)
                .Select(x => x.Tooth).ToListAsync();
                res.SaleOrderLine.Teeth = _mapper.Map<IEnumerable<ToothDisplay>>(teeth);
            }
            else
            {
                if (res.SaleOrderLine.ToothCategoryId.HasValue)
                {
                    var teeth = await GetTeeth(type, res.SaleOrderLine.ToothCategoryId.Value);
                    res.SaleOrderLine.Teeth = _mapper.Map<IEnumerable<ToothDisplay>>(teeth);
                }
            }
            var teethOrderLine = labo.LaboOrderToothRel.Select(x => x.Tooth);
            res.TeethOrderLine = _mapper.Map<IEnumerable<ToothDisplay>>(teethOrderLine);
            var attachments = await attachmentObj.GetAttachments("labo", res.Id);
            res.Images = _mapper.Map<IEnumerable<IrAttachmentBasic>>(attachments);
            return res;
        }

        public async Task<LaboOrder> CreateLabo(LaboOrderSave val)
        {
            var labo = _mapper.Map<LaboOrder>(val);
            labo.CompanyId = CompanyId;
            ///thêm răng
            foreach (var tooth in val.Teeth)
            {
                labo.LaboOrderToothRel.Add(new LaboOrderToothRel() { ToothId = tooth.Id });
            }

            ///thêm danh sach gửi kèm
            if (val.LaboOrderProducts.Any())
            {
                foreach (var attach in val.LaboOrderProducts)
                {
                    labo.LaboOrderProductRel.Add(new LaboOrderProductRel()
                    {
                        ProductId = attach.Id
                    });
                }
            }

            //labo.AmountTotal = labo.PriceUnit * labo.Quantity;
            await CreateAsync(labo);


            ///update image
            await UploadAttachment(val, labo);

            return labo;
        }



        private async Task UploadAttachment(LaboOrderSave val, LaboOrder labo)
        {
            var attachmentObj = GetService<IIrAttachmentService>();
            var imageslabo = await attachmentObj.GetAttachments("labo", labo.Id);
            //remove line
            var lineToRemoves = new List<IrAttachment>();
            foreach (var existLine in imageslabo)
            {
                if (!val.Images.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            await attachmentObj.DeleteAsync(lineToRemoves);

            var listadd = new List<IrAttachment>();
            foreach (var img in val.Images)
            {
                if (img.Id == Guid.Empty)
                {
                    var image = new IrAttachment();
                    image.ResModel = "labo";
                    image.ResId = labo.Id;
                    image.Name = img.Name;
                    image.Url = img.Url;

                    listadd.Add(image);
                }

            }

            await attachmentObj.CreateAsync(listadd);
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

        public async Task UpdateLabo(Guid id, LaboOrderSave val)
        {
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.LaboBridge)
                .Include(x => x.LaboBiteJoint)
                .Include(x => x.LaboFinishLine)
                .Include(x => x.Product)
                .Include(x => x.SaleOrderLine)
                .Include(x => x.LaboOrderProductRel)
                .Include(x => x.LaboOrderToothRel)
                .Include("SaleOrderLine.Product").FirstOrDefaultAsync();

            labo = _mapper.Map(val, labo);

            ///thêm răng
            labo.LaboOrderToothRel.Clear();
            foreach (var tooth in val.Teeth)
            {
                labo.LaboOrderToothRel.Add(new LaboOrderToothRel() { ToothId = tooth.Id });
            }

            labo.LaboOrderProductRel.Clear();
            foreach (var product in val.LaboOrderProducts)
            {
                labo.LaboOrderProductRel.Add(new LaboOrderProductRel
                {
                    ProductId = product.Id
                });
            }
            //labo.CompanyId = CompanyId;        
            // labo.AmountTotal = labo.PriceUnit * labo.Quantity;
            await UpdateAsync(labo);

            ///update image
            await UploadAttachment(val, labo);
        }



        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Partner)
                .Include(x => x.LaboBridge)
                .Include(x => x.LaboBiteJoint)
                .Include(x => x.LaboFinishLine)
                .Include(x => x.Product)
                .Include(x => x.SaleOrderLine)
                .Include(x => x.LaboOrderProductRel)
                .Include(x => x.LaboOrderToothRel)
                .Include("SaleOrderLine.Product").ToListAsync();

            foreach (var order in self)
            {
                if (order.State != "draft")
                    throw new Exception("Chỉ có thể xác nhận ở trạng thái nháp.");
                order.State = "confirmed";
            }

            var invoices = await _CreateInvoices(self);
            await moveObj.ActionPost(invoices);

            await UpdateAsync(self);
        }

        public async Task<IEnumerable<AccountMove>> _CreateInvoices(IEnumerable<LaboOrder> self)
        {
            var moveObj = GetService<IAccountMoveService>();
            var invoice_vals_list = new List<AccountMove>();
            foreach (var order in self)
            {
                var invoice_vals = await PrepareInvoice(order);
                invoice_vals.InvoiceLines.Add(_PrepareAccountMoveLine(order, invoice_vals));
                invoice_vals_list.Add(invoice_vals);

                var moves = await moveObj.CreateMoves(new List<AccountMove>() { invoice_vals }, default_type: "in_invoice");
                order.AccountMoveId = invoice_vals.Id;

                invoice_vals_list.Add(invoice_vals);
            }

            return invoice_vals_list;
        }

        public AccountMoveLine _PrepareAccountMoveLine(LaboOrder self, AccountMove move)
        {
            var qty = self.Quantity;
            return new AccountMoveLine
            {
                Name = self.SaleOrderLine.Name,
                Move = move,
                ProductUoMId = self.SaleOrderLine.ProductUOMId,
                ProductId = self.SaleOrderLine.ProductId,
                PriceUnit = self.PriceUnit,
                Quantity = qty,
                PartnerId = move.PartnerId,
            };
        }

        private async Task<AccountMove> PrepareInvoice(LaboOrder self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "in_invoice");
            if (journal == null)
                throw new Exception($"Please define an accounting purchase journal for the company {CompanyId}.");

            return new AccountMove()
            {
                Type = "in_invoice",
                PartnerId = self.PartnerId,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = self.CompanyId,
                InvoiceOrigin = self.Name,
            };
        }

        public async Task ButtonCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();

            var move_ids = self.Where(x => x.AccountMoveId.HasValue).Select(x => x.AccountMoveId.Value).ToList();
            foreach (var labo in self)
            {
                if (labo.DateReceipt.HasValue || labo.DateExport.HasValue)
                    throw new Exception("Phiếu Labo đã nhận từ NCC Labo hoặc đã xuất cho khách hàng không thể hủy phiếu");
                labo.AccountMoveId = null;
                labo.State = "draft";
            }

            await UpdateAsync(self);

            if (move_ids.Any())
            {
                var moveObj = GetService<IAccountMoveService>();
                await moveObj.ButtonDraft(move_ids);
                await moveObj.Unlink(move_ids);
            }
        }

        public async Task ActionCancelReceipt(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .ToListAsync();

            foreach (var order in self)
            {
                if (order.DateExport.HasValue)
                {
                    order.DateExport = null;
                }

                order.DateReceipt = null;
                //order.
            }

            await UpdateAsync(self);
        }
        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.OrderLines).ToListAsync();
            foreach (var order in self)
            {
                foreach (var line in order.OrderLines)
                    line.State = "done";
            }

            await UpdateAsync(self);
        }

        public async Task<LaboOrderDisplay> DefaultGet(LaboOrderDefaultGet val)
        {
            var res = new LaboOrderDisplay();
            if (val.DotKhamId.HasValue)
            {
                var dkObj = GetService<IDotKhamService>();
                var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId).Include(x => x.Partner).FirstOrDefaultAsync();
            }

            if (val.SaleOrderLineId.HasValue)
            {
                var saleOrderLineObj = GetService<ISaleOrderLineService>();
                var toothObj = GetService<IToothService>();
                var orderLine = await saleOrderLineObj.SearchQuery(x => x.Id == val.SaleOrderLineId)
                    .Include(x => x.Product)
                    .Include(x => x.OrderPartner)
                    .Include(x => x.Employee)
                    .Include(x => x.Order)
                    .FirstOrDefaultAsync();

                var type = orderLine.ToothType;
                res.SaleOrderLine = _mapper.Map<SaleOrderLineDisplay>(orderLine);
                res.Employee = _mapper.Map<EmployeeSimple>(orderLine.Employee);
                res.Customer = _mapper.Map<PartnerSimple>(orderLine.OrderPartner);
                if (orderLine.ToothType == "manual")
                {
                    var teeth = await saleOrderLineObj.SearchQuery(x => x.Id == val.SaleOrderLineId).SelectMany(x => x.SaleOrderLineToothRels)
                    .Select(x => x.Tooth).ToListAsync();
                    res.SaleOrderLine.Teeth = _mapper.Map<IEnumerable<ToothDisplay>>(teeth);
                }
                else
                {
                    var teeth = await GetTeeth(type, orderLine.ToothCategoryId.Value);
                    res.SaleOrderLine.Teeth = _mapper.Map<IEnumerable<ToothDisplay>>(teeth);
                }

                res.State = "draft";
                res.SaleOrderLineId = val.SaleOrderLineId;
            }

            return res;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft", "cancel" };
            foreach (var labo in self)
            {
                if (!states.Contains(labo.State))
                    throw new Exception("Chỉ có thể xóa phiếu Labo ở trạng thái nháp");
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
                //foreach (var item in val.OrderLines)
                //{
                //    if (item.Id == existLine.Id)
                //    {
                //        found = true;
                //        break;
                //    }
                //}

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                if (line.State != "draft")
                    continue;
                order.OrderLines.Remove(line);
            }

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            //foreach (var line in val.OrderLines)
            //{
            //    line.Sequence = sequence++;
            //}

            //foreach (var line in val.OrderLines)
            //{
            //    if (line.Id == Guid.Empty)
            //    {
            //        var lbLine = _mapper.Map<LaboOrderLine>(line);
            //        lbLine.CompanyId = order.CompanyId;
            //        lbLine.PartnerId = order.PartnerId;
            //        foreach (var tooth in line.Teeth)
            //        {
            //            lbLine.LaboOrderLineToothRels.Add(new LaboOrderLineToothRel
            //            {
            //                ToothId = tooth.Id
            //            });
            //        }
            //        order.OrderLines.Add(lbLine);
            //    }
            //    else
            //    {
            //        var lbLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
            //        if (lbLine != null)
            //        {
            //            _mapper.Map(line, lbLine);
            //            lbLine.LaboOrderLineToothRels.Clear();
            //            foreach (var tooth in line.Teeth)
            //            {
            //                lbLine.LaboOrderLineToothRels.Add(new LaboOrderLineToothRel
            //                {
            //                    ToothId = tooth.Id
            //                });
            //            }
            //        }
            //    }
            //}
        }

        public async Task<LaboOrderPrintVM> GetPrint(Guid id)
        {
            var order = await SearchQuery(x => x.Id == id)
               .Include(x => x.Partner)
               .Include(x => x.Company)
               .Include(x => x.Company.Partner)
               .Include(x => x.OrderLines)
               .Include("OrderLines.Product")
               .FirstOrDefaultAsync();
            var res = _mapper.Map<LaboOrderPrintVM>(order);
            var partnerObj = GetService<IPartnerService>();
            //res.PartnerAddress = partnerObj.GetFormatAddress(order.Partner);
            return res;
        }

        public override async Task<IEnumerable<LaboOrder>> CreateAsync(IEnumerable<LaboOrder> entities)
        {
            await _UpdateProperties(entities);
            return await base.CreateAsync(entities);
        }

        public override async Task UpdateAsync(IEnumerable<LaboOrder> entities)
        {
            await _UpdateProperties(entities);
            await base.UpdateAsync(entities);
        }

        private async Task _UpdateProperties(IEnumerable<LaboOrder> self)
        {
            foreach (var labo in self)
            {
                if (string.IsNullOrEmpty(labo.Name))
                {
                    var sequenceObj = GetService<IIRSequenceService>();
                    labo.Name = await sequenceObj.NextByCode("labo.order");
                }

                if (!labo.CustomerId.HasValue)
                {
                    var saleLineObj = GetService<ISaleOrderLineService>();
                    var saleLine = await saleLineObj.GetByIdAsync(labo.SaleOrderLineId);
                    labo.CustomerId = saleLine.OrderPartnerId;
                }
            }
        }

        public async Task<long> GetCountLaboOrder(LaboOrderGetCount val)
        {
            var query = SearchQuery();

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.DateOrder >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateOrder <= dateTo);
            }
            var now = DateTime.Now;
            if (!string.IsNullOrEmpty(val.State))
            {
                if (val.State == "danhan")
                {
                    query = query.Where(x => x.DateReceipt.HasValue);
                }
                else if (val.State == "toihan")
                {
                    query = query.Where(x => x.DatePlanned.HasValue && now.Date == x.DatePlanned.Value && !x.DateReceipt.HasValue);
                }
            }

            return await query.LongCountAsync();
        }

        public async Task<bool> CheckExistWarrantyCode(LaboOrderCheck val)
        {
            if (string.IsNullOrEmpty(val.Code) || val.Code.Trim() == "") return false;
            var exist = await SearchQuery(x => x.WarrantyCode.ToLower().Contains(val.Code.Trim().ToLower())).FirstOrDefaultAsync();
            if (exist == null || val.Id.HasValue && val.Id == exist.Id) return false;
            if (exist != null)
            {
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<ToothDisplay>> GetTeeth(string type, Guid toothCateId)
        {
            var toothObj = GetService<IToothService>();
            if (type == "upper_jaw")
                return _mapper.Map<IEnumerable<ToothDisplay>>(await toothObj.SearchQuery(x => x.CategoryId == toothCateId && x.ViTriHam == "0_up").ToListAsync());
            else if (type == "lower_jaw")
                return _mapper.Map<IEnumerable<ToothDisplay>>(await toothObj.SearchQuery(x => x.CategoryId == toothCateId && x.ViTriHam == "1_down").ToListAsync());
            else if (type == "whole_jaw")
                return _mapper.Map<IEnumerable<ToothDisplay>>(await toothObj.SearchQuery(x => x.CategoryId == toothCateId).ToListAsync());
            else
                return null;
        }
    }
}
