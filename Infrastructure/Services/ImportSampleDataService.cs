using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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

        public ImportSampleDataService(IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor,
            CatalogDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public async Task ImportSampleData()
        {
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\importSampleData.xml");
            XElement xml = XElement.Load(file_path);
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSampleDataXml));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToString()));
            ImportSampleDataXml sampleData = (ImportSampleDataXml)serializer.Deserialize(memStream);

            var partnerCategObj = GetService<IPartnerCategoryService>();
            var partnerObj = GetService<IPartnerService>();
            var productCategObj = GetService<IProductCategoryService>();
            var productObj = GetService<IProductService>();
            var historyObj = GetService<IHistoryService>();
            var productStepObj = GetService<IProductStepService>();
            var partnerSourceObj = GetService<IPartnerSourceService>();

            var partner_category_dict = new Dictionary<string, PartnerCategory>();
            var partner_dict = new Dictionary<string, Partner>();
            var history_dict = new Dictionary<string, History>();
            var product_category_dict = new Dictionary<string, ProductCategory>();
            var product_dict = new Dictionary<string, Product>();
            var product_step_dict = new Dictionary<string, ProductStep>();

            var partner_source_dict = new Dictionary<string, PartnerSource>();

            if (sampleData != null && sampleData.Data != null && sampleData.Data.Record.Count > 0)
            {
                var record = sampleData.Data.Record;
                var uomObj = GetService<IUoMService>();
                var uom = await uomObj.DefaultUOM();
                foreach (var itemRecord in record.ToList())
                {
                    switch (itemRecord.Model)
                    {
                        case "res.partner.category":
                            var partnerCateg = new PartnerCategory();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        partnerCateg.Name = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            partner_category_dict.Add(itemRecord.Id, partnerCateg);
                            break;

                        case "res.partner":
                            var partner = new Partner();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        partner.Name = itemField.Text;
                                        break;
                                    case "email":
                                        partner.Email = itemField.Text;
                                        break;
                                    case "phone":
                                        partner.Phone = itemField.Text;
                                        break;
                                    case "gender":
                                        partner.Gender = itemField.Text;
                                        break;
                                    case "birth_day":
                                        partner.BirthDay = Int32.Parse(itemField.Text);
                                        break;
                                    case "birth_month":
                                        partner.BirthMonth = Int32.Parse(itemField.Text);
                                        break;
                                    case "birth_year":
                                        partner.BirthYear = Int32.Parse(itemField.Text);
                                        break;
                                    case "supplier":
                                        partner.Supplier = Boolean.Parse(itemField.Text);
                                        break;
                                    case "customer":
                                        partner.Customer = Boolean.Parse(itemField.Text);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            partner_dict.Add(itemRecord.Id, partner);
                            break;

                        case "product.category":

                            var productCategory = new ProductCategory();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        productCategory.Name = itemField.Text;
                                        break;
                                    case "type":
                                        productCategory.Type = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            product_category_dict.Add(itemRecord.Id, productCategory);
                            break;

                        case "product.product":
                            var product = new Product();
                            product.UOMId = uom.Id;
                            product.UOMPOId = uom.Id;
                            product.CompanyId = CompanyId;
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        product.Name = itemField.Text;
                                        break;
                                    case "list_price":
                                        product.ListPrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "labo_price":
                                        product.LaboPrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "purchase_price":
                                        product.PurchasePrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "is_labo":
                                        product.IsLabo = Boolean.Parse(itemField.Text);
                                        break;
                                    case "purchase_ok":
                                        product.PurchaseOK = Boolean.Parse(itemField.Text);
                                        break;
                                    case "sale_ok":
                                        product.SaleOK = Boolean.Parse(itemField.Text);
                                        break;
                                    case "ketoa_ok":
                                        product.KeToaOK = Boolean.Parse(itemField.Text);
                                        break;
                                    case "type":
                                        product.Type = itemField.Text;
                                        break;
                                    case "type_2":
                                        product.Type2 = itemField.Text;
                                        break;
                                    case "categ_id":
                                        product.Categ = product_category_dict[itemField.Ref];
                                        break;
                                    default:
                                        break;
                                }
                            }
                            product_dict.Add(itemRecord.Id, product);
                            break;

                        case "product.step":
                            var productStep = new ProductStep();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                if (itemField.Name == "name")
                                    productStep.Name = itemField.Text;
                                if (itemField.Name == "product_id")
                                    productStep.Product = product_dict[itemField.Ref];
                                if (itemField.Name == "order")
                                    productStep.Order = Convert.ToInt32(itemField.Text);
                            }
                            product_step_dict.Add(itemRecord.Id, productStep);
                            break;

                        case "history":
                            var history = new History();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        history.Name = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            history_dict.Add(itemRecord.Id, history);
                            break;
                        case "res.partner.source":
                            var source = new PartnerSource();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        source.Name = itemField.Text;
                                        break;
                                    case "type":
                                        source.Type = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            partner_source_dict.Add(itemRecord.Id, source);
                            break;

                        default:
                            break;
                    }

                }

                await partnerCategObj.CreateAsync(partner_category_dict.Values.ToList());
                await partnerObj.CreateAsync(partner_dict.Values.ToList());
                await historyObj.CreateAsync(history_dict.Values.ToList());
                await productCategObj.CreateAsync(product_category_dict.Values.ToList());
                await productObj.CreateAsync(product_dict.Values.ToList());
                await productStepObj.CreateAsync(product_step_dict.Values.ToList());
                await partnerSourceObj.CreateAsync(partner_source_dict.Values);
            }
        }

        public async Task DeleteSampleData()
        {
            var irConfigParameterObj = GetService<IIrConfigParameterService>();
            var userObj = GetService<IUserService>();
            var valueRemove = await irConfigParameterObj.GetParam("remove_sample_data");
            var valueImport = await irConfigParameterObj.GetParam("import_sample_data");
            var user = await userObj.GetCurrentUser();

            if (!user.IsUserRoot)
                throw new Exception("Chỉ có admin mới có thể thực hiện chức năng này !!!");
            if (valueImport == "Installed" && valueRemove == "True")
                throw new Exception("Xóa dữ liệu mẫu chỉ được phép xóa một lần duy nhất !!!");
            if (valueImport != "Installed" && valueRemove != "True")
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

            await _dbContext.ExecuteSqlCommandAsync("update AccountJournals set DefaultCreditAccountId = null, DefaultDebitAccountId = null");
            await _dbContext.ExecuteSqlCommandAsync("update Companies set AccountIncomeId = null, AccountExpenseId = null");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountPartialReconciles");
            await _dbContext.ExecuteSqlCommandAsync("Delete IRProperties");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookConnectPages");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookConnects");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookMassMessagings");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookMessagingTraces");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookPages");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookScheduleAppointmentConfigs");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookTags");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookUserProfiles");
            await _dbContext.ExecuteSqlCommandAsync("Delete FacebookUserProfileTagRels");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountFullReconciles");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountMoveLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountMoves");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountAccounts");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountAccountTypes");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountPayments");
            await _dbContext.ExecuteSqlCommandAsync("Delete AccountJournals");
            await _dbContext.ExecuteSqlCommandAsync("Delete PhieuThuChis");
            await _dbContext.ExecuteSqlCommandAsync("Delete LoaiThuChis");
            await _dbContext.ExecuteSqlCommandAsync("Delete DotKhamSteps");
            await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrderLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrderLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete LaboOrders");
            await _dbContext.ExecuteSqlCommandAsync("Delete ToaThuocLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete ToaThuocs");
            await _dbContext.ExecuteSqlCommandAsync("Delete SamplePrescriptionLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete SamplePrescriptions");
            await _dbContext.ExecuteSqlCommandAsync("Delete DotKhams");
            await _dbContext.ExecuteSqlCommandAsync("Delete SaleCoupons");
            await _dbContext.ExecuteSqlCommandAsync("Delete SaleCouponPrograms");
            await _dbContext.ExecuteSqlCommandAsync("Delete SaleOrders");
            await _dbContext.ExecuteSqlCommandAsync("Delete StockMoves");
            await _dbContext.ExecuteSqlCommandAsync("Delete StockQuants");
            await _dbContext.ExecuteSqlCommandAsync("Delete StockPickings");
            await _dbContext.ExecuteSqlCommandAsync("Delete PurchaseOrderLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete PurchaseOrders");
            await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardCards");
            await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrderLines");
            await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardOrders");
            await _dbContext.ExecuteSqlCommandAsync("Delete ServiceCardTypes");
            await _dbContext.ExecuteSqlCommandAsync("Delete CardCards");
            await _dbContext.ExecuteSqlCommandAsync("Delete CardTypes");
            await _dbContext.ExecuteSqlCommandAsync("Delete ProductSteps");
            await _dbContext.ExecuteSqlCommandAsync("Delete Products");
            await _dbContext.ExecuteSqlCommandAsync("Delete ProductCategories");
            await _dbContext.ExecuteSqlCommandAsync("Delete Appointments");
            await _dbContext.ExecuteSqlCommandAsync("update StockPickingTypes set DefaultLocationSrcId = null, DefaultLocationDestId = null, WarehouseId = null");
            await _dbContext.ExecuteSqlCommandAsync("Delete StockWarehouses");
            await _dbContext.ExecuteSqlCommandAsync("Delete StockLocations");
            await _dbContext.ExecuteSqlCommandAsync("Delete StockPickingTypes");
            await _dbContext.ExecuteSqlCommandAsync("Delete IRSequences");
            await _dbContext.ExecuteSqlCommandAsync("update Partners set CompanyId = null, CreatedById = null, WriteById = null where Id != '" + partner_main.Id.ToString() + "' and Id != '" + partner_root.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("Delete PartnerCategories");
            await _dbContext.ExecuteSqlCommandAsync("Delete Histories");
            await _dbContext.ExecuteSqlCommandAsync("Update IrConfigParameters set CreatedById = null , WriteById = null");
            await _dbContext.ExecuteSqlCommandAsync("Delete IrConfigParameters");
            await _dbContext.ExecuteSqlCommandAsync("Delete ResConfigSettings");
            await _dbContext.ExecuteSqlCommandAsync("Delete IrModuleCategories");
            await _dbContext.ExecuteSqlCommandAsync("Update Companies set CreatedById = null , WriteById = null where Id !='" + company_main.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("Delete AspNetUsers where Id != '" + user_root.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("Delete Companies where Id != '" + company_main.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("Delete Partners where Id != '" + partner_main.Id.ToString() + "' and Id != '" + partner_root.Id.ToString() + "'");
            await _dbContext.ExecuteSqlCommandAsync("Delete IRModelAccesses");
            await _dbContext.ExecuteSqlCommandAsync("Delete PartnerSources");
            await _dbContext.ExecuteSqlCommandAsync("Delete ResGroups");
            await _dbContext.ExecuteSqlCommandAsync("Delete IRRules");
            await _dbContext.ExecuteSqlCommandAsync("Delete IRModels");
            await _dbContext.ExecuteSqlCommandAsync("Delete ProductPricelists");
            await _dbContext.ExecuteSqlCommandAsync("Delete UoMs");
            await _dbContext.ExecuteSqlCommandAsync("Delete UoMCategories");
            await _dbContext.ExecuteSqlCommandAsync("Delete Teeth");
            await _dbContext.ExecuteSqlCommandAsync("Delete ToothCategories");
            await _dbContext.ExecuteSqlCommandAsync("Delete IrAttachments");
            await _dbContext.ExecuteSqlCommandAsync("Delete IRModelDatas");

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
            await irConfigParameterObj.SetParam("remove_sample_data", "True");
            await irConfigParameterObj.SetParam("import_sample_data", "Installed");
        }

        public async Task OldSaleOrderPaymentProcessUpdate()
        {
            var orderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var moveObj = GetService<IAccountMoveService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var orderPaymentObj = GetService<ISaleOrderPaymentService>();
            var journalObj = GetService<IAccountJournalService>();
            var accPaymentObj = GetService<IAccountPaymentService>();
            var move_ids = new List<Guid>().AsEnumerable();

            var payment_ids = new List<Guid>().AsEnumerable();

            ///tìm các chi nhánh của tenant
            var companies = _dbContext.Companies.ToList();

            foreach (var company in companies)
            {
                //lấy các phiếu điều trị đã xác nhận và thanh toán
                var orders = await orderObj.SearchQuery(x => x.CompanyId == company.Id && x.State != "draft")
                    .Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLinePaymentRels).ThenInclude(x => x.Payment)
                    .Include(x => x.OrderLines).ThenInclude(x => x.PartnerCommissions)
                    .Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLineInvoice2Rels)
                    .ToListAsync();

                //vòng lặp các phiếu điều trị xóa các hóa đơn 
                foreach (var order in orders)
                {
                    payment_ids = await accPaymentObj.SearchQuery(x => x.SaleOrderPaymentRels.Any(s => s.SaleOrderId == order.Id)).Select(x => x.Id).ToListAsync();

                    ///cancel các payment cũ
                    if (payment_ids.Any())
                        await accPaymentObj.CancelAsync(payment_ids);


                    var mIds = await amlObj.SearchQuery(x => x.SaleLineRels.Any(s => s.OrderLine.OrderId == order.Id)).Select(x => x.MoveId).Distinct().ToListAsync();
                    move_ids = move_ids.Union(mIds);

                    ///xóa hóa đơn doanh thu của phiếu điều trị khi xác nhận
                    await orderObj.UpdateAsync(order);

                    if (move_ids.Any())
                    {
                        await moveObj.ButtonDraft(move_ids);

                        await moveObj.Unlink(move_ids);
                    }

                    foreach (var line in order.OrderLines)
                    {
                        if (line.State == "cancel")
                            continue;

                        if (line.SaleOrderLinePaymentRels.Any())
                        {                          
                            await linePaymentRelObj.DeleteAsync(line.SaleOrderLinePaymentRels);
                            line.AmountPaid = 0;
                            line.AmountResidual = 0;
                        }

                    }


                    saleLineObj._GetInvoiceQty(order.OrderLines);
                    saleLineObj._GetToInvoiceQty(order.OrderLines);
                    saleLineObj._GetInvoiceAmount(order.OrderLines);
                    saleLineObj._GetToInvoiceAmount(order.OrderLines);
                    saleLineObj._ComputeInvoiceStatus(order.OrderLines);
                    await saleLineObj._RemovePartnerCommissions(order.OrderLines.Select(x => x.Id).ToList());
                    order.Residual = 0;

                    orderObj._GetInvoiced(new List<SaleOrder>() { order });
                    await orderObj.UpdateAsync(order);

                    //tạo thanh toán phiếu điều trị 
                    //vòng lặp các thanh toán của phiếu điều trị 
                    if (payment_ids.Any())
                    {
                        var accountPayments = accPaymentObj.SearchQuery(x => payment_ids.Contains(x.Id) && x.State == "Cancel").Include(x => x.SaleOrderLinePaymentRels).ToList();
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


                            await orderPaymentObj.CreateAsync(salePayment);
                            await orderPaymentObj.ActionPayment(new List<Guid>() { salePayment.Id });
                        }

                        await accPaymentObj.DeleteAsync(accountPayments);

                    }


                }

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
}
