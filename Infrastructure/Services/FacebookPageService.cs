using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookPageService : BaseService<FacebookPage>, IFacebookPageService
    {
        private readonly IMapper _mapper;
        private readonly FacebookAuthSettings _fbAuthSettings;
        private readonly AppTenant _tenant;
        private readonly ConnectionStrings _connectionStrings;
        public FacebookPageService(IAsyncRepository<FacebookPage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IOptions<FacebookAuthSettings> fbAuthSettingsAccessor,
            ITenant<AppTenant> tenant, IOptions<ConnectionStrings> connectionStrings)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _fbAuthSettings = fbAuthSettingsAccessor?.Value;
            _tenant = tenant?.Value;
            _connectionStrings = connectionStrings?.Value;
        }

        public override async Task<FacebookPage> CreateAsync(FacebookPage entity)
        {
            await _CheckConstraint(entity);
            return await base.CreateAsync(entity);
        }

        private async Task _CheckConstraint(FacebookPage self)
        {
            var exist = await SearchQuery(x => x.PageId == self.PageId).FirstOrDefaultAsync();
            if (exist != null)
                throw new Exception($"Bạn đã kết nối với page {exist.PageName}");
        }

        public async Task<PagedResult2<FacebookPageBasic>> GetPagedResultAsync(FacebookPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.UserName.Contains(val.Search) || x.PageName.Contains(val.Search));

            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookPageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<FacebookPageBasic>>(items)
            };
        }

        public async Task<string> GetFacebookAppAccessToken(string accesstoken)
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(_fbAuthSettings.AppId, _fbAuthSettings.AppSecret, accesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"oauth/access_token";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("client_id", _fbAuthSettings.AppId);
            getRequest.AddQueryParameter("client_secret", _fbAuthSettings.AppSecret);
            getRequest.AddQueryParameter("grant_type", "client_credentials");
            var response = await getRequest.ExecuteAsync<dynamic>();
            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();

                return (string)result["access_token"];

            }
        }

        public async Task<string> GetLongTermAccessToken(string Accesstoken)
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(_fbAuthSettings.AppId, _fbAuthSettings.AppSecret, Accesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"oauth/access_token";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("grant_type", "fb_exchange_token");
            getRequest.AddQueryParameter("client_id", _fbAuthSettings.AppId);
            getRequest.AddQueryParameter("client_secret", _fbAuthSettings.AppSecret);
            getRequest.AddQueryParameter("fb_exchange_token", Accesstoken);
            var response = (await getRequest.ExecuteAsync<dynamic>());

            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();
                return (string)result["access_token"];
            }
        }

        public async Task<FacebookPage> GetFacebookPage(string accesstoken, string pageId)
        {
            string errorMaessage = null;
            var page = new FacebookPage();
            var a = new Object();
            var apiClient = new ApiClient(accesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"me?fields=id,name,accounts";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("access_token", accesstoken);
            var response = (await getRequest.ExecuteAsync<FacebookUserData>());
            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();
                page.UserId = result.Id;
                page.UserName = result.Name;
                page.UserAccesstoken = accesstoken;
                var fbpage = result.Accounts.Data.Where(x => x.Id == pageId).SingleOrDefault();
                page.PageId = fbpage.Id;
                page.PageName = fbpage.Name;
                page.PageAccesstoken = fbpage.PageAccesstoken;
                return page;
            }
        }

        public async Task<FacebookPage> CheckFacebookPage(string userid, string pageid)
        {

            var facebookpage = await SearchQuery(x => x.UserId == userid && x.PageId == pageid).FirstOrDefaultAsync();
            return facebookpage;
        }

        public async Task<FacebookPage> CreateFacebookPage(FacebookPageLinkSave val)
        {

            //generate an app access token
            var userAccessTokenData = await GetFacebookAppAccessToken(val.Accesstoken);
            if (userAccessTokenData == null)
            {
                throw new Exception("App facebook không tồn tại !");
            }
            //Get long term access token
            var longTermAccessToken = await GetLongTermAccessToken(val.Accesstoken);
            if (longTermAccessToken == null)
            {
                throw new Exception("Chưa lấy được access_token dài hạn");
            }

            //Get page facebook
            var page = await GetFacebookPage(longTermAccessToken, val.PageId);

            //check
            var check = await CheckFacebookPage(page.UserId, page.PageId);
            if (check != null)
            {
                throw new Exception("Fanpage đã được kết nối !");
            }
            var fbpage = new FacebookPage
            {
                UserId = page.UserId,
                UserName = page.UserName,
                UserAccesstoken = longTermAccessToken,
                PageId = page.PageId,
                PageName = page.PageName,
                PageAccesstoken = page.PageAccesstoken
            };
            await CreateAsync(fbpage);


            return fbpage;

        }


        public async Task LoadUserProfileFromConversations(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            //Tải tất cả những conversations
            var limit = 100;
            var list = new List<FacebookUserProfile>();

            var result = await GetPageConversations(self.PageId, self.PageAccesstoken, limit);

            //Lấy ra 1 list psids duy nhất
            var psids = new List<string>().AsEnumerable();
            psids = psids.Union(ExtractPSIDsUserProfile(result, self.PageId));

            while (!string.IsNullOrEmpty(result.paging.next))
            {
                result = await GetPageConversations(self.PageId, self.PageAccesstoken, limit, after: result.paging.cursors.after);
                psids = psids.Union(ExtractPSIDsUserProfile(result, self.PageId));
            }

            var tasks = psids.Select(x => GetUserProfileFromPSID(x, self.PageAccesstoken));
            var all_profiles = await Task.WhenAll(tasks);
            foreach(var profile in all_profiles)
            {
                if (profile == null)
                    continue;
                list.Add(new FacebookUserProfile
                {
                    Name = profile.name,
                    FirstName = profile.first_name,
                    LastName = profile.last_name,
                    FbPageId = self.Id,
                    PSID = profile.id,
                    Gender = profile.gender,
                });
            }

            var userProfileObj = GetService<IFacebookUserProfileService>();
            await userProfileObj.CreateAsync(list);
        }

        public IEnumerable<string> ExtractPSIDsUserProfile(ApiPageConversationsResponse response, string pageId)
        {
            if (response.data == null)
                return new List<string>();

            var list = new List<string>().AsEnumerable();
            foreach(var item in response.data)
            {
                var ids = item.participants.data.Where(x => x.id != pageId).Select(x => x.id);
                list = list.Union(ids);
            }

            return list;
        }

        public async Task<ApiPageConversationsResponse> GetPageConversations(string page_id, string access_token, int limit, string after = "")
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{page_id}/conversations";
            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("fields", "participants");
            getRequest.AddQueryParameter("limit", limit.ToString());
            if (!string.IsNullOrEmpty(after))
            {
                getRequest.AddQueryParameter("pretty", "0");
                getRequest.AddQueryParameter("after", after);
            }
            var response = (await getRequest.ExecuteAsync<ApiPageConversationsResponse>());

            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }

        public async Task<ApiUserProfileResponse> GetUserProfileFromPSID(string psid, string access_token)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{psid}";
            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("fields", "name,first_name,last_name,profile_pic,gender");
            var response = (await getRequest.ExecuteAsync<ApiUserProfileResponse>());

            if (response.GetExceptions().Any())
            {
                var error_message = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                return null;
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }


        /// <summary>
        /// lấy ra danh sách các PSID đã inbox với Fanpage
        /// </summary>
        /// <param name="FBpageId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetListPSId(Guid FBpageId)
        {
            var facebookpage = GetService<IFacebookPageService>();
            var page = facebookpage.SearchQuery(x => x.Id == FBpageId).FirstOrDefault();
            if (page == null)
                throw new Exception($"trang {page.PageName} vui lòng kiểm tra lại !");
            var errorMaessage = "";
            var PSid = new List<string>();
            var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
            //Paged API request
            var pagedRequestUrl = $"{page.PageId}/conversations?fields=senders";
            var pagedRequest = (IPagedRequest)ApiRequest.Create(ApiRequest.RequestType.Paged, pagedRequestUrl, apiClient);
            pagedRequest.AddQueryParameter("access_token", page.PageAccesstoken);
            pagedRequest.AddPageLimit(25);

            var res = new List<FacebookSenders>();
            var lstCus = new List<string>();
            var pagedRequestResponse = await pagedRequest.ExecutePageAsync<FacebookSenders>();
            if (pagedRequestResponse.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", pagedRequestResponse.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                if (pagedRequestResponse.IsDataAvailable())
                {
                    res.AddRange(pagedRequestResponse.GetResultData());
                    //kiểm tra nếu có phân trang
                    while (pagedRequestResponse.IsNextPageDataAvailable())
                    {
                        //lấy dữ liệu lần lượt các trang kế
                        pagedRequestResponse = pagedRequestResponse.GetNextPageData();
                        //add vào data
                        res.AddRange(pagedRequestResponse.GetResultData());
                    }
                    PSid = res.Select(x => x.Senders.Data[0].Id).ToList();

                }

                return PSid;
            }

        }


        /// <summary>
        /// Kiểm tra danh sách PSID của Fanpage với danh sách facebookUser lấy ra các PSID mới
        /// </summary>
        /// <param name="FBpageId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FacebookCustomer>> CheckCustomerNew(Guid FBpageId)
        {
            var facebookuser = GetService<IFacebookUserProfileService>();
            var page = SearchQuery(x => x.Id == FBpageId).FirstOrDefault();
            if (page == null)
                throw new Exception($"trang {page.PageName} vui lòng kiểm tra lại !");

            var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
            var lstFBCus = new List<FacebookCustomer>().AsEnumerable();
            var lstPsid = await GetListPSId(FBpageId);
            var lstFBUser = facebookuser.SearchQuery().Select(x => x.PSID).ToList();
            var lstCusNew = lstPsid.Except(lstFBUser);
            if (lstCusNew.Any())
            {                             
                    var tasks = lstCusNew.Select(id => LoadFacebookCustomer(id, page.PageAccesstoken));
                    lstFBCus = await Task.WhenAll(tasks); 
               
            }
            if(lstCusNew.Count() == 0)
            {
                throw new Exception($"không tìm thấy khách hàng mới từ Fanpage {page.PageName} !");
            }
            return lstFBCus;


        }


        public async Task<FacebookCustomer> LoadFacebookCustomer(string id, string accesstoken)
        {

            var apiClient = new ApiClient(accesstoken, FacebookApiVersions.V6_0);
            var errorMaessage = "";
            var getRequestUrl = $"{id}?fields=id,name,first_name,last_name";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("access_token", accesstoken);
            var response = (await getRequest.ExecuteAsync<FacebookCustomer>());
            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                return null;
            }
            else
            {
                var result = response.GetResult();
                var facebookCus = new FacebookCustomer
                {
                    PSId = result.PSId,
                    Name = result.Name,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                };
                return facebookCus;

            }
        }

        public async Task<List<FacebookUserProfile>> CreateFacebookUser()
        {
            var userservice = GetService<UserManager<ApplicationUser>>();
            var user = await userservice.FindByIdAsync(UserId);
            if (user.FacebookPageId == null)
            {
                throw new Exception($"{user.Name} chưa liên kết với Fanpage nào !");
            }
            var facebookuser = GetService<IFacebookUserProfileService>();
            var page = SearchQuery(x => x.Id == user.FacebookPageId).FirstOrDefault();
            var lstFBUser = new List<FacebookUserProfile>();
            var lstCusNew = await CheckCustomerNew(page.Id);
            if (lstCusNew.Any())
            {
                foreach (var item in lstCusNew)
                {
                    if (item == null)
                        continue;
                    var fbuser = new FacebookUserProfile
                    {
                        PSID = item.PSId,
                        Name = item.Name,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender,
                        FbPageId = page.Id

                    };
                    await facebookuser.CheckPsid(fbuser.PSID);
                    await facebookuser.CreateAsync(fbuser);
                    lstFBUser.Add(fbuser);


                }
            }

            return lstFBUser;

        }






        /// <summary>
        /// Automation Config
        /// </summary>
        /// <returns></returns>
        public async Task<FacebookScheduleAppointmentConfigBasic> _GetAutoConfig()
        {
            var userService = GetService<UserManager<ApplicationUser>>();
            var user = await userService.FindByIdAsync(UserId);
            var autoConfigService = GetService<IFacebookScheduleAppointmentConfigService>();
            if (user.FacebookPageId == null)
            {
                throw new Exception($"Tài khoản {user.Name} chưa kết nối với Fanpage nào !");
            }

            var page = await SearchQuery(x => x.Id == user.FacebookPageId).Include(x => x.AutoConfig).FirstOrDefaultAsync();
            var autoConfig = page.AutoConfig;
            if (autoConfig == null)
            {
                autoConfig = await autoConfigService.CreateAsync(new FacebookScheduleAppointmentConfig());

                page.AutoConfigId = autoConfig.Id;
                await UpdateAsync(page);
            }

            var basic = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(autoConfig);
            return basic;
        }

        public async Task SaveAutoConfig(FacebookScheduleAppointmentConfigSave val)
        {
            var userService = GetService<UserManager<ApplicationUser>>();
            var autoConfigService = GetService<IFacebookScheduleAppointmentConfigService>();
            var user = await userService.FindByIdAsync(UserId);
            if (user.FacebookPageId == null)
            {
                throw new Exception($"Tài khoản {user.Name} chưa kết nối với Fanpage nào !");
            }
            var page = await SearchQuery(x => x.Id == user.FacebookPageId).Include(x => x.AutoConfig).FirstOrDefaultAsync();
            var autoConfig = page.AutoConfig;

            if (autoConfig == null)
            {
                autoConfig = await autoConfigService.CreateAsync(new FacebookScheduleAppointmentConfig());

                page.AutoConfigId = autoConfig.Id;
                await UpdateAsync(page);
            }
            if (val.ScheduleType == "minutes" && val.ScheduleNumber <= 0 || val.ScheduleType == "hours" && val.ScheduleNumber <= 0)
            {
                throw new Exception("thời gian nhắc lịch không được nhỏ hơn hoặc bằng 0 !");
            }

            autoConfig = _mapper.Map(val, autoConfig);
            await autoConfigService.UpdateAsync(autoConfig);

            if (autoConfig.AutoScheduleAppoint)
            {
                var host = _tenant != null ? _tenant.Hostname : "localhost";
                var ScheduleNumber = autoConfig.ScheduleNumber ?? 0;
                var RecurringJobId = $"{host}-{user.FacebookPageId.Value}-AutoMesAppointmentFB";

                if (autoConfig.ScheduleType == "minutes")

                    RecurringJob.AddOrUpdate(RecurringJobId, () => RunSendMessageAppointmentFB(host, user.FacebookPageId.Value), $"*/{ScheduleNumber} * * * *");
                else if (autoConfig.ScheduleType == "hours")
                    RecurringJob.AddOrUpdate(RecurringJobId, () => RunSendMessageAppointmentFB(host, user.FacebookPageId.Value), $"* */{ScheduleNumber}  * * *");

                autoConfig.RecurringJobId = RecurringJobId;
                await autoConfigService.UpdateAsync(autoConfig);
            }
            else
            {
                if (!string.IsNullOrEmpty(autoConfig.RecurringJobId))
                    RecurringJob.RemoveIfExists(autoConfig.RecurringJobId);
            }
        }

        public async Task RunSendMessageAppointmentFB(string db, Guid fbpageId)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            if (db == "localhost")
                builder["Database"] = "TMTDentalCatalogDb";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var page = conn.Query<FacebookPage>("" +
                        "SELECT * " +
                        "FROM FacebookPages " +
                        "where Id = @id" +
                        "", new { id = fbpageId }).FirstOrDefault();
                    if (page == null)
                        return;
                    var fbshedule = conn.Query<FacebookScheduleAppointmentConfig>("" +
                        "Select * " +
                        "FROM FacebookScheduleAppointmentConfigs " +
                         "where Id = @Id" +
                         "", new { Id = page.AutoConfigId }
                        ).FirstOrDefault();


                    if (fbshedule == null)
                        return;

                   
                   
                    var date = DateTime.Now;
                    if (fbshedule.ScheduleType == "minutes")
                    {
                        var datefrom = date;
                        var dateto = date.AddMinutes(fbshedule.ScheduleNumber.Value);
                        var fbSheduleConfigMinutes = conn.Query<PartnerAppointment>("" +
                            "Select app.PartnerId, fbuser.PSID , app.Date , pn.Name , company.Name as CompanyName  , company.AddressCompany as CompanyAddress " +
                            "From Appointments as app " +
                            "Left Join FacebookUserProfiles as fbuser On app.PartnerId = fbuser.PartnerId " +
                            "Left Join Partners as pn ON pn.Id = fbuser.PartnerId " +
                            "Left Join (Select cps.Id as companyid, pns.Id, pns.Name, (pns.Street + ',' + pns.CityName + ',' + pns.DistrictName + ',' + pns.WardName) as AddressCompany From Partners as pns Left Join Companies as cps  On cps.PartnerId = pns.Id) company ON company.companyid = pn.CompanyId " +
                            "Where fbuser.FbPageId = @pageId AND DATEADD(MINUTE,-@scheduleNumber,app.Date) BETWEEN @dateFrom AND @dateTo " + "", new { pageId = fbpageId, dateFrom = datefrom, dateTo = dateto, scheduleNumber = fbshedule.ScheduleNumber.Value }).ToList();
                        if (fbSheduleConfigMinutes == null)
                            return;
                        var tasks = fbSheduleConfigMinutes.Select(x => SendMessageAppointmentFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage.Replace("{{tenchinhanh}}", x.CompanyName).Replace("{{diachichinhanh}}", x.CompanyAddress == null ? "đang cập nhật ..." : x.CompanyAddress).Replace("{{tenkhachhang}}", x.Name).Replace("{{giohen}}", x.Date.ToString("hh:mm tt")))).ToList();
                        var limit = 200;
                        var offset = 0;
                        var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        while (subTasks.Any())
                        {
                            var result = await Task.WhenAll(subTasks);
                            offset += limit;
                            subTasks = tasks.Skip(offset).Take(limit).ToList();
                        }
                    }
                    else if (fbshedule.ScheduleType == "hours")
                    {
                        var datefrom = date;
                        var dateto = date.AddHours(fbshedule.ScheduleNumber.Value);
                        var fbSheduleConfigHours = conn.Query<PartnerAppointment>("" +
                            "Select app.PartnerId, fbuser.PSID , app.Date , pn.Name , company.Name as CompanyName  , company.AddressCompany as CompanyAddress " +
                            "From Appointments as app " +
                            "Left Join FacebookUserProfiles as fbuser On app.PartnerId = fbuser.PartnerId " +
                            "Left Join Partners as pn ON pn.Id = fbuser.PartnerId " +
                            "Left Join (Select cps.Id as companyid, pns.Id, pns.Name, (pns.Street + ',' + pns.CityName + ',' + pns.DistrictName + ',' + pns.WardName) as AddressCompany From Partners as pns Left Join Companies as cps  On cps.PartnerId = pns.Id) company ON company.companyid = pn.CompanyId " +
                            "Where fbuser.FbPageId = @pageId AND DATEADD(HOUR,-(@scheduleNumber),app.Date) BETWEEN @dateFrom AND @dateTo " + "", new { pageId = fbpageId, dateFrom = datefrom, dateTo = dateto, scheduleNumber = fbshedule.ScheduleNumber.Value }).ToList();


                        if (fbSheduleConfigHours == null)
                            return;
                        var tasks = fbSheduleConfigHours.Select(x => SendMessageAppointmentFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage.Replace("{{tenchinhanh}}", x.CompanyName).Replace("{{diachichinhanh}}", x.CompanyAddress == null ? "đang cập nhật ..." : x.CompanyAddress).Replace("{{tenkhachhang}}", x.Name).Replace("{{giohen}}", x.Date.ToString("hh:mm tt")))).ToList();
                        var limit = 200;
                        var offset = 0;
                        var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        while (subTasks.Any())
                        {
                            var result = await Task.WhenAll(subTasks);
                            offset += limit;
                            subTasks = tasks.Skip(offset).Take(limit).ToList();
                        }
                    }




                }
                catch
                {
                }
            }
        }


        public async Task<SendFacebookMessage> SendMessageAppointmentFBAsync(string accessTokenPage, string psid, string message)
        {
            var errorMaessage = "";
            var apiClient = new ApiClient(accessTokenPage, FacebookApiVersions.V6_0);
            var url = $"me/messages";
            //var messagefb = message.Replace("{{name}}", partner.Name).Replace("{{date}}", date.ToString("hh:mm tt"));
            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("messaging_type", "MESSAGE_TAG");
            request.AddParameter("tag", "CONFIRMED_EVENT_UPDATE");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));


            var response = await request.ExecuteAsync<SendFacebookMessage>();

            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                return new SendFacebookMessage() { error = errorMaessage };
            }
            else
            {
                var result = response.GetResult();

                return result;

            }



        }


    }
   

}
