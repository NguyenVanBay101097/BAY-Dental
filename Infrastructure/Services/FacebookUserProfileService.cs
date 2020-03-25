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
            var userService = GetService<UserManager<ApplicationUser>>();
            var user = await userService.FindByIdAsync(UserId);
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.PSID.Contains(val.Search));

            if (user.FacebookPageId.HasValue)
            {
                query = query.Where(x => x.FbPageId == user.FacebookPageId);
            }

            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookUserProfileBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<FacebookUserProfileBasic>>(items)
            };
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

        public override async Task<IEnumerable<FacebookUserProfile>> CreateAsync(IEnumerable<FacebookUserProfile> entities)
        {
            var psids = entities.Select(x => x.PSID);
            var exists = await SearchQuery(x => psids.Contains(x.PSID)).Select(x => x.PSID).ToListAsync();
            entities = entities.Where(x => !exists.Contains(x.PSID));
            return await base.CreateAsync(entities);
        }

        public async Task ActionRemovePartner(IEnumerable<Guid> ids)
        {
            var FBCus = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            if (FBCus == null) {
                throw new Exception("Chưa được kết nối với khách hàng !");
            }
            foreach (var cus in FBCus)
            {
                cus.PartnerId = null;
                await UpdateAsync(cus);
            }        
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
