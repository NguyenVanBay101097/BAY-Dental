using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookUserProfileService : BaseService<FacebookUserProfile>, IFacebookUserProfileService
    {
        private readonly IMapper _mapper;
       
        
        public FacebookUserProfileService(IAsyncRepository<FacebookUserProfile> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper )
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;           
            
        }

        public async Task<PagedResult2<FacebookUserProfileBasic>> GetPagedResultAsync(FacebookUserProfilePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.PSID.Contains(val.Search));

            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookUserProfileBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<FacebookUserProfileBasic>>(items)
            };
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
        public async Task<List<FacebookCustomer>> CheckCustomerNew(Guid FBpageId)
        {
            var facebookpage = GetService<IFacebookPageService>();
            var page = facebookpage.SearchQuery(x => x.Id == FBpageId).FirstOrDefault();
            if (page == null)
                throw new Exception($"trang {page.PageName} vui lòng kiểm tra lại !");
            //var lstCusNew = ""; 
            var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
            //var faceboonCus = new FacebookCustomer();
            var lstFBCus = new List<FacebookCustomer>();
            var errorMaessage = "";
            //var lstCus = new List<string>();
            var lstPsid = await GetListPSId(FBpageId);
            //foreach (var psid in lstPsid)
            //{
            //    lstCus.Add(psid.Id);
            //}
            var lstFBUser = SearchQuery().Select(x => x.PSID).ToList();
            if (lstFBUser.Any())
            {
                var lstCusNew = lstPsid.Except(lstFBUser);
                if (lstCusNew.Any())
                {
                    foreach (var item in lstCusNew)
                    {
                        var getRequestUrl = $"{item}?fields=id,name,first_name,last_name";
                        var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
                        getRequest.AddQueryParameter("access_token", page.PageAccesstoken);
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
                            lstFBCus.Add(facebookCus);

                        }

                    }

                }

            }
            return lstFBCus;


        }

        public async Task<List<FacebookUserProfile>> CreateFacebookUser(Guid FBpageId)
        {
            var facebookpage = GetService<IFacebookPageService>();
            var page = facebookpage.SearchQuery(x => x.Id == FBpageId).FirstOrDefault();
            var lstFBUser = new List<FacebookUserProfile>();
            var lstCusNew = await CheckCustomerNew(FBpageId);
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
                        FbPageId = FBpageId

                    };
                    await CheckPsid(fbuser.PSID);
                    await CreateAsync(fbuser);
                    lstFBUser.Add(fbuser);


                }
            }
            else
            {
                throw new Exception($"không tìm thấy khách hàng mới từ Fanpage {page.PageName} !");
            }
            return lstFBUser;

        }

        public async Task CheckConnectPartner(Guid partnerId) {

            var result = await SearchQuery(x => x.PartnerId == partnerId).FirstOrDefaultAsync();
            if(result != null)
            {
                
                throw new Exception($"Khách hàng đã được kết nối với {result.Name}!");
            }
           
        }


        /// <summary>
        /// kết nối với khách hàng
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<FacebookUserProfile> ActionConectPartner(ConnectPartner val)
        {
            var FBCus = await SearchQuery(x => x.Id == val.FacebookUserId).FirstOrDefaultAsync();
            await CheckConnectPartner(val.PartnerId);
            if(FBCus.PartnerId == null)
            {
                FBCus.PartnerId = val.PartnerId;               
                await UpdateAsync(FBCus);
            }
            else
            {
                throw new Exception($"{FBCus.Name} Đã được kết nối với khách hàng !");
            }
           
            return FBCus;
        }


        public async Task CheckPsid(string Psid)
        {
            var lstFBUser = await SearchQuery().Where(x => x.PSID == Psid).FirstOrDefaultAsync();
            if (lstFBUser != null)
            {
                throw new Exception($"khách hàng {lstFBUser.Name} đã tồn tại !");
            }

        }


    }

    public class FacebookCustomer
    {
        [DeserializeAs(Name = "id")]
        public string PSId { get; set; }
        [DeserializeAs(Name = "name")]
        public string Name { get; set; }
        [DeserializeAs(Name = "first_name")]
        public string FirstName { get; set; }
        [DeserializeAs(Name = "last_name")]
        public string LastName { get; set; }
        [DeserializeAs(Name = "gender")]
        public string Gender { get; set; }
        // public string ProfilePic { get; set; }

    }

    public class ConnectPartner
    {
        public Guid FacebookUserId { get; set; }
        public Guid PartnerId { get; set; }
    }

}
