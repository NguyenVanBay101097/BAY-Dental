using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ImportSampleDataService : IImportSampleDataService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CatalogDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMyCache _cache;
        private readonly AppTenant _tenant;

        public ImportSampleDataService(IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IMyCache cache, ITenant<AppTenant> tenant,
            CatalogDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
            _tenant = tenant?.Value;
        }

        public async Task ImportSampleData()
        {
            var xmlObj = GetService<IXmlService>();
            var irModelDataObj = GetService<IIRModelDataService>();
            var companyId = CompanyId;

            #region Danh mục
            #region partner
            var partnerDict = new Dictionary<string, Partner>();
            var partnerObj = GetService<IPartnerService>();
            //add data customer
            var partnerCustomerFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\partner_customer.xml");
            var partnerCustomerData = xmlObj.GetObject<XmlSampleData<PartnerCustomerXmlSampleDataRecord>>(partnerCustomerFilePath);
            foreach (var item in partnerCustomerData.Records)
            {
                var customer = _mapper.Map<Partner>(item);
                customer.Customer = true;
                customer.CompanyId = companyId;
                customer.Date = DateTime.Today.AddDays(-item.DateRound);
                partnerDict.Add(item.Id, customer);
            }
            //add data supplier
            var partnerSupplierFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\partner_supplier.xml");
            var partnerSupplierData = xmlObj.GetObject<XmlSampleData<PartnerSupplierXmlSampleDataRecord>>(partnerSupplierFilePath);
            foreach (var item in partnerSupplierData.Records)
            {
                var supplier = _mapper.Map<Partner>(item);
                supplier.Customer = false;
                supplier.Supplier = true;
                supplier.CompanyId = companyId;
                supplier.Date = DateTime.Today.AddDays(-item.DateRound);
                partnerDict.Add(item.Id, supplier);
            }
            await partnerObj.CreateAsync(partnerDict.Values);
            #endregion

            #region uom
            var uomDict = new Dictionary<string, UoM>();
            var uomObj = GetService<IUoMService>();
            var uomFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\uom.xml");
            var uomData = xmlObj.GetObject<XmlSampleData<UomXmlSampleDataRecord>>(uomFilePath);
            foreach (var item in uomData.Records)
            {
                var uom = _mapper.Map<UoM>(item);
                uom.CategoryId = (await irModelDataObj.GetRef<UoMCategory>(item.CategoryId)).Id;
                uomDict.Add(item.Id, uom);
            }
            await uomObj.CreateAsync(uomDict.Values);
            #endregion

            #region productCategory
            var productCategoryDict = new Dictionary<string, ProductCategory>();
            var productCategObj = GetService<IProductCategoryService>();
            var productCategoryFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_category.xml");
            var productCategoryData = xmlObj.GetObject<XmlSampleData<ProductCategoryXmlSampleDataRecord>>(productCategoryFilePath);
            foreach (var item in productCategoryData.Records)
            {
                var cate = _mapper.Map<ProductCategory>(item);
                productCategoryDict.Add(item.Id, cate);
            }
            await productCategObj.CreateAsync(productCategoryDict.Values);
            #endregion

            #region product
            var productDict = new Dictionary<string, Product>();
            var productObj = GetService<IProductService>();
            var defaultUoM = await irModelDataObj.GetRef<UoM>("product.product_uom_unit");
            var productStandardPrices = new List<ProductStandardPriceTmp>();
            var propertyObj = GetService<IIRPropertyService>();

            //add service
            var productServicePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_service.xml");
            var productServiceData = xmlObj.GetObject<XmlSampleData<ProductServiceXmlSampleDataRecord>>(productServicePath);
            foreach (var item in productServiceData.Records)
            {
                var service = _mapper.Map<Product>(item);
                service.CompanyId = companyId;
                service.Type = "service";
                service.Type2 = "service";
                service.UOMId = defaultUoM.Id;
                service.UOMPOId = defaultUoM.Id;
                service.CategId = productCategoryDict.ContainsKey(item.CategId) ? productCategoryDict[item.CategId].Id : (await irModelDataObj.GetRef<ProductCategory>(item.CategId)).Id;
                productDict.Add(item.Id, service);
                productStandardPrices.Add(new ProductStandardPriceTmp
                {
                    Product = service,
                    StandardPrice = item.StandardPrice
                });
            }
            //add product
            var productProductPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_product.xml");
            var productProductData = xmlObj.GetObject<XmlSampleData<ProductProductXmlSampleDataRecord>>(productProductPath);
            foreach (var item in productProductData.Records)
            {
                var product = _mapper.Map<Product>(item);
                product.SaleOK = false;
                product.CompanyId = companyId;
                product.Type = "product";
                product.Type2 = "product";
                product.UOMId = uomDict.ContainsKey(item.UOMId) ? uomDict[item.UOMId].Id : (await irModelDataObj.GetRef<UoM>(item.UOMId)).Id; ;
                product.UOMPOId = uomDict.ContainsKey(item.UOMPOId) ? uomDict[item.UOMPOId].Id : (await irModelDataObj.GetRef<UoM>(item.UOMPOId)).Id; ;
                product.CategId = productCategoryDict.ContainsKey(item.CategId) ? productCategoryDict[item.CategId].Id : (await irModelDataObj.GetRef<ProductCategory>(item.CategId)).Id;
                productDict.Add(item.Id, product);
            }
            //add medicine
            var productMeidicnePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_medicine.xml");
            var productMeidicneData = xmlObj.GetObject<XmlSampleData<ProductMedicineXmlSampleDataRecord>>(productMeidicnePath);
            foreach (var item in productMeidicneData.Records)
            {
                var medicine = _mapper.Map<Product>(item);
                medicine.SaleOK = false;
                medicine.KeToaOK = true;
                medicine.CompanyId = companyId;
                medicine.Type = "product";
                medicine.Type2 = "medicine";
                medicine.UOMId = uomDict.ContainsKey(item.UOMId) ? uomDict[item.UOMId].Id : (await irModelDataObj.GetRef<UoM>(item.UOMId)).Id;
                medicine.UOMPOId = defaultUoM.Id;
                medicine.CategId = productCategoryDict.ContainsKey(item.CategId) ? productCategoryDict[item.CategId].Id : (await irModelDataObj.GetRef<ProductCategory>(item.CategId)).Id;
                productDict.Add(item.Id, medicine);
                productStandardPrices.Add(new ProductStandardPriceTmp
                {
                    Product = medicine,
                    StandardPrice = item.StandardPrice
                });
            }
            //add labo
            var productLaboPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_labo.xml");
            var productLaboData = xmlObj.GetObject<XmlSampleData<ProductLaboXmlSampleDataRecord>>(productLaboPath);
            foreach (var item in productLaboData.Records)
            {
                var labo = _mapper.Map<Product>(item);
                labo.SaleOK = false;
                labo.PurchaseOK = false;
                labo.CompanyId = companyId;
                labo.Type = "consu";
                labo.Type2 = "labo";
                labo.UOMId = defaultUoM.Id;
                labo.UOMPOId = defaultUoM.Id;
                productDict.Add(item.Id, labo);
            }
            //add labo_attach
            var productLaboAttachPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_labo_attach.xml");
            var productLaboAttachData = xmlObj.GetObject<XmlSampleData<ProductLaboXmlSampleDataRecord>>(productLaboAttachPath);
            foreach (var item in productLaboAttachData.Records)
            {
                var laboAttach = _mapper.Map<Product>(item);
                laboAttach.SaleOK = false;
                laboAttach.PurchaseOK = false;
                laboAttach.CompanyId = companyId;
                laboAttach.Type = "consu";
                laboAttach.Type2 = "labo_attach";
                laboAttach.UOMId = defaultUoM.Id;
                laboAttach.UOMPOId = defaultUoM.Id;
                productDict.Add(item.Id, laboAttach);
            }

            await productObj.CreateAsync(productDict.Values);
            propertyObj.set_multi("standard_price", "product.product", productStandardPrices.ToDictionary(x => string.Format("product.product,{0}", x.Product.Id), x => (object)x.StandardPrice), force_company: companyId);

            var priceHistoryObj = GetService<IProductPriceHistoryService>();
            var priceHistories = productStandardPrices.Select(x => new ProductPriceHistory
            {
                ProductId = x.Product.Id,
                Cost = (double)x.StandardPrice,
                CompanyId = companyId
            }).ToList();

            await priceHistoryObj.CreateAsync(priceHistories);
            #endregion

            #region labobitejoint
            var labobitejointDict = new Dictionary<string, LaboBiteJoint>();
            var labobitejointObj = GetService<ILaboBiteJointService>();
            var labobitejointFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\labo_bite_joint.xml");
            var labobitejointData = xmlObj.GetObject<XmlSampleData<SimpleXmlSampleDataRecord>>(labobitejointFilePath);
            foreach (var item in labobitejointData.Records)
            {
                var entity = _mapper.Map<LaboBiteJoint>(item);
                labobitejointDict.Add(item.Id, entity);
            }
            await labobitejointObj.CreateAsync(labobitejointDict.Values);
            #endregion

            #region labobridge
            var labobridgeDict = new Dictionary<string, LaboBridge>();
            var labobridgeObj = GetService<ILaboBridgeService>();
            var labobridgeFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\labo_bridge.xml");
            var labobridgeData = xmlObj.GetObject<XmlSampleData<SimpleXmlSampleDataRecord>>(labobridgeFilePath);
            foreach (var item in labobridgeData.Records)
            {
                var entity = _mapper.Map<LaboBridge>(item);
                labobridgeDict.Add(item.Id, entity);
            }
            await labobridgeObj.CreateAsync(labobridgeDict.Values);
            #endregion

            #region LaboFinishLine
            var labofinishlineDict = new Dictionary<string, LaboFinishLine>();
            var labofinishlineObj = GetService<ILaboFinishLineService>();
            var labofinishlineFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\labo_finish_line.xml");
            var labofinishlineData = xmlObj.GetObject<XmlSampleData<SimpleXmlSampleDataRecord>>(labofinishlineFilePath);
            foreach (var item in labofinishlineData.Records)
            {
                var entity = _mapper.Map<LaboFinishLine>(item);
                labofinishlineDict.Add(item.Id, entity);
            }
            await labofinishlineObj.CreateAsync(labofinishlineDict.Values);
            #endregion

            #region employee
            var employeeDict = new Dictionary<string, Employee>();
            var employeeObj = GetService<IEmployeeService>();
            var employeeFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\employee.xml");
            var employeeData = xmlObj.GetObject<XmlSampleData<EmployeeXmlSampleDataRecord>>(employeeFilePath);
            foreach (var item in employeeData.Records)
            {
                var entity = _mapper.Map<Employee>(item);
                entity.CompanyId = companyId;
                employeeDict.Add(item.Id, entity);
            }
            await employeeObj.CreateAsync(employeeDict.Values);
            #endregion

            #region loaithuchi

            var loaithuchiDict = new Dictionary<string, LoaiThuChi>();
            var loaithuchiObj = GetService<ILoaiThuChiService>();
            //add data loaithu
            var loaithuFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\loai_thu.xml");
            var loaithuData = xmlObj.GetObject<XmlSampleData<LoaiThuChiXmlSampleDataRecord>>(loaithuFilePath);
            foreach (var item in loaithuData.Records)
            {
                var loaithu = _mapper.Map<LoaiThuChi>(item);
                loaithu.CompanyId = companyId;
                loaithu.Type = "thu";
                loaithuchiDict.Add(item.Id, loaithu);
            }
            //add data loaichi
            var loaichiFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\loai_chi.xml");
            var loaichiData = xmlObj.GetObject<XmlSampleData<LoaiThuChiXmlSampleDataRecord>>(loaichiFilePath);
            foreach (var item in loaichiData.Records)
            {
                var loaithu = _mapper.Map<LoaiThuChi>(item);
                loaithu.CompanyId = companyId;
                loaithu.Type = "chi";
                loaithuchiDict.Add(item.Id, loaithu);
            }

            //xử lý logic
            foreach (var self in loaithuchiDict.Values)
            {
                string reference_account_type = "";
                if (self.Type == "thu")
                {
                    if (self.IsAccounting)
                        reference_account_type = "account.data_account_type_revenue";
                    else
                        reference_account_type = "account.data_account_type_thu";
                }
                else if (self.Type == "chi")
                {
                    if (self.IsAccounting)
                        reference_account_type = "account.data_account_type_expenses";
                    else
                        reference_account_type = "account.data_account_type_chi";
                }

                var usertype = await loaithuchiObj.GetAccountTypeThuChi(reference_account_type);
                var account = new AccountAccount
                {
                    Name = self.Name,
                    Code = self.Code,
                    Note = self.Note,
                    CompanyId = self.CompanyId ?? CompanyId,
                    InternalType = usertype.Type,
                    UserTypeId = usertype.Id,
                };
                self.Account = account;
            }
            await loaithuchiObj.CreateAsync(loaithuchiDict.Values);
            #endregion
            #endregion

            #region bussiness
            #region saleorder
            var saleOrderDict = new Dictionary<string, SaleOrder>();
            var saleOrderLineDict = new Dictionary<string, SaleOrderLine>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrderLineObj = GetService<ISaleOrderLineService>();

            //add service
            var saleOrderPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\sale_order.xml");
            var saleOrderData = xmlObj.GetObject<XmlSampleData<SaleOrderXmlSampleDataRecord>>(saleOrderPath);
            foreach (var item in saleOrderData.Records)
            {
                var saleOrder = _mapper.Map<SaleOrder>(item);
                saleOrder.DateOrder = DateTime.Today.AddDays(-item.DateRound).AddHours(item.TimeHour).AddMinutes(item.TimeMinute);
                saleOrder.CompanyId = companyId;
                saleOrder.PartnerId = partnerDict.ContainsKey(item.PartnerId) ? partnerDict[item.PartnerId].Id : (await irModelDataObj.GetRef<Partner>(item.PartnerId)).Id;
                var sequence = 0;
                foreach (var lineItem in item.OrderLines)
                {
                    var line = _mapper.Map<SaleOrderLine>(lineItem);
                    line.Date = DateTime.Today.AddDays(-lineItem.DateRound);
                    line.CompanyId = companyId;
                    line.Sequence = sequence++;
                    line.ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id;
                    line.ToothCategoryId = (await irModelDataObj.GetRef<ToothCategory>(lineItem.ToothCategoryId)).Id;
                    foreach (var lineItemTooth in lineItem.SaleOrderLineToothRels)
                    {
                        line.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel() 
                        { ToothId = (await irModelDataObj.GetRef<Tooth>(lineItemTooth.ToothId)).Id });
                    }

                    saleOrderLineDict.Add(lineItem.Id, line);
                    saleOrder.OrderLines.Add(line);
                }
                saleOrderDict.Add(item.Id, saleOrder);
            }
            // xử lý logic trước khi tạo
            foreach (var saleOrder in saleOrderDict.Values)
            {
                saleOrderLineObj.UpdateOrderInfo(saleOrder.OrderLines, saleOrder);
                saleOrderLineObj.ComputeAmount(saleOrder.OrderLines);
                saleOrderObj._AmountAll(saleOrder);
            }
            await saleOrderObj.CreateAsync(saleOrderDict.Values);
            await saleOrderObj.ActionConfirm(saleOrderDict.Values.Select(x => x.Id));
            #endregion

            #region saleorder payment
            var saleOrderPaymentDict = new Dictionary<string, SaleOrderPayment>();
            var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();

            //add service
            var saleOrderPaymentPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\sale_order_payment.xml");
            var saleOrderPaymentData = xmlObj.GetObject<XmlSampleData<SaleOrderPaymentXmlSampleDataRecord>>(saleOrderPaymentPath);
            foreach (var item in saleOrderPaymentData.Records)
            {
                var saleOrderPayment = _mapper.Map<SaleOrderPayment>(item);
                saleOrderPayment.Date = DateTime.Today.AddDays(-item.DateRound);
                saleOrderPayment.CompanyId = companyId;
                saleOrderPayment.OrderId = saleOrderDict.ContainsKey(item.OrderId) ? saleOrderDict[item.OrderId].Id : (await irModelDataObj.GetRef<SaleOrder>(item.OrderId)).Id;
                foreach (var lineItem in item.Lines)
                {
                    var line = new SaleOrderPaymentHistoryLine()
                    {
                        Amount = lineItem.Amount,
                        SaleOrderLineId = saleOrderLineDict.ContainsKey(lineItem.SaleOrderLineId) ? saleOrderLineDict[lineItem.SaleOrderLineId].Id : (await irModelDataObj.GetRef<SaleOrderLine>(lineItem.SaleOrderLineId)).Id
                    };
                    saleOrderPayment.Lines.Add(line);
                }
                foreach (var lineItem in item.JournalLines)
                {
                    var line = new SaleOrderPaymentJournalLine()
                    {
                        Amount = lineItem.Amount,
                        JournalId = (await irModelDataObj.GetRef<AccountJournal>(lineItem.JournalId)).Id
                    };
                    saleOrderPayment.JournalLines.Add(line);
                }

                saleOrderPaymentDict.Add(item.Id, saleOrderPayment);
            }

            await saleOrderPaymentObj.CreateAsync(saleOrderPaymentDict.Values);
            await saleOrderPaymentObj.ActionPayment(saleOrderPaymentDict.Values.Select(x => x.Id));
            #endregion

            #region laboorder
            var laboOrderDict = new Dictionary<string, LaboOrder>();
            var laboOrderObj = GetService<ILaboOrderService>();

            var laboOrderPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\labo_order.xml");
            var laboOrderData = xmlObj.GetObject<XmlSampleData<LaboOrderXmlSampleDataRecord>>(laboOrderPath);
            foreach (var item in laboOrderData.Records)
            {
                var laboOrder = _mapper.Map<LaboOrder>(item);
                laboOrder.DateOrder = DateTime.Today.AddDays(-item.DateRound);
                laboOrder.CompanyId = companyId;
                laboOrder.PartnerId = partnerDict[item.PartnerId].Id;
                laboOrder.ProductId = productDict.ContainsKey(item.ProductId) ? (Guid?)productDict[item.ProductId].Id : null;
                laboOrder.SaleOrderLineId = saleOrderLineDict.ContainsKey(item.SaleOrderLineId) ? (Guid?)saleOrderLineDict[item.SaleOrderLineId].Id : null;
                laboOrder.LaboFinishLineId = labofinishlineDict.ContainsKey(item.LaboFinishLineId) ? (Guid?)labofinishlineDict[item.LaboFinishLineId].Id : null;
                laboOrder.LaboBiteJointId = labobitejointDict.ContainsKey(item.LaboBiteJointId) ? (Guid?)labobitejointDict[item.LaboBiteJointId].Id : null;
                laboOrder.LaboBridgeId = labobridgeDict.ContainsKey(item.LaboBridgeId) ? (Guid?)labobridgeDict[item.LaboBridgeId].Id : null;

                foreach (var lineItem in item.LaboOrderProductRel)
                {
                    var line = new LaboOrderProductRel()
                    {
                        ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id
                    };
                    laboOrder.LaboOrderProductRel.Add(line);
                }
                foreach (var lineItem in item.LaboOrderToothRel)
                {
                    var line = new LaboOrderToothRel()
                    {
                        ToothId = (await irModelDataObj.GetRef<Tooth>(lineItem.ToothId)).Id
                    };
                    laboOrder.LaboOrderToothRel.Add(line);
                }
                laboOrderDict.Add(item.Id, laboOrder);
            }

            await laboOrderObj.CreateAsync(laboOrderDict.Values);
            await laboOrderObj.ButtonConfirm(laboOrderDict.Values.Select(x => x.Id));
            #endregion

            #region appointment
            var appointmentDict = new Dictionary<string, Appointment>();
            var appointmentObj = GetService<IAppointmentService>();

            var appointmentPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\appointment.xml");
            var appointmentData = xmlObj.GetObject<XmlSampleData<AppointmentXmlSampleDataRecord>>(appointmentPath);
            foreach (var item in appointmentData.Records)
            {
                var appointment = _mapper.Map<Appointment>(item);
                appointment.Date = DateTime.Today.AddDays(-item.DateRound).AddHours(item.TimeHour).AddMinutes(item.TimeMinute);
                appointment.CompanyId = companyId;
                appointment.PartnerId = partnerDict[item.PartnerId].Id;
                appointment.DoctorId = employeeDict.ContainsKey(item.DoctorId) ? (Guid?)employeeDict[item.DoctorId].Id : null;
                appointment.SaleOrderId = saleOrderDict.ContainsKey(item.SaleOrderId) ? (Guid?)saleOrderDict[item.SaleOrderId].Id : null;

                foreach (var lineItem in item.AppointmentServices)
                {
                    var line = new ProductAppointmentRel()
                    {
                        ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id
                    };
                    appointment.AppointmentServices.Add(line);
                }
                appointmentDict.Add(item.Id, appointment);
            }

            await appointmentObj.CreateAsync(appointmentDict.Values);
            #endregion

            #region CustomerReceipt
            var customerReceiptDict = new Dictionary<string, CustomerReceipt>();
            var customerReceiptObj = GetService<ICustomerReceiptService>();

            var customerReceiptPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\customer_receipt.xml");
            var customerReceiptData = xmlObj.GetObject<XmlSampleData<CustomerReceiptXmlSampleDataRecord>>(customerReceiptPath);
            foreach (var item in customerReceiptData.Records)
            {
                var customerReceipt = _mapper.Map<CustomerReceipt>(item);
                customerReceipt.DateWaiting = DateTime.Today.AddDays(-item.DateRound).AddHours(item.WaitingTimeHour).AddMinutes(item.WaitingTimeMinute);
                if (item.ExaminationTimeHour.HasValue)
                    customerReceipt.DateExamination = DateTime.Today.AddDays(-item.DateRound).AddHours(item.ExaminationTimeHour.Value).AddMinutes(item.ExaminationTimeMinute.Value);
                if (item.DoneTimeHour.HasValue)
                    customerReceipt.DateDone = DateTime.Today.AddDays(-item.DateRound).AddHours(item.DoneTimeHour.Value).AddMinutes(item.DoneTimeMinute.Value);

                customerReceipt.CompanyId = companyId;
                customerReceipt.PartnerId = partnerDict[item.PartnerId].Id;
                customerReceipt.DoctorId = employeeDict.ContainsKey(item.DoctorId) ? (Guid?)employeeDict[item.DoctorId].Id : null;

                foreach (var lineItem in item.CustomerReceiptProductRels)
                {
                    var line = new CustomerReceiptProductRel()
                    {
                        ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id
                    };
                    customerReceipt.CustomerReceiptProductRels.Add(line);
                }
                customerReceiptDict.Add(item.Id, customerReceipt);
            }

            await customerReceiptObj.CreateAsync(customerReceiptDict.Values);
            #endregion

            #region phieuthuchi

            var phieuthuchiDict = new Dictionary<string, PhieuThuChi>();
            var phieuthuchiObj = GetService<IPhieuThuChiService>();
            //add data phieuthu
            var phieuthuFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\phieu_thu.xml");
            var phieuthuData = xmlObj.GetObject<XmlSampleData<PhieuThuChiXmlSampleDataRecord>>(phieuthuFilePath);
            foreach (var item in phieuthuData.Records)
            {
                var phieuthu = _mapper.Map<PhieuThuChi>(item);
                phieuthu.Date = DateTime.Today.AddDays(-item.DateRound);
                phieuthu.CompanyId = companyId;
                phieuthu.JournalId = (await irModelDataObj.GetRef<AccountJournal>(item.JournalId)).Id;
                phieuthu.LoaiThuChiId = loaithuchiDict[item.LoaiThuChiId].Id;
                phieuthu.Type = "thu";
                phieuthu.AccountId = loaithuchiDict[item.LoaiThuChiId].AccountId;
                phieuthuchiDict.Add(item.Id, phieuthu);
            }
            //add data phieuchi
            var phieuchiFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\phieu_chi.xml");
            var phieuchiData = xmlObj.GetObject<XmlSampleData<PhieuThuChiXmlSampleDataRecord>>(phieuchiFilePath);
            foreach (var item in phieuchiData.Records)
            {
                var phieuchi = _mapper.Map<PhieuThuChi>(item);
                phieuchi.Date = DateTime.Today.AddDays(-item.DateRound);
                phieuchi.CompanyId = companyId;
                phieuchi.JournalId = (await irModelDataObj.GetRef<AccountJournal>(item.JournalId)).Id;
                phieuchi.LoaiThuChiId = loaithuchiDict[item.LoaiThuChiId].Id;
                phieuchi.Type = "chi";
                phieuchi.AccountId = loaithuchiDict[item.LoaiThuChiId].AccountId;
                phieuthuchiDict.Add(item.Id, phieuchi);
            }

            //xử lý logic
            foreach (var phieuThuChi in phieuthuchiDict.Values)
            {
                await phieuthuchiObj.GenerateNamePhieuThuChi(phieuThuChi);
            }
            await phieuthuchiObj.ComputeProps(phieuthuchiDict.Values);
            await phieuthuchiObj.CreateAsync(phieuthuchiDict.Values);
            await phieuthuchiObj.ActionConfirm(phieuthuchiDict.Values.Select(x => x.Id));
            #endregion

            #region purchaseorder

            var accpaymentObj = GetService<IAccountPaymentService>();
            var purchaseorderDict = new Dictionary<string, PurchaseOrder>();
            var purchaseorderObj = GetService<IPurchaseOrderService>();
            //add data order
            var purchaseorderFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\purchase_order_order.xml");
            var purchaseorderData = xmlObj.GetObject<XmlSampleData<PurchaseOrderXmlSampleDataRecord>>(purchaseorderFilePath);
            foreach (var item in purchaseorderData.Records)
            {
                var pickingType = await irModelDataObj.GetRef<StockPickingType>("stock.stock_picking_type_incoming");
                var purchaseorder = _mapper.Map<PurchaseOrder>(item);

                purchaseorder.DateOrder = DateTime.Today.AddDays(-item.DateRound);
                purchaseorder.CompanyId = companyId;
                purchaseorder.PartnerId = partnerDict[item.PartnerId].Id;
                purchaseorder.PickingTypeId = pickingType.Id;
                purchaseorder.JournalId = (await irModelDataObj.GetRef<AccountJournal>(item.JournalId)).Id;
                purchaseorder.Type = "order";
                purchaseorder.UserId = UserId;
                var sequence = 0;
                foreach (var lineItem in item.OrderLines)
                {
                    var line = _mapper.Map<PurchaseOrderLine>(lineItem);
                    line.CompanyId = companyId;
                    line.PartnerId = purchaseorder.PartnerId;
                    line.Sequence = sequence++;
                    line.Name = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Name : "";
                    line.ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id;
                    line.ProductUOMId = uomDict.ContainsKey(lineItem.ProductUOMId) ? productDict[lineItem.ProductUOMId].Id : (await irModelDataObj.GetRef<UoM>(lineItem.ProductUOMId)).Id;
                    purchaseorder.OrderLines.Add(line);
                }
                purchaseorderDict.Add(item.Id, purchaseorder);
            }
            //add data refund
            var purchaserefundFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\purchase_order_refund.xml");
            var purchaserefundData = xmlObj.GetObject<XmlSampleData<PurchaseOrderXmlSampleDataRecord>>(purchaserefundFilePath);
            foreach (var item in purchaserefundData.Records)
            {
                var purchaseorder = _mapper.Map<PurchaseOrder>(item);
                purchaseorder.DateOrder = DateTime.Today.AddDays(-item.DateRound);
                purchaseorder.CompanyId = companyId;
                purchaseorder.PartnerId = partnerDict[item.PartnerId].Id;
                purchaseorder.PickingTypeId = (await irModelDataObj.GetRef<StockPickingType>(item.PickingTypeId)).Id;
                purchaseorder.JournalId = (await irModelDataObj.GetRef<AccountJournal>(item.JournalId)).Id;
                purchaseorder.Type = "refund";
                var sequence = 0;
                foreach (var lineItem in item.OrderLines)
                {
                    var line = _mapper.Map<PurchaseOrderLine>(lineItem);
                    line.CompanyId = companyId;
                    line.Sequence = sequence++;
                    line.PartnerId = purchaseorder.PartnerId;
                    line.ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id;
                    line.ProductUOMId = uomDict.ContainsKey(lineItem.ProductUOMId) ? productDict[lineItem.ProductUOMId].Id : (await irModelDataObj.GetRef<UoM>(lineItem.ProductUOMId)).Id;
                    purchaseorder.OrderLines.Add(line);
                }
                purchaseorderDict.Add(item.Id, purchaseorder);
            }

            //xử lý logic
            var purchaseLineObj = GetService<IPurchaseOrderLineService>();
            foreach (var item in purchaseorderDict.Values)
            {
                item.UserId = UserId;
                var sequence = 1;
                foreach (var line in item.OrderLines)
                {
                    line.State = item.State;
                    line.Sequence = sequence++;
                }
                purchaseLineObj._ComputeAmount(item.OrderLines);

            }

            purchaseorderObj._AmountAll(purchaseorderDict.Values);
            await purchaseorderObj.CreateAsync(purchaseorderDict.Values);
            await purchaseorderObj.ButtonConfirm(purchaseorderDict.Values.Select(x => x.Id));
            //thanh toán mua hàng
            var payments = new List<AccountPayment>();

            foreach (var item in purchaseorderDict.Values)
            {
                var accpaymentDefaultGet = await accpaymentObj.PurchaseDefaultGet(new List<Guid>() { item.Id });

                var payment = _mapper.Map<AccountPayment>(accpaymentDefaultGet);
                payment.Journal = null;
                payment.CompanyId = CompanyId;
                foreach (var invoice_id in accpaymentDefaultGet.InvoiceIds)
                    payment.AccountMovePaymentRels.Add(new AccountMovePaymentRel { MoveId = invoice_id });

                payments.Add(payment);
            }
            await accpaymentObj._ComputeDestinationAccount(payments);
            await accpaymentObj.CreateAsync(payments);
            await accpaymentObj.Post(payments.Select(x => x.Id));
            #endregion

            #region stockpicking
            var stockMoveObj = GetService<IStockMoveService>();
            var stockPickingDict = new Dictionary<string, StockPicking>();
            var stockPickingObj = GetService<IStockPickingService>();

            //add data order
            var stockPickingFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\stock_picking.xml");
            var stockPickingData = xmlObj.GetObject<XmlSampleData<StockPickingXmlSampleDataRecord>>(stockPickingFilePath);
            foreach (var item in stockPickingData.Records)
            {
                var pickingType = (await irModelDataObj.GetRef<StockPickingType>("stock.stock_picking_type_outgoing"));

                var stockPicking = _mapper.Map<StockPicking>(item);
                stockPicking.Date = DateTime.Today.AddDays(-item.DateRound);
                stockPicking.CompanyId = companyId;
                stockPicking.PartnerId = employeeDict[item.PartnerId].PartnerId;
                stockPicking.PickingTypeId = pickingType.Id;
                stockPicking.LocationId = pickingType.DefaultLocationSrcId.Value;
                stockPicking.LocationDestId = pickingType.DefaultLocationDestId.Value;
                var sequence = 0;
                foreach (var lineItem in item.MoveLines)
                {
                    var line = _mapper.Map<StockMove>(lineItem);
                    line.ProductUOMQty = line.ProductQty.Value;
                    line.CompanyId = companyId;
                    line.Date = stockPicking.Date.Value;
                    line.Sequence = sequence++;
                    line.PickingTypeId = pickingType.Id;
                    line.PartnerId = employeeDict[lineItem.PartnerId].PartnerId;
                    line.ProductUOMId = uomDict.ContainsKey(lineItem.ProductUOMId) ? productDict[lineItem.ProductUOMId].Id : (await irModelDataObj.GetRef<UoM>(lineItem.ProductUOMId)).Id;
                    line.ProductId = productDict.ContainsKey(lineItem.ProductId) ? productDict[lineItem.ProductId].Id : (await irModelDataObj.GetRef<Product>(lineItem.ProductId)).Id;
                    line.LocationId = pickingType.DefaultLocationSrcId.Value;
                    line.LocationDestId = pickingType.DefaultLocationDestId.Value;
                    stockPicking.MoveLines.Add(line);
                }
                stockPickingDict.Add(item.Id, stockPicking);
            }

            //xử lý logic sau khi tạo
            stockMoveObj._Compute(stockPickingDict.Values.SelectMany(x => x.MoveLines));
            await stockPickingObj.CreateAsync(stockPickingDict.Values);
            await stockPickingObj.ActionDone(stockPickingDict.Values.Select(x => x.Id));
            #endregion
            #endregion
        }

        public async Task DeleteSampleData()
        {
            var irConfigParameterObj = GetService<IIrConfigParameterService>();
            var userObj = GetService<IUserService>();
            //var valueRemove = await irConfigParameterObj.GetParam("remove_sample_data");
            var valueImport = await irConfigParameterObj.GetParam("import_sample_data");
            var user = await userObj.GetCurrentUser();

            if (!user.IsUserRoot)
                throw new Exception("Chỉ có admin mới có thể thực hiện chức năng này !!!");
            if (valueImport == "Removed")
                throw new Exception("Xóa dữ liệu mẫu chỉ được phép xóa một lần duy nhất !!!");
            if (valueImport != "Installed")
                throw new Exception("Bạn chưa import dữ liệu mẫu !!!");

            var companyObj = GetService<ICompanyService>();
            var groupObj = GetService<IResGroupService>();
            var partnerObj = GetService<IPartnerService>();
            var irModelDataObj = GetService<IIRModelDataService>();
            var partner_main = await irModelDataObj.GetRef<Partner>("base.main_partner");
            var company_main = await irModelDataObj.GetRef<Company>("base.main_company");
            var user_root = await irModelDataObj.GetRef<ApplicationUser>("base.user_root");
            var partner_root = await irModelDataObj.GetRef<Partner>("base.partner_root");

            if (partner_root == null)
                partner_root = await partnerObj.GetByIdAsync(user_root.PartnerId);
            if (company_main == null)
                company_main = await companyObj.GetByIdAsync(user_root.CompanyId);
            if (partner_main == null)
                partner_main = await partnerObj.GetByIdAsync(company_main.PartnerId);

            #region xóa các bảng
            // các câu lệnh ưu tiên
            //await _dbContext.ExecuteSqlCommandAsync("update AccountJournals set DefaultCreditAccountId = null, DefaultDebitAccountId = null");
            //await _dbContext.ExecuteSqlCommandAsync("update Companies set AccountIncomeId = null, AccountExpenseId = null");
            ////marketing
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookConnectPages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookConnects");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookMassMessagings");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookMessagingTraces");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookPages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookScheduleAppointmentConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookTags");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookUserProfiles");
            //await _dbContext.ExecuteSqlCommandAsync("Delete FacebookUserProfileTagRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerMapPSIDFacebookPages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MarketingCampaignActivityFacebookTagRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MarketingMessageButtons");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MarketingMessages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MarketingTraces");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MarketingCampaignActivities");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MarketingCampaigns");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ZaloOAConfigs");
            ////thanh toán
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPaymentJournalLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPaymentHistoryLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPaymentAccountPaymentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPaymentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPayments");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountRegisterPaymentInvoiceRel");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountRegisterPayments");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountFinancialReportAccountAccountTypeRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountFinancialReports");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountFinancialRevenueReportAccountAccountRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountFinancialRevenueReportAccountAccountTypeRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountFinancialRevenueReports");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountInvoiceAccountMoveLineRel");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountInvoiceLineToothRel");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountInvoiceLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountInvoicePaymentRel");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountInvoices");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountMovePaymentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountPayments");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PaymentQuotations");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SalaryPayments");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLineInvoice2Rels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLineInvoiceRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLinePaymentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrderLineInvoiceRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrderPaymentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrderPayments");
            ////bussiness
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerAdvances");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerPartnerCategoryRel");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MailTrackingValues");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MailMessageResPartnerRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MailNotifications");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MailMessages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MedicineOrderLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MedicineOrders");
            //await _dbContext.ExecuteSqlCommandAsync("Delete MemberLevels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AdvisoryToothDiagnosisRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AdvisoryToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AdvisoryProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Advisory");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrderLineToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrderLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrderToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete laboOrderProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboWarrantyToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboWarranty");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrders");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CustomerReceiptProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CustomerReceipts");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrderLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrders");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ToaThuocLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ToaThuocs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SamplePrescriptionLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SamplePrescriptions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete DotKhamLineOperations");
            //await _dbContext.ExecuteSqlCommandAsync("Delete DotKhamSteps");
            //await _dbContext.ExecuteSqlCommandAsync("Delete DotKhamLineToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete DotKhamLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete DotKhams");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleCouponProgramPartnerRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleCouponProgramProductCategoryRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleCouponProgramMemberLevelRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleCouponProgramProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleCoupons");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderNoCodePromoPrograms");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleCouponPrograms");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AppointmentMailMessageRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductAppointmentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Appointments");

            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleSettings");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderServiceCardCardRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLinePartnerCommissions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLineProductRequesteds");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLineToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPromotionLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderPromotions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrders");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductStockInventoryCriteriaRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockQuantMoveRel");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockQuants");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockMoves");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PurchaseOrderLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PurchaseOrders");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockPickings");

            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardCards");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrderLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrders");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardTypes");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CardHistories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CardCards");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CardTypes");

            //await _dbContext.ExecuteSqlCommandAsync("update StockPickingTypes set DefaultLocationSrcId = null, DefaultLocationDestId = null, WarehouseId = null");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockWarehouses");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockLocations");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockPickingTypes");
            //await _dbContext.ExecuteSqlCommandAsync("update Partners set CompanyId = null, CreatedById = null, WriteById = null where Id != '" + partner_main.Id.ToString() + "' and Id != '" + partner_root.Id.ToString() + "'");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerHistoryRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Histories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerImages");


            //await _dbContext.ExecuteSqlCommandAsync("Update IrConfigParameters set CreatedById = null , WriteById = null");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IrConfigParameters");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResGroupImpliedRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResConfigSettings");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IrModuleCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Update Companies set CreatedById = null , WriteById = null where Id !='" + company_main.Id.ToString() + "'");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Companies where Id != '" + company_main.Id.ToString() + "'");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CommissionProductRules");
            //await _dbContext.ExecuteSqlCommandAsync("Delete CommissionSettlements");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Commissions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ChamCongs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrPayrollStructureTypes");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrPayrollStructures");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrPayslipLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrPayslipWorkedDays");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrPayslips");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrPayslipRuns");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrSalaryRuleCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrSalaryConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrSalaryRules");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ConfigPrints");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PrintPaperSizes");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PrintTemplateConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PrintTemplates");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductRequestLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductRequests");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PromotionProgramCompanyRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PromotionPrograms");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PromotionRuleProductCategoryRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PromotionRuleProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PromotionRules");
            //await _dbContext.ExecuteSqlCommandAsync("Delete QuotationLineToothRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete QuotationPromotionLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete QuotationPromotions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete QuotationLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Quotations");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResCompanyUsersRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResPartnerBanks");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResBanks");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResourceCalendarAttendances");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResourceCalendarLeaves");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResourceCalendars");
            //await _dbContext.ExecuteSqlCommandAsync("Delete RoutingLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Routings");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsAccounts");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsAppointmentAutomationConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsBirthdayAutomationConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsCampaign");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsCareAfterOrderAutomationConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsComposers");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsConfigProductCategoryRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsConfigProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsMessageAppointmentRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsMessageDetails");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsMessagePartnerRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsMessages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsMessageSaleOrderLineRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsMessageSaleOrderRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsTemplates");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SmsThanksCustomerAutomationConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockInventoryLine");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockInventoryCriterias");
            //await _dbContext.ExecuteSqlCommandAsync("Delete StockInventory");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyAnswers");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyAssignments");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyCallContents");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyQuestions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyTags");
            //await _dbContext.ExecuteSqlCommandAsync("Delete surveyUserInputLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyUserInputs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete SurveyUserInputSurveyTagRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareCampaigns");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareConfigs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareMessages");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareMessageTemplates");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareMessagingPartnerRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareMessagings");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareRules");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareProperties");
            //await _dbContext.ExecuteSqlCommandAsync("Delete TCareScenarios");

            ////core
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResGroupsUsersRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PhieuThuChis");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LoaiThuChis");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountMoveLines");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountMoves");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountPartialReconciles");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRProperties");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountFullReconciles");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ApplicationRoleFunctions");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AspNetRoleClaims");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AspNetUserRoles");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AspNetRoles");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AspNetUsers where Id != '" + user_root.Id.ToString() + "'");
            //await _dbContext.ExecuteSqlCommandAsync("Delete UserRefreshTokens");

            ////danh muc
            //await _dbContext.ExecuteSqlCommandAsync("Delete HrJobs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Agents");
            //await _dbContext.ExecuteSqlCommandAsync("Delete EmployeeCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Employees");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Partners where Id != '" + partner_main.Id.ToString() + "' and Id != '" + partner_root.Id.ToString() + "'");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRModelAccesses");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerSources");
            //await _dbContext.ExecuteSqlCommandAsync("Delete PartnerTitles");
            //await _dbContext.ExecuteSqlCommandAsync("Delete RuleGroupRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ResGroups");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRRules");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRModels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductPricelists");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductBoms");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductPricelistItems");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductPriceHistories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductCompanyRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductUoMRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductSteps");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ToothDiagnosisProductRels");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Products");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ProductCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete UoMs");
            //await _dbContext.ExecuteSqlCommandAsync("Delete UoMCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ToothDiagnosis");
            //await _dbContext.ExecuteSqlCommandAsync("Delete Teeth");
            //await _dbContext.ExecuteSqlCommandAsync("Delete ToothCategories");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IrAttachments");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRModelDatas");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRModelFields");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountJournals");
            //await _dbContext.ExecuteSqlCommandAsync("Delete IRSequences");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountAccounts");
            //await _dbContext.ExecuteSqlCommandAsync("Delete AccountAccountTypes");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboBiteJoints");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboBridges");
            //await _dbContext.ExecuteSqlCommandAsync("Delete LaboFinishLines");
            #endregion

            //set constraint
            await _dbContext.ExecuteSqlCommandAsync("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            // delete all tables except some tables
            string exceptTables = "('__EFMigrationsHistory','Partners','AspNetUsers','Companies')";
            await _dbContext.ExecuteSqlCommandAsync($@"DECLARE @strSQL Varchar(MAX) = '';
                                                       SELECT @strSQL = @strSQL + 'Delete ' + name + ' ; '
                                                       FROM sys.tables
                                                       where name not in {exceptTables}
                                                       ORDER BY name;
                                                       SELECT @strSQL;
                                                       Exec(@strSQL); ");
            await _dbContext.ExecuteSqlCommandAsync("Delete Partners where Id != '" + partner_main.Id.ToString() + "' and Id != '" + partner_root.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("update Companies set AccountIncomeId = null, AccountExpenseId = null");
            await _dbContext.ExecuteSqlCommandAsync("Delete Companies where Id != '" + company_main.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("Delete AspNetUsers where Id != '" + user_root.Id.ToString() + "'");
            //set constraint
            await _dbContext.ExecuteSqlCommandAsync("EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");

            await irModelDataObj.CreateAsync(new IRModelData
            {
                Name = "main_partner",
                Module = "base",
                Model = "res.partner",
                ResId = partner_main.Id.ToString(),
            });

            await irModelDataObj.CreateAsync(new IRModelData
            {
                Name = "main_company",
                Module = "base",
                Model = "res.company",
                ResId = company_main.Id.ToString(),
            });

            await irModelDataObj.CreateAsync(new IRModelData
            {
                Name = "partner_root",
                Module = "base",
                Model = "res.partner",
                ResId = partner_root.Id.ToString(),
            });

            await irModelDataObj.CreateAsync(new IRModelData
            {
                Name = "user_root",
                Module = "base",
                Model = "res.users",
                ResId = user_root.Id,
            });

            await companyObj.InsertModuleAccountData(company_main);
            await companyObj.InsertModuleStockData(company_main);
            await companyObj.InsertModuleProductData();
            await companyObj.InsertModuleDentalData();
            await groupObj.InsertSecurityData();
            await companyObj.InsertIrModelFieldData();

            var appRoleService = GetService<IApplicationRoleService>();
            await appRoleService.CreateBaseUserRole();
            //await irConfigParameterObj.SetParam("remove_sample_data", "True");
            await irConfigParameterObj.SetParam("import_sample_data", "Removed");
            //xóa cache
            _cache.Remove(_tenant != null ? _tenant.Hostname.ToLower() : "localhost");
            _cache.RemoveByPattern(_tenant != null ? _tenant.Hostname : "localhost");
        }

        public async Task OldSaleOrderPaymentProcessUpdate()
        {
            var orderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var moveObj = GetService<IAccountMoveService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var commissionSettlementObj = GetService<ICommissionSettlementService>();
            var orderPaymentObj = GetService<ISaleOrderPaymentService>();
            var journalObj = GetService<IAccountJournalService>();
            var accPaymentObj = GetService<IAccountPaymentService>();
            var move_ids = new List<Guid>().AsEnumerable();

            var payment_ids = new List<Guid>().AsEnumerable();

            var offset = 0;
            var limit = 100;

            //lấy các phiếu điều trị đã xác nhận và thanh toán
            var orders = await orderObj.SearchQuery(x => x.State != "draft" && (!x.IsQuotation.HasValue || x.IsQuotation == false))
                .Include(x => x.SaleOrderPaymentRels).ThenInclude(x => x.Payment)
                .Include(x => x.OrderLines)
                //.ThenInclude(x => x.SaleOrderLinePaymentRels).ThenInclude(x => x.Payment)
                //.Include(x => x.OrderLines).ThenInclude(x => x.PartnerCommissions)
                //.Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLineInvoice2Rels).ThenInclude(x => x.InvoiceLine)
                .OrderBy(x => x.DateCreated).Skip(offset).Take(limit)
                .ToListAsync();

            while (orders.Any())
            {
                var orderPaymentIds = orders.SelectMany(x => x.SaleOrderPaymentRels).Where(x => x.Payment.State == "posted").Select(x => x.PaymentId).Distinct().ToList();
                payment_ids = payment_ids.Union(orderPaymentIds);

                var orderLineIds = orders.SelectMany(x => x.OrderLines).Select(x => x.Id).ToList();
                var invoiceIds = amlObj.SearchQuery(x => x.SaleLineRels.Any(s => orderLineIds.Contains(s.OrderLineId))).Select(x => x.MoveId).Distinct().ToList();

                //var invoicePaymentIds = await accPaymentObj.SearchQuery(x => x.AccountMovePaymentRels.Any(s => invoiceIds.Contains(s.MoveId))).Select(x => x.Id).ToListAsync();
                //payment_ids = payment_ids.Union(invoicePaymentIds);

                //cancel payment
                if (payment_ids.Any())
                    await accPaymentObj.CancelAsync(payment_ids);

                //cancel invoice and unlink
                if (invoiceIds.Any())
                {
                    var invoices = await moveObj.ButtonDraft(invoiceIds);
                    await moveObj.DeleteAsync(invoices);
                }

                var orderLines = orders.SelectMany(x => x.OrderLines).ToList();
                //recompute lại phiếu điều trị
                saleLineObj._GetInvoiceAmount(orderLines);
                await saleLineObj.UpdateAsync(orderLines);

                orderObj._AmountAll(orders);
                await orderObj.UpdateAsync(orders);

                if (payment_ids.Any())
                {
                    var payments = await accPaymentObj.SearchQuery(x => payment_ids.Contains(x.Id))
                        .Include(x => x.SaleOrderPaymentRels).ThenInclude(x => x.SaleOrder)
                        .Include(x => x.SaleOrderLinePaymentRels).ToListAsync();

                    var newPaymensts = new List<SaleOrderPayment>();
                    foreach (var payment in payments)
                    {
                        if (payment.SaleOrderLinePaymentRels.Any() && payment.SaleOrderPaymentRels.Any())
                        {
                            var order = payment.SaleOrderPaymentRels.First().SaleOrder;
                            var salePayment = new SaleOrderPayment()
                            {
                                Amount = payment.Amount,
                                Date = payment.PaymentDate,
                                OrderId = order.Id,
                                CompanyId = order.CompanyId
                            };

                            foreach (var line in payment.SaleOrderLinePaymentRels)
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
                                JournalId = payment.JournalId,
                                Amount = payment.Amount
                            });

                            newPaymensts.Add(salePayment);
                        }
                    }

                    await orderPaymentObj.CreateAsync(newPaymensts);
                    await orderPaymentObj.ActionPayment(newPaymensts.Select(x => x.Id).ToList());

                    await accPaymentObj.DeleteAsync(payments);
                }

                offset += limit;

                orders = await orderObj.SearchQuery(x => x.State != "draft" && (!x.IsQuotation.HasValue || x.IsQuotation == false))
               .Include(x => x.SaleOrderPaymentRels).ThenInclude(x => x.Payment)
               .Include(x => x.OrderLines)
               //.ThenInclude(x => x.SaleOrderLinePaymentRels).ThenInclude(x => x.Payment)
               //.Include(x => x.OrderLines).ThenInclude(x => x.PartnerCommissions)
               //.Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLineInvoice2Rels).ThenInclude(x => x.InvoiceLine)
               .OrderBy(x => x.DateCreated).Skip(offset).Take(limit)
               .ToListAsync();
            }
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

        protected string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }
    }

    public class ProductStandardPriceTmp
    {
        public Product Product { get; set; }

        public decimal StandardPrice { get; set; }
    }
}
