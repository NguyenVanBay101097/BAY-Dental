using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Entities.ApiEngine;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookConfigPageService : BaseService<FacebookConfigPage>, IFacebookConfigPageService
    {
        public FacebookConfigPageService(IAsyncRepository<FacebookConfigPage> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task SyncData(Guid id)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();

            var self = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var page_access_token = self.PageAccessToken;
            var apiClient = new ApiClient(page_access_token, FacebookApiVersions.V6_0);
            var page_id = self.PageId;
            var url = $"/{page_id}";

            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            getRequest.AddQueryParameter("fields", "conversations{message_count,id,snippet,unread_count,participants}");


            var response = getRequest.Execute<SyncAllUserCanChatWithPageResponse>();
            if (response.GetExceptions().Any())
            {
                var errorMessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMessage);
            }
            else
            {
                var result = response.GetResult();
                var cvss = new List<FacebookConversation>();
                if (result.conversations != null && result.conversations.data != null)
                {
                    foreach (var conversation in result.conversations.data)
                    {
                        var user = conversation.participants.data.Where(x => x.id != self.PageId).FirstOrDefault();
                        if (user == null)
                            continue;

                        var u = await userProfileObj.SearchQuery(x => x.PsId == user.id && x.PageId == self.PageId).FirstOrDefaultAsync();
                        if (u == null)
                        {
                            u = new FacebookUserProfile()
                            {
                                PsId = user.id,
                                Name = user.name,
                                PageId = self.PageId
                            };
                            await userProfileObj.CreateAsync(u);
                        }

                        var cvs = new FacebookConversation()
                        {
                            FacebookObjectId = conversation.id,
                            FacebookPageId = self.PageId,
                            User = u,
                            MessageCount = conversation.message_count,
                            Snippet = conversation.snippet,
                            UnreadCount = conversation.unread_count,
                        };
                        cvss.Add(cvs);
                    }
                }

                var conversationObj = GetService<IFacebookConversationService>();
                await conversationObj.CreateAsync(cvss);
            }
        }

        public async Task SyncAllUserCanChatWithPage(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var page_access_token = self.PageAccessToken;
            var apiClient = new ApiClient(page_access_token, FacebookApiVersions.V6_0);
            var page_id = self.PageId;
            var url = $"/{page_id}";

            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            getRequest.AddQueryParameter("fields", "conversations{message_count, participants}");

            var response = getRequest.Execute<SyncAllUserCanChatWithPageResponse>();
            if (response.GetExceptions().Any())
            {
                var errorMessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMessage);
            }
            else
            {
                var result = response.GetResult();
            }
        }

        public class SyncAllUserCanChatWithPageResponse
        {
            public string id { get; set; }
            public SyncAllUserCanChatWithPageConversation conversations { get; set; }
        }

        public class SyncAllUserCanChatWithPageConversation
        {
            public IEnumerable<SyncAllUserCanChatWithPageConversationData> data { get; set; }
        }

        public class SyncAllUserCanChatWithPageConversationData
        {
            public SyncAllUserCanChatWithPageConversationDataParticipant participants { get; set; }
            public string id { get; set; }
            public int message_count { get; set; }
            public string snippet { get; set; }
            public int unread_count { get; set; }
        }

        public class SyncAllUserCanChatWithPageConversationDataParticipant
        {
            public IEnumerable<SyncAllUserCanChatWithPageConversationDataParticipantData> data { get; set; }
        }

        public class SyncAllUserCanChatWithPageConversationDataParticipantData
        {
            public string name { get; set; }
            public string email { get; set; }
            public string id { get; set; }
        }
    }
}
