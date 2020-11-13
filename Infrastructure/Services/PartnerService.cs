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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyERP.Utilities;
using Newtonsoft.Json;
using NPOI.HSSF.Record.Chart;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class PartnerService : BaseService<Partner>, IPartnerService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        // private readonly IPartnerMapPSIDFacebookPageService _partnerMapPSIDFacebookPageService;
        public PartnerService(IAsyncRepository<Partner> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, UserManager<ApplicationUser> userManager)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _userManager = userManager;
            //_partnerMapPSIDFacebookPageService = partnerMapPSIDFacebookPageService;
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

        public async Task<Partner> GetPartnerForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .Include(x => x.ReferralUser)
                .Include(x => x.PartnerHistoryRels)
                .Include(x => x.Source)
                .Include(x => x.ReferralUser)
                .Include(x => x.Title)
                .Include(x => x.Consultant)
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
            var cateObj = GetService<IPartnerCategoryService>();

            var query = GetQueryPaged(val);
            var items = await _mapper.ProjectTo<PartnerBasic>(query.Skip(val.Offset).Take(val.Limit))
                .ToListAsync();
            var totalItems = await query.CountAsync();

            var cateList = await cateObj.SearchQuery(x => x.PartnerPartnerCategoryRels.Any(s => items.Select(i => i.Id).Contains(s.PartnerId)))
                                                                                        .Include(x => x.PartnerPartnerCategoryRels).ToListAsync();

            if (val.ComputeCreditDebit)
            {

                var creditDebitDict = CreditDebitGet(items.Select(x => x.Id).ToList());
                foreach (var item in items)
                {
                    item.Credit = creditDebitDict[item.Id].Credit;
                    item.Debit = creditDebitDict[item.Id].Debit;
                    item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(cateList.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.PartnerId == item.Id)));
                }
            }
            else
            {
                foreach (var item in items)
                {
                    item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(cateList.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.PartnerId == item.Id)));
                }
            }

            return new PagedResult2<PartnerBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }


        public async Task<AppointmentBasic> GetNextAppointment(Guid id)
        {
            var apObj = GetService<IAppointmentService>();
            var res = await _mapper.ProjectTo<AppointmentBasic>(apObj.SearchQuery(x => x.PartnerId == id, orderBy: x => x.OrderByDescending(s => s.Date))).FirstOrDefaultAsync();
            return res;
        }

        public override async Task<IEnumerable<Partner>> CreateAsync(IEnumerable<Partner> entities)
        {
            _UpdateCityName(entities);
            await _GenerateRefIfEmpty(entities);
            _ComputeDisplayName(entities);
            _SetCompanyIfNull(entities);

            await base.CreateAsync(entities);

            await _CheckUniqueRef(entities);
            return entities;
        }

        private void _SetCompanyIfNull(IEnumerable<Partner> self)
        {
            //Nếu khởi tạo công ty ban đầu thì CompanyId ko có giá trị mà gán vào sẽ bị lỗi insert 
            foreach (var partner in self)
            {
                var company_id = CompanyId;
                if (!partner.CompanyId.HasValue && company_id != Guid.Empty)
                    partner.CompanyId = company_id;
            }
        }

        public async Task AddOrRemoveTags(PartnerAddRemoveTagsVM val, bool isAdd)
        {
            var partner = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .FirstOrDefaultAsync();

            if (partner == null)
                return;

            if (isAdd)
            {
                foreach (var tagId in val.TagIds)
                {
                    if (!partner.PartnerPartnerCategoryRels.Any(x => x.CategoryId == tagId))
                        partner.PartnerPartnerCategoryRels.Add(new PartnerPartnerCategoryRel { CategoryId = tagId });
                }
            }
            else
            {
                foreach (var tagId in val.TagIds)
                {
                    var rel = partner.PartnerPartnerCategoryRels.FirstOrDefault(x => x.CategoryId == tagId);
                    if (rel != null)
                        partner.PartnerPartnerCategoryRels.Remove(rel);
                }
            }

            await UpdateAsync(partner);
        }

        public async Task UpdateTags(PartnerAddRemoveTagsVM val)
        {
            var partner = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .FirstOrDefaultAsync();

            if (partner == null)
                return;

            partner.PartnerPartnerCategoryRels.Clear();
            foreach (var tagId in val.TagIds)
            {
                partner.PartnerPartnerCategoryRels.Add(new PartnerPartnerCategoryRel { CategoryId = tagId });
            }

            await UpdateAsync(partner);
        }

        private async Task _GenerateRefIfEmpty(IEnumerable<Partner> self)
        {
            var seqObj = GetService<IIRSequenceService>();
            var customers = self.Where(x => x.Customer == true && string.IsNullOrEmpty(x.Ref));
            foreach (var cus in customers)
                cus.Ref = await GenerateUniqueRef("customer");

            var suppliers = self.Where(x => x.Supplier == true && string.IsNullOrEmpty(x.Ref));
            foreach (var sup in suppliers)
                sup.Ref = await GenerateUniqueRef("supplier");
        }

        private async Task<string> GenerateUniqueRef(string type = "customer")
        {
            var seqObj = GetService<IIRSequenceService>();
            if (type == "customer")
            {
                var code = await seqObj.NextByCode(type);
                var count = 0;
                while ((await SearchQuery(x => x.Customer == true && x.Ref == code).CountAsync()) > 0 && count < 100)
                {
                    code = await seqObj.NextByCode(type);
                }

                return code;
            }
            else if (type == "supplier")
            {
                var code = await seqObj.NextByCode(type);
                var count = 0;
                while ((await SearchQuery(x => x.Supplier == true && x.Ref == code).CountAsync()) > 0 && count < 100)
                {
                    code = await seqObj.NextByCode(type);
                }

                return code;
            }

            return string.Empty;
        }

        private async Task _CheckUniqueRef(IEnumerable<Partner> self)
        {
            foreach (var partner in self)
            {
                if (partner.Customer == true && !string.IsNullOrEmpty(partner.Ref))
                {
                    var exist = await SearchQuery(x => x.Ref == partner.Ref && x.Customer == true).ToListAsync();
                    if (exist.Count >= 2)
                        throw new Exception($"Đã tồn tại khách hàng với mã {partner.Ref}");
                }

                if (partner.Supplier == true && !string.IsNullOrEmpty(partner.Ref))
                {
                    var exist = await SearchQuery(x => x.Ref == partner.Ref && x.Supplier == true).ToListAsync();
                    if (exist.Count >= 2)
                        throw new Exception($"Đã tồn tại nhà cung cấp với mã {partner.Ref}");
                }
            }
        }

        public override async Task UpdateAsync(IEnumerable<Partner> entities)
        {
            _ComputeDisplayName(entities);
            _UpdateCityName(entities);
            await _GenerateRefIfEmpty(entities);
            //_SetCompanyIfNull(entities);

            await base.UpdateAsync(entities);

            await _CheckUniqueRef(entities);
        }

        private void _UpdateCityName(IEnumerable<Partner> self)
        {
            foreach (var partner in self)
            {
                if (string.IsNullOrEmpty(partner.CityName))
                    continue;
                if (partner.CityName == "TP Hồ Chí Minh")
                    partner.CityName = "Thành phố Hồ Chí Minh";
            }
        }

        private void _ComputeDisplayName(IEnumerable<Partner> self)
        {
            foreach (var partner in self)
                partner.DisplayName = _NameGet(partner);
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

        public void FetchAllPSIDFromFacebookFanpage()
        {
            var page_access_token = "EAAJRTJsjvegBAOvgQmu16mdl9iZAgFT2pS7WUHN2nZAAgkSZAHvLZAe6smKBpK89pt9VJ3QLLneZAy88ffuRgn5Cw77FZB3K6MZCiBWfu8AdsNLjM2OuxQZBon9jGIDZCla8snZAOumpYaSMAD0nURDpCA6JQxP5k0rKdStTZBCLpVSk76nSCUHkKUWp8SiSh7FUkKBXWDhPXyc2wZDZD";
            var apiClient = new ApiClient(page_access_token, FacebookApiVersions.V6_0);
            //var page_id = "565832646833390";
            //var url = $"/{page_id}";

            //var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            //getRequest.AddQueryParameter("fields", "conversations{id}");

            //var getRequestResponse = getRequest.Execute();

            //var conversation_id = "t_1038688119828988";
            //var url = $"/{conversation_id}";

            //var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            //getRequest.AddQueryParameter("fields", "messages{message}");

            //var getRequestResponse = getRequest.Execute();

            //var message_id = "m_GN4KINrpp1zXQq06H-VTwr6db0nndO6EOyLMhdCTspc_Wli1Shf8ch3jKd9TdadSVolp5Osp1xKvG6PAkV7j7g";
            //var message_id = "m_MbBf09HGKbUb3AV1JPcQR76db0nndO6EOyLMhdCTspelklgu2YcEi4vJHfisv7Gnxs1nB1eoCpmT0OyqOjcCkA";
            //var url = $"/{message_id}";

            //var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            //getRequest.AddQueryParameter("fields", "from,to");

            //var getRequestResponse = getRequest.Execute();

            //var conversation_id = "t_1038688119828988";
            //var url = $"/{conversation_id}";

            //var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            //getRequest.AddQueryParameter("fields", "participants,senders");

            //var getRequestResponse = getRequest.Execute();

            var psid = "2495342403910921";
            var url = $"/me/messages";

            var postRequest = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            postRequest.AddParameter("messaging_type", "RESPONSE");
            postRequest.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            postRequest.AddParameter("message", JsonConvert.SerializeObject(new { text = "hello world, i'm testing" }));

            var getRequestResponse = postRequest.Execute();

        }

        public async Task<IEnumerable<PartnerSimple>> SearchPartnersCbx(PartnerPaged val)
        {
            var partners = await GetQueryPaged(val).Skip(val.Offset).Take(val.Limit).Select(x => new PartnerSimple
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Name = x.Name,
            }).ToListAsync();
            return partners;
        }

        public async Task<IEnumerable<PartnerSimpleContact>> SearchPartnersConnectSocial(PartnerPaged val)
        {
            var partners = await GetQueryPaged(val).Skip(val.Offset).Take(val.Limit).Select(x => new PartnerSimpleContact
            {
                Id = x.Id,
                Phone = x.Phone,
                Name = x.Name

            }).ToListAsync();
            return partners;
        }

        public async Task<IEnumerable<PartnerSimpleInfo>> SearchPartnerInfosCbx(PartnerPaged val)
        {
            var cateObj = GetService<IPartnerCategoryService>();
            var partners = await GetQueryPaged(val).Skip(val.Offset).Take(val.Limit).Select(x => new PartnerSimpleInfo
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Name = x.Name,
                Phone = x.Phone,
                BirthYear = x.BirthYear,
            }).ToListAsync();

            var cateList = await cateObj.SearchQuery(x => x.PartnerPartnerCategoryRels.Any(s => partners.Select(i => i.Id).Contains(s.PartnerId)))
                                                                                       .Include(x => x.PartnerPartnerCategoryRels).ToListAsync();

            foreach (var partner in partners)
                partner.Categories = _mapper.Map<List<PartnerCategoryBasic>>(cateList.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.PartnerId == partner.Id)));

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
            if (!string.IsNullOrEmpty(val.Search))
            {

                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search)
               || x.Ref.Contains(val.Search) || x.Phone.Contains(val.Search));
            }

            query = query.OrderBy(s => s.DisplayName);
            return query;
        }



        /// <summary>
        /// Search Phone 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PartnerInfoChangePhone>> OnChangePartner(string phone)
        {
            var query = await SearchQuery(x => x.Phone.Contains(phone) && x.Active == true).ToListAsync();
            var result = _mapper.Map<IEnumerable<PartnerInfoChangePhone>>(query);
            return result;
        }

        public async Task<IEnumerable<PartnerCustomerExportExcelVM>> GetExcel(PartnerPaged val)
        {
            var query = GetQueryPaged(val);
            if (val.TagIds.Any())
            {
                //filter query
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();
                var filterPartnerIds = await partnerCategoryRelService.SearchQuery(x => val.TagIds.Contains(x.CategoryId)).Select(x => x.PartnerId).Distinct().ToListAsync();
                query = query.Where(x => filterPartnerIds.Contains(x.Id));
            }

            var res = await query.OrderBy(x => x.DisplayName).Select(x => new PartnerCustomerExportExcelVM
            {
                Name = x.Name,
                Ref = x.Ref,
                Date = x.Date,
                Gender = x.Gender,
                BirthDay = x.BirthDay,
                BirthMonth = x.BirthMonth,
                BirthYear = x.BirthYear,
                Phone = x.Phone,
                MedicalHistories = x.PartnerHistoryRels.Select(s => s.History.Name),
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                WardName = x.WardName,
                Street = x.Street,
                Job = x.JobTitle,
                Email = x.Email,
                Note = x.Comment
            }).ToListAsync();

            return res;
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
            return string.Format("{0}/{1}/{2}", partner.BirthDay.HasValue ?
                partner.BirthDay.Value.ToString() : "",
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

        public async Task UpdateCustomersZaloId()
        {
            var zaloConfigObj = GetService<IZaloOAConfigService>();
            var config = await zaloConfigObj.SearchQuery().FirstOrDefaultAsync();
            if (config == null)
                throw new Exception("Bạn cần kết nối 1 tài khoản zalo official account");

            var zaloClient = new ZaloClient(config.AccessToken);
            var partners = await SearchQuery(x => x.Customer && !string.IsNullOrEmpty(x.Phone)).ToListAsync();
            var phones = partners.Select(x => x.Phone).Distinct().ToList();
            var checkPhoneResult = await Task.WhenAll(phones.Select(x => _CheckZaloPhoneProfile(zaloClient, x)));
            var checkPhoneDict = checkPhoneResult.ToDictionary(x => x.Phone, x => x.Profile);

            foreach (var partner in partners)
            {
                var phone = partner.Phone;
                if (phone.StartsWith("0"))
                    phone = "84" + phone.Substring(1);
                var profile = checkPhoneDict[phone];
                if (profile.error != 0)
                    continue;
                partner.ZaloId = profile.data.user_id.ToString();
            }

            await UpdateAsync(partners);
        }

        public Task<CheckZaloPhoneProfile> _CheckZaloPhoneProfile(ZaloClient zaloClient, string phone)
        {
            return Task.Run(() =>
            {
                if (phone.StartsWith("0"))
                    phone = "84" + phone.Substring(1);
                var profile = zaloClient.getProfileOfFollower(phone).ToObject<GetProfileOfFollowerResponse>();
                return new CheckZaloPhoneProfile { Phone = phone, Profile = profile };
            });
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
            else
            {
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

        public async Task<PartnerImportResponse> ActionImport(PartnerImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<PartnerImportRowExcel>();
            var errors = new List<string>();
            var partner_code_list = new List<string>();
            var partner_history_list = new List<string>();
            var wards = new List<WardVm>();

            var title_dict = new Dictionary<string, string>()
            {
                { "customer", "KH" },
                { "supplier", "NCC" },
            };

            using (var stream = new MemoryStream(fileData))
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var errs = new List<string>();

                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        if (string.IsNullOrWhiteSpace(name))
                            errs.Add($"Tên {title_dict[val.Type]} là bắt buộc");

                        var reference = Convert.ToString(worksheet.Cells[row, 2].Value);
                        if (!string.IsNullOrWhiteSpace(reference))
                        {
                            if (partner_code_list.Contains(reference))
                                errs.Add($"Đã tồn tại mã {title_dict[val.Type]} {reference}");
                            else
                                partner_code_list.Add(reference);
                        }



                        if (val.Type == "customer")
                        {
                            var medicalHistory = Convert.ToString(worksheet.Cells[row, 13].Value);
                            if (!string.IsNullOrWhiteSpace(medicalHistory))
                            {
                                var medicalHistoryTmp = medicalHistory.Split(",");
                                foreach (var historyTmp in medicalHistoryTmp)
                                {
                                    if (!partner_history_list.Contains(historyTmp))
                                        partner_history_list.Add(historyTmp);
                                }
                            }

                            try
                            {
                                DateTime? date = null;
                                var dateExcel = Convert.ToString(worksheet.Cells[row, 3].Value);
                                long dateLong;
                                if (!string.IsNullOrEmpty(dateExcel) && long.TryParse(dateExcel, out dateLong))
                                    date = DateTime.FromOADate(dateLong);

                                var birthDayStr = Convert.ToString(worksheet.Cells[row, 5].Value);
                                var birthMonthStr = Convert.ToString(worksheet.Cells[row, 6].Value);
                                var birthYearStr = Convert.ToString(worksheet.Cells[row, 7].Value);

                                data.Add(new PartnerImportRowExcel
                                {
                                    Name = name,
                                    Ref = reference,
                                    Date = date,
                                    Gender = Convert.ToString(worksheet.Cells[row, 4].Value),
                                    BirthDay = !string.IsNullOrWhiteSpace(birthDayStr) ? Convert.ToInt32(birthDayStr) : (int?)null,
                                    BirthMonth = !string.IsNullOrWhiteSpace(birthMonthStr) ? Convert.ToInt32(birthMonthStr) : (int?)null,
                                    BirthYear = !string.IsNullOrWhiteSpace(birthYearStr) ? Convert.ToInt32(birthYearStr) : (int?)null,
                                    Phone = Convert.ToString(worksheet.Cells[row, 8].Value),
                                    Street = Convert.ToString(worksheet.Cells[row, 9].Value),
                                    WardName = Convert.ToString(worksheet.Cells[row, 10].Value),
                                    DistrictName = Convert.ToString(worksheet.Cells[row, 11].Value),
                                    CityName = Convert.ToString(worksheet.Cells[row, 12].Value),
                                    MedicalHistory = medicalHistory,
                                    Job = Convert.ToString(worksheet.Cells[row, 14].Value),
                                    Email = Convert.ToString(worksheet.Cells[row, 15].Value),
                                    Note = Convert.ToString(worksheet.Cells[row, 16].Value),
                                });
                            }
                            catch (Exception e)
                            {
                                errors.Add($"Dòng {row}: {e.Message}");
                                continue;
                            }
                        }
                        else if (val.Type == "supplier")
                        {

                            try
                            {
                                data.Add(new PartnerImportRowExcel
                                {
                                    Name = name,
                                    Ref = reference,
                                    Phone = Convert.ToString(worksheet.Cells[row, 3].Value),
                                    Fax = Convert.ToString(worksheet.Cells[row, 4].Value),
                                    Street = Convert.ToString(worksheet.Cells[row, 5].Value),
                                    WardName = Convert.ToString(worksheet.Cells[row, 6].Value),
                                    DistrictName = Convert.ToString(worksheet.Cells[row, 7].Value),
                                    CityName = Convert.ToString(worksheet.Cells[row, 8].Value),
                                    Email = Convert.ToString(worksheet.Cells[row, 9].Value),
                                    Note = Convert.ToString(worksheet.Cells[row, 10].Value),
                                });
                            }
                            catch (Exception e)
                            {
                                errors.Add($"Dòng {row}: {e.Message}");
                                continue;
                            }
                        }
                        else
                        {
                            throw new Exception($"Not support type {val.Type}");
                        }

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }
                    }
                }
            }

            if (errors.Any())
                return new PartnerImportResponse { Success = false, Errors = errors };

            //Get list partner ref
            var partner_dict = await GetPartnerDictByRefs(partner_code_list);


            var address_check_dict = new Dictionary<string, AddressCheckApi>();
            var address_list = data.Where(x => !string.IsNullOrWhiteSpace(x.Address)).Select(x => x.Address).Distinct().ToList();
            //Get list Api check address distionary
            address_check_dict = await CheckAddressAsync(address_list);


            var medical_history_dict = new Dictionary<string, History>();
            if (partner_history_list.Any())
            {
                var historyObj = GetService<IHistoryService>();
                var histories = await historyObj.SearchQuery(x => partner_history_list.Contains(x.Name)).ToListAsync();
                foreach (var history in histories)
                {
                    if (!medical_history_dict.ContainsKey(history.Name))
                        medical_history_dict.Add(history.Name, history);
                }

                var histories_name_to_insert = partner_history_list.Except(histories.Select(x => x.Name));
                var histories_to_insert = histories_name_to_insert.Select(x => new History { Name = x }).ToList();
                await historyObj.CreateAsync(histories_to_insert);

                foreach (var history in histories_to_insert)
                {
                    if (!medical_history_dict.ContainsKey(history.Name))
                        medical_history_dict.Add(history.Name, history);
                }
            }



            var partnersCreate = new List<Partner>();
            var partnersUpdate = new List<Partner>();
            foreach (var item in data)
            {
                var partner = !string.IsNullOrEmpty(item.Ref) && partner_dict.ContainsKey(item.Ref) ? partner_dict[item.Ref] : null;
                var addResult = address_check_dict.ContainsKey(item.Address) ? address_check_dict[item.Address] : null;
                if (partner == null)
                {
                    partner = new Partner();
                    partner.CompanyId = CompanyId;
                    partner.Name = item.Name;
                    partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);
                    partner.Ref = item.Ref;
                    partner.Phone = item.Phone;
                    partner.Comment = item.Note;
                    partner.Email = item.Email;
                    partner.Street = item.Street;

                    if (addResult != null)
                    {

                        partner.WardCode = addResult.WardCode != null ? addResult.WardCode : null;
                        partner.WardName = addResult.WardName != null ? addResult.WardName : null;
                        partner.DistrictCode = addResult.DistrictCode != null ? addResult.DistrictCode : null;
                        partner.DistrictName = addResult.DistrictName != null ? addResult.DistrictName : null;
                        partner.CityCode = addResult.CityCode != null ? addResult.CityCode : null;
                        partner.CityName = addResult.CityName != null ? addResult.CityName : null;
                    }
                    if (val.Type == "customer")
                    {
                        partner.Customer = true;
                        partner.JobTitle = item.Job;
                        partner.BirthDay = item.BirthDay;
                        partner.BirthMonth = item.BirthMonth;
                        partner.BirthYear = item.BirthYear;
                        GetGenderPartner(partner, item);
                        partner.Date = item.Date ?? DateTime.Today;
                        partner.PartnerHistoryRels.Clear();
                        if (!string.IsNullOrEmpty(item.MedicalHistory))
                        {
                            var medical_history_list = item.MedicalHistory.Split(",");
                            foreach (var mh in medical_history_list)
                            {
                                if (!medical_history_dict.ContainsKey(mh))
                                    continue;

                                partner.PartnerHistoryRels.Add(new PartnerHistoryRel { History = medical_history_dict[mh] });
                            }
                        }
                    }
                    else if (val.Type == "supplier")
                    {
                        partner.Customer = false;
                        partner.Supplier = true;
                    }
                    partnersCreate.Add(partner);

                }
                else
                {
                    partner.Name = item.Name;
                    partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);
                    partner.Ref = item.Ref;
                    partner.Phone = item.Phone;
                    partner.Comment = item.Note;
                    partner.Email = item.Email;
                    partner.Street = item.Street;
                    if (addResult != null)
                    {
                        partner.WardCode = addResult.WardCode != null ? addResult.WardCode : null;
                        partner.WardName = addResult.WardName != null ? addResult.WardName : null;
                        partner.DistrictCode = addResult.DistrictCode != null ? addResult.DistrictCode : null;
                        partner.DistrictName = addResult.DistrictName != null ? addResult.DistrictName : null;
                        partner.CityCode = addResult.CityCode != null ? addResult.CityCode : null;
                        partner.CityName = addResult.CityName != null ? addResult.CityName : null;
                    }

                    if (val.Type == "customer")
                    {
                        partner.JobTitle = item.Job;
                        partner.BirthDay = item.BirthDay;
                        partner.BirthMonth = item.BirthMonth;
                        partner.BirthYear = item.BirthYear;
                        GetGenderPartner(partner, item);
                        partner.Date = item.Date ?? DateTime.Today;
                        partner.PartnerHistoryRels.Clear();
                        if (!string.IsNullOrEmpty(item.MedicalHistory))
                        {
                            var medical_history_list = item.MedicalHistory.Split(",");
                            foreach (var mh in medical_history_list)
                            {
                                if (!medical_history_dict.ContainsKey(mh))
                                    continue;

                                partner.PartnerHistoryRels.Add(new PartnerHistoryRel { History = medical_history_dict[mh] });
                            }
                        }
                    }

                    partnersUpdate.Add(partner);
                }
            }


            if (partnersCreate.Any())
                await CreateAsync(partnersCreate);

            if (partnersUpdate.Any())
                await UpdateAsync(partnersUpdate);

            return new PartnerImportResponse { Success = true };
        }

        public void GetGenderPartner(Partner partner, PartnerImportRowExcel val)
        {
            var gender_dict = new Dictionary<string, string>()
            {
                { "nam", "male" },
                { "nữ", "female" },
                { "khác", "other" }
            };

            partner.Gender = !string.IsNullOrEmpty(val.Gender) && gender_dict.ContainsKey(val.Gender.ToLower()) ?
                       gender_dict[val.Gender.ToLower()] : "male";
        }

        public async Task<PartnerImportResponse> ImportSupplier(PartnerImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<PartnerSupplierRowExcel>();
            var errors = new List<string>();
            using (var stream = new MemoryStream(fileData))
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var errs = new List<string>();

                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        if (string.IsNullOrWhiteSpace(name))
                            errs.Add("Tên khách hàng là bắt buộc");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }

                        data.Add(new PartnerSupplierRowExcel
                        {
                            Name = name,
                            Ref = Convert.ToString(worksheet.Cells[row, 2].Value),
                            Phone = Convert.ToString(worksheet.Cells[row, 3].Value),
                            Fax = Convert.ToString(worksheet.Cells[row, 4].Value),
                            Address = Convert.ToString(worksheet.Cells[row, 5].Value),
                            Email = Convert.ToString(worksheet.Cells[row, 6].Value),
                            Note = Convert.ToString(worksheet.Cells[row, 7].Value),
                        });
                    }
                }
            }

            if (errors.Any())
                return new PartnerImportResponse { Success = false, Errors = errors };

            var partner_code_list = await SearchQuery(x => x.Supplier == true && x.Active && !string.IsNullOrEmpty(x.Ref))
                .GroupBy(x => x.Ref).Select(x => x.Key).ToListAsync();

            var address_list = data.Where(x => !string.IsNullOrWhiteSpace(x.Address)).Select(x => x.Address).ToList();
            var address_dict = await CheckAddressAsync(address_list);

            var data_to_insert = data.Where(x => string.IsNullOrEmpty(x.Ref) ||
                !partner_code_list.Contains(x.Ref)).ToList();

            var partners_to_insert = new List<Partner>();
            var medical_history_dict = new Dictionary<string, History>();
            var historyObj = GetService<IHistoryService>();
            foreach (var item in data_to_insert)
            {
                var partner = new Partner();
                partner.Supplier = true;
                partner.Customer = false;
                partner.CompanyId = CompanyId;
                partner.Name = item.Name;
                partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                partner.Ref = item.Ref;
                partner.Phone = item.Phone;
                partner.Fax = item.Fax;
                partner.Comment = item.Note;
                partner.Email = item.Email;

                if (!string.IsNullOrEmpty(item.Address))
                {
                    var addResult = address_dict.ContainsKey(item.Address) ? address_dict[item.Address] : null;
                    if (addResult != null)
                    {
                        partner.Street = addResult.ShortAddress;
                        partner.WardCode = addResult.WardCode;
                        partner.WardName = addResult.WardName;
                        partner.DistrictCode = addResult.DistrictCode;
                        partner.DistrictName = addResult.DistrictName;
                        partner.CityCode = addResult.CityCode;
                        partner.CityName = addResult.CityName;
                    }
                }

                partners_to_insert.Add(partner);
            }

            try
            {
                await CreateAsync(partners_to_insert);
            }
            catch (Exception e)
            {
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { e.Message } };
            }

            var data_to_update = data.Where(x => !string.IsNullOrEmpty(x.Ref) &&
              partner_code_list.Contains(x.Ref)).ToList();

            var partner_update_dict = await GetPartnerDictByRefs(partner_code_list, type: "supplier");
            var partners_to_update = new List<Partner>();
            foreach (var item in data_to_update)
            {
                var partner = partner_update_dict[item.Ref];
                partner.Name = item.Name;
                partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                partner.Ref = item.Ref;
                partner.Phone = item.Phone;
                partner.Fax = item.Fax;
                partner.Comment = item.Note;
                partner.Email = item.Email;

                if (!string.IsNullOrEmpty(item.Address))
                {
                    var addResult = address_dict.ContainsKey(item.Address) ? address_dict[item.Address] : null;
                    if (addResult != null)
                    {
                        partner.Street = addResult.ShortAddress;
                        partner.WardCode = addResult.WardCode;
                        partner.WardName = addResult.WardName;
                        partner.DistrictCode = addResult.DistrictCode;
                        partner.DistrictName = addResult.DistrictName;
                        partner.CityCode = addResult.CityCode;
                        partner.CityName = addResult.CityName;
                    }
                }

                partners_to_update.Add(partner);
            }

            try
            {
                await UpdateAsync(partners_to_update);
            }
            catch (Exception e)
            {
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { e.Message } };
            }

            return new PartnerImportResponse { Success = true };
        }

        public async Task<PartnerPrintProfileVM> GetPrint(Guid id)
        {
            var result = new PartnerPrintProfileVM();

            var userObj = GetService<IUserService>();
            var user = await userObj.GetCurrentUser();

            var companyObj = GetService<ICompanyService>();
            var company = await companyObj.SearchQuery(x => x.Id == user.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();

            var partnerObj = GetService<IPartnerService>();
            var partner = await GetByIdAsync(id);

            var companyPrint = _mapper.Map<CompanyPrintVM>(company);
            companyPrint.Address = partnerObj.GetFormatAddress(company.Partner);

            var partnerPrint = _mapper.Map<PartnerPrintVM>(partner);
            partnerPrint.Address = partnerObj.GetFormatAddress(partner);
            partnerPrint.Gender = partnerObj.GetGenderDisplay(partner);
            partnerPrint.DateOfBirth = this.GetDateOfBirthDisplay(partner);

            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var states = new string[] { "draft", "cancel" };
            var saleOrderLines = saleOrderLineObj.SearchQuery(x => x.OrderPartnerId == id && x.Product.Type2 == "service" && !states.Contains(x.State)).Select(x => new PartnerPrintProfileService
            {
                DateOrder = x.Order.DateOrder,
                UserName = x.Salesman.Name,
                ProductName = x.Product.Name,
                Teeth = x.SaleOrderLineToothRels.Select(s => s.Tooth.Name)
            }).ToList();

            var accountPaymentObj = GetService<IAccountPaymentService>();
            var accountPayments = accountPaymentObj.SearchQuery(x => x.PartnerId == id).Select(x => new PartnerPrintProfilePayment
            {
                PaymentDate = x.PaymentDate,
                JournalName = x.Journal.Name,
                Amount = x.Amount
            }).ToList();

            result.Company = companyPrint;
            result.Partner = partnerPrint;
            result.ServiceList = saleOrderLines;
            result.PaymentList = accountPayments;

            return result;
        }

        public async Task<IDictionary<Guid, Partner>> GetPartnerDictByIds(IEnumerable<Guid> ids)
        {
            var limit = 200;
            var offset = 0;
            var sub_ids = ids.Skip(offset).Take(limit).ToList();
            var res = new Dictionary<Guid, Partner>();
            while (sub_ids.Count > 0)
            {
                var list = await SearchQuery(x => sub_ids.Contains(x.Id)).Include(x => x.PartnerHistoryRels).ToListAsync();
                foreach (var item in list)
                    res.Add(item.Id, item);

                offset += limit;
                sub_ids = sub_ids.Skip(offset).Take(limit).ToList();
            }

            return res;
        }

        public async Task<IDictionary<string, Partner>> GetPartnerDictByRefs(IEnumerable<string> refs, string type = "customer")
        {
            var limit = 200;
            var offset = 0;

            var res = new Dictionary<string, Partner>();
            while (offset < refs.Count())
            {
                var sub_ids = refs.Skip(offset).Take(limit).ToList();
                var query = SearchQuery(x => sub_ids.Contains(x.Ref));
                if (type == "supplier")
                    query = query.Where(x => x.Supplier);
                else if (type == "customer")
                    query = query.Where(x => x.Customer);

                var list = await query.Include(x => x.PartnerHistoryRels).ToListAsync();
                foreach (var item in list)
                {
                    if (!res.ContainsKey(item.Ref))
                        res.Add(item.Ref, item);
                }

                offset += limit;
            }

            return res;
        }


        private async Task<Dictionary<string, AddressCheckApi>> CheckAddressAsync(List<string> strs, int limit = 100)
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
                var dict = await CheckAddressAsync(strs: adList.Distinct().ToList());
                var genderDict = new Dictionary<string, string>()
                {
                    { "nam","male" },
                    { "nữ","female" }
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
                        item.CityName = dict[address.ToLower()].CityName.Replace("TP", "Thành phố");
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

        public async Task<IEnumerable<PartnerReportLocationItem>> ReportLocationCompanyDistrict(PartnerReportLocationCompanySearch val)
        {
            var companyObj = GetService<ICompanyService>();
            var company = await companyObj.SearchQuery(x => x.Id == CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
            var query = SearchQuery(x => x.Customer && x.Active && x.CityCode == company.Partner.CityCode);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo);

            var res = await query.GroupBy(x => new { x.DistrictCode, x.DistrictName })
                .Select(x => new PartnerReportLocationItem
                {
                    Name = x.Key.DistrictName,
                    Total = x.Count(),
                    Percentage = x.Count() * 100f / query.Count()
                }).ToListAsync();
            return res;
        }

        public async Task<IEnumerable<PartnerReportLocationItem>> ReportLocationCompanyWard(PartnerReportLocationCompanySearch val)
        {
            var companyObj = GetService<ICompanyService>();
            var company = await companyObj.SearchQuery(x => x.Id == CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
            var query = SearchQuery(x => x.Customer && x.Active && x.CityCode == company.Partner.CityCode && x.DistrictCode == company.Partner.DistrictCode);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo);

            var res = await query.GroupBy(x => new { x.WardCode, x.WardName }).Select(x => new PartnerReportLocationItem
            {
                Name = x.Key.WardName,
                Total = x.Count(),
                Percentage = x.Count() * 100f / query.Count()
            }).ToListAsync();

            return res;
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

            foreach (var item in res)
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

        //public async Task<PartnerInfoViewModel> CheckPartner(CheckMergeFacebookPage val)
        //{
        //    var partnerid = await _partnerMapPSIDFacebookPageService.SearchQuery(x=>x.PageId == val.PageId && x.PSId == val.PSId).FirstOrDefaultAsync();
        //    var result = await SearchQuery(x => x.Id == partnerid.Id && x.Active == true).FirstOrDefaultAsync();
        //    if (result == null) {
        //        return new PartnerInfoViewModel();
        //    }
        //    var query = _mapper.Map<PartnerInfoViewModel>(result);
        //    return query;

        //}

        public IQueryable<PartnerViewModel> GetViewModels()
        {
            return SearchQuery().Select(x => new PartnerViewModel
            {
                Id = x.Id,
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                Street = x.Street,
                DisplayName = x.DisplayName,
                Name = x.Name,
                NameNoSign = x.NameNoSign,
                Phone = x.Phone,
                WardName = x.WardName,
                Gender = x.Gender,
                BirthYear = x.BirthYear,
                BirthMonth = x.BirthMonth,
                BirthDay = x.BirthDay,
                Customer = x.Customer,
                Supplier = x.Supplier,
                Ref = x.Ref,
                //LastAppointmentDate = x.Appointments.OrderByDescending(s => s.Date).FirstOrDefault().Date,
                Date = x.Date,
                //Source = x.Source != null ? new PartnerSourceViewModel
                //{
                //    Id = x.Source.Id,
                //    Name = x.Source.Name
                //} : null,
                Comment = x.Comment,
                Email = x.Email,
                JobTitle = x.JobTitle,
                Tags = x.PartnerPartnerCategoryRels.Select(s => new PartnerCategoryViewModel
                {
                    Id = s.CategoryId,
                    Name = s.Category.Name,
                })
            });
        }

        public IQueryable<GridPartnerViewModel> GetGridViewModels()
        {
            return SearchQuery().Select(x => new GridPartnerViewModel
            {
                Id = x.Id,
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                Street = x.Street,
                DisplayName = x.DisplayName,
                Name = x.Name,
                NameNoSign = x.NameNoSign,
                Phone = x.Phone,
                WardName = x.WardName,
                Gender = x.Gender,
                BirthYear = x.BirthYear,
                BirthMonth = x.BirthMonth,
                BirthDay = x.BirthDay,
                Customer = x.Customer,
                Supplier = x.Supplier,
                Ref = x.Ref,
                Date = x.Date,
                Comment = x.Comment,
                Email = x.Email,
                JobTitle = x.JobTitle,
                SourceName = x.Source.Name,
                DateCreated = x.DateCreated
            });
        }

        public Task<IQueryable<PartnerViewModel>> GetViewModelsAsync()
        {
            return Task.Run(() => GetViewModels());
        }

        public Task<IQueryable<GridPartnerViewModel>> GetGridViewModelsAsync()
        {
            return Task.Run(() => GetGridViewModels());
        }
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

    public class CheckZaloPhoneProfile
    {
        public string Phone { get; set; }
        public GetProfileOfFollowerResponse Profile { get; set; }
    }


}
