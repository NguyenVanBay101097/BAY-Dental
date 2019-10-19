﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
        public CompanyService(IAsyncRepository<Company> repository, IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment, IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }

        public async Task SetupCompany(string companyName, string userName, string email, string password, string name = "")
        {
            var partnerObj = GetService<IPartnerService>();
            var companyObj = GetService<ICompanyService>();
            //tạo công ty và user_root
            var mainPartner = new Partner
            {
                Name = companyName,
                Customer = false,
                Email = email,
            };

            await partnerObj.CreateAsync(mainPartner);

            var mainCompany = new Company
            {
                Name = companyName,
                Partner = mainPartner
            };

            await companyObj.CreateAsync(mainCompany);

            var partnerRoot = new Partner
            {
                Name = !string.IsNullOrEmpty(name) ? name: userName,
                Company = mainCompany,
                Customer = false,
                Email = email,
            };
            partnerObj.Create(partnerRoot);

            var userRoot = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                Company = mainCompany,
                CompanyId = mainCompany.Id,
                Partner = partnerRoot,
                Name = partnerRoot.Name,
                IsUserRoot = true
            };

            var userObj = GetService<UserManager<ApplicationUser>>();
            await userObj.CreateAsync(userRoot, password);

            mainPartner.Company = mainCompany;
            await partnerObj.UpdateAsync(mainPartner);

            await InsertModuleAccountData(mainCompany);

            await InsertModuleStockData(mainCompany);

            await InsertModuleProductData();

            await InsertModuleDentalData();

            await InsertSecurityData();
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

            var modelDatas = new List<IRModelData>();
            modelDatas.AddRange(PrepareModelData(stock_warehouses_dict, "stock.warehouse"));
            modelDatas.AddRange(PrepareModelData(stock_locations_dict, "stock.location"));
            modelDatas.AddRange(PrepareModelData(stock_sequence_dict, "ir.sequence"));
            var modelDataObj = GetService<IIRModelDataService>();
            await modelDataObj.CreateAsync(modelDatas);

            var locObj = GetService<IStockLocationService>();
            await locObj.CreateAsync(stock_locations_dict.Values);

            var whObj = GetService<IStockWarehouseService>();
            foreach(var wh in stock_warehouses_dict.Values)
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
                        } else if (field_name == "measure_type")
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
                            uom.Factor = XmlConvert.ToDecimal(field.GetAttribute("eval"));
                        }
                        else if (field_name == "rounding")
                        {
                            uom.Rounding = XmlConvert.ToDecimal(field.GetAttribute("eval"));
                        }
                        else if (field_name == "factor_inv")
                        {
                            uom.UOMType = "bigger";
                            uom.Factor = 1 / XmlConvert.ToDecimal(field.GetAttribute("eval"));
                        }
                        else if (field_name == "uom_type")
                        {
                            uom.UOMType = field.InnerText;
                        }
                    }

                    product_uom_dict.Add(id, uom);
                }
            }

            var productCategoryObj = GetService<IProductCategoryService>();
            await productCategoryObj.CreateAsync(product_category_dict.Values);

            var productUOMCategObj = GetService<IUoMCategoryService>();
            await productUOMCategObj.CreateAsync(product_uom_categ_dict.Values);

            var productUOMObj = GetService<IUoMService>();
            await productUOMObj.CreateAsync(product_uom_dict.Values);
        }

        public async Task InsertModuleDentalData()
        {
            var tooth_category_dict = new Dictionary<string, ToothCategory>();
            var tooth_dict = new Dictionary<string, Tooth>();
            var sequence_dict = new Dictionary<string, IRSequence>();
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
                    ResId = item.Value.Id
                });
            }

            return res;
        }

        public async Task SetupTenant(CompanySetupTenant val)
        {
            await SetupCompany(val.CompanyName, val.Username, val.Email, val.Password, name: val.Name);
        }

        public async Task InsertSecurityData()
        {
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_model_data.csv");
            if (!File.Exists(file_path))
                return;
            var models = new List<IRModel>();
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
                    };

                    model.ModelAccesses.Add(new IRModelAccess
                    {
                        Name = model.Name,
                        Active = false,
                        PermRead = false,
                        PermCreate = false,
                        PermWrite = false,
                        PermUnlink = false
                    });
                }
            }
            var modelObj = GetService<IIRModelService>();
            await modelObj.CreateAsync(models);
        }

        public async Task<PagedResult2<CompanyBasic>> GetPagedResultAsync(CompanyPaged val)
        {
            var query = GetQueryPaged(val);

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

            query = query.OrderBy(s => s.Name);
            return query;
        }

        public class IRModelCsvLine
        {
            public string model { get; set; }

            public string name { get; set; }
        }
    }
}
