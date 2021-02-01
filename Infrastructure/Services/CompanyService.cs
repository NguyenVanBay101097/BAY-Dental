using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using CsvHelper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class CompanyService : BaseService<Company>, ICompanyService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompanyService(IAsyncRepository<Company> repository, IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        : base(repository, httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task SetupCompany(string companyName, string userName, string email, string password, string name = "")
        {
            //var dbContext = GetService<CatalogDbContext>();
            //await dbContext.Database.MigrateAsync();
            var partnerObj = GetService<IPartnerService>();
            var companyObj = GetService<ICompanyService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var groupObj = GetService<IResGroupService>();

            //tạo công ty và user_root
            var mainPartner = new Partner
            {
                Name = companyName,
                Customer = false,
                Email = email,
            };

            await partnerObj.CreateAsync(mainPartner);

            await modelDataObj.CreateAsync(new IRModelData
            {
                Name = "main_partner",
                Module = "base",
                Model = "res.partner",
                ResId = mainPartner.Id.ToString(),
            });

            var mainCompany = new Company
            {
                Name = companyName,
                Partner = mainPartner
            };

            await companyObj.CreateAsync(mainCompany);

            await modelDataObj.CreateAsync(new IRModelData
            {
                Name = "main_company",
                Module = "base",
                Model = "res.company",
                ResId = mainCompany.Id.ToString(),
            });

            var partnerRoot = new Partner
            {
                Name = !string.IsNullOrEmpty(name) ? name : userName,
                Company = mainCompany,
                Customer = false,
                Email = email,
            };

            await partnerObj.CreateAsync(partnerRoot);

            await modelDataObj.CreateAsync(new IRModelData
            {
                Name = "partner_root",
                Module = "base",
                Model = "res.partner",
                ResId = partnerRoot.Id.ToString(),
            });

            var userRoot = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                Company = mainCompany,
                CompanyId = mainCompany.Id,
                Partner = partnerRoot,
                PartnerId = partnerRoot.Id,
                Name = partnerRoot.Name,
                IsUserRoot = true,
            };

            userRoot.ResCompanyUsersRels.Add(new ResCompanyUsersRel { Company = mainCompany });

            var userObj = GetService<UserManager<ApplicationUser>>();
            await userObj.CreateAsync(userRoot, password);
            //await _userManager.CreateAsync(userRoot, password);

            await modelDataObj.CreateAsync(new IRModelData
            {
                Name = "user_root",
                Module = "base",
                Model = "res.users",
                ResId = userRoot.Id,
            });

            mainPartner.Company = mainCompany;
            await partnerObj.UpdateAsync(mainPartner);

            await InsertModuleAccountData(mainCompany);

            await InsertModuleStockData(mainCompany);

            await InsertModuleProductData();

            await InsertModuleDentalData();

            await groupObj.InsertSecurityData();

            //insert những irmodelfield
            await InsertIrModelFieldData();
            await AddIrDataForSurvey();
        }

        public async Task AddIrDataForSurvey()
        {
            var groupObj = GetService<IResGroupService>();
            await groupObj.AddMissingIrDataForSurvey();
        }

        public async Task InsertIrModelFieldData()
        {
            var fieldObj = GetService<IIRModelFieldService>();
            var modelObj = GetService<IIRModelService>();

            var model = await modelObj.SearchQuery(x => x.Model == "Product").FirstOrDefaultAsync();
            var toAdd = new List<IRModelField>();

            toAdd.Add(new IRModelField
            {
                IRModelId = model.Id,
                Model = "product.product",
                Name = "list_price",
                TType = "decimal",
            });

            toAdd.Add(new IRModelField
            {
                IRModelId = model.Id,
                Model = "product.product",
                Name = "standard_price",
                TType = "float",
            });

            await fieldObj.CreateAsync(toAdd);
        }

        public async Task InsertCompanyData(Company company)
        {
            await CreateAccountData(company);

            await CreateStockData(company);
        }

        private async Task CreateStockData(Company company)
        {
            #region StockWarehouse
            var whObj = GetService<IStockWarehouseService>();
            var wh = await whObj.CreateAsync(new StockWarehouse
            {
                Name = company.Name,
                Code = "WH",
                CompanyId = company.Id,
            });

            #endregion
        }

        private async Task CreateAccountData(Company company)
        {
            #region Account
            var accountObj = GetService<IAccountAccountService>();
            var irModelDataObj = GetService<IIRModelDataService>();
            #region For receiableAccType
            var receivableAccType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_receivable");
            var creadiorsAcc = new AccountAccount
            {
                Name = "Phải thu của khách hàng",
                Code = "131",
                InternalType = receivableAccType.Type,
                UserTypeId = receivableAccType.Id,
                Reconcile = true,
                CompanyId = company.Id,
            };
            #endregion

            #region For cashBankAccType
            var cashBankAccType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_liquidity");
            var cashAcc = new AccountAccount
            {
                Name = "Tiền mặt",
                Code = "1111",
                InternalType = cashBankAccType.Type,
                UserTypeId = cashBankAccType.Id,
                CompanyId = company.Id,
            };

            var bankAcc = new AccountAccount
            {
                Name = "Ngân hàng",
                Code = "1112",
                InternalType = cashBankAccType.Type,
                UserTypeId = cashBankAccType.Id,
                CompanyId = company.Id,
            };
            #endregion

            #region for PayrollDiary
            var currentLiabilities = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
            var acc334 = new AccountAccount
            {
                Name = "Phải trả người lao động",
                Code = "334",
                InternalType = currentLiabilities.Type,
                UserTypeId = currentLiabilities.Id,
                CompanyId = company.Id,
            };
            var expensesType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_expenses");
            var acc642 = new AccountAccount
            {
                Name = "Chi phí quản lý doanh nghiệp",
                Code = "642",
                InternalType = expensesType.Type,
                UserTypeId = expensesType.Id,
                CompanyId = company.Id,
            };
            #endregion

            #region For payableAccType
            var payableAccType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_payable");
            var debtorsAcc = new AccountAccount
            {
                Name = "Phải trả người bán",
                Code = "331",
                InternalType = payableAccType.Type,
                UserTypeId = payableAccType.Id,
                Reconcile = true,
                CompanyId = company.Id,
            };
            #endregion

            #region For incomeAccType
            var incomeAccType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_revenue");
            var incomeAcc = new AccountAccount
            {
                Name = "Doanh thu bán hàng hóa",
                Code = "5111",
                InternalType = incomeAccType.Type,
                UserTypeId = incomeAccType.Id,
                CompanyId = company.Id,
            };
            #endregion

            #region For expenseAccType
            var expenseAccType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_expenses");
            var expenseAccount = new AccountAccount
            {
                Name = "Giá vốn bán hàng",
                Code = "632",
                InternalType = expenseAccType.Type,
                UserTypeId = expenseAccType.Id,
                CompanyId = company.Id,
            };

            #endregion

            #region For currentAssetsType
            var currentAssetsType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_assets");

            var acc1561 = new AccountAccount
            {
                Name = "Giá mua hàng hoá",
                Code = "1561",
                InternalType = currentAssetsType.Type,
                UserTypeId = currentAssetsType.Id,
                CompanyId = company.Id,
            };

            #endregion



            await accountObj.CreateAsync(new List<AccountAccount>() { creadiorsAcc, debtorsAcc, cashAcc, bankAcc, incomeAcc, expenseAccount, acc1561, acc334, acc642 });

            #endregion

            #region AccountJournal
            var journalObj = GetService<IAccountJournalService>();
            var cashJournal = new AccountJournal
            {
                Name = "Tiền mặt",
                Type = "cash",
                UpdatePosted = true,
                Code = "CSH1",
                DefaultDebitAccountId = cashAcc.Id,
                DefaultCreditAccountId = cashAcc.Id,
                CompanyId = company.Id,
            };

            var bankJournal = new AccountJournal
            {
                Name = "Ngân hàng",
                Type = "bank",
                UpdatePosted = true,
                Code = "CSH2",
                DefaultDebitAccountId = bankAcc.Id,
                DefaultCreditAccountId = bankAcc.Id,
                CompanyId = company.Id,
            };

            var saleJournal = new AccountJournal
            {
                Name = "Nhật ký bán hàng",
                Type = "sale",
                UpdatePosted = true,
                DedicatedRefund = true,
                Code = "INV",
                DefaultDebitAccountId = incomeAcc.Id,
                DefaultCreditAccountId = incomeAcc.Id,
                CompanyId = company.Id,
            };

            var purchaseJournal = new AccountJournal
            {
                Name = "Nhật ký mua hàng",
                Type = "purchase",
                UpdatePosted = true,
                DedicatedRefund = true,
                Code = "BILL",
                DefaultDebitAccountId = acc1561.Id,
                DefaultCreditAccountId = acc1561.Id,
                CompanyId = company.Id,
            };

            var salaryJournal = new AccountJournal
            {
                Name = "Nhật ký lương",
                Type = "payroll",
                UpdatePosted = true,
                Code = "SALARY",
                DefaultDebitAccountId = acc642.Id,
                DefaultCreditAccountId = acc334.Id,
                CompanyId = company.Id,
            };

            await journalObj.CreateAsync(new List<AccountJournal>() { cashJournal, bankJournal, saleJournal, purchaseJournal, salaryJournal });

            #endregion
        }

        public async Task InsertModuleAccountData(Company main_company)
        {
            var account_account_dict = new Dictionary<string, AccountAccount>();
            var account_type_dict = new Dictionary<string, AccountAccountType>();
            var account_journal_dict = new Dictionary<string, AccountJournal>();
            var account_sequence_dict = new Dictionary<string, IRSequence>();
            var account_data_file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\account_data.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(account_data_file_path);
            var records = doc.GetElementsByTagName("record");
            for (var i = 0; i < records.Count; i++)
            {
                XmlElement record = (XmlElement)records[i];
                var model = record.GetAttribute("model");
                var id = "account." + record.GetAttribute("id");
                if (model == "ir.sequence")
                {
                    var seq = new IRSequence();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            seq.Name = field.InnerText;
                        }
                        else if (field_name == "code")
                        {
                            seq.Code = field.InnerText;
                        }
                        else if (field_name == "prefix")
                        {
                            seq.Prefix = field.InnerText;
                        }
                        else if (field_name == "number_next")
                        {
                            seq.NumberNext = XmlConvert.ToInt32(field.GetAttribute("eval"));
                        }
                        else if (field_name == "number_increment")
                        {
                            seq.NumberIncrement = XmlConvert.ToInt32(field.GetAttribute("eval"));
                        }
                        else if (field_name == "padding")
                        {
                            seq.Padding = XmlConvert.ToInt32(field.InnerText);
                        }
                    }
                    account_sequence_dict.Add(id, seq);
                }
                else if (model == "account.account.type")
                {
                    var account_type = new AccountAccountType();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            account_type.Name = field.InnerText;
                        }
                        else if (field_name == "type")
                        {
                            account_type.Type = field.InnerText;
                        }
                        else if (field_name == "include_initial_balance")
                        {
                            account_type.IncludeInitialBalance = Boolean.Parse(field.GetAttribute("eval"));
                        }
                    }
                    account_type_dict.Add(id, account_type);
                }
                else if (model == "account.account")
                {
                    var account = new AccountAccount()
                    {
                        Company = main_company,
                    };
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            account.Name = field.InnerText;
                        }
                        else if (field_name == "code")
                        {
                            account.Code = field.InnerText;
                        }
                        else if (field_name == "user_type_id")
                        {
                            var user_type = account_type_dict[field.GetAttribute("ref")];
                            account.UserType = user_type;
                            account.InternalType = user_type.Type;
                        }
                        else if (field_name == "reconcile")
                        {
                            account.Reconcile = bool.Parse(field.GetAttribute("eval"));
                        }
                    }
                    account_account_dict.Add(id, account);
                }
                else if (model == "account.journal")
                {
                    var journal = new AccountJournal()
                    {
                        Company = main_company,
                        CompanyId = main_company.Id,
                    };
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            journal.Name = field.InnerText;
                        }
                        else if (field_name == "type")
                        {
                            journal.Type = field.InnerText;
                        }
                        else if (field_name == "code")
                        {
                            journal.Code = field.InnerText;
                        }
                        else if (field_name == "update_posted")
                        {
                            journal.UpdatePosted = bool.Parse(field.GetAttribute("eval"));
                        }
                        else if (field_name == "dedicated_refund")
                        {
                            journal.DedicatedRefund = bool.Parse(field.GetAttribute("eval"));
                        }
                        else if (field_name == "default_debit_account_id")
                        {
                            journal.DefaultDebitAccount = account_account_dict[field.GetAttribute("ref")];
                        }
                        else if (field_name == "default_credit_account_id")
                        {
                            journal.DefaultCreditAccount = account_account_dict[field.GetAttribute("ref")];
                        }
                    }
                    account_journal_dict.Add(id, journal);
                }
            }

            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(account_sequence_dict.Values);

            var accountTypeObj = GetService<IAccountAccountTypeService>();
            await accountTypeObj.CreateAsync(account_type_dict.Values);

            var accountObj = GetService<IAccountAccountService>();
            await accountObj.CreateAsync(account_account_dict.Values);

            var journalObj = GetService<IAccountJournalService>();
            await journalObj.CreateAsync(account_journal_dict.Values);

            //update account income and account expense cho company
            var accountIncome = account_account_dict["account.data_account_5111"];
            var accountExpense = account_account_dict["account.data_account_632"];
            main_company.AccountIncomeId = accountIncome.Id;
            main_company.AccountExpenseId = accountExpense.Id;
            await UpdateAsync(main_company);

            var modelDatas = new List<IRModelData>();
            modelDatas.AddRange(PrepareModelData(account_type_dict, "account.account.type"));
            var modelDataObj = GetService<IIRModelDataService>();
            await modelDataObj.CreateAsync(modelDatas);
        }

        public async Task InsertModuleStockData(Company main_company)
        {
            var stock_locations_dict = new Dictionary<string, StockLocation>();
            var stock_warehouses_dict = new Dictionary<string, StockWarehouse>();
            var stock_sequence_dict = new Dictionary<string, IRSequence>();
            var account_data_file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\stock_data.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(account_data_file_path);
            var records = doc.GetElementsByTagName("record");
            for (var i = 0; i < records.Count; i++)
            {
                XmlElement record = (XmlElement)records[i];
                var model = record.GetAttribute("model");
                var id = "stock." + record.GetAttribute("id");
                if (model == "stock.location")
                {
                    var loc = new StockLocation();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            loc.Name = field.InnerText;
                        }
                        else if (field_name == "usage")
                        {
                            loc.Usage = field.InnerText;
                        }
                        else if (field_name == "scrap_location")
                        {
                            loc.ScrapLocation = bool.Parse(field.InnerText);
                        }
                        else if (field_name == "location_id")
                        {
                            loc.ParentLocation = stock_locations_dict["stock." + field.GetAttribute("ref")];
                        }
                    }
                    stock_locations_dict.Add(id, loc);
                }
                else if (model == "stock.warehouse")
                {
                    var wh = new StockWarehouse
                    {
                        Name = main_company.Name,
                        Company = main_company,
                        CompanyId = main_company.Id,
                        PartnerId = main_company.PartnerId
                    };
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "code")
                        {
                            wh.Code = field.InnerText;
                        }
                    }
                    stock_warehouses_dict.Add(id, wh);
                }
                if (model == "ir.sequence")
                {
                    var seq = new IRSequence();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            seq.Name = field.InnerText;
                        }
                        else if (field_name == "code")
                        {
                            seq.Code = field.InnerText;
                        }
                        else if (field_name == "prefix")
                        {
                            seq.Prefix = field.InnerText;
                        }
                        else if (field_name == "padding")
                        {
                            seq.Padding = XmlConvert.ToInt32(field.InnerText);
                        }
                    }
                    stock_sequence_dict.Add(id, seq);
                }
            }

            var locObj = GetService<IStockLocationService>();
            await locObj.CreateAsync(stock_locations_dict.Values);

            var modelDatas = new List<IRModelData>();
            modelDatas.AddRange(PrepareModelData(stock_locations_dict, "stock.location"));
            var modelDataObj = GetService<IIRModelDataService>();
            await modelDataObj.CreateAsync(modelDatas);

            var whObj = GetService<IStockWarehouseService>();
            foreach (var wh in stock_warehouses_dict.Values)
            {
                await whObj.CreateAsync(wh);
            }

            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(stock_sequence_dict.Values);
        }

        public async Task InsertModuleProductData()
        {
            var product_category_dict = new Dictionary<string, ProductCategory>();
            var product_uom_categ_dict = new Dictionary<string, UoMCategory>();
            var product_uom_dict = new Dictionary<string, UoM>();
            var product_pricelist_dict = new Dictionary<string, ProductPricelist>();
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\product_data.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(file_path);
            var records = doc.GetElementsByTagName("record");
            for (var i = 0; i < records.Count; i++)
            {
                XmlElement record = (XmlElement)records[i];
                var model = record.GetAttribute("model");
                var id = "product." + record.GetAttribute("id");
                if (model == "product.category")
                {
                    var category = new ProductCategory();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            category.Name = field.InnerText;
                        }
                        else if (field_name == "parent_id")
                        {
                            category.Parent = product_category_dict["product." + field.GetAttribute("ref")];
                        }
                    }
                    product_category_dict.Add(id, category);
                }
                else if (model == "product.uom.categ")
                {
                    var uom_categ = new UoMCategory();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            uom_categ.Name = field.InnerText;
                        }
                        else if (field_name == "measure_type")
                        {
                            uom_categ.MeasureType = field.InnerText;
                        }
                    }
                    product_uom_categ_dict.Add(id, uom_categ);
                }
                else if (model == "product.uom")
                {
                    var uom = new UoM();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            uom.Name = field.InnerText;
                        }
                        else if (field_name == "category_id")
                        {
                            var categ = product_uom_categ_dict["product." + field.GetAttribute("ref")];
                            uom.Category = categ;
                            uom.MeasureType = categ.MeasureType;
                        }
                        else if (field_name == "factor")
                        {
                            uom.Factor = XmlConvert.ToDouble(field.GetAttribute("eval"));
                        }
                        else if (field_name == "rounding")
                        {
                            uom.Rounding = XmlConvert.ToDecimal(field.GetAttribute("eval"));
                        }
                        else if (field_name == "factor_inv")
                        {
                            uom.UOMType = "bigger";
                            uom.Factor = 1 / XmlConvert.ToDouble(field.GetAttribute("eval"));
                        }
                        else if (field_name == "uom_type")
                        {
                            uom.UOMType = field.InnerText;
                        }
                    }

                    product_uom_dict.Add(id, uom);
                }
                else if (model == "product.pricelist")
                {
                    var pricelist = new ProductPricelist();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            pricelist.Name = field.InnerText;
                        }
                        else if (field_name == "sequence")
                        {
                            pricelist.Sequence = XmlConvert.ToInt32(field.InnerText);
                        }
                    }

                    product_pricelist_dict.Add(id, pricelist);
                }
            }

            var productCategoryObj = GetService<IProductCategoryService>();
            await productCategoryObj.CreateAsync(product_category_dict.Values);

            var productUOMCategObj = GetService<IUoMCategoryService>();
            await productUOMCategObj.CreateAsync(product_uom_categ_dict.Values);

            var productUOMObj = GetService<IUoMService>();
            await productUOMObj.CreateAsync(product_uom_dict.Values);

            var pricelistObj = GetService<IProductPricelistService>();
            await pricelistObj.CreateAsync(product_pricelist_dict.Values);

            var modelDatas = new List<IRModelData>();
            modelDatas.AddRange(PrepareModelData(product_uom_dict, "uom"));
            var modelDataObj = GetService<IIRModelDataService>();
            await modelDataObj.CreateAsync(modelDatas);
        }

        public async Task InsertModuleDentalData()
        {
            var tooth_category_dict = new Dictionary<string, ToothCategory>();
            var tooth_dict = new Dictionary<string, Tooth>();
            var sequence_dict = new Dictionary<string, IRSequence>();
            var partner_title_dict = new Dictionary<string, PartnerTitle>();

            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\dental_data.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(file_path);
            var records = doc.GetElementsByTagName("record");
            for (var i = 0; i < records.Count; i++)
            {
                XmlElement record = (XmlElement)records[i];
                var model = record.GetAttribute("model");
                var id = record.GetAttribute("id");
                if (model == "tooth.category")
                {
                    var category = new ToothCategory();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            category.Name = field.InnerText;
                        }
                        else if (field_name == "sequence")
                        {
                            category.Sequence = XmlConvert.ToInt32(field.GetAttribute("eval"));
                        }
                    }
                    tooth_category_dict.Add(id, category);
                }
                else if (model == "partner.title")
                {
                    var partnerTitle = new PartnerTitle();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            partnerTitle.Name = field.InnerText;
                        }
                    }
                    partner_title_dict.Add(id, partnerTitle);
                }
                else if (model == "tooth")
                {
                    var tooth = new Tooth();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            tooth.Name = field.InnerText;
                        }
                        else if (field_name == "category_id")
                        {
                            var categ = tooth_category_dict[field.GetAttribute("ref")];
                            tooth.Category = categ;
                        }
                        else if (field_name == "vi_tri_ham")
                        {
                            tooth.ViTriHam = field.InnerText;
                        }
                        else if (field_name == "position")
                        {
                            tooth.Position = field.InnerText;
                        }
                    }

                    tooth_dict.Add(id, tooth);
                }
                else if (model == "ir.sequence")
                {
                    var seq = new IRSequence();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                        {
                            seq.Name = field.InnerText;
                        }
                        else if (field_name == "code")
                        {
                            seq.Code = field.InnerText;
                        }
                        else if (field_name == "prefix")
                        {
                            seq.Prefix = field.InnerText;
                        }
                        else if (field_name == "number_next")
                        {
                            seq.NumberNext = XmlConvert.ToInt32(field.InnerText);
                        }
                        else if (field_name == "number_increment")
                        {
                            seq.NumberIncrement = XmlConvert.ToInt32(field.InnerText);
                        }
                        else if (field_name == "padding")
                        {
                            seq.Padding = XmlConvert.ToInt32(field.InnerText);
                        }
                    }
                    sequence_dict.Add(id, seq);
                }
            }

            var toothCategoryObj = GetService<IToothCategoryService>();
            await toothCategoryObj.CreateAsync(tooth_category_dict.Values);

            var toothObj = GetService<IToothService>();
            await toothObj.CreateAsync(tooth_dict.Values);

            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(sequence_dict.Values);

            var partnerTitlte = GetService<IPartnerTitleService>();
            await partnerTitlte.CreateAsync(partner_title_dict.Values);

            var modelDataObj = GetService<IIRModelDataService>();
            await modelDataObj.CreateAsync(PrepareModelData(partner_title_dict, "res.partner.title"));

        }

        private IEnumerable<IRModelData> PrepareModelData<T>(IDictionary<string, T> dict, string model) where T : BaseEntity
        {
            var res = new List<IRModelData>();
            foreach (var item in dict)
            {
                var tmps = item.Key.Split('.');
                res.Add(new IRModelData
                {
                    Module = tmps[0],
                    Name = tmps[1],
                    Model = model,
                    ResId = item.Value.Id.ToString()
                });
            }

            return res;
        }

        public async Task SetupTenant(CompanySetupTenant val)
        {
            try
            {
                await SetupCompany(val.CompanyName, val.Username, val.Email, val.Password, name: val.Name);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task InsertSecurityData()
        {
            var modelDict = await InsertModels();
            var groupDict = new Dictionary<string, ResGroup>();
            var ruleDict = new Dictionary<string, IRRule>();
            var accessDict = new Dictionary<string, IRModelAccess>();

            async Task InsertGroupsRules()
            {
                var xml_file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\dental_security.xml");
                if (!File.Exists(xml_file_path))
                    return;
                XmlDocument doc = new XmlDocument();

                var groupObj = GetService<IResGroupService>();
                var ruleObj = GetService<IIRRuleService>();

                doc.Load(xml_file_path);
                var records = doc.GetElementsByTagName("record");
                for (var i = 0; i < records.Count; i++)
                {
                    var errs = new List<string>();
                    XmlElement record = (XmlElement)records[i];
                    var model = record.GetAttribute("model");
                    var id = record.GetAttribute("id");
                    if (model == "res.groups")
                    {
                        var mdl = new ResGroup();
                        var fields = record.GetElementsByTagName("field");
                        for (var j = 0; j < fields.Count; j++)
                        {
                            XmlElement field = (XmlElement)fields[j];
                            var field_name = field.GetAttribute("name");
                            if (field_name == "name")
                            {
                                mdl.Name = field.InnerText;
                            }
                        }
                        groupDict.Add(id, mdl);
                    }
                    else if (model == "ir.rule")
                    {
                        var rule = new IRRule();
                        rule.Code = id;
                        var fields = record.GetElementsByTagName("field");
                        for (var j = 0; j < fields.Count; j++)
                        {
                            XmlElement field = (XmlElement)fields[j];
                            var field_name = field.GetAttribute("name");
                            if (field_name == "name")
                            {
                                rule.Name = field.InnerText;
                            }
                            else if (field_name == "model_id")
                            {
                                var vals = field.GetAttribute("ref");
                                rule.Model = modelDict[vals];
                            }
                            else if (field_name == "global")
                            {
                                var vals = field.GetAttribute("eval");
                                rule.Global = Boolean.Parse(vals);
                            }
                        }
                        ruleDict.Add(id, rule);
                    }
                }

                await groupObj.CreateAsync(groupDict.Values);
                await ruleObj.CreateAsync(ruleDict.Values);
            }
            await InsertGroupsRules();

            async Task InsertAccesses()
            {
                var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_model_access.csv");
                if (!File.Exists(file_path))
                    return;
                var accessObj = GetService<IIRModelAccessService>();
                using (TextReader reader = File.OpenText(file_path))
                {
                    var csv = new CsvReader(reader);
                    var records = csv.GetRecords<IRModelAccessCsvLine>().ToList();

                    var errors = new List<string>();
                    foreach (var record in records)
                    {
                        var errs = new List<string>();
                        var model_id = record.model_id;
                        var group_id = record.group_id;

                        accessDict.Add(record.id, new IRModelAccess
                        {
                            Name = record.name,
                            Model = modelDict[model_id],
                            Group = !string.IsNullOrEmpty(group_id) ? groupDict[group_id] : null,
                            PermRead = record.perm_read ?? false,
                            PermWrite = record.perm_write ?? false,
                            PermCreate = record.perm_create ?? false,
                            PermUnlink = record.perm_unlink ?? false,
                        });
                    }
                }
                await accessObj.CreateAsync(accessDict.Values);
            }
            await InsertAccesses();
        }

        public async Task Unlink(Company self)
        {
            if (CompanyId == self.Id)
                throw new Exception("Không thể xóa chi nhánh đang làm việc");

            var modalDataObj = GetService<IIRModelDataService>();
            var user_root = await modalDataObj.GetRef<ApplicationUser>("base.user_root");
            if (user_root != null && user_root.CompanyId == self.Id)
                throw new Exception("Không thể xóa chi nhánh của tài khoản admin");

            try
            {
                var partnerId = self.PartnerId;
                await ExcuteSqlCommandAsync("update StockWarehouses set InTypeId = null, OutTypeId = null " +
                    "where CompanyId = @p0", self.Id);

                await ExcuteSqlCommandAsync("delete StockPickingTypes where Id in ( " +
                    "select pt.Id " +
                    "from StockPickingTypes pt " +
                    "left join StockWarehouses wh on pt.WarehouseId = wh.Id " +
                    "where wh.CompanyId = @p0)", self.Id);

                await ExcuteSqlCommandAsync("update Companies set AccountIncomeId = null, AccountExpenseId = null where Id = @p0", self.Id);

                await ExcuteSqlCommandAsync("delete StockWarehouses where CompanyId=@p0", self.Id);
                await ExcuteSqlCommandAsync("delete StockLocations where CompanyId=@p0", self.Id);
                await ExcuteSqlCommandAsync("delete AccountJournals where CompanyId=@p0", self.Id);
                await ExcuteSqlCommandAsync("delete IRSequences where CompanyId=@p0", self.Id);
                await ExcuteSqlCommandAsync("delete AccountAccounts where CompanyId=@p0", self.Id);

                var partnerObj = GetService<IPartnerService>();
                partnerObj.Sudo = true;
                var partner_ids = await partnerObj.SearchQuery(x => x.CompanyId == self.Id).Select(x => x.Id).ToListAsync();
                foreach (var partner_id in partner_ids)
                    await ExcuteSqlCommandAsync("update Partners set CompanyId = null where Id = @p0", partner_id);

                await ExcuteSqlCommandAsync("delete Companies where Id=@p0", self.Id);

                foreach (var partner_id in partner_ids)
                    await ExcuteSqlCommandAsync("delete Partners where Id=@p0", partner_id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Dữ liệu đã phát sinh cho chi nhánh này, không thể xóa!.");
            }
        }

        private async Task<IDictionary<string, IRModel>> InsertModels()
        {
            var dict = new Dictionary<string, IRModel>();
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_model_data.csv");
            if (!File.Exists(file_path))
                return dict;
            using (TextReader reader = File.OpenText(file_path))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.BadDataFound = null;
                var records = csv.GetRecords<IRModelCsvLine>().ToList();
                foreach (var record in records)
                {
                    var model = new IRModel
                    {
                        Model = record.model,
                        Name = record.name,
                        Transient = record.transient
                    };
                    dict.Add(record.id, model);
                }
            }

            var modelObj = GetService<IIRModelService>();
            await modelObj.CreateAsync(dict.Values);
            return dict;
        }

        public async Task<PagedResult2<CompanyBasic>> GetPagedResultAsync(CompanyPaged val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = GetQueryPaged(val);
            query = query.Where(x => company_ids.Contains(x.Id));
            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<CompanyBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<CompanyBasic>>(items)
            };
        }

        private IQueryable<Company> GetQueryPaged(CompanyPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.Active != null)
            {
                query = query.Where(x => x.Active == val.Active);
            }

            query = query.OrderBy(s => s.Name);
            return query;
        }

        public override ISpecification<Company> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "base.res_company_rule_employee":
                    return new InitialSpecification<Company>(x => companyIds.Contains(x.Id));
                default:
                    return null;
            }

        }


        public async Task ActionArchive(IEnumerable<Guid> ids)
        {
            var companies = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach(var company in companies)
            {
                if (company.Id == CompanyId)
                    throw new Exception("Không thể đóng chi nhánh đang làm việc");
                company.Active = false;
            }
            await UpdateAsync(companies);
        }

        public async Task ActionUnArchive(IEnumerable<Guid> ids)
        {
            var companies = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var company in companies)
                company.Active = true;

            await UpdateAsync(companies);
        }
    }

    public class IRModelCsvLine
    {
        public string id { get; set; }
        public string model { get; set; }

        public string name { get; set; }
        public bool transient { get; set; }
    }

    public class IRModelAccessCsvLine
    {
        public string id { get; set; }

        public string name { get; set; }

        public string model_id { get; set; }

        public string group_id { get; set; }

        public bool? perm_read { get; set; }

        public bool? perm_write { get; set; }

        public bool? perm_create { get; set; }

        public bool? perm_unlink { get; set; }
    }
}
