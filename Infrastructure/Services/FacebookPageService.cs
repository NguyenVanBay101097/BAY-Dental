using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookPageService : BaseService<FacebookPage>, IFacebookPageService
    {
        private readonly IMapper _mapper;
        private readonly FacebookAuthSettings _fbAuthSettings;

        public FacebookPageService(IAsyncRepository<FacebookPage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IOptions<FacebookAuthSettings> fbAuthSettingsAccessor)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _fbAuthSettings = fbAuthSettingsAccessor?.Value;
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

        public async Task LoadUserProfile()
        {
            //tải khách hàng từ conversations
        }

        public async Task LoadUserProfileFromConversations(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            //tải khách hàng từ conversations
        }

        public async Task<string> GetPageConversations(string page_id, string access_token, int limit)
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{page_id}/access_token";
            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("fields", "conversations.limit(" + limit + "){participants}");
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
            if (lstCusNew.Count() == 0)
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
                throw new Exception(errorMaessage);
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
            var user = userservice.FindByIdAsync(UserId);
            if (user.Result.FacebookPageId == null)
            {
                throw new Exception($"{user.Result.Name} chưa liên kết với Fanpage nào !");
            }
            var facebookuser = GetService<IFacebookUserProfileService>();
            var page = SearchQuery(x => x.Id == user.Result.FacebookPageId).FirstOrDefault();
            var lstFBUser = new List<FacebookUserProfile>();
            var lstCusNew = await CheckCustomerNew(page.Id);
            if (lstCusNew.Any())
            {
                foreach (var item in lstCusNew)
                {

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
            if (page.AutoConfigId == null)
            {
                var fbSheduleApp = new FacebookScheduleAppointmentConfigSave();
                fbSheduleApp.ScheduleType = "minutes";
                fbSheduleApp.ScheduleNumber = 30;
                fbSheduleApp.AutoScheduleAppoint = false;
                fbSheduleApp.ContentMessage = "";
                var autoConfig = await autoConfigService.CreateFBSheduleConfig(fbSheduleApp);
                page.AutoConfigId = autoConfig.Id;
                await UpdateAsync(page);

                var basicautoConfig = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(autoConfig);
                return basicautoConfig;

            }
            var basic = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(page.AutoConfig);
            return basic;
        }

        public async Task<FacebookScheduleAppointmentConfigBasic> CreateAutoConfig(FacebookScheduleAppointmentConfigSave val)
        {
            var userService = GetService<UserManager<ApplicationUser>>();
            var autoConfigService = GetService<IFacebookScheduleAppointmentConfigService>();
            var user = await userService.FindByIdAsync(UserId);
            if (user.FacebookPageId == null)
            {
                throw new Exception($"Tài khoản {user.Name} chưa kết nối với Fanpage nào !");
            }
            var page = await SearchQuery(x => x.Id == user.FacebookPageId).Include(x => x.AutoConfig).FirstOrDefaultAsync();
            if (page.AutoConfigId == null)
            {
                var autoconfig = await autoConfigService.CreateFBSheduleConfig(val);
            }
            else
            {
                var autoconfig = await autoConfigService.UpdateFBSheduleConfig(page.AutoConfigId.Value, val);

            }

            //await UpdateAsync(page);
            var basicautoConfig = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(page.AutoConfig);
            return basicautoConfig;
           

        }
        //public async Task<FacebookPage> UpdateAutoConfig(FacebookScheduleAppointmentConfigSave val)
        //{
        //    var userService = GetService<UserManager<ApplicationUser>>();
        //    var autoConfigService = GetService<IFacebookScheduleAppointmentConfigService>();
        //    var user = await userService.FindByIdAsync(UserId);
        //    if (user.FacebookPageId == null)
        //    {
        //        throw new Exception($"Tài khoản {user.Name} chưa kết nối với Fanpage nào !");
        //    }
        //    var page = await SearchQuery(x => x.Id == user.FacebookPageId).Include(x => x.AutoConfig).FirstOrDefaultAsync();
        //    var autoconfig = await autoConfigService.UpdateFBSheduleConfig(page.AutoConfigId.Value, val);         


        //    await UpdateAsync(page);

        //    return page;

        //}

    }

}
