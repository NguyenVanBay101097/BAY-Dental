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
using NPOI.SS.Formula.Functions;
using RestSharp.Deserializers;
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
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class FacebookPageService : BaseService<FacebookPage>, IFacebookPageService
    {
        private readonly IMapper _mapper;
        private readonly FacebookAuthSettings _fbAuthSettings;
        private readonly AppTenant _tenant;
        private readonly ConnectionStrings _connectionStrings;
        private readonly AppSettings _appSettings;

        public FacebookPageService(IAsyncRepository<FacebookPage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IOptions<FacebookAuthSettings> fbAuthSettingsAccessor,
            ITenant<AppTenant> tenant, IOptions<ConnectionStrings> connectionStrings, IOptions<AppSettings> appSettings)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _fbAuthSettings = fbAuthSettingsAccessor?.Value;
            _tenant = tenant?.Value;
            _connectionStrings = connectionStrings?.Value;
            _appSettings = appSettings?.Value;
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
            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type == val.Type);

            var items = await _mapper.ProjectTo<FacebookPageBasic>(query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookPageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
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

        /// <summary>
        /// lấy ra danh sách các PSID đã inbox với Fanpage
        /// </summary>
        /// <param name="FBpageId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetListPSId(Guid FBpageId)
        {
            var facebookpage = GetService<IFacebookPageService>();
            var page = await facebookpage.SearchQuery(x => x.Id == FBpageId).FirstOrDefaultAsync();
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

            var res = new List<ApiPagedConversationsDataItem>();
            var lstCus = new List<string>();
            var pagedRequestResponse = await pagedRequest.ExecutePageAsync<ApiPagedConversationsDataItem>();
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
                    PSid = res.Select(x => x.Participants.Data[0].Id).ToList();

                }

                return PSid;
            }

        }


        /// <summary>
        /// Kiểm tra danh sách PSID của Fanpage với danh sách facebookUser lấy ra các PSID mới
        /// </summary>
        /// <param name="FBpageId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiUserProfileResponse>> CheckCustomerNew(Guid FBpageId)
        {
            var facebookuser = GetService<IFacebookUserProfileService>();
            var page = SearchQuery(x => x.Id == FBpageId).FirstOrDefault();
            if (page == null)
                throw new Exception($"trang {page.PageName} vui lòng kiểm tra lại !");

            var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
            var lstFBCus = new List<ApiUserProfileResponse>().AsEnumerable();
            var lstPsid = await GetListPSId(FBpageId);
            var lstFBUser = facebookuser.SearchQuery().Select(x => x.PSID).ToList();
            var lstCusNew = lstPsid.Except(lstFBUser);
            if (lstCusNew.Any())
            {
                var tasks = lstCusNew.Select(id => _LoadUserProfile(id, page.PageAccesstoken));
                lstFBCus = await Task.WhenAll(tasks);

            }

            if (lstCusNew.Count() == 0)
            {
                throw new Exception($"không tìm thấy khách hàng mới từ Fanpage {page.PageName} !");
            }

            return lstFBCus;
        }

        public async Task<ApiUserProfileResponse> _LoadUserProfile(string psid, string access_token)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{psid}?fields=id,name,first_name,last_name,profile_pic";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            var response = (await getRequest.ExecuteAsync<ApiUserProfileResponse>());
            if (!response.GetExceptions().Any())
            {
                return response.GetResult();
            }

            return null;
        }

        /// <summary>
        /// Đồng bộ khách hàng tương tác
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task SyncUsers(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            //var fb_pages = self.Where(x => x.Type == "facebook").ToList();
            if (self.Where(x => x.Type == "facebook").Any())
                await SyncFacebookUsers(self);
            else if (self.Where(x => x.Type == "zalo").Any())
                await SyncZaloUsers(self);
            //var zl_pages = self.Where(x => x.Type == "zalo").ToList();
            //if (zl_pages.Any())

        }

        public async Task SyncNumberPhoneUsers(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var zl_pages = self.Where(x => x.Type == "zalo").ToList();
            var fb_pages = self.Where(x => x.Type == "facebook").ToList();
            if (fb_pages.Any())
                await ProcessUpdateNumberPhone(fb_pages);
            if (zl_pages.Any())
                await ProcessUpdateNumberPhoneZalo(zl_pages);


        }

        public async Task SyncPartnersForMultiUsers(MultiUserProfilesVm val)
        {

            var userProfileObj = GetService<IFacebookUserProfileService>();
            var page = await SearchQuery(x => x.Id == val.PageId).FirstOrDefaultAsync();

            var profiles = await userProfileObj.SearchQuery(x => val.UserIds.Contains(x.Id) && x.FbPageId == page.Id).ToListAsync();
            await ProcessSyncPartners(profiles);

        }

        public async Task SyncPhoneForMultiUsers(MultiUserProfilesVm val)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();
            var page = await SearchQuery(x => x.Id == val.PageId).FirstOrDefaultAsync();

            var users = await userProfileObj.SearchQuery(x => val.UserIds.Contains(x.Id) && x.FbPageId == page.Id).ToListAsync();
            if(page.Type == "facebook")
            {
                await UpdatePhoneForMultiUsersFB(page, users);
            }
            else if(page.Type == "zalo") 
            {
                await UpdatePhoneForMultiUsersZaLo(page, users);
            }
       
        }

        public async Task UpdatePhoneForMultiUsersFB(FacebookPage page, IEnumerable<FacebookUserProfile> profiles)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();
            var psIds = profiles.Select(x => x.PSID).ToList();
            var conversations = await LoadConversations(page);
            //task whenall
            var conversationsOfMultiUser = conversations.Where(x => x.Sender.Data.Any(s => psIds.Contains(s.Id))).ToList();
            var tasks = conversationsOfMultiUser.Select(x => LoadMessagesOfConversation(page, x.Id)).ToList();
            var results = await Task.WhenAll(tasks);

            var allMessages = new List<ApiPagedConversationMessages>();
            foreach (var item in results)
            {
                if (item != null)
                    allMessages.AddRange(item);
            }


            /// update phone
            IDictionary<string, List<string>> psidPhoneDict = new Dictionary<string, List<string>>();
            foreach (var message in allMessages)
            {
                var psid = message.From.Id;
                if (psid == page.PageId)
                    continue;

                var phones = GetPhonesFromText(message.Message);
                if (!psidPhoneDict.ContainsKey(psid))
                    psidPhoneDict.Add(psid, new List<string>());

                psidPhoneDict[psid].AddRange(phones);
            }

            var psids = psidPhoneDict.Keys.ToArray();
            var profileDict = profiles.ToDictionary(x => x.PSID, x => x);

            foreach (var item in psidPhoneDict)
            {
                if (!profileDict.ContainsKey(item.Key))
                    continue;
                var profile = profileDict[item.Key];
                var phones = item.Value;
                profile.Phone = string.Join(",", phones.Distinct().ToList());
            }

            await userProfileObj.UpdateAsync(profiles);
        }

        public async Task UpdatePhoneForMultiUsersZaLo(FacebookPage page, IEnumerable<FacebookUserProfile> profiles)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();
            var tasks = profiles.Select(x => LoadMessagesZaloPage(page, long.Parse(x.PSID))).ToList();
            var results = await Task.WhenAll(tasks);


            var allMessages = new List<GetMessageFollowersData>();
            foreach (var item in results)
            {
                if (item != null)
                    allMessages.AddRange(item);
            }

            IDictionary<string, List<string>> psidPhoneDict = new Dictionary<string, List<string>>();
            foreach (var message in allMessages)
            {
                var psid = message.from_id;

                var phones = GetPhonesFromText(message.message);
                if (!psidPhoneDict.ContainsKey(psid))
                    psidPhoneDict.Add(psid, new List<string>());

                psidPhoneDict[psid].AddRange(phones);
            }

            var psids = psidPhoneDict.Keys.ToArray();
            var profileDict = profiles.ToDictionary(x => x.PSID, x => x);

            foreach (var item in psidPhoneDict)
            {
                if (!profileDict.ContainsKey(item.Key))
                    continue;
                var profile = profileDict[item.Key];
                var phones = item.Value;
                profile.Phone = string.Join(",", phones.Distinct().ToList());
            }

            await userProfileObj.UpdateAsync(profiles);
        }


        public async Task SyncPartnersForNumberPhone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var partnerObj = GetService<IPartnerService>();
            var userProfileObj = GetService<IFacebookUserProfileService>();
            ///get list facebook page
            foreach (var page in self)
            {
                ///get list facebook user profile 
                var profiles = await userProfileObj.SearchQuery(x => x.FbPageId == page.Id && !string.IsNullOrEmpty(x.Phone)).ToListAsync();
                await ProcessSyncPartners(profiles);

            }

        }

        public async Task ProcessSyncPartners(IEnumerable<FacebookUserProfile> profiles)
        {
            var partnerObj = GetService<IPartnerService>();
            var userProfileObj = GetService<IFacebookUserProfileService>();
            IDictionary<string, List<string>> psidPhoneDict = new Dictionary<string, List<string>>();
            foreach (var profile in profiles)
            {
                var psid = profile.PSID;
                var phones = new List<string>(profile.Phone.Split(','));

                if (!psidPhoneDict.ContainsKey(psid))
                    psidPhoneDict.Add(psid, new List<string>());

                psidPhoneDict[psid].AddRange(phones);
            }

            var profileDict = profiles.ToDictionary(x => x.PSID, x => x);

            //get partner
            var partnes = await partnerObj.SearchQuery(x => !string.IsNullOrEmpty(x.Phone) && x.Customer).ToListAsync();
            var partnerDict = partnes.ToDictionary(x => x.Phone, x => x);
            //get list had condition


            foreach (var item in psidPhoneDict)
            {
                if (!profileDict.ContainsKey(item.Key))
                    continue;

                var profile = profileDict[item.Key];
                var phones = item.Value;

                foreach (var phone in phones)
                {
                    if (partnerDict.ContainsKey(phone))
                    {
                        var partnerId = partnerDict[phone];
                        profile.PartnerId = partnerId.Id;
                    }
                }
            }

            await userProfileObj.UpdateAsync(profiles);
        }


        /// <summary>
        /// lấy ra tất cả userprofile của page
        /// </summary>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public async Task ProcessUpdateNumberPhone(IEnumerable<FacebookPage> self)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();
            foreach (var page in self)
            {
                var conversations = await LoadConversations(page);
                //task whenall
                var tasks = conversations.Select(x => LoadMessagesOfConversation(page, x.Id)).ToList();
                var results = await Task.WhenAll(tasks);

                var allMessages = new List<ApiPagedConversationMessages>();
                foreach (var item in results)
                {
                    if (item != null)
                        allMessages.AddRange(item);
                }

                IDictionary<string, List<string>> psidPhoneDict = new Dictionary<string, List<string>>();
                foreach (var message in allMessages)
                {
                    var psid = message.From.Id;
                    if (psid == page.PageId)
                        continue;

                    var phones = GetPhonesFromText(message.Message);
                    if (!psidPhoneDict.ContainsKey(psid))
                        psidPhoneDict.Add(psid, new List<string>());

                    psidPhoneDict[psid].AddRange(phones);
                }

                var psids = psidPhoneDict.Keys.ToArray();
                var profiles = await userProfileObj.SearchQuery(x => psids.Contains(x.PSID)).ToListAsync();
                var profileDict = profiles.ToDictionary(x => x.PSID, x => x);

                foreach (var item in psidPhoneDict)
                {
                    if (!profileDict.ContainsKey(item.Key))
                        continue;
                    var profile = profileDict[item.Key];
                    var phones = item.Value;
                    profile.Phone = string.Join(",", phones.Distinct().ToList());
                }

                await userProfileObj.UpdateAsync(profiles);
            }
        }

        public async Task ProcessUpdateNumberPhoneZalo(IEnumerable<FacebookPage> self)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();
            foreach (var page in self)
            {
                //get all profiles zalo
                var profiles = await userProfileObj.SearchQuery(x => x.FbPageId == page.Id).ToListAsync();

                // load messages
                //task whenall
                var tasks = profiles.Select(x => LoadMessagesZaloPage(page, long.Parse(x.PSID))).ToList();
                var results = await Task.WhenAll(tasks);


                var allMessages = new List<GetMessageFollowersData>();
                foreach (var item in results)
                {
                    if (item != null)
                        allMessages.AddRange(item);
                }

                IDictionary<string, List<string>> psidPhoneDict = new Dictionary<string, List<string>>();
                foreach (var message in allMessages)
                {
                    var psid = message.from_id;               

                    var phones = GetPhonesFromText(message.message);
                    if (!psidPhoneDict.ContainsKey(psid))
                        psidPhoneDict.Add(psid, new List<string>());

                    psidPhoneDict[psid].AddRange(phones);
                }

                var psids = psidPhoneDict.Keys.ToArray();
                var profileDict = profiles.ToDictionary(x => x.PSID, x => x);

                foreach (var item in psidPhoneDict)
                {
                    if (!profileDict.ContainsKey(item.Key))
                        continue;
                    var profile = profileDict[item.Key];
                    var phones = item.Value;
                    profile.Phone = string.Join(",", phones.Distinct().ToList());
                }

                await userProfileObj.UpdateAsync(profiles);


            }
        }

        public async Task<IEnumerable<GetMessageFollowersData>> LoadMessagesZaloPage(FacebookPage self , long psid)
        {
            var zaloClient = new ZaloClient(self.PageAccesstoken);         
            var offset = 0;
            var count = 10;
            var res = zaloClient.getListConversationWithUser(psid, offset, count).ToObject<GetMessageUserFollowersResponse>();
            var list = new List<GetMessageFollowersData>();
            while (res.data != null && res.data.Any())
            {
                list.AddRange(res.data);
                offset += count;
                res = zaloClient.getListConversationWithUser(psid, offset, count).ToObject<GetMessageUserFollowersResponse>();
            }

            return list;

        }

        public async Task<List<ApiPagedConversationsData>> LoadConversations(FacebookPage page)
        {
            var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
            var pagedRequestUrl = $"{page.PageId}" + "/conversations?fields=senders";
            var pagedRequest = (IPagedRequest)ApiRequest.Create(ApiRequest.RequestType.Paged, pagedRequestUrl, apiClient);
            pagedRequest.AddQueryParameter("access_token", page.PageAccesstoken);
            pagedRequest.AddPageLimit(100);
            var errorMessage = "";
            var res = new List<ApiPagedConversationsData>();
            var pagedRequestResponse = await pagedRequest.ExecutePageAsync<ApiPagedConversationsData>();
            if (pagedRequestResponse.GetExceptions().Any())
            {
                errorMessage = string.Join("; ", pagedRequestResponse.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMessage);
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
                }
            }

            return res;
        }



        public async Task<IEnumerable<ApiPagedConversationMessages>> LoadMessagesOfConversation(FacebookPage page, string conversationId)
        {
            //List<string> list = new List<string>();
            var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
            var pagedRequestUrl = $"{conversationId}" + "/messages?fields=from,message";
            var pagedRequest = (IPagedRequest)ApiRequest.Create(ApiRequest.RequestType.Paged, pagedRequestUrl, apiClient);
            pagedRequest.AddQueryParameter("access_token", page.PageAccesstoken);
            pagedRequest.AddPageLimit(100);
            var errorMessage = "";
            var res = new List<ApiPagedConversationMessages>();
            var pagedRequestResponse = await pagedRequest.ExecutePageAsync<ApiPagedConversationMessages>();
            if (pagedRequestResponse.GetExceptions().Any())
            {
                errorMessage = string.Join("; ", pagedRequestResponse.GetExceptions().Select(x => x.Message));
                return res;
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
                }
            }

            return res;
        }


        /// <summary>
        /// lấy ra các số điện thoại trong 1 textbox
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private IEnumerable<string> GetPhonesFromText(string text)
        {
            var listphone = new List<string>();
            string MatchPhonePattern = @"((09|03|07|08|05)+([0-9]{8})\b)";

            Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // Find matches.
            MatchCollection matches = rx.Matches(text);

            // Report the number of matches found.
            //int noOfMatches = matches.Count;

            //Do something with the matches

            foreach (System.Text.RegularExpressions.Match match in matches)
                //Do something with the matches
                listphone.Add(match.Value.ToString());


            return listphone.Distinct().ToList();
        }




        private async Task SyncZaloUsers(IEnumerable<FacebookPage> self)
        {
            foreach (var page in self)
            {
                //Lấy danh sách người quan tâm
                var profiles = await _GetFollowersZaloOAAsync(page);
                if (profiles.Count() == 0)
                    continue;
                //var profiles = await _GetProfileFollowers(page, user_ids);

                await _ProcessProfilesZalo(page, profiles);
            }
        }

        private async Task _ProcessProfilesZalo(FacebookPage self, IEnumerable<GetProfileOfFollowerResponse> profiles)
        {
            profiles = profiles.Where(x => x != null && x.data != null).ToList();

            var userProfileObj = GetService<IFacebookUserProfileService>();
            var limit = 100;
            var offset = 0;
            var tmp = profiles.Skip(offset).Take(limit).ToList();
            var dict = profiles.ToDictionary(x => x.data.user_id.ToString(), x => x.data);
            while (tmp.Any())
            {
                var user_ids = tmp.Select(x => x.data.user_id.ToString()).ToList();
                //var exist_psids = await userProfileObj.SearchQuery(x => user_ids.Contains(x.PSID) && x.FbPageId == self.Id).Select(x => x.PSID).ToListAsync();
                //user_ids = user_ids.Except(exist_psids).ToList();

                var list = new List<FacebookUserProfile>();
                foreach (var psid in user_ids)
                {
                    var user_info = dict[psid];
                    list.Add(new FacebookUserProfile
                    {
                        Name = user_info.display_name,
                        Avatar = user_info.avatar,
                        FbPageId = self.Id,
                        PSID = user_info.user_id.ToString(),
                    });
                }

                await userProfileObj.CreateAsync(list);

                offset += limit;
                tmp = profiles.Skip(offset).Take(limit).ToList();
            }
        }

        public Task<GetProfileOfFollowerResponse> _GetProfileZalo(FacebookPage self, string user_id)
        {
            return Task.Run(() =>
            {
                var zaloClient = new ZaloClient(self.PageAccesstoken);
                var res = zaloClient.getProfileOfFollower(user_id).ToObject<GetProfileOfFollowerResponse>();
                return res;
            });
        }

        private async Task<IEnumerable<GetProfileOfFollowerResponse>> _GetProfileFollowers(FacebookPage self, IEnumerable<string> user_ids)
        {
            var tasks = user_ids.Select(x => _GetProfileZalo(self, x));
            return await Task.WhenAll(tasks);
        }

        private async Task<IEnumerable<GetProfileOfFollowerResponse>> _GetFollowersZaloOAAsync(FacebookPage self)
        {

            var zaloClient = new ZaloClient(self.PageAccesstoken);
            var userProfileObj = GetService<IFacebookUserProfileService>();
            var offset = 0;
            var count = 25;
            var res = zaloClient.getListFollower(offset, count).ToObject<GetFollowersResponse>();

            var list = new List<string>().AsEnumerable();
            while (res.data != null && res.data.followers.Any())
            {
                list = res.data.followers.Select(x => x.user_id);
                //list = list.Union(tmp_ids);

                offset += count;
                res = zaloClient.getListFollower(offset, count).ToObject<GetFollowersResponse>();
            }

            list = list.Except(userProfileObj.SearchQuery(x => x.FbPageId == self.Id).Select(x => x.PSID));


            var tasks = list.Select(x => _GetProfileZalo(self, x));

            return await Task.WhenAll(tasks);
        }

        private async Task SyncFacebookUsers(IEnumerable<FacebookPage> self)
        {
            foreach (var page in self)
            {
                //Tìm tất cả psid từ conversations
                var user_infos = await _FindPSIDsFromConversations(page);
                if (user_infos.Count() == 0)
                    return;

                // var user_infos = await _GetUserInfosFromPSIDs(page, psids);
                await _ProcessUserInfos(page, user_infos);
            }
        }

        private async Task _ProcessUserInfos(FacebookPage self, IEnumerable<ApiUserProfileResponse> user_infos)
        {
            user_infos = user_infos.Where(x => x != null).ToList();

            var userProfileObj = GetService<IFacebookUserProfileService>();
            var limit = 100;
            var offset = 0;
            var tmp = user_infos.Skip(offset).Take(limit).ToList();
            var dict = user_infos.ToDictionary(x => x.PSId, x => x);
            while (tmp.Any())
            {
                var psids = tmp.Select(x => x.PSId).ToList();
                //var exist_psids = await userProfileObj.SearchQuery(x => psids.Contains(x.PSID) && x.FbPageId == self.Id).Select(x => x.PSID).ToListAsync();
                //psids = psids.Except(exist_psids).ToList();

                var list = new List<FacebookUserProfile>();
                foreach (var psid in psids)
                {
                    var user_info = dict[psid];
                    list.Add(new FacebookUserProfile
                    {
                        Name = user_info.Name,
                        FirstName = user_info.FirstName,
                        LastName = user_info.LastName,
                        Avatar = $"https://graph.facebook.com/{user_info.PSId}/picture?access_token={self.PageAccesstoken}",
                        FbPageId = self.Id,
                        PSID = user_info.PSId,
                        Gender = user_info.Gender
                    });
                }

                await userProfileObj.CreateAsync(list);

                offset += limit;
                tmp = user_infos.Skip(offset).Take(limit).ToList();
            }
        }

        private async Task<IEnumerable<ApiUserProfileResponse>> _GetUserInfosFromPSIDs(FacebookPage self, IEnumerable<string> psids)
        {
            var tasks = psids.Select(id => _LoadUserProfile(id, self.PageAccesstoken));
            return await Task.WhenAll(tasks);
        }

        private async Task<IEnumerable<ApiUserProfileResponse>> _FindPSIDsFromConversations(FacebookPage self)
        {
            var psids = new List<string>().AsEnumerable();
            var UserProfileObj = GetService<IFacebookUserProfileService>();
            var apiClient = new ApiClient(self.PageAccesstoken, FacebookApiVersions.V6_0);
            var pagedRequestUrl = $"{self.PageId}/conversations?fields=participants";
            var pagedRequest = (IPagedRequest)ApiRequest.Create(ApiRequest.RequestType.Paged, pagedRequestUrl, apiClient);
            pagedRequest.AddQueryParameter("access_token", self.PageAccesstoken);
            pagedRequest.AddPageLimit(25);
            var errorMaessage = "";
            var res = new List<ApiPagedConversationsDataItem>();
            var pagedRequestResponse = await pagedRequest.ExecutePageAsync<ApiPagedConversationsDataItem>();
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
                    psids = res.Select(x => x.Participants.Data[0].Id).ToList();

                }
                psids = psids.Except(UserProfileObj.SearchQuery(x => x.FbPageId == self.Id).Select(x => x.PSID)).ToList();

                var user_infos = psids.Select(id => _LoadUserProfile(id, self.PageAccesstoken));

                return await Task.WhenAll(user_infos);
            }
        }

        private IEnumerable<string> _GetPSIDsFromApiConversationsResponse(FacebookPage self, IEnumerable<ApiPagedConversationsDataItem> items)
        {
            var res = new HashSet<string>();
            var participants = items.SelectMany(x => x.Participants.Data);
            foreach (var participant in participants)
            {
                if (participant.Id == self.PageId)
                    continue;
                res.Add(participant.Id);
            }

            return res;
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
                        var tasks = fbSheduleConfigMinutes.Select(x => SendMessageAppointmentFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage.Replace("{{ten_chi_nhanh}}", x.CompanyName).Replace("{{dia_chi_chi_nhanh}}", x.CompanyAddress == null ? "đang cập nhật ..." : x.CompanyAddress).Replace("{{ten_khach_hang}}", x.Name).Replace("{{gio_hen}}", x.Date.ToString("hh:mm tt")))).ToList();
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
                        var tasks = fbSheduleConfigHours.Select(x => SendMessageAppointmentFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage.Replace("{{ten_chi_nhanh}}", x.CompanyName).Replace("{{dia_chi_chi_nhanh}}", x.CompanyAddress == null ? "đang cập nhật ..." : x.CompanyAddress).Replace("{{ten_khach_hang}}", x.Name).Replace("{{gio_hen}}", x.Date.ToString("hh:mm tt")))).ToList();
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

        public async Task RefreshSocial(FacebookPageSimple val)
        {
            var connectObj = GetService<IFacebookConnectService>();
            var connectPageObj = GetService<IFacebookConnectPageService>();
            var socialChannel = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();


            if (socialChannel.Type == "facebook")
            {
                var account = await connectObj.GetUserAccounts(socialChannel.UserAccesstoken, socialChannel.UserId);
                var page = account.Accounts.Data.Where(x => x.Id == socialChannel.PageId).SingleOrDefault();

                //update accesstoken connect page
                var connectPage = await connectPageObj.SearchQuery(x => x.PageId == page.Id).SingleOrDefaultAsync();
                if (connectPage == null)
                {
                    throw new Exception("tài khoản facebook này không có quyền với trang này!");
                }
                connectPage.PageAccessToken = page.PageAccesstoken;

                await connectPageObj.UpdateAsync(connectPage);

                //update accesstoken facebook page

                socialChannel.PageAccesstoken = page.PageAccesstoken;

                await UpdateAsync(socialChannel);


                GetConnectWebhooksForPage(socialChannel.PageId, socialChannel.PageAccesstoken);

            }
        }

        private bool GetConnectWebhooksForPage(string pageId, string pageAccesstoken)
        {
            var host = _tenant != null ? _tenant.Hostname : "localhost";

            var content = new ConnectWebhooks
            {
                Host = $"{host}.{_appSettings.Domain}",
                FacebookId = pageId,
                FacebookToken = pageAccesstoken,
                FacebookName = null,
                FacebookAvatar = null,
                FacebookCover = null,
                FacebookLink = null,
                FacebookType = 2,
                CallbackUrl = $"{_appSettings.Schema}://{host}.{_appSettings.Domain}/api/FacebookWebHook",
                IsCallbackWithRaw = true
            };

            HttpResponseMessage response = null;
            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                response = client.PostAsJsonAsync("https://fba.tpos.vn/api/facebook/updatetoken", content).Result;
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception("Lỗi đăng ký fba.tpos.vn");

            return true;
        }






    }





    public class ApiPagedConversationsData
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }

        [DeserializeAs(Name = "senders")]
        public ApiPagedConversationSender Sender { get; set; }
    }

    public class ApiPagedConversationMessages
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }

        [JsonProperty("from")]
        public ApiPagedConversationDataFrom From { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }


    }

    public class ApiPagedConversationSender
    {
        [DeserializeAs(Name = "data")]
        public List<ApiPagedConversationSenderItem> Data { get; set; } = new List<ApiPagedConversationSenderItem>();

    }

    public class ApiPagedConversationSenderItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

    }

    public class ApiPagedConversationDataFrom
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class MultiUserProfilesVm
    {
        public Guid PageId { get; set; }
        public IEnumerable<Guid> UserIds { get; set; } = new List<Guid>();
    }

    public class objUserPhone
    {
        public string PSID { get; set; }
        public string Phone { get; set; }
    }

}
