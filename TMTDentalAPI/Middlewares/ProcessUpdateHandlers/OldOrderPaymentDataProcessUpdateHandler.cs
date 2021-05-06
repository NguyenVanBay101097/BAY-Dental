using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class OldOrderPaymentDataProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        public OldOrderPaymentDataProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {

            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var orderObj = notification._context.RequestServices.GetService<ISaleOrderService>();
                var saleLineObj = notification._context.RequestServices.GetService<ISaleOrderLineService>();
                var moveObj = notification._context.RequestServices.GetService<IAccountMoveService>();
                var amlObj = notification._context.RequestServices.GetService<IAccountMoveLineService>();
                var linePaymentRelObj = notification._context.RequestServices.GetService<ISaleOrderLinePaymentRelService>();
                var orderPaymentObj = notification._context.RequestServices.GetService<ISaleOrderPaymentService>();
                var journalObj = notification._context.RequestServices.GetService<IAccountJournalService>();
                var accPaymentObj = notification._context.RequestServices.GetService<IAccountPaymentService>();
                var move_ids = new List<Guid>().AsEnumerable();
                var payment_ids = new List<Guid>().AsEnumerable();
                ///tìm các chi nhánh của tenant
                var companies = context.Companies.ToList();

                foreach (var company in companies)
                {
                    //lấy các phiếu điều trị đã xác nhận và thanh toán
                    var orders = orderObj.SearchQuery(x => x.CompanyId == company.Id && x.State != "draft")
                        .Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLinePaymentRels).ThenInclude(x => x.Payment)
                        .Include(x => x.OrderLines).ThenInclude(x => x.PartnerCommissions)
                        .Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLineInvoice2Rels)
                        .ToList();

                    //vòng lặp các phiếu điều trị xóa các hóa đơn 
                    foreach (var order in orders)
                    {
                        var mIds = amlObj.SearchQuery(x => x.SaleLineRels.Any(s => s.OrderLine.OrderId == order.Id)).Select(x => x.MoveId).Distinct().ToList();
                        move_ids = move_ids.Union(mIds);

                        ///xóa hóa đơn doanh thu của phiếu điều trị khi xác nhận
                        if (move_ids.Any())
                        {
                            moveObj.ButtonDraft(move_ids);
                            moveObj.Unlink(move_ids);
                        }

                        foreach (var line in order.OrderLines)
                        {
                            if (line.State == "cancel")
                                continue;

                            if (line.SaleOrderLinePaymentRels.Any())
                            {
                                payment_ids = payment_ids.Union(line.SaleOrderLinePaymentRels.Select(x => x.PaymentId).ToList());
                                linePaymentRelObj.DeleteAsync(line.SaleOrderLinePaymentRels);
                                var amountPaid = linePaymentRelObj.SearchQuery(x => x.SaleOrderLineId == line.Id).SumAsync(x => x.AmountPrepaid ?? 0);
                                line.AmountPaid = amountPaid.Result;
                                line.AmountResidual = 0;
                            }
                                                                                
                        }

                        saleLineObj._GetInvoiceQty(order.OrderLines);
                        saleLineObj._GetToInvoiceQty(order.OrderLines);
                        saleLineObj._GetInvoiceAmount(order.OrderLines);
                        saleLineObj._GetToInvoiceAmount(order.OrderLines);
                        saleLineObj._ComputeInvoiceStatus(order.OrderLines);
                        saleLineObj._RemovePartnerCommissions(order.OrderLines.Select(x => x.Id).ToList());
                        order.Residual = 0;

                        orderObj._GetInvoiced(new List<SaleOrder>() { order });
                        orderObj.UpdateAsync(order);

                        //tạo thanh toán phiếu điều trị 
                        //vòng lặp các thanh toán của phiếu điều trị 
                        if (payment_ids.Any())
                        {
                            var accountPayments = accPaymentObj.SearchQuery(x => payment_ids.Contains(x.Id) && x.State != "Cancel").Include(x => x.SaleOrderLinePaymentRels).ToList();
                            foreach (var accPayment in accountPayments)
                            {
                                var salePayment = new SaleOrderPayment()
                                {
                                    Amount = accPayment.Amount,
                                    Date = accPayment.PaymentDate,
                                    OrderId = order.Id,
                                    CompanyId = order.CompanyId
                                };

                                ///tạo SaleOrderPaymentHistoryLineSave từ orderlines          
                                foreach (var line in accPayment.SaleOrderLinePaymentRels)
                                {
                                    salePayment.Lines.Add(new SaleOrderPaymentHistoryLine
                                    {
                                        SaleOrderLineId = line.SaleOrderLineId,
                                        Amount = line.AmountPrepaid.HasValue ? line.AmountPrepaid.Value : 0,
                                        SaleOrderPayment = salePayment
                                    });
                                }


                                //tạo ra phương thức thanh toán 
                                salePayment.JournalLines.Add(new SaleOrderPaymentJournalLine()
                                {
                                    SaleOrderPayment = salePayment,
                                    JournalId = accPayment.JournalId,
                                    Amount = accPayment.Amount
                                });


                                orderPaymentObj.CreateAsync(salePayment);
                                orderPaymentObj.ActionPayment(new List<Guid>() { salePayment.Id });
                            }

                            ///xóa các payment cũ
                            accPaymentObj.CancelAsync(payment_ids);
                            accPaymentObj.DeleteAsync(accountPayments);

                        }


                    }

                }



            }

            return Task.CompletedTask;
        }
    }
}
