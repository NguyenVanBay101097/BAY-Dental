using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class PartnerService : BaseService<Partner>, IPartnerService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppTenant _tenant;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // private readonly IPartnerMapPSIDFacebookPageService _partnerMapPSIDFacebookPageService;
        public PartnerService(CatalogDbContext context, IAsyncRepository<Partner> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, UserManager<ApplicationUser> userManager, ITenant<AppTenant> tenant,
            IConfiguration configuration)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            //_partnerMapPSIDFacebookPageService = partnerMapPSIDFacebookPageService;
            _tenant = tenant?.Value;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public override ISpecification<Partner> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "base.res_partner_rule":
                    return new InitialSpecification<Partner>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
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
                .Include(x => x.Agent)
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
            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            if (val.ComputeCreditDebit)
            {
                var cateList = await cateObj.SearchQuery(x => x.PartnerPartnerCategoryRels.Any(s => items.Select(i => i.Id).Contains(s.PartnerId)))
                                                                                            .Include(x => x.PartnerPartnerCategoryRels).ToListAsync();
                var partnerBasics = _mapper.Map<List<PartnerBasic>>(items);
                var creditDebitDict = CreditDebitGet(items.Select(x => x.Id).ToList());
                foreach (var item in partnerBasics)
                {
                    item.Credit = creditDebitDict[item.Id].Credit;
                    item.Debit = creditDebitDict[item.Id].Debit;
                    item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(cateList.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.PartnerId == item.Id)));
                }
                return new PagedResult2<PartnerBasic>(totalItems, val.Offset, val.Limit)
                {
                    Items = partnerBasics
                };
            }
            else
            {
                return new PagedResult2<PartnerBasic>(totalItems, val.Offset, val.Limit)
                {
                    Items = _mapper.Map<IEnumerable<PartnerBasic>>(items)
                };
            }

        }



        public async Task<AppointmentBasic> GetNextAppointment(Guid id)
        {
            var apObj = GetService<IAppointmentService>();
            var appointment = await apObj.SearchQuery(x => x.PartnerId == id, orderBy: x => x.OrderByDescending(s => s.Date))
                .Include(x => x.Doctor)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();
            var res = _mapper.Map<AppointmentBasic>(appointment);
            return res;
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
            return await seqObj.NextByCode(type);
        }

        private async Task _CheckUniqueRef(IEnumerable<Partner> self)
        {
            var customers = self.Where(x => x.Customer).ToList();
            if (customers.Any())
            {
                var customerRefs = customers.Where(x => !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).ToList();
                var group = await SearchQuery(x => x.Customer && customerRefs.Contains(x.Ref))
                    .GroupBy(x => x.Ref)
                    .Select(x => new
                    {
                        Ref = x.Key,
                        Count = x.Count()
                    })
                    .ToListAsync();

                var existCodes = group.Where(x => x.Count > 1).Select(x => x.Ref).ToList();
                if (existCodes.Any())
                    throw new Exception($"Đã tồn tại khách hàng với mã: {string.Join(", ", existCodes)}");
            }

            var suppliers = self.Where(x => x.Supplier).ToList();
            if (suppliers.Any())
            {
                var supplierRefs = suppliers.Where(x => !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).ToList();
                var group = await SearchQuery(x => x.Supplier && supplierRefs.Contains(x.Ref))
                    .GroupBy(x => x.Ref)
                    .Select(x => new
                    {
                        Ref = x.Key,
                        Count = x.Count()
                    })
                    .ToListAsync();

                var existCodes = group.Where(x => x.Count > 1).Select(x => x.Ref).ToList();
                if (existCodes.Any())
                    throw new Exception($"Đã tồn tại nhà cung cấp với mã: {string.Join(", ", existCodes)}");
            }
        }

        public override async Task<IEnumerable<Partner>> CreateAsync(IEnumerable<Partner> entities)
        {
            await base.CreateAsync(entities);
            await _CheckUniqueRef(entities);
            return entities;
        }

        public override async Task UpdateAsync(IEnumerable<Partner> entities)
        {
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
            {
                partner.DisplayName = _NameGet(partner);
                partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);
            }
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

        public async Task<decimal> GetAmountAdvanceBalance(Guid id)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();

            var query = moveLineObj._QueryGet(state: "posted", companyId: CompanyId);
            query = query.Where(x => x.PartnerId == id && (x.Account.Code == "KHTU" || x.Account.Code == "HTU"));

            var amounBalance = await query.SumAsync(x => x.Debit - x.Credit);
            var sign = -1;
            return amounBalance * sign;
        }

        public async Task<decimal> GetAmountAdvanceUsed(Guid id)
        {
            ///lay tu moveline journal amount advance used
            var paymentObj = GetService<IAccountPaymentService>();
            var journalObj = GetService<IAccountJournalService>();
            var journalAdvance = await journalObj.SearchQuery(x => x.CompanyId == CompanyId && x.Type == "advance" && x.Active).FirstOrDefaultAsync();
            var amounAdvance = await paymentObj.SearchQuery(x => x.PartnerId == id && x.JournalId == journalAdvance.Id && x.State != "cancel").Select(x => x.Amount).SumAsync();
            return Math.Abs(amounAdvance);
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
                BirthDay = x.BirthDay,
                BirthMonth = x.BirthMonth,
                Email = x.Email,
                JobTitle = x.JobTitle,
                CityName = x.CityName,
                Street = x.Street,
                Gender = x.Gender,
                DistrictName = x.DistrictName,
                WardName = x.WardName
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
            if (val.Active.HasValue)
                query = query.Where(x => x.Active == val.Active);
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
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

        public async Task<IEnumerable<PartnerCustomerExportExcelVM>> GetExcel(PartnerInfoPaged val)
        {
            var query = await GetQueryPartnerInfoPaged2(val);
            if (val.CategIds.Any())
            {
                //filter query
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();
                var filterPartnerIds = await partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId)).Select(x => x.PartnerId).Distinct().ToListAsync();
                query = query.Where(x => filterPartnerIds.Contains(x.Id));
            }

            var res = await query.OrderByDescending(x => x.DateCreated).Select(x => new PartnerCustomerExportExcelVM
            {
                Name = x.Name,
                Ref = x.Ref,
                Date = x.Date,
                Gender = x.Gender,
                BirthDay = x.BirthDay,
                BirthMonth = x.BirthMonth,
                BirthYear = x.BirthYear,
                Phone = x.Phone,
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                WardName = x.WardName,
                Street = x.Street,
                Job = x.JobTitle,
                Email = x.Email,
                Note = x.Comment,
                Id = x.Id,
                SourceName = x.SourceName,
                TitleName = x.TitleName
            }).ToListAsync();

            var historyRelObj = GetService<IHistoryService>();
            var pnIds = res.Select(x => x.Id).ToList();
            var Histories = await historyRelObj.SearchQuery(x => x.PartnerHistoryRels.Any(z => pnIds.Contains(z.PartnerId)))
                                                .Include(x => x.PartnerHistoryRels).ToListAsync();

            var partnerCategoryRelObj = GetService<IPartnerPartnerCategoryRelService>();
            var cateList = await partnerCategoryRelObj.SearchQuery(x => res.Select(i => i.Id).Contains(x.PartnerId)).Include(x => x.Category).ToListAsync();
            var categDict = cateList.GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.Select(s => s.Category));

            foreach (var item in res)
            {
                item.MedicalHistories = Histories.Where(x => x.PartnerHistoryRels.Any(z => z.PartnerId == item.Id)).Select(x => x.Name).ToList();
                item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(categDict.ContainsKey(item.Id) ? categDict[item.Id] : new List<PartnerCategory>());
            }

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

        public dynamic ReadExcelData(PartnerImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<PartnerImportRowExcel>();
            var errors = new List<string>();
            var partner_code_list = new List<string>();

            var title_dict = new Dictionary<string, string>()
            {
                { "customer", "KH" },
                { "supplier", "NCC" },
            };

            using (var stream = new MemoryStream(fileData))
            {
                try
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

                            if (val.Type == "customer")
                            {
                                var reference = Convert.ToString(worksheet.Cells[row, 2].Value);
                                if (!string.IsNullOrWhiteSpace(reference))
                                {
                                    if (partner_code_list.Contains(reference))
                                        errs.Add($"Đã tồn tại mã {title_dict[val.Type]} {reference}");
                                    else
                                        partner_code_list.Add(reference);
                                }

                                var medicalHistory = Convert.ToString(worksheet.Cells[row, 13].Value);

                                try
                                {
                                    DateTime? date = null;
                                    var dateExcel = Convert.ToString(worksheet.Cells[row, 3].Value);
                                    if (!string.IsNullOrEmpty(dateExcel))
                                    {
                                        if (double.TryParse(dateExcel, out double tmp))
                                            date = DateTime.FromOADate(tmp);
                                        else if (DateTime.TryParse(dateExcel, out DateTime tmp2))
                                            date = tmp2;
                                        else
                                            errs.Add($"Ngày tạo không đúng định dạng");
                                    }

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
                                catch
                                {
                                    errors.Add($"Dòng {row}: {"Dữ liệu import chưa đúng định dạng"}");
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
                                        Phone = Convert.ToString(worksheet.Cells[row, 2].Value),
                                        Fax = Convert.ToString(worksheet.Cells[row, 3].Value),
                                        Street = Convert.ToString(worksheet.Cells[row, 4].Value),
                                        WardName = Convert.ToString(worksheet.Cells[row, 5].Value),
                                        DistrictName = Convert.ToString(worksheet.Cells[row, 6].Value),
                                        CityName = Convert.ToString(worksheet.Cells[row, 7].Value),
                                        Email = Convert.ToString(worksheet.Cells[row, 8].Value),
                                        Note = Convert.ToString(worksheet.Cells[row, 9].Value),
                                    });
                                }
                                catch
                                {
                                    errors.Add($"Dòng {row}: Dữ liệu import chưa đúng định dạng");
                                    continue;
                                }
                            }

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    throw new Exception("File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng");
                }
            }

            return new
            {
                Errors = errors,
                Data = data
            };
        }

        public dynamic ReadExcelSupplierImportData(string fileBase64)
        {
            var fileData = Convert.FromBase64String(fileBase64);
            var data = new List<PartnerSupplierImportRowExcel>();
            var errors = new List<string>();
            var partner_code_list = new List<string>();

            using (var stream = new MemoryStream(fileData))
            {
                try
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
                                errs.Add($"Tên NCC là bắt buộc");

                            try
                            {
                                data.Add(new PartnerSupplierImportRowExcel
                                {
                                    Name = name,
                                    Fax = Convert.ToString(worksheet.Cells[row, 3].Value),
                                    Phone = Convert.ToString(worksheet.Cells[row, 2].Value),
                                    Street = Convert.ToString(worksheet.Cells[row, 4].Value),
                                    WardName = Convert.ToString(worksheet.Cells[row, 5].Value),
                                    DistrictName = Convert.ToString(worksheet.Cells[row, 6].Value),
                                    CityName = Convert.ToString(worksheet.Cells[row, 7].Value),
                                    Email = Convert.ToString(worksheet.Cells[row, 8].Value),
                                    Note = Convert.ToString(worksheet.Cells[row, 9].Value),
                                });
                            }
                            catch
                            {
                                errors.Add($"Dòng {row}: {"Dữ liệu import chưa đúng định dạng"}");
                                continue;
                            }

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    throw new Exception("File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng");
                }
            }

            return new
            {
                Errors = errors,
                Data = data
            };
        }

        public dynamic ReadExcelCustomerUpdateData(string fileBase64)
        {
            var fileData = Convert.FromBase64String(fileBase64);
            var data = new List<PartnerCustomerExportExcelVM>();
            var errors = new List<string>();
            var partner_code_list = new List<string>();

            using (var stream = new MemoryStream(fileData))
            {
                try
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
                                errs.Add($"Tên KH là bắt buộc");

                            var reference = Convert.ToString(worksheet.Cells[row, 2].Value);
                            if (!string.IsNullOrWhiteSpace(reference))
                            {
                                if (partner_code_list.Contains(reference))
                                    errs.Add($"Đã tồn tại mã KH {reference}");
                                else
                                    partner_code_list.Add(reference);
                            }
                            else
                                errs.Add($"Mã KH là bắt buộc");

                            var medicalHistory = Convert.ToString(worksheet.Cells[row, 14].Value);

                            try
                            {
                                DateTime? date = null;
                                var dateExcel = Convert.ToString(worksheet.Cells[row, 3].Value);
                                if (!string.IsNullOrEmpty(dateExcel))
                                {
                                    if (double.TryParse(dateExcel, out double dateLong))
                                        date = DateTime.FromOADate(dateLong);
                                    else if (DateTime.TryParse(dateExcel, out DateTime dateTmp))
                                        date = dateTmp;
                                    else
                                        errs.Add($"Ngày tạo không đúng định dạng");
                                }

                                var birthDayStr = Convert.ToString(worksheet.Cells[row, 6].Value);
                                var birthMonthStr = Convert.ToString(worksheet.Cells[row, 7].Value);
                                var birthYearStr = Convert.ToString(worksheet.Cells[row, 8].Value);

                                data.Add(new PartnerCustomerExportExcelVM
                                {
                                    Name = name,
                                    Ref = reference,
                                    Date = date,
                                    Gender = Convert.ToString(worksheet.Cells[row, 5].Value),
                                    BirthDay = !string.IsNullOrWhiteSpace(birthDayStr) ? Convert.ToInt32(birthDayStr) : (int?)null,
                                    BirthMonth = !string.IsNullOrWhiteSpace(birthMonthStr) ? Convert.ToInt32(birthMonthStr) : (int?)null,
                                    BirthYear = !string.IsNullOrWhiteSpace(birthYearStr) ? Convert.ToInt32(birthYearStr) : (int?)null,
                                    Phone = Convert.ToString(worksheet.Cells[row, 4].Value),
                                    Street = Convert.ToString(worksheet.Cells[row, 9].Value),
                                    WardName = Convert.ToString(worksheet.Cells[row, 10].Value),
                                    DistrictName = Convert.ToString(worksheet.Cells[row, 11].Value),
                                    CityName = Convert.ToString(worksheet.Cells[row, 12].Value),
                                    PartnerCategories = Convert.ToString(worksheet.Cells[row, 13].Value),
                                    MedicalHistory = medicalHistory,
                                    Job = Convert.ToString(worksheet.Cells[row, 16].Value),
                                    Email = Convert.ToString(worksheet.Cells[row, 15].Value),
                                    SourceName = Convert.ToString(worksheet.Cells[row, 17].Value),
                                    Note = Convert.ToString(worksheet.Cells[row, 18].Value),
                                });
                            }
                            catch
                            {
                                errors.Add($"Dòng {row}: {"Dữ liệu import chưa đúng định dạng"}");
                                continue;
                            }

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    throw new Exception("File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng");
                }
            }

            return new
            {
                Errors = errors,
                Data = data
            };
        }

        public dynamic ReadExcelCustomerImportData(string fileBase64)
        {
            var fileData = Convert.FromBase64String(fileBase64);
            var data = new List<PartnerCustomerImportRowExcel>();
            var errors = new List<string>();
            var partner_code_list = new List<string>();

            using (var stream = new MemoryStream(fileData))
            {
                try
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
                                errs.Add($"Tên KH là bắt buộc");

                            var reference = Convert.ToString(worksheet.Cells[row, 2].Value);
                            if (!string.IsNullOrWhiteSpace(reference))
                            {
                                if (partner_code_list.Contains(reference))
                                    errs.Add($"Đã tồn tại mã KH {reference}");
                                else
                                    partner_code_list.Add(reference);
                            }

                            var medicalHistory = Convert.ToString(worksheet.Cells[row, 13].Value);

                            try
                            {
                                DateTime? date = null;
                                var dateExcel = Convert.ToString(worksheet.Cells[row, 3].Value);
                                if (!string.IsNullOrEmpty(dateExcel))
                                {
                                    if (double.TryParse(dateExcel, out double dateLong))
                                        date = DateTime.FromOADate(dateLong);
                                    else if (DateTime.TryParse(dateExcel, out DateTime dateTmp))
                                        date = dateTmp;
                                    else
                                        errs.Add($"Ngày tạo không đúng định dạng");
                                }

                                var birthDayStr = Convert.ToString(worksheet.Cells[row, 5].Value);
                                var birthMonthStr = Convert.ToString(worksheet.Cells[row, 6].Value);
                                var birthYearStr = Convert.ToString(worksheet.Cells[row, 7].Value);

                                data.Add(new PartnerCustomerImportRowExcel
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
                            catch
                            {
                                errors.Add($"Dòng {row}: {"Dữ liệu import chưa đúng định dạng"}");
                                continue;
                            }

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    throw new Exception("File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng");
                }
            }

            return new
            {
                Errors = errors,
                Data = data
            };
        }

        public async Task<PartnerImportResponse> SupplierImport(string fileBase64)
        {
            //hàm này chỉ thêm partner, ko có update
            var readResult = ReadExcelSupplierImportData(fileBase64);
            var errors = readResult.Errors as IEnumerable<string>;
            var data = readResult.Data as IEnumerable<PartnerSupplierImportRowExcel>;

            if (errors.Any())
                return new PartnerImportResponse { Success = false, Errors = errors };

            var matchedCodes = await SearchQuery(x => x.Supplier && !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).ToListAsync();
            var seqObj = GetService<IIRSequenceService>();
            foreach (var num in Enumerable.Range(1, 100))
            {
                var tmp = data.Where(x => string.IsNullOrEmpty(x.Ref)).ToList();
                if (tmp.Any())
                {
                    var codeList = await seqObj.NextByCode("supplier", tmp.Count).ConfigureAwait(false);
                    for (var i = 0; i < tmp.Count; i++)
                    {
                        var item = tmp[i];
                        if (!matchedCodes.Contains(codeList[i]))
                            item.Ref = codeList[i];
                    }
                }
                else
                    break;
            }

            if (data.Any(x => string.IsNullOrEmpty(x.Ref)))
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { $"Có những dòng không phát sinh mã đc" } };

            var partners = new List<Partner>();
            foreach (var item in data)
            {
                var partner = new Partner();
                partner.CompanyId = CompanyId;
                partner.Name = item.Name;
                partner.Phone = item.Phone;
                partner.Comment = item.Note;
                partner.Email = item.Email;
                partner.Street = item.Street;
                partner.Customer = false;
                partner.Supplier = true;
                partner.Fax = item.Fax;
                partner.Ref = item.Ref;

                partners.Add(partner);
            }

            try
            {
                if (partners.Any())
                    await CreateAsync(partners);
            }
            catch (Exception ex)
            {
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { ex.Message } };
            }

            return new PartnerImportResponse { Success = true };
        }

        public async Task<PartnerImportResponse> ActionImportUpdate(PartnerImportExcelViewModel val)
        {
            //Hàm chỉ update
            var readResult = ReadExcelData(val);
            var errors = readResult.Errors as IEnumerable<string>;
            var data = readResult.Data as IEnumerable<PartnerImportRowExcel>;

            if (errors.Any())
                return new PartnerImportResponse { Success = false, Errors = errors };

            //Get list partner ref
            var partner_code_list = data.Where(x => !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).Distinct().ToList();
            var partner_dict = await GetPartnerDictByRefs(partner_code_list);

            var notFoundPartnerCodes = partner_code_list.Where(x => !partner_dict.ContainsKey(x)).ToList();
            if (notFoundPartnerCodes.Any())
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { $"Mã khách hàng không tồn tại: {string.Join(", ", notFoundPartnerCodes)}" } };

            var address_check_dict = new Dictionary<string, AddressCheckApi>();
            var address_list = data.Where(x => !string.IsNullOrWhiteSpace(x.Address)).Select(x => x.Address).Distinct().ToList();
            //Get list Api check address distionary
            address_check_dict = await CheckAddressAsync(address_list);

            var medical_history_dict = new Dictionary<string, History>();
            var partner_history_list = data.Where(x => !string.IsNullOrEmpty(x.MedicalHistory)).Select(x => x.MedicalHistory.Split(",")).SelectMany(x => x).Distinct().ToList();
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

            var partners = new List<Partner>();
            foreach (var item in data)
            {
                var partner = !string.IsNullOrEmpty(item.Ref) && partner_dict.ContainsKey(item.Ref) ? partner_dict[item.Ref] : null;
                if (partner == null)
                    continue;

                var addResult = address_check_dict.ContainsKey(item.Address) ? address_check_dict[item.Address] : null;

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

                partners.Add(partner);
            }

            try
            {
                if (partners.Any())
                    await UpdateAsync(partners);
            }
            catch (Exception ex)
            {
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { ex.Message } };
            }

            return new PartnerImportResponse { Success = true };
        }

        public async Task<PartnerImportResponse> CustomerImportUpdate(string fileBase64)
        {
            //Hàm chỉ update
            var readResult = ReadExcelCustomerUpdateData(fileBase64);
            var errors = readResult.Errors as IEnumerable<string>;
            var data = readResult.Data as IEnumerable<PartnerCustomerExportExcelVM>;

            if (errors.Any())
                return new PartnerImportResponse { Success = false, Errors = errors };

            //Get list partner ref
            var partner_code_list = data.Where(x => !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).Distinct().ToList();
            var customerData = await SearchQuery(x => x.Customer && !string.IsNullOrEmpty(x.Ref) && partner_code_list.Contains(x.Ref)).Select(x => new
            {
                Id = x.Id,
                Ref = x.Ref
            }).ToListAsync();
            var customerIds = customerData.Select(x => x.Id).ToList();
            var customerCodes = customerData.Select(x => x.Ref).ToList();
            var notFoundPartnerCodes = partner_code_list.Except(customerCodes);
            if (notFoundPartnerCodes.Any())
                return new PartnerImportResponse { Success = false, Errors = new List<string>() { $"Mã khách hàng không tồn tại: {string.Join(", ", notFoundPartnerCodes)}" } };

            var address_check_dict = new Dictionary<string, AddressCheckApi>();
            var address_list = data.Where(x => !string.IsNullOrWhiteSpace(x.CheckAddress))
                .Select(x => x.CheckAddress).Distinct().ToList();
            //Get list Api check address distionary
            address_check_dict = await CheckAddressAsync(address_list);

            var gender_dict = new Dictionary<string, string>()
            {
                { "nam", "male" },
                { "nữ", "female" },
                { "khác", "other" }
            };

            using (TransactionScope scope = new TransactionScope(
             TransactionScopeOption.Required,
             new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
             TransactionScopeAsyncFlowOption.Enabled))
            {
                CatalogDbContext context = new CatalogDbContext(new DbContextOptionsBuilder<CatalogDbContext>().Options, _tenant, _configuration);

                var medical_history_dict = new Dictionary<string, History>();
                var partner_history_list = data.Where(x => !string.IsNullOrEmpty(x.MedicalHistory)).Select(x => x.MedicalHistory.Split(",")).SelectMany(x => x).Distinct().ToList();
                if (partner_history_list.Any())
                {
                    var historyObj = new EfRepository<History>(context);
                    var histories = await historyObj.SearchQuery(x => partner_history_list.Contains(x.Name)).ToListAsync();
                    foreach (var history in histories)
                    {
                        if (!medical_history_dict.ContainsKey(history.Name))
                            medical_history_dict.Add(history.Name, history);
                    }

                    var histories_name_to_insert = partner_history_list.Except(histories.Select(x => x.Name));
                    var histories_to_insert = histories_name_to_insert.Select(x => new History { Name = x }).ToList();
                    await historyObj.InsertAsync(histories_to_insert).ConfigureAwait(false);

                    foreach (var history in histories_to_insert)
                    {
                        if (!medical_history_dict.ContainsKey(history.Name))
                            medical_history_dict.Add(history.Name, history);
                    }
                }

                var partner_category_dict = new Dictionary<string, PartnerCategory>();
                var partner_category_list = data.Where(x => !string.IsNullOrEmpty(x.PartnerCategories)).Select(x => x.PartnerCategories.Split(",")).SelectMany(x => x).Distinct().ToList();
                if (partner_category_list.Any())
                {
                    var partnerCategObj = new EfRepository<PartnerCategory>(context);
                    var categs = await partnerCategObj.SearchQuery(x => partner_category_list.Contains(x.Name)).ToListAsync();
                    foreach (var categ in categs)
                    {
                        if (!partner_category_dict.ContainsKey(categ.Name))
                            partner_category_dict.Add(categ.Name, categ);
                    }

                    var categories_name_to_insert = partner_category_list.Except(categs.Select(x => x.Name));
                    var categories_to_insert = categories_name_to_insert.Select(x => new PartnerCategory { Name = x }).ToList();
                    await partnerCategObj.InsertAsync(categories_to_insert);

                    foreach (var categ in categories_to_insert)
                    {
                        if (!partner_category_dict.ContainsKey(categ.Name))
                            partner_category_dict.Add(categ.Name, categ);
                    }
                }

                var partner_source_dict = new Dictionary<string, PartnerSource>();
                var partner_source_list = data.Where(x => !string.IsNullOrEmpty(x.SourceName)).Select(x => x.SourceName).Distinct().ToList();
                if (partner_source_list.Any())
                {
                    var partnerSourceObj = new EfRepository<PartnerSource>(context);
                    var sources = await partnerSourceObj.SearchQuery(x => partner_source_list.Contains(x.Name)).ToListAsync();
                    foreach (var src in sources)
                    {
                        if (!partner_source_dict.ContainsKey(src.Name))
                            partner_source_dict.Add(src.Name, src);
                    }

                    var sources_name_to_insert = partner_source_list.Except(sources.Select(x => x.Name));
                    var sources_to_insert = sources_name_to_insert.Select(x => new PartnerSource { Name = x }).ToList();
                    await partnerSourceObj.InsertAsync(sources_to_insert);

                    foreach (var src in sources_to_insert)
                    {
                        if (!partner_source_dict.ContainsKey(src.Name))
                            partner_source_dict.Add(src.Name, src);
                    }
                }

                var offset = 0;
                var limit = 1000;
                var subData = data.Skip(offset).Take(limit).ToList();
                while (subData.Any())
                {
                    var subCustomerIds = customerData.Where(x => subData.Select(x => x.Ref).Contains(x.Ref)).Select(x => x.Id).ToList();
                    var partners = await context.Partners.Where(x => subCustomerIds.Contains(x.Id))
                        .Include(x => x.PartnerHistoryRels)
                        .Include(x => x.PartnerPartnerCategoryRels)
                        .ToListAsync();
                    var partner_dict = partners.GroupBy(x => x.Ref).ToDictionary(x => x.Key, x => x.FirstOrDefault());

                    foreach (var item in data)
                    {
                        var partner = !string.IsNullOrEmpty(item.Ref) && partner_dict.ContainsKey(item.Ref) ? partner_dict[item.Ref] : null;
                        if (partner == null)
                            continue;

                        var addResult = !string.IsNullOrEmpty(item.CheckAddress) && address_check_dict.ContainsKey(item.CheckAddress) ? address_check_dict[item.CheckAddress] : null;

                        partner.Name = item.Name;
                        partner.Ref = item.Ref;
                        partner.Phone = item.Phone;
                        partner.Comment = item.Note;
                        partner.Email = item.Email;
                        partner.Street = item.Street;
                        if (addResult != null)
                        {
                            partner.WardCode = addResult.WardCode;
                            partner.WardName = addResult.WardName;
                            partner.DistrictCode = addResult.DistrictCode;
                            partner.DistrictName = addResult.DistrictName;
                            partner.CityCode = addResult.CityCode;
                            partner.CityName = addResult.CityName;
                        }

                        partner.JobTitle = item.Job;
                        partner.BirthDay = item.BirthDay;
                        partner.BirthMonth = item.BirthMonth;
                        partner.BirthYear = item.BirthYear;
                        partner.Gender = !string.IsNullOrEmpty(item.Gender) && gender_dict.ContainsKey(item.Gender.ToLower()) ?
                           gender_dict[item.Gender.ToLower()] : "male";
                        partner.Date = item.Date ?? DateTime.Today;
                        partner.Source = !string.IsNullOrEmpty(item.SourceName) && partner_source_dict.ContainsKey(item.SourceName) ? partner_source_dict[item.SourceName] : null;
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

                        partner.PartnerPartnerCategoryRels.Clear();
                        if (!string.IsNullOrEmpty(item.PartnerCategories))
                        {
                            foreach (var ct in item.PartnerCategories.Split(","))
                            {
                                if (!partner_category_dict.ContainsKey(ct))
                                    continue;

                                partner.PartnerPartnerCategoryRels.Add(new PartnerPartnerCategoryRel { Category = partner_category_dict[ct] });
                            }
                        }
                    }

                    context.SaveChanges();
                    context.Dispose();
                    context = new CatalogDbContext(new DbContextOptionsBuilder<CatalogDbContext>().Options, _tenant, _configuration);

                    offset += limit;
                    subData = data.Skip(offset).Take(limit).ToList();
                }

                scope.Complete();
            }

            return new PartnerImportResponse { Success = true };
        }

        public async Task<PartnerImportResponse> CustomerImport(string fileBase64)
        {
            //Hàm chỉ update
            var readResult = ReadExcelCustomerImportData(fileBase64);
            var errors = readResult.Errors as IEnumerable<string>;
            var data = readResult.Data as IEnumerable<PartnerCustomerImportRowExcel>;

            if (errors.Any())
                return new PartnerImportResponse { Success = false, Errors = errors };

            var address_check_dict = new Dictionary<string, AddressCheckApi>();
            var address_list = data.Where(x => !string.IsNullOrWhiteSpace(x.CheckAddress)).Select(x => x.CheckAddress).Distinct().ToList();
            //Get list Api check address distionary
            address_check_dict = await CheckAddressAsync(address_list);

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                CatalogDbContext context = new CatalogDbContext(new DbContextOptionsBuilder<CatalogDbContext>().Options, _tenant, _configuration);

                var itemsNoCode = data.Where(x => string.IsNullOrEmpty(x.Ref)).ToList();
                if (itemsNoCode.Any())
                {
                    var seqRepository = new EfRepository<IRSequence>(context);
                    var seqObj = new IRSequenceService(seqRepository, _httpContextAccessor);
                    var matchedCodes = await SearchQuery(x => x.Customer && !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).ToListAsync();
                    foreach (var num in Enumerable.Range(1, 100))
                    {
                        var tmp = itemsNoCode.Where(x => string.IsNullOrEmpty(x.Ref)).ToList();
                        if (tmp.Any())
                        {
                            var codeList = await seqObj.NextByCode("customer", tmp.Count).ConfigureAwait(false);
                            for (var i = 0; i < tmp.Count; i++)
                            {
                                var item = tmp[i];
                                if (!matchedCodes.Contains(codeList[i]))
                                    item.Ref = codeList[i];
                            }
                        }
                        else
                            break;
                    }
                }

                if (data.Any(x => string.IsNullOrEmpty(x.Ref)))
                    return new PartnerImportResponse { Success = false, Errors = new List<string>() { $"Có những dòng không phát sinh mã đc" } };

                var group = data.GroupBy(x => x.Ref).Select(x => new
                {
                    DefaultCode = x.Key,
                    Count = x.Count()
                });

                if (group.Any(x => x.Count > 1))
                {
                    var duplicateCodes = group.Where(x => x.Count > 1).Select(x => x.DefaultCode).ToList();
                    return new PartnerImportResponse { Success = false, Errors = new List<string>() { $"Dữ liệu file excel có những mã khách hàng bị trùng: {string.Join(", ", duplicateCodes)}" } };
                }

                //check mã duy nhất
                var customerCodes = await SearchQuery(x => x.Customer && !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).ToListAsync();
                var existCodes = data.Select(x => x.Ref).Intersect(customerCodes);
                if (existCodes.Any())
                    return new PartnerImportResponse { Success = false, Errors = new List<string>() { $"Đã tồn tại khách hàng với mã: {string.Join(", ", existCodes)}" } };

                var medical_history_dict = new Dictionary<string, History>();
                var partner_history_list = data.Where(x => !string.IsNullOrEmpty(x.MedicalHistory)).Select(x => x.MedicalHistory.Split(",")).SelectMany(x => x).Distinct().ToList();
                if (partner_history_list.Any())
                {
                    var historyObj = new EfRepository<History>(context);
                    var histories = await historyObj.SearchQuery(x => partner_history_list.Contains(x.Name)).ToListAsync();
                    foreach (var history in histories)
                    {
                        if (!medical_history_dict.ContainsKey(history.Name))
                            medical_history_dict.Add(history.Name, history);
                    }

                    var histories_name_to_insert = partner_history_list.Except(histories.Select(x => x.Name));
                    var histories_to_insert = histories_name_to_insert.Select(x => new History { Name = x }).ToList();
                    await historyObj.InsertAsync(histories_to_insert).ConfigureAwait(false);

                    foreach (var history in histories_to_insert)
                    {
                        if (!medical_history_dict.ContainsKey(history.Name))
                            medical_history_dict.Add(history.Name, history);
                    }
                }

                var gender_dict = new Dictionary<string, string>()
                {
                    { "Nam", "male" },
                    { "Nữ", "female" },
                    { "Khác", "other" }
                };



                var partners = new List<Partner>();
                foreach (var item in data)
                {
                    var addResult = address_check_dict.ContainsKey(item.CheckAddress) ? address_check_dict[item.CheckAddress] : null;
                    var partner = new Partner();
                    partner.CompanyId = CompanyId;
                    partner.Name = item.Name;
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

                    partner.Customer = true;
                    partner.JobTitle = item.Job;
                    partner.BirthDay = item.BirthDay;
                    partner.BirthMonth = item.BirthMonth;
                    partner.BirthYear = item.BirthYear;
                    partner.Gender = !string.IsNullOrEmpty(item.Gender) && gender_dict.ContainsKey(item.Gender) ? gender_dict[item.Gender] : "male";
                    partner.Date = item.Date ?? DateTime.Today;
                    if (!string.IsNullOrEmpty(item.MedicalHistory))
                    {
                        var medical_history_list = item.MedicalHistory.Split(",");
                        foreach (var mh in medical_history_list)
                        {
                            if (!medical_history_dict.ContainsKey(mh))
                                continue;

                            partner.PartnerHistoryRels.Add(new PartnerHistoryRel { HistoryId = medical_history_dict[mh].Id });
                        }
                    }

                    partners.Add(partner);
                }


                try
                {
                    int count = 0;
                    foreach (var entityToInsert in partners)
                    {
                        ++count;
                        context = AddToContext(context, entityToInsert, count, 100, true);
                    }

                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                scope.Complete();
            }

            return new PartnerImportResponse { Success = true };
        }

        private CatalogDbContext AddToContext(CatalogDbContext context,
    Partner entity, int count, int commitCount, bool recreateContext)
        {
            context.Partners.Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new CatalogDbContext(new DbContextOptionsBuilder<CatalogDbContext>().Options, _tenant, _configuration);
                    //context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
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
                    query = query.Where(x => x.Customer)
                        .Include(x => x.PartnerHistoryRels)
                        .Include(x => x.PartnerPartnerCategoryRels);

                var list = await query.ToListAsync();
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
                    Color = s.Category.Color
                })
            });
        }

        public async Task<PartnerDisplay> GetInfoPartner(Guid id)
        {
            var partner = await SearchQuery(x => x.Id == id).Include(x => x.PartnerHistoryRels).Include(x => x.PartnerPartnerCategoryRels).Include("PartnerHistoryRels.History").Include("PartnerPartnerCategoryRels.Category").FirstOrDefaultAsync();

            var partnerDisplay = _mapper.Map<PartnerDisplay>(partner);
            partnerDisplay.Histories = partner.PartnerHistoryRels.Select(s => new HistorySimple
            {
                Id = s.HistoryId,
                Name = s.History.Name
            }).ToList();

            partnerDisplay.Categories = partner.PartnerPartnerCategoryRels.Select(s => new PartnerCategoryBasic
            {
                Id = s.CategoryId,
                Name = s.Category.Name,
                Color = s.Category.Color
            }).ToList();

            return partnerDisplay;
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

        public async Task<PartnerCustomerReportOutput> GetPartnerCustomerReport(PartnerCustomerReportInput val)
        {
            ISpecification<SaleOrder> spec = new InitialSpecification<SaleOrder>(x => true && (!x.IsQuotation.HasValue || x.IsQuotation.Value == false));
            if (val.DateFrom.HasValue)
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.DateOrder >= val.DateFrom));

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.DateOrder <= dateTo));
            }

            if (val.CompanyId.HasValue)
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.CompanyId == val.CompanyId.Value));

            var saleOrderObj = GetService<ISaleOrderService>();

            var query_saleOrder = saleOrderObj.SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            IEnumerable<Guid> ids_partner = query_saleOrder.Select(x => x.PartnerId).ToList();

            var temp = await saleOrderObj.SearchQuery(x => ids_partner.Contains(x.PartnerId))
                .GroupBy(x => x.PartnerId)
                .Select(x => new
                {
                    PartnerId = x.Key,
                    PartnerCount = x.Count()
                })
                .OrderBy(x => x.PartnerId).ToListAsync();

            var result = new PartnerCustomerReportOutput
            {
                CustomerOld = temp.Where(x => x.PartnerCount > 1).Count(),
                CustomerNew = temp.Where(x => x.PartnerCount == 1).Count()
            };

            return result;
        }

        public async Task<PartnerCustomerReportOutput> GetPartnerCustomerReportV2(PartnerCustomerReportInput val)
        {
            var result = new PartnerCustomerReportOutput();
            var partnerIds = SearchQuery(x => x.Customer == true).Select(x => x.Id);
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var querySaleOrderLine = saleOrderLineObj.SearchQuery(x => true);
            if (val.DateFrom.HasValue)
                querySaleOrderLine = querySaleOrderLine.Where(x => x.Order.DateOrder >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                querySaleOrderLine = querySaleOrderLine.Where(x => x.Order.DateOrder <= val.DateTo.Value);

            if (val.CompanyId.HasValue)
                querySaleOrderLine = querySaleOrderLine.Where(x => x.CompanyId == val.CompanyId.Value);

            var temp = await querySaleOrderLine.Where(x => partnerIds.Contains(x.OrderPartnerId.Value))
           .GroupBy(x => x.OrderPartnerId)
           .Select(x => new
           {
               PartnerId = x.Key,
               PartnerCount = x.Count()
           })
           .OrderBy(x => x.PartnerId).ToListAsync();

            result.CustomerOld = temp.Where(x => x.PartnerCount >= 1).Count();
            result.CustomerNew = partnerIds.Count() - result.CustomerOld;
            return result;
        }

        public IQueryable<Partner> GetQueryablePartnerFilter(PartnerQueryableFilter val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();
            var irProperyObj = GetService<IIRPropertyService>();
            var cardCardObj = GetService<ICardCardService>();
            var serviceCardCardObj = GetService<IServiceCardCardService>();
            var partnerObj = GetService<IPartnerService>();
            var partnerSourceObj = GetService<IPartnerSourceService>();

           

            var mainQuery = partnerObj.SearchQuery(x => x.Active && x.Customer);

            if (val.CompanyId.HasValue)
                mainQuery = mainQuery.Where(x => x.CompanyId == val.CompanyId);

            if (!string.IsNullOrEmpty(val.CityCode))
                mainQuery = mainQuery.Where(x => x.CityCode == val.CityCode);

            if (!string.IsNullOrEmpty(val.DistrictCode))
                mainQuery = mainQuery.Where(x => x.DistrictCode == val.DistrictCode);

            if (!string.IsNullOrEmpty(val.WardCode))
                mainQuery = mainQuery.Where(x => x.WardCode == val.WardCode);


            if (val.CategIds.Any())
            {
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();

                var filterPartnerQr = from pcr in partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId))
                                      group pcr by pcr.PartnerId into g
                                      select g.Key;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            if (val.CardTypeIds.Any())
            {
                var filterCardTypeQr = from ps in cardCardObj.SearchQuery(x => val.CardTypeIds.Contains(x.TypeId))
                                       group ps by ps.PartnerId into g
                                       select g.Key;

                mainQuery = from a in mainQuery
                            join pbk in filterCardTypeQr on a.Id equals pbk
                            select a;

            }

            if (val.PartnerSourceIds.Any())
            {

                var filterPartnerSourceQr = from ps in SearchQuery(x => val.PartnerSourceIds.Contains(x.SourceId.Value))
                                            group ps by ps.Id into g
                                            select g.Key;


                mainQuery = from a in mainQuery
                            join pbk in filterPartnerSourceQr on a.Id equals pbk
                            select a;
            }


            if (!string.IsNullOrEmpty(val.OrderState))
            {
                var partnerOrderStateQr = from v in saleOrderObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                          group v by v.PartnerId into g
                                          select new
                                          {
                                              PartnerId = g.Key,
                                              OrderState = g.Sum(x => x.State == "sale" ? 1 : 0) > 0 ? "sale" : (g.Sum(x => x.State == "done" ? 1 : 0) > 0 ? "done" : "draft"),
                                          };

                mainQuery = from a in mainQuery
                            join pbk in partnerOrderStateQr on a.Id equals pbk.PartnerId
                            where pbk.OrderState == val.OrderState
                            select a;
            }


            if (val.RevenueFrom.HasValue || val.RevenueTo.HasValue)
            {
                var PartnerRevenueQr = (from s in saleOrderLineObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                        where s.State == "sale" || s.State == "done"
                                        group s by s.OrderPartnerId into g
                                        select new
                                        {
                                            PartnerId = g.Key.Value,
                                            TotalPaid = g.Sum(x => x.AmountInvoiced)
                                        });

                if (val.RevenueFrom.HasValue)
                    PartnerRevenueQr = PartnerRevenueQr.Where(x => x.TotalPaid >= val.RevenueFrom);

                if (val.RevenueTo.HasValue)
                    PartnerRevenueQr = PartnerRevenueQr.Where(x => x.TotalPaid <= val.RevenueTo);


                mainQuery = from a in mainQuery
                            join pbk in PartnerRevenueQr on a.Id equals pbk.PartnerId
                            select a;

            }

            var partnerRevenueExpectQr = (from pre in saleOrderLineObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                          where pre.State == "sale" || pre.State == "done"
                                          group pre by pre.OrderPartnerId into g
                                          select new
                                          {
                                              PartnerId = g.Key.Value,
                                              OrderResidual = g.Sum(x => x.PriceTotal - x.AmountInvoiced),
                                          });

            var debtQr = from aml in amlObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                         join acc in accObj.SearchQuery()
                         on aml.AccountId equals acc.Id
                         where acc.Code == "CNKH"
                         group aml by aml.PartnerId into g
                         select new
                         {
                             PartnerId = g.Key.Value,
                             TotalDebt = g.Sum(x => x.Balance)
                         };


            if (val.RevenueExpectFrom.HasValue || val.RevenueExpectTo.HasValue)
            {

                if (val.RevenueExpectFrom.HasValue)
                    partnerRevenueExpectQr = partnerRevenueExpectQr.Where(x => x.OrderResidual >= val.RevenueExpectFrom);

                if (val.RevenueExpectTo.HasValue)
                    partnerRevenueExpectQr = partnerRevenueExpectQr.Where(x => x.OrderResidual <= val.RevenueExpectTo);

                mainQuery = from a in mainQuery
                            join pbk in partnerRevenueExpectQr on a.Id equals pbk.PartnerId
                            select a;

            }

            if (val.IsRevenueExpect.HasValue)
            {
                IQueryable<Guid> partnersByKeywords;

                partnersByKeywords = from p in mainQuery select p.Id;

                if (val.IsRevenueExpect == true)
                    partnersByKeywords = partnersByKeywords.Where(x => partnerRevenueExpectQr.Select(s => s.PartnerId).Contains(x));
                else
                    partnersByKeywords = partnersByKeywords.Except(from pre in partnerRevenueExpectQr select pre.PartnerId);

                mainQuery = from a in mainQuery
                            join pbk in partnersByKeywords on a.Id equals pbk
                            select a;
            }


            if (val.DebtFrom.HasValue || val.DebtTo.HasValue)
            {


                if (val.DebtFrom.HasValue)
                    debtQr = debtQr.Where(x => x.TotalDebt >= val.DebtFrom);

                if (val.DebtTo.HasValue)
                    debtQr = debtQr.Where(x => x.TotalDebt <= val.DebtTo);

                mainQuery = from a in mainQuery
                            join pbk in debtQr on a.Id equals pbk.PartnerId
                            select a;
            }

            if (val.IsDebt.HasValue)
            {
                IQueryable<Guid> partnersByKeywords;

                partnersByKeywords = from p in mainQuery select p.Id;

                if (val.IsDebt == true)
                    partnersByKeywords = partnersByKeywords.Where(x => debtQr.Select(s => s.PartnerId).Contains(x));
                else
                    partnersByKeywords = partnersByKeywords.Except(from dp in debtQr select dp.PartnerId);

                mainQuery = from a in mainQuery
                            join pbk in partnersByKeywords on a.Id equals pbk
                            select a;
            }

            if (val.AgeFrom.HasValue || val.AgeTo.HasValue)
            {
                if (val.AgeFrom.HasValue && val.AgeFrom > 0)
                    mainQuery = mainQuery.Where(x => (DateTime.Now.Year - x.BirthYear) >= val.AgeFrom.Value);

                if (val.AgeTo.HasValue && val.AgeTo > 0)
                    mainQuery = mainQuery.Where(x => (DateTime.Now.Year - x.BirthYear) <= val.AgeTo.Value);
            }

            return mainQuery;
        }

        public async Task<CustomerStatisticsOutput> GetCustomerStatistics(CustomerStatisticsInput val)
        {
            ISpecification<SaleOrder> spec = new InitialSpecification<SaleOrder>(x => true);

            var dateFrom = DateTime.Now;
            var dateTo = DateTime.Now.AbsoluteEndOfDate();

            if (val.DateFrom.HasValue)
            {
                dateFrom = val.DateFrom.Value;
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.DateOrder >= dateFrom));
            }

            if (val.DateTo.HasValue)
            {
                dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.DateOrder <= dateTo));
            }

            var saleOrderObj = GetService<ISaleOrderService>();

            var query_saleOrder = saleOrderObj.SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            IEnumerable<Guid> ids_partner = query_saleOrder.Select(x => x.PartnerId).ToList();

            var temp = await saleOrderObj.SearchQuery(x => ids_partner.Contains(x.PartnerId) && x.DateOrder <= dateTo)
                .GroupBy(x => new { x.PartnerId, x.Partner.DistrictName })
                .Select(x => new
                {
                    PartnerId = x.Key.PartnerId,
                    PartnerCount = x.Count(),
                    PartnerLocation = x.Key.DistrictName
                })
                .OrderBy(x => x.PartnerId).ToListAsync();

            var temp_groupLocation = temp.GroupBy(x => x.PartnerLocation);

            var details = new List<CustomerStatisticsDetails>();
            foreach (var item in temp_groupLocation)
            {
                details.Add(new CustomerStatisticsDetails
                {
                    Location = item.Key,
                    CustomerTotal = item.Count(),
                    CustomerOld = item.Where(x => x.PartnerCount > 1).Count(),
                    CustomerNew = item.Where(x => x.PartnerCount == 1).Count()
                });
            }

            var result = new CustomerStatisticsOutput
            {
                CustomerTotal = temp.Count(),
                CustomerOld = temp.Where(x => x.PartnerCount > 1).Count(),
                CustomerNew = temp.Where(x => x.PartnerCount == 1).Count(),
                Details = details
            };

            return result;
        }

        public async Task<IEnumerable<AccountMove>> GetUnreconcileInvoices(Guid id, string search = "")
        {
            var amObj = GetService<IAccountMoveService>();
            //lấy những hóa đơn còn nợ
            var types = new string[] { "in_invoice", "in_refund", "out_invoice", "out_refund" };
            var query = amObj.SearchQuery(x => x.PartnerId == id && types.Contains(x.Type) && x.AmountResidual != 0);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.InvoiceOrigin.Contains(search));

            var moves = await query.OrderBy(x => x.DateCreated).ToListAsync();
            return moves;
        }

        public async Task<List<SearchAllViewModel>> SearchAll(PartnerPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => (x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search) || x.Phone.Contains(val.Search)));
            }

            if (val.Customer.HasValue)
            {
                query = query.Where(x => x.Customer == true);
            }

            if (val.Supplier.HasValue)
            {
                query = query.Where(x => x.Supplier == true);
            }

            var res = await query
                         .Include("PartnerPartnerCategoryRels.Category")
                         .Skip(val.Offset)
                         .Take(val.Limit)
                          .Select(x => new SearchAllViewModel
                          {
                              Id = x.Id,
                              Name = "[" + x.Ref + "]" + " " + x.Name,
                              Address = x.GetAddress(),
                              Phone = x.Phone,
                              Type = "customer",
                              Tags = x.PartnerPartnerCategoryRels != null && x.PartnerPartnerCategoryRels.Count > 0 ? _mapper.Map<List<PartnerCategoryBasic>>(x.PartnerPartnerCategoryRels) : new List<PartnerCategoryBasic>()
                          }).ToListAsync();

            return res;
        }

        public async Task<PagedResult2<PartnerGetDebtPagedItem>> GetDebtPaged(Guid id, PartnerGetDebtPagedFilter val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var query = amlObj._QueryGet(companyId: val.CompanyId, state: "posted");
            var types = new[] { "receivable", "payable" };
            query = query.Where(x => x.PartnerId == id && types.Contains(x.Account.InternalType) && x.Reconciled == false);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Move.InvoiceOrigin.Contains(val.Search));

            var total = await query.CountAsync();
            var items = await query.OrderBy(x => x.Date).Skip(val.Offset).Take(val.Limit).Select(x => new PartnerGetDebtPagedItem
            {
                Date = x.Date,
                AmountResidual = x.AccountInternalType == "payable" ? -x.AmountResidual : x.AmountResidual,
                Balance = x.AccountInternalType == "payable" ? -x.Balance : x.Balance,
                Origin = x.Move.InvoiceOrigin,
                MoveId = x.MoveId,
                MoveType = x.Move.Type
            }).ToListAsync();

            return new PagedResult2<PartnerGetDebtPagedItem>(total, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public string _GetLevel(decimal? point, IEnumerable<MemberLevel> levels)
        {
            if (levels == null)
                throw new Exception("Không có danh sách hạng thành viên");
            for (int i = 0; i < levels.Count(); i++)
            {
                var pointValue = levels.ElementAt(i).Point;
                if (point >= pointValue)
                    return levels.ElementAt(i).Id.ToString();
                else
                    continue;

            }
            return null;

        }

        public async Task<decimal> _GetDebit(Partner partner)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var account = await accountObj.SearchQuery(x => x.Code == "5111" && x.CompanyId == partner.CompanyId).FirstOrDefaultAsync();
            var amounBalance = await moveLineObj.SearchQuery(x => x.PartnerId == partner.Id && x.AccountId == account.Id).SumAsync(x => x.Debit - x.Credit);
            var sign = -1;
            return amounBalance * sign;
        }


        public async Task<IEnumerable<PartnerBasic>> GetCustomerBirthDay(PartnerPaged val)
        {
            var query = SearchQuery();
            var saleReportObj = GetService<ISaleReportService>();
            if (val.Customer.HasValue && val.Customer.Value)
                query = query.Where(x => x.Customer == val.Customer);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search));

            var partnerIds = await query.Select(x => x.Id).ToListAsync();

            var SaleReportSearch = new SaleReportSearch()
            {
                CompanyId = CompanyId,
                GroupBy = "customer"
            };

            var saleReportDict = (await saleReportObj.GetReportForSmsMessage(partnerIds)).ToDictionary(x => x.PartnerId, x => x);

            var listPartner = await query.Select(x => new PartnerBasic
            {
                Id = x.Id,
                Name = x.Name,
                Age = x.GetAge,
                BirthDay = x.BirthDay,
                BirthMonth = x.BirthMonth,
                BirthYear = x.BirthYear,
                Street = x.Street,
                DistrictName = x.DistrictName,
                WardName = x.WardName,
                CityName = x.CityName,
                DateOfBirth = x.GetDateOfBirth(),
                Phone = x.Phone,
                Address = x.GetAddress(),
                Gender = x.GetGender,
                CountLine = saleReportDict.ContainsKey(x.Id) ? saleReportDict[x.Id].ProductUOMQty : 0,
                Debit = saleReportDict.ContainsKey(x.Id) ? saleReportDict[x.Id].PriceTotal : 0,
            }).ToListAsync();

            return listPartner;
        }

        public async Task<IEnumerable<PartnerBasic>> GetCustomerAppointments(PartnerPaged val)
        {
            DateTime currentDate = DateTime.Today;
            var apObj = GetService<IAppointmentService>();
            var partnerList = await apObj.SearchQuery().Include(x => x.Partner)
                .Where(x => x.Date.Date == currentDate)
                .Select(x => new PartnerBasic
                {
                    Id = x.Partner.Id,
                    Name = x.Partner.Name,
                    Age = x.Partner.GetAge,
                    BirthDay = x.Partner.BirthDay,
                    BirthMonth = x.Partner.BirthMonth,
                    BirthYear = x.Partner.BirthYear,
                    Street = x.Partner.Street,
                    DistrictName = x.Partner.DistrictName,
                    WardName = x.Partner.WardName,
                    CityName = x.Partner.CityName,
                    DateOfBirth = x.Partner.GetDateOfBirth(),
                    Phone = x.Partner.Phone,
                    Address = x.Partner.GetAddress(),
                    Gender = x.Partner.GetGender,
                    AppointmnetId = x.Id,
                    AppointmnetName = x.Name,
                    Time = x.Time,
                    Date = x.Date.ToShortDateString()

                })
                .OrderBy(x => x.Name)
                .ToListAsync();


            return partnerList;
            //return _mapper.Map<IEnumerable<PartnerBasic>>(apList);
        }

        private IQueryable<Partner> GetQueryable(PartnerForTCarePaged val)
        {
            var query = SearchQuery(x => x.Customer == true);
            if (val.BirthDay.HasValue)
                query = query.Where(x => x.BirthDay.HasValue && x.BirthDay.Value == val.BirthDay.Value);

            if (val.BirthMonth.HasValue)
                query = query.Where(x => x.BirthMonth.HasValue && x.BirthMonth.Value == val.BirthMonth.Value);

            if (!string.IsNullOrEmpty(val.Op) && val.PartnerCategoryId.HasValue)
            {
                if (val.Op == "not_contains")
                {
                    query = query.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.CategoryId == val.PartnerCategoryId));
                }
                else
                {
                    query = query.Where(x => !x.PartnerPartnerCategoryRels.Any(s => s.CategoryId == val.PartnerCategoryId));
                }
            }
            return query;
        }

        public async Task<IEnumerable<Guid>> GetPartnerForTCare(PartnerForTCarePaged val)
        {
            var query = GetQueryable(val);
            var partnerIds = await query.Select(x => x.Id).ToListAsync();
            return partnerIds;
        }

        public async Task<IEnumerable<PartnerSaleOrderDone>> GetPartnerOrderDone(PartnerPaged val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var today = DateTime.Today;
            var today1 = DateTime.Now.Date;
            var list = await saleOrderLineObj.SearchQuery(x => x.OrderPartnerId.HasValue && x.Order.State == "done" && x.Order.DateDone.HasValue && x.Order.DateDone.Value.Date == today).Include(x => x.Order).GroupBy(x => new { PartnerId = x.OrderPartnerId, PartnerName = x.OrderPartner.DisplayName, PartnerPhone = x.OrderPartner.Phone })
                .Select(x => new PartnerSaleOrderDone
                {
                    Id = x.Key.PartnerId.Value,
                    DisplayName = x.Key.PartnerName,
                    Phone = x.Key.PartnerPhone,
                    Debit = x.Sum(s => s.PriceSubTotal),
                    SaleOrderLineName = string.Join(",", x.Select(s => s.Name)),
                    SaleOrderName = string.Join(",", x.Select(s => s.Order.Name)),
                    DateDone = x.Select(s => s.Order.DateDone).Max()
                }).ToListAsync();
            return list;
        }

        public async Task<IQueryable<PartnerInfoTemplate>> GetQueryPartnerInfoPaged2(PartnerInfoPaged val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var appointmentObj = GetService<IAppointmentService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();
            var irProperyObj = GetService<IIRPropertyService>();
            var cardCardObj = GetService<ICardCardService>();
            var serviceCardCardObj = GetService<IServiceCardCardService>();

            var companyId = CompanyId;
            var partnerOrderStateQr = from v in saleOrderObj.SearchQuery(x => x.CompanyId == companyId)
                                      group v by v.PartnerId into g
                                      select new
                                      {
                                          PartnerId = g.Key,
                                          CountSale = g.Sum(x => x.State == "sale" ? 1 : 0),
                                          CountDone = g.Sum(x => x.State == "done" ? 1 : 0),
                                          Date = g.Max(x => x.DateOrder)
                                      };



            var PartnerResidualQr = (from s in saleOrderObj.SearchQuery(x => x.CompanyId == companyId)
                                     where s.State == "sale" || s.State == "done"
                                     group s by s.PartnerId into g
                                     select new
                                     {
                                         PartnerId = g.Key,
                                         OrderResidual = g.Sum(x => x.AmountTotal - x.TotalPaid)
                                     });

            var partnerDebQr = from aml in amlObj.SearchQuery(x => x.CompanyId == companyId)
                               join acc in accObj.SearchQuery()
                               on aml.AccountId equals acc.Id
                               where acc.Code == "CNKH"
                               group aml by aml.PartnerId into g
                               select new
                               {
                                   PartnerId = g.Key,
                                   TotalDebit = g.Sum(x => x.Balance)
                               };

            var irPropertyQr = from ir in irProperyObj.SearchQuery(x => x.Name == "member_level" && x.Field.Model == "res.partner" && x.CompanyId == companyId)
                               select ir;

            var cardCardQr = from card in cardCardObj.SearchQuery()
                             select card;

            var appointmentQr = from appoint in appointmentObj.SearchQuery(x => x.CompanyId == companyId)
                                group appoint by appoint.PartnerId into g
                                select new
                                {
                                    PartnerId = g.Key,
                                    Date = g.Max(s => s.Date)
                                };

            var mainQuery = SearchQuery(x => x.Active && x.Customer);
            if (val.CompanyId.HasValue)
                mainQuery = mainQuery.Where(x => x.CompanyId == val.CompanyId);

            if (!string.IsNullOrEmpty(val.Search))
            {
                IQueryable<Guid> partnersByKeywords;

                partnersByKeywords =
                      from p in mainQuery
                      where p.Name.Contains(val.Search) || p.NameNoSign.Contains(val.Search)
                || p.Ref.Contains(val.Search) || p.Phone.Contains(val.Search)
                      select p.Id;

                partnersByKeywords = partnersByKeywords.Union(
                        from card in cardCardObj.SearchQuery()
                        where card.Barcode.Contains(val.Search) && card.PartnerId.HasValue
                        select card.PartnerId.Value
                    );

                partnersByKeywords = partnersByKeywords.Union(
                       from card in serviceCardCardObj.SearchQuery()
                       where card.Barcode.Contains(val.Search) && card.PartnerId.HasValue
                       select card.PartnerId.Value
                   );

                mainQuery = from a in mainQuery
                            join pbk in partnersByKeywords on a.Id equals pbk
                            select a;
            }

            if (val.CategIds.Any())
            {
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();

                var filterPartnerQr = from pcr in partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId))
                                      group pcr by pcr.PartnerId into g
                                      select g.Key;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            if (val.CardTypeId.HasValue)
            {
                var filterPartnerQr =
                   from pcr in cardCardObj.SearchQuery(x => x.TypeId == val.CardTypeId)
                   select pcr.PartnerId;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }


            var ResponseQr = from p in mainQuery
                             from pr in PartnerResidualQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from pd in partnerDebQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from pos in partnerOrderStateQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from ir in irPropertyQr.Where(x => !string.IsNullOrEmpty(x.ResId) && x.ResId.Contains(p.Id.ToString().ToLower())).DefaultIfEmpty()
                             from card in cardCardQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from appoint in appointmentQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             select new PartnerInfoTemplate
                             {
                                 Id = p.Id,
                                 Name = p.Name,
                                 NameNoSign = p.NameNoSign,
                                 Ref = p.Ref,
                                 Phone = p.Phone,
                                 Avatar = p.Avatar,
                                 BirthDay = p.BirthDay,
                                 BirthMonth = p.BirthMonth,
                                 BirthYear = p.BirthYear,
                                 DisplayName = p.DisplayName,
                                 Email = p.Email,
                                 CityName = p.CityName,
                                 DistrictName = p.DistrictName,
                                 JobTitle = p.JobTitle,
                                 Street = p.Street,
                                 WardName = p.WardName,
                                 Gender = p.Gender,
                                 Comment = p.Comment,
                                 Date = p.Date,
                                 OrderState = pos.CountSale > 0 ? "sale" : (pos.CountDone > 0 ? "done" : "draft"),
                                 OrderResidual = pr.OrderResidual,
                                 TotalDebit = pd.TotalDebit,
                                 MemberLevelId = ir.ValueReference,
                                 DateCreated = p.DateCreated,
                                 SourceName = p.Source.Name,
                                 TitleName = p.Title.Name,
                                 CardTypeName = card.Type.Name,
                                 CompanyName = p.Company.Name,
                                 AppointmentDate = appoint.Date,
                                 SaleOrderDate = pos.Date
                             };

            if (val.HasOrderResidual.HasValue && val.HasOrderResidual.Value == 1)
            {
                ResponseQr = ResponseQr.Where(x => x.OrderResidual > 0);
            }

            if (val.HasOrderResidual.HasValue && val.HasOrderResidual.Value == 0)
            {
                ResponseQr = ResponseQr.Where(x => x.OrderResidual == 0 || x.OrderResidual == null);
            }

            if (val.HasTotalDebit.HasValue && val.HasTotalDebit.Value == 1)
            {
                ResponseQr = ResponseQr.Where(x => x.TotalDebit > 0);
            }

            if (val.HasTotalDebit.HasValue && val.HasTotalDebit.Value == 0)
            {
                ResponseQr = ResponseQr.Where(x => x.TotalDebit == 0 || x.TotalDebit == null);
            }

            if (!string.IsNullOrEmpty(val.OrderState))
            {
                ResponseQr = ResponseQr.Where(x => x.OrderState == val.OrderState);
            }

            return ResponseQr;
        }
        public async Task<PagedResult2<PartnerInfoDisplay>> GetPartnerInfoPaged2(PartnerInfoPaged val)
        {
            var memberLevelObj = GetService<IMemberLevelService>();
            var cateObj = GetService<IPartnerCategoryService>();
            var partnerCategoryRelObj = GetService<IPartnerPartnerCategoryRelService>();
            var cardCardObj = GetService<ICardCardService>();

            var ResponseQr = await GetQueryPartnerInfoPaged2(val);
            var count = await ResponseQr.CountAsync();
            var res = await ResponseQr.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();

            var cateList = await partnerCategoryRelObj.SearchQuery(x => res.Select(i => i.Id).Contains(x.PartnerId)).Include(x => x.Category).ToListAsync();
            var categDict = cateList.GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.Select(s => s.Category));
            foreach (var item in res)
            {
                item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(categDict.ContainsKey(item.Id) ? categDict[item.Id] : new List<PartnerCategory>());
            }
            var items = _mapper.Map<IEnumerable<PartnerInfoDisplay>>(res);

            return new PagedResult2<PartnerInfoDisplay>(count, val.Offset, val.Limit)
            {
                Items = items
            };

        }

        public async Task<PagedResult2<PartnerInfoDisplay>> GetPartnerInfoPaged(PartnerInfoPaged val)
        {

            var query = _context.PartnerInfos.FromSqlRaw(@$"select * from fn_PartnerInfoList('{CompanyId}')");
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search)
                || x.Ref.Contains(val.Search) || x.Phone.Contains(val.Search));
            }

            if (val.CategIds.Any())
            {
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();
                var filterPartnerIds = await partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId)).Select(x => x.PartnerId).Distinct().ToListAsync();
                query = query.Where(x => filterPartnerIds.Contains(x.Id));
            }

            if (val.HasOrderResidual.HasValue && val.HasOrderResidual.Value == 1)
            {
                query = query.Where(x => x.OrderResidual > 0);
            }

            if (val.HasOrderResidual.HasValue && val.HasOrderResidual.Value == 0)
            {
                query = query.Where(x => x.OrderResidual == 0 || x.OrderResidual == null);
            }

            if (val.HasTotalDebit.HasValue && val.HasTotalDebit.Value == 1)
            {
                query = query.Where(x => x.TotalDebit > 0);
            }

            if (val.HasTotalDebit.HasValue && val.HasTotalDebit.Value == 0)
            {
                query = query.Where(x => x.TotalDebit == 0 || x.TotalDebit == null);
            }

            if (val.MemberLevelId.HasValue)
            {
                query = query.Where(x => x.MemberLevelId == val.MemberLevelId);
            }

            if (!string.IsNullOrEmpty(val.OrderState))
            {
                query = query.Where(x => x.OrderState == val.OrderState);
            }

            var count = await query.CountAsync();
            var res = await query.Include(x => x.MemberLevel).Skip(val.Offset).Take(val.Limit).ToListAsync();
            var items = _mapper.Map<IEnumerable<PartnerInfoDisplay>>(res);

            var cateObj = GetService<IPartnerCategoryService>();
            var cateList = await cateObj.SearchQuery(x => x.PartnerPartnerCategoryRels.Any(s => items.Select(i => i.Id).Contains(s.PartnerId)))
                                                                                           .Include(x => x.PartnerPartnerCategoryRels).ToListAsync();
            foreach (var item in items)
            {
                item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(cateList.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.PartnerId == item.Id)));
            }

            return new PagedResult2<PartnerInfoDisplay>(count, val.Offset, val.Limit)
            {
                Items = items
            };

        }
        public async Task<IEnumerable<IrAttachment>> GetListAttachment(Guid id)
        {
            var attObj = GetService<IIrAttachmentService>();
            var dotkhamObj = GetService<IDotKhamService>();
            var saleObj = GetService<ISaleOrderService>();
            //check company
            var attQr = attObj.SearchQuery(x => x.CompanyId == CompanyId);
            var saleQr = saleObj.SearchQuery(x => x.CompanyId == CompanyId);
            var dotkhamQr = dotkhamObj.SearchQuery();

            var resQr = from att in attQr
                        from so in saleQr.Where(x => x.Id == att.ResId).DefaultIfEmpty()
                        from dk in dotkhamQr.Where(x => x.Id == att.ResId).DefaultIfEmpty()
                        where att.ResId == id || so.PartnerId == id || dk.PartnerId == id
                        select att;
            var res = await resQr.OrderByDescending(x => x.DateCreated).ToListAsync();
            return res;
        }

        public async Task<IEnumerable<Partner>> GetPublicPartners(int limit, int offset, string search)
        {
            var query = SearchQuery(x => x.Active && x.Customer);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search) || x.NameNoSign.Contains(search)
              || x.Ref.Contains(search) || x.Phone.Contains(search));
            }

            if (limit > 0)
                query = query.Skip(offset).Take(limit);

            var items = await query.OrderByDescending(x => x.DateCreated).ToListAsync();

            return items;
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
