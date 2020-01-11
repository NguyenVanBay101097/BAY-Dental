using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PartnerService : BaseService<Partner>, IPartnerService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public PartnerService(IAsyncRepository<Partner> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, UserManager<ApplicationUser> userManager)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public override ISpecification<Partner> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "base.res_partner_rule":
                    return new InitialSpecification<Partner>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }

        public async Task<PagedResult2<Partner>> GetPagedResultAsync(int offset, int limit, string search = "", string searchBy = "", bool? customer = null)
        {
            //Tìm kiếm partner search theo tên hoặc mã khách hàng, hoặc theo số điện thoại
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "name_ref":
                        {
                            query = query.Where(x => x.Name.Contains(search) || x.Ref.Contains(search));
                            break;
                        }
                    case "phone":
                        {
                            query = query.Where(x => x.Phone.Contains(search));
                            break;
                        }
                    default:
                        {
                            query = query.Where(x => x.Name.Contains(search) || x.Ref.Contains(search));
                            break;
                        }
                }
            }

            if (customer.HasValue)
                query = query.Where(x => x.Customer == customer.Value);

            query = query.OrderBy(x => x.Name); //default order by
            query = query.Skip(offset).Take(limit);

            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<Partner>(totalItems, offset, limit)
            {
                Items = items
            };
        }


        public async Task ComputeProductPricelist(Guid id)
        {

        }
        public async Task<Partner> GetPartnerForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .Include(x => x.Employees)
                .Include(x => x.PartnerHistoryRels)
                .Include("PartnerPartnerCategoryRels.Category")
                .Include("PartnerHistoryRels.History")
                .FirstOrDefaultAsync();
        }

        public string GetFormatAddress(Partner partner)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(partner.Street))
                list.Add(partner.Street);
            if (!string.IsNullOrEmpty(partner.WardName))
                list.Add(partner.WardName);
            if (!string.IsNullOrEmpty(partner.DistrictName))
                list.Add(partner.DistrictName);
            if (!string.IsNullOrEmpty(partner.CityName))
                list.Add(partner.CityName);
            var res = string.Join(", ", list);
            return res;
        }

        public string GetGenderDisplay(Partner partner)
        {
            switch (partner.Gender)
            {
                case "female":
                    return "Nữ";
                case "male":
                    return "Nam";
                case "other":
                    return "Khác";
                default:
                    return string.Empty;
            }
        }

        public async Task<PagedResult2<PartnerBasic>> GetPagedResultAsync(PartnerPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            var partnerList = new List<PartnerBasic>();
            var creditDebitDict = CreditDebitGet(items.Select(x => x.Id).ToList());
            foreach (var i in items)
            {
                var p = new PartnerBasic();
                var list = new List<string>();
                p = _mapper.Map<PartnerBasic>(i);
                if (!string.IsNullOrEmpty(i.Street))
                    list.Add(i.Street);
                if (!string.IsNullOrEmpty(i.WardName))
                    list.Add(i.WardName);
                if (!string.IsNullOrEmpty(i.DistrictName))
                    list.Add(i.DistrictName);
                if (!string.IsNullOrEmpty(i.CityName))
                    list.Add(i.CityName);
                if (list.Count > 0)
                {
                    p.Address = String.Join(", ", list);
                }
                partnerList.Add(p);

                if (i.Customer)
                {
                    p.Debt = creditDebitDict[p.Id].Credit;
                }
                else if (i.Supplier)
                {
                    p.Debt = creditDebitDict[p.Id].Debit;
                }
            }
            return new PagedResult2<PartnerBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = partnerList
            };
        }

        public override async Task<Partner> CreateAsync(Partner entity)
        {
            if (string.IsNullOrEmpty(entity.Ref) || entity.Ref == "/")
            {
                var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                if (entity.Customer == true)
                {
                    entity.Ref = await sequenceService.NextByCode("customer");
                    if (string.IsNullOrEmpty(entity.Ref))
                    {
                        await InsertCustomerSequence();
                        entity.Ref = await sequenceService.NextByCode("customer");
                    }
                }
                else if (entity.Supplier == true)
                {
                    entity.Ref = await sequenceService.NextByCode("supplier");
                    if (string.IsNullOrEmpty(entity.Ref))
                    {
                        await InsertSupplierSequence();
                        entity.Ref = await sequenceService.NextByCode("supplier");
                    }
                }
            } else
            {
                if (entity.Customer == true)
                {
                    var partner = SearchQuery(x => x.Ref == entity.Ref && x.Customer == true && x.Supplier == false).FirstOrDefault();
                    if (partner != null)
                        throw new Exception("Mã Khách hàng bị trùng !");
                }
                else if (entity.Supplier == true)
                {
                    var partner = SearchQuery(x => x.Ref == entity.Ref && x.Supplier == true && x.Customer == false).FirstOrDefault();
                    if (partner != null)
                        throw new Exception("Mã Nhà cung cấp bị trùng !");
                }
            }

            entity.DisplayName = _NameGet(entity);
            return await base.CreateAsync(entity);
        }

        private async Task InsertCustomerSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "customer",
                Name = "Mã khách hàng",
                Prefix = "KH",
                Padding = 5,
            });
        }

        private async Task InsertSupplierSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "supplier",
                Name = "Mã nhà cung cấp",
                Prefix = "NCC",
                Padding = 5,
            });
        }

        private string _NameGet(Partner partner)
        {
            var name = partner.Name;
            if (!string.IsNullOrEmpty(partner.Ref))
                name = "[" + partner.Ref + "] " + name;
            return name;
        }

        public override Task UpdateAsync(Partner entity)
        {
            entity.DisplayName = _NameGet(entity);
            //if (!string.IsNullOrWhiteSpace(entity.Ref))
            //{
            //    var partner = SearchQuery(x => x.Ref == entity.Ref && x.Id != entity.Id).FirstOrDefault();
            //    if (partner != null)
            //        throw new Exception("Mã này đã tồn tại !");
            //}
            //else {
            //    throw new Exception("Vui lòng nhập mã !");
            //}
            return base.UpdateAsync(entity);
        }

        public async Task<IEnumerable<PartnerSimple>> SearchAutocomplete(string filter = "", bool? customer = null)
        {
            var res = await SearchQuery(domain: x => (string.IsNullOrEmpty(filter) || x.Name.Contains(filter)) && (!customer.HasValue || x.Customer == customer))
                .Select(x => new PartnerSimple
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<PartnerSimple>> SearchPartnersCbx(PartnerPaged val)
        {
            var partners = await GetQueryPaged(val).Skip(val.Offset).Take(val.Limit).Select(x => new PartnerSimple {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Name = x.Name
            }).ToListAsync();
            return partners;
        }

        public IQueryable<Partner> GetQueryPaged(PartnerPaged val)
        {
            var query = SearchQuery();
            if (val.Customer.HasValue)
                query = query.Where(x => x.Customer == val.Customer);
            if (val.Employee.HasValue)
                query = query.Where(x => x.Employee == val.Employee);
            if (val.Supplier.HasValue)
                query = query.Where(x => x.Supplier == val.Supplier);
            if (!string.IsNullOrEmpty(val.SearchNamePhoneRef))
                query = query.Where(x => x.Name.Contains(val.SearchNamePhoneRef) || x.NameNoSign.Contains(val.SearchNamePhoneRef)
                || x.Ref.Contains(val.SearchNamePhoneRef) || x.Phone.Contains(val.SearchNamePhoneRef));

            //query = query.Skip(val.Offset).Take(val.Limit);
            query = query.OrderBy(s => s.Name);
            return query;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null) throw new Exception("File is null");

            //format đường dẫn cố định
            var suffixPath = "assets/images/uploads/files/{0}/{1:yyyy-MM}";
            //đường dẫn upload trên local
            var prefixPath = "C:/Users/TPOS-002/Documents/Visual Studio 2019/TMTDental Branches/TMTDentalAPI-quan/TMTDentalAPI/ClientApp/src";
            //Tên nha khoa
            var dentalName = "Nha Khoa ABC";

            var relativePath = string.Format(suffixPath, dentalName, DateTime.Now).Trim('/');
            var uploadPath = Path.Combine(prefixPath, relativePath).Replace("\\", "/");

            var fileName = file.FileName;

            int index = 0;

            //Ten file ton tai ??
            while (System.IO.File.Exists(Path.Combine(uploadPath, fileName)))
            {
                index++;
                fileName = string.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), index, Path.GetExtension(file.FileName));
            }
            //Folder ton tai ??
            if (!System.IO.Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var newPath = Path.Combine(uploadPath, fileName).Replace("\\", "/");
            //Chep file vao thu muc
            using (var fileStream = new FileStream(newPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var returnPath = Path.Combine("../../../", relativePath, fileName).Replace("\\", "/");
            return returnPath;
        }

        public async Task<PartnerInfoViewModel> GetInfo(Guid id)
        {
            var partner = await SearchQuery(x => x.Id == id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .Include("PartnerPartnerCategoryRels.Category")
                .FirstOrDefaultAsync();
            var res = _mapper.Map<PartnerInfoViewModel>(partner);
            res.Address = GetFormatAddress(partner);
            res.Gender = GetGenderDisplay(partner);
            res.Categories = string.Join(", ", partner.PartnerPartnerCategoryRels.Select(x => x.Category.Name));
            res.DateOfBirth = GetDateOfBirthDisplay(partner);
            return res;
        }

        private string GetDateOfBirthDisplay(Partner partner)
        {
            return string.Format("{0}/{1}/{2}", partner.BirthDay.HasValue ? partner.BirthDay.Value.ToString() : "",
                partner.BirthMonth.HasValue ? partner.BirthMonth.Value.ToString() : "",
                partner.BirthYear.HasValue ? partner.BirthYear.Value.ToString() : "");
        }

        public async Task<PagedResult2<AccountInvoiceDisplay>> GetCustomerInvoices(AccountInvoicePaged val)
        {
            var accInvObj = GetService<IAccountInvoiceService>();

            var items = await accInvObj.SearchQuery(domain: null, orderBy: x => x.OrderByDescending(s => s.DateInvoice).ThenByDescending(s => s.Number), offSet: val.Offset, limit: val.Limit)
                .Where(x => x.PartnerId == val.PartnerId)
                .Include(x => x.Partner).Include(x => x.User)
                .ToListAsync();
            var totalItems = await accInvObj.SearchQuery(domain: null, orderBy: x => x.OrderByDescending(s => s.DateInvoice).ThenByDescending(s => s.Number)).CountAsync();

            return new PagedResult2<AccountInvoiceDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<AccountInvoiceDisplay>>(items)
            };

        }

        public async Task ImportExcel(IFormFile file)
        {
            //    if (file == null) throw new Exception("File is null");

            //    //đường dẫn upload trên local
            //    var prefixPath = "C:/Users/TPOS-002/Desktop/tempFiles";

            //    var uploadPath = Path.Combine(prefixPath).Replace("\\", "/");

            //    var fileName = file.FileName;

            //    int index = 0;

            //    //Ten file ton tai ??
            //    while (System.IO.File.Exists(Path.Combine(uploadPath, fileName)))
            //    {
            //        index++;
            //        fileName = string.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), index, Path.GetExtension(file.FileName));
            //    }
            //    //Folder ton tai ??
            //    if (!System.IO.Directory.Exists(uploadPath))
            //    {
            //        Directory.CreateDirectory(uploadPath);
            //    }
            //    var newPath = Path.Combine(uploadPath, fileName).Replace("\\", "/");
            //    //Chep file vao thu muc
            //    using (var fileStream = new FileStream(newPath, FileMode.Create))
            //    {
            //        await file.CopyToAsync(fileStream);
            //    }

            //    var returnPath = Path.Combine(prefixPath, fileName).Replace("\\", "/");

            //    var sqlTable = "dbo.Partners";
            //    var excelQuery = "Select * from [Sheet1$]";

            //    var dt = new DataTable();
            //    dt.Columns.Add("Id", typeof(Guid));

            //    dt.Columns.Add("CreatedById");
            //    dt.Columns.Add("WriteById");
            //    dt.Columns.Add("DateCreated");
            //    dt.Columns.Add("LastUpdated");

            //    dt.Columns.Add("DisplayName");
            //    dt.Columns.Add("Name");
            //    dt.Columns.Add("NameNoSign");
            //    dt.Columns.Add("Street");
            //    dt.Columns.Add("Phone");
            //    dt.Columns.Add("Email");
            //    dt.Columns.Add("Supplier");
            //    dt.Columns.Add("Customer");
            //    dt.Columns.Add("CompanyId");
            //    dt.Columns.Add("Ref");
            //    dt.Columns.Add("Comment");

            //    dt.Columns.Add("Active");
            //    dt.Columns.Add("Employee");
            //    dt.Columns.Add("CityCode");
            //    dt.Columns.Add("CityName");
            //    dt.Columns.Add("DistrictName");
            //    dt.Columns.Add("DistrictCode");
            //    dt.Columns.Add("WardCode");
            //    dt.Columns.Add("WardName");
            //    dt.Columns.Add("MedicalHisory");
            //    dt.Columns.Add("BirthDay");
            //    dt.Columns.Add("BirthMonth");
            //    dt.Columns.Add("BirthYear");
            //    dt.Columns.Add("Gender");
            //    dt.Columns.Add("JobTitle");

            //    var adrList = new List<KeyValuePair<Guid,string>>();
            //    try
            //    {
            //        string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + returnPath + "; Extended Properties='Excel 12.0 Xml; HDR=YES;'";
            //        string connectionString = "Server=.\\SQLEXPRESS;User Id=sa;Password=123123;Initial Catalog=TMTDentalCatalogDb;";
            //        OleDbConnection oleConn = new OleDbConnection(excelConnectionString);
            //        OleDbCommand oleCmd = new OleDbCommand(excelQuery, oleConn);
            //        oleConn.Open();
            //        OleDbDataReader reader = oleCmd.ExecuteReader();
            //        SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);
            //        bulkCopy.DestinationTableName = sqlTable;

            //        var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            //        dt.Load(reader);
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            var row = dt.Rows[i];
            //            row["Id"] = GuidComb.GenerateComb();
            //            row["Active"] = true;
            //            row["Employee"] = false;
            //            row["Customer"] = true;
            //            row["Supplier"] = false;
            //            row["DateCreated"] = DateTime.Now;
            //            row["LastUpdated"] = DateTime.Now;
            //            row["WriteById"] = UserId;
            //            row["CreatedById"] = UserId;                    
            //            row["NameNoSign"] = StringUtils.RemoveSignVietnameseV2(row["Tên KH"].ToString());

            //            if (string.IsNullOrEmpty(row["Mã KH"].ToString()))
            //            {
            //                row["Mã KH"] = await sequenceService.NextByCode("customer");
            //            }
            //            var pt = new Partner();
            //            pt.Name = row["Tên KH"].ToString();
            //            pt.Ref = row["Mã KH"].ToString();
            //            row["DisplayName"] = _NameGet(pt);
            //            if (!string.IsNullOrEmpty(row["Ngày sinh"].ToString()))
            //            {
            //                var str = row["Ngày sinh"].ToString();
            //                var ar = new string[3];
            //                if (str.IndexOf("-") > -1)
            //                {
            //                   ar = row["Ngày sinh"].ToString().Split("-");

            //                } else if(str.IndexOf(".") > -1)
            //                {
            //                    ar = row["Ngày sinh"].ToString().Split(".");
            //                } else if (str.IndexOf("/") > -1)
            //                {
            //                    ar = row["Ngày sinh"].ToString().Split("/");
            //                }
            //                row["BirthDay"] = ar[0];
            //                row["BirthMonth"] = ar[1];
            //                row["BirthYear"] = ar[2];

            //            }
            //            if (!string.IsNullOrEmpty(row["Địa chỉ"].ToString()))
            //            {
            //                //AddressCheckApi address = await AddressHandleAsync(row["Địa chỉ"].ToString());
            //                //row["Street"] = address.ShortAddress;
            //                //row["CityCode"] = address.CityCode;
            //                //row["CityName"] = address.CityName;
            //                //row["DistrictCode"] = address.DistrictCode;
            //                //row["DistrictName"] = address.DistrictName;
            //                //row["WardCode"] = address.WardCode;
            //                //row["WardName"] = address.WardName;
            //                adrList.Add(new KeyValuePair<Guid,string>(Guid.Parse(row["Id"].ToString()), row["Địa chỉ"].ToString()));
            //            }

            //            if (!string.IsNullOrEmpty(row["Giới tính"].ToString()))
            //            {
            //                var gender = row["Giới tính"].ToString();
            //                if(gender.ToLower() == "nam")
            //                {
            //                    row["Gender"] = "Male";
            //                } else if (gender.ToLower() == "nữ")
            //                {
            //                    row["Gender"] = "Female";
            //                }
            //                else
            //                {
            //                    row["Gender"] = "Other";
            //                }

            //            }
            //        }

            //        var listTask = await RunTaskAsync(pairs: adrList);
            //        for(int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            var row = dt.Rows[i];
            //            var address = listTask.Where(x => x.Key == Guid.Parse(row["Id"].ToString())).FirstOrDefault().Value;
            //            row["Street"] = address.ShortAddress;
            //            row["CityCode"] = address.CityCode;
            //            row["CityName"] = address.CityName;
            //            row["DistrictCode"] = address.DistrictCode;
            //            row["DistrictName"] = address.DistrictName;
            //            row["WardCode"] = address.WardCode;
            //            row["WardName"] = address.WardName;
            //        }

            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Id", "Id"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Tên KH", "Name"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Mã KH", "Ref"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SĐT", "Phone"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Email", "Email"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Tiền căn", "MedicalHistory"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Gender", "Gender"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Nghề nghiệp", "JobTitle"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Ghi chú", "Comment"));

            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Supplier", "Supplier"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Customer", "Customer"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Active", "Active"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Employee", "Employee"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("NameNoSign", "NameNoSign"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("DisplayName", "DisplayName"));

            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Street", "Street"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CityCode", "CityCode"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CityName", "CityName"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("DistrictCode", "DistrictCode"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("DistrictName", "DistrictName"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("WardCode", "WardCode"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("WardName", "WardName"));

            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("BirthDay", "BirthDay"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("BirthMonth", "BirthMonth"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("BirthYear", "BirthYear"));

            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("DateCreated", "DateCreated"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("LastUpdated", "LastUpdated"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("WriteById", "WriteById"));
            //        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CreatedById", "CreatedById"));

            //        bulkCopy.WriteToServer(dt);

            //        oleConn.Close();
            //        File.Delete(returnPath);
            //    }
            //    catch (Exception ex)
            //    {
            //        File.Delete(returnPath);
            //        throw ex;
            //    }
        }

        public async Task ImportExcel2(IFormFile file, Ex_ImportExcelDirect dir)
        {
            if (file == null) throw new Exception("File is null");
            var list = new List<PartnerImportExcel>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var fileData = stream.ToArray();
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    list = await HandleExcelRowsByCustomerOrSupplierAsync(worksheet, dir);
                }
            }

            if (dir.IsCreateNew)
            {
                foreach (var pn in list)
                {
                    var entity = _mapper.Map<Partner>(pn);
                    await CreateAsync(entity);
                }

            }
            else {
                foreach (var pn in list)
                {
                    var entity = SearchQuery(x => x.Ref == pn.Ref).FirstOrDefault();
                    if (entity != null)
                    {
                        entity = _mapper.Map(pn, entity);
                        //entity = _mapper.Map<Partner>(pn);-->generate Guid
                        await UpdateAsync(entity);
                    }
                }
            }
        }

        public async Task<Dictionary<string, AddressCheckApi>> RunTaskAsync(List<string> strs, int limit = 100)
        {
            int offset = 0;
            var dict = new Dictionary<string, AddressCheckApi>();
            while (offset < strs.Count)
            {
                var subStrs = strs.Skip(offset).Take(limit);
                var allTasks = subStrs.Select(x => AddressHandleAsync(x));
                var res = await Task.WhenAll(allTasks);
                foreach (var item in res)
                {
                    dict.Add(item.Key, item.Value);
                }

                offset += limit;
            }

            //var dictionary = new Dictionary<string, AddressCheckApi>();
            //for (int i = 0; i < strs.Count; i += count)
            //{
            //    var listTask = new List<Task<KeyValuePair<string, AddressCheckApi>>>();
            //    for (int j = i;j<((i+count)<= strs.Count ? i+count : strs.Count) ; j++)
            //    {
            //        listTask.Add(Task.Run(async () => await AddressHandleAsync(strs.ElementAt(j))));
            //    }
            //    var res = await Task.WhenAll(listTask.ToList());
            //    foreach (var item in res)
            //    {
            //        dictionary.Add(item.Key, item.Value);
            //    }
            //}
            //return dictionary;


            return dict;
        }

        private async Task<KeyValuePair<string, AddressCheckApi>> AddressHandleAsync(string text)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://dc.tpos.vn/home/checkaddress?address=" + text);
            var res = response.Content.ReadAsAsync<AddressCheckApi[]>().Result.ToList().FirstOrDefault();
            var pair = new KeyValuePair<string, AddressCheckApi>(text, res);
            return pair;
        }


        //public override ISpecification<Partner> RuleDomainGet(IRRule rule)
        //{
        //    var companyId = CompanyId;
        //    switch (rule.Code)
        //    {
        //        case "partner_comp_rule":
        //            return new InitialSpecification<Partner>(x => x.CompanyId == companyId);
        //        default:
        //            return null;
        //    }
        //}


        public Dictionary<Guid, PartnerCreditDebitItem> CreditDebitGet(IEnumerable<Guid> ids = null,
       DateTime? fromDate = null,
       DateTime? toDate = null)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var spec = amlObj._QueryGetSpec(companyId: CompanyId, state: "posted", dateFrom: fromDate,
                dateTo: toDate);
            var types = new[] { "receivable", "payable" };
            if (ids != null)
            {
                spec = spec.And(new InitialSpecification<AccountMoveLine>(x => ids.Contains(x.Partner.Id)));
            }

            spec = spec.And(new InitialSpecification<AccountMoveLine>(x => x.PartnerId.HasValue && types.Contains(x.Account.InternalType) &&
                x.Reconciled == false));

            var res = amlObj.SearchQuery(spec.AsExpression())
                .GroupBy(x => new { PartnerId = x.PartnerId.Value, Type = x.Account.InternalType })
                .Select(x => new
                {
                    PartnerId = x.Key.PartnerId,
                    Type = x.Key.Type,
                    Amount = x.Sum(s => s.AmountResidual)
                }).ToList();


            var dict = new Dictionary<Guid, PartnerCreditDebitItem>();
            if (ids != null)
            {
                foreach (var id in ids)
                    dict.Add(id, new PartnerCreditDebitItem());
            }

            foreach (var item in res)
            {
                if (!dict.ContainsKey(item.PartnerId))
                    dict.Add(item.PartnerId, new PartnerCreditDebitItem());

                var val = item.Amount;

                if (item.Type == "receivable")
                    dict[item.PartnerId].Credit = val;
                else if (item.Type == "payable")
                    dict[item.PartnerId].Debit = -val;
            }

            return dict;
        }

        public async Task<List<PartnerImportExcel>> HandleExcelRowsByCustomerOrSupplierAsync(ExcelWorksheet worksheet, Ex_ImportExcelDirect dir)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            var list = new List<PartnerImportExcel>();
            if (dir.IsCustomer)
            {
                var adList = new List<string>();
                for (var i = 2; i <= worksheet.Dimension.Rows; i++)
                {
                    var add = Convert.ToString(worksheet.Cells[i, 6].Value);
                    if (!string.IsNullOrEmpty(add))
                        adList.Add(add.ToLower());
                }
                var dict = await RunTaskAsync(strs: adList.Distinct().ToList());
                var genderDict = new Dictionary<string, string>()
                {
                    { "nam","Male" },
                    { "nữ","Female" }
                };

                for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (string.IsNullOrWhiteSpace(name))
                        throw new Exception("Tên Khách hàng là bắt buộc");

                    var custRef = Convert.ToString(worksheet.Cells[row, 2].Value);
                    if (!dir.IsCreateNew)
                    {

                        if (string.IsNullOrWhiteSpace(custRef))
                            throw new Exception("Mã Khách hàng là bắt buộc");
                    }
                    else
                    {

                    }


                    var gender = Convert.ToString(worksheet.Cells[row, 3].Value).ToLower();
                    var dob = Convert.ToString(worksheet.Cells[row, 4].Value);
                    var ar = new string[3];
                    var address = Convert.ToString(worksheet.Cells[row, 6].Value);
                    if (dob.IndexOf("-") > -1)
                        ar = dob.Split("-");
                    else if (dob.IndexOf(".") > -1)
                        ar = dob.Split(".");
                    else if (dob.IndexOf("/") > -1)
                        ar = dob.Split("/");

                    var item = new PartnerImportExcel
                    {
                        Name = name,
                        NameNoSign = StringUtils.RemoveSignVietnameseV2(name),
                        Ref = !string.IsNullOrEmpty(custRef) ? custRef : await sequenceService.NextByCode("customer"),
                        Gender = genderDict.ContainsKey(gender) ? genderDict[gender] : "Other",
                        Phone = Convert.ToString(worksheet.Cells[row, 5].Value),
                        MedicalHistory = Convert.ToString(worksheet.Cells[row, 7].Value),
                        JobTitle = Convert.ToString(worksheet.Cells[row, 8].Value),
                        Email = Convert.ToString(worksheet.Cells[row, 9].Value),
                        BirthDay = !string.IsNullOrWhiteSpace(ar[0]) ? ar[0] : null,
                        BirthMonth = !string.IsNullOrWhiteSpace(ar[1]) ? ar[1] : null,
                        BirthYear = !string.IsNullOrWhiteSpace(ar[2]) ? ar[2] : null,
                        Comment = Convert.ToString(worksheet.Cells[row, 10].Value),
                        Customer = true,
                        Supplier = false
                    };
                    if (dict.ContainsKey(address.ToLower()))
                    {
                        item.Street = dict[address.ToLower()].ShortAddress;
                        item.CityName = dict[address.ToLower()].CityName;
                        item.CityCode = dict[address.ToLower()].CityCode;
                        item.DistrictName = dict[address.ToLower()].DistrictName;
                        item.DistrictCode = dict[address.ToLower()].DistrictCode;
                        item.WardName = dict[address.ToLower()].WardName;
                        item.WardCode = dict[address.ToLower()].WardCode;
                    }
                    list.Add(item);

                }
            }
            else
            {
                for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (string.IsNullOrWhiteSpace(name))
                        throw new Exception("Tên NCC là bắt buộc");

                    var suppRef = Convert.ToString(worksheet.Cells[row, 2].Value);
                    if (!dir.IsCreateNew)
                        if (string.IsNullOrWhiteSpace(suppRef)) throw new Exception("Mã NCC là bắt buộc");

                    var item = new PartnerImportExcel
                    {
                        Name = name,
                        NameNoSign = StringUtils.RemoveSignVietnameseV2(name),
                        Ref = !string.IsNullOrEmpty(suppRef) ? suppRef : await sequenceService.NextByCode("supplier"),
                        Phone = Convert.ToString(worksheet.Cells[row, 3].Value),
                        Fax = Convert.ToString(worksheet.Cells[row, 4].Value),
                        Street = Convert.ToString(worksheet.Cells[row, 5].Value),
                        Email = Convert.ToString(worksheet.Cells[row, 6].Value),
                        Comment = Convert.ToString(worksheet.Cells[row, 7].Value),
                        Customer = false,
                        Supplier = true
                    };

                    list.Add(item);

                }
            }

            return list;
        }

        public async Task<IEnumerable<PartnerReportLocationCity>> ReportLocationCity(ReportLocationCitySearch val)
        {
            var query = SearchQuery(x => x.Customer && x.Active);
            if (!string.IsNullOrEmpty(val.CityCode))
                query = query.Where(x => x.CityCode == val.CityCode);

            var res = await query.GroupBy(x => new
            {
                x.CityCode,
                x.CityName
            }).Select(x => new PartnerReportLocationCity
            {
                CityCode = x.Key.CityCode,
                CityName = x.Key.CityName,
                Total = x.Count(),
                Percentage = x.Count() * 100f / query.Count()
            }).ToListAsync();

            foreach (var item in res)
            {
                item.SearchCityCode = val.CityCode;
                item.SearchDistrictCode = val.DistrictCode;
                item.SearchWardCode = val.WardCode;
            }
            return res;
        }
        public async Task<IEnumerable<PartnerReportLocationDistrict>> ReportLocationDistrict(PartnerReportLocationCity val)
        {
            var query = SearchQuery(x => x.Customer && x.Active && x.CityCode == val.CityCode && x.CityName == val.CityName);
            if (!string.IsNullOrEmpty(val.SearchDistrictCode))
                query = query.Where(x => x.DistrictCode == val.SearchDistrictCode);
            var res = await query.GroupBy(x => new
            {
                x.DistrictCode,
                x.DistrictName
            }).Select(x => new PartnerReportLocationDistrict
            {
                DistrictCode = x.Key.DistrictCode,
                DistrictName = x.Key.DistrictName,
                Total = x.Count(),
                Percentage = x.Count() * 100f / query.Count()
            }).ToListAsync();

            foreach(var item in res)
            {
                item.CityCode = val.CityCode;
                item.CityName = val.CityName;
                item.SearchWardCode = val.SearchWardCode;
            }

            return res;
        }

        public async Task<IEnumerable<PartnerReportLocationWard>> ReportLocationWard(PartnerReportLocationDistrict val)
        {
            var query = SearchQuery(x => x.Customer && x.Active && x.DistrictCode == val.DistrictCode && x.DistrictName == val.DistrictName &&
            x.CityCode == val.CityCode && x.CityName == val.CityName);
            if (!string.IsNullOrEmpty(val.SearchWardCode))
                query = query.Where(x => x.WardCode == val.SearchWardCode);
            var res = await query.GroupBy(x => new
            {
                x.WardCode,
                x.WardName
            }).Select(x => new PartnerReportLocationWard
            {
                WardCode = x.Key.WardCode,
                WardName = x.Key.WardName,
                Total = x.Count(),
                Percentage = x.Count() * 100f / query.Count()
            }).ToListAsync();
            return res;
        }

        //public async Task<File> ExportExcelFile(PartnerPaged val)
        //{
        //    val.Offset = 0;
        //    val.Limit = int.MaxValue;

        //    var partners = await GetQueryPaged(val).ToListAsync();
        //    var memory = new MemoryStream();

        //    using (var fs = new FileStream(Path.Combine(), FileMode.Create, FileAccess.Write))
        //    {
        //        var workbook = new XSSFWorkbook();
        //        var sheet = workbook.CreateSheet("Thông tin Đối tác");
        //        var row = sheet.CreateRow(0);

        //        row.CreateCell(0).SetCellValue("Tên KH");
        //        row.CreateCell(1).SetCellValue("Mã KH");
        //        row.CreateCell(2).SetCellValue("Giới tính");
        //        row.CreateCell(3).SetCellValue("Ngày sinh");
        //        row.CreateCell(4).SetCellValue("SĐT");
        //        row.CreateCell(5).SetCellValue("Địa chỉ");
        //        row.CreateCell(6).SetCellValue("Tiền căn");
        //        row.CreateCell(7).SetCellValue("Nghề nghiệp");
        //        row.CreateCell(8).SetCellValue("Email");
        //        row.CreateCell(9).SetCellValue("Ghi chú");

        //        foreach (var item in partners)
        //        {
        //            var entity = await GetPartnerForDisplayAsync(item.Id);
        //            var address = String.Join(", ",new string[4] { item.Street, item.WardName, item.DistrictName, item.CityName });
        //            var histories = entity.PartnerHistoryRels.Select(x => x.History.Name).ToList();
        //            histories.Add(item.MedicalHistory);

        //            row.CreateCell(0).SetCellValue(item.Name);
        //            row.CreateCell(1).SetCellValue(item.Ref);
        //            row.CreateCell(2).SetCellValue(item.Gender);
        //            row.CreateCell(3).SetCellValue(item.BirthDay+"/"+item.BirthMonth+"/"+item.BirthYear);
        //            row.CreateCell(4).SetCellValue(item.Phone);
        //            row.CreateCell(5).SetCellValue(address);
        //            row.CreateCell(6).SetCellValue(string.Join(", ", histories));
        //            row.CreateCell(7).SetCellValue(item.JobTitle);
        //            row.CreateCell(8).SetCellValue(item.Email);
        //            row.CreateCell(9).SetCellValue(item.Comment);
        //        }

        //        workbook.Write(fs);
        //    }


        //}

    }

    public class PartnerCreditDebitItem
    {
        public decimal Credit { get; set; }

        public decimal Debit { get; set; }
    }

    public class Ex_ImportExcelDirect
    {
        public bool IsCustomer { get; set; }
        public bool IsCreateNew { get; set; }
    }
}
