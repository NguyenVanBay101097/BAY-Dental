using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ApiPagedConversationsDataItem
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }

        [DeserializeAs(Name = "participants")]
        public ApiPagedConversationParticipants Participants { get; set; }
    }

    public class ApiPagedConversationParticipants
    {
        [DeserializeAs(Name = "data")]
        public List<ApiPagedConversationParticipant> Data { get; set; } = new List<ApiPagedConversationParticipant>();
    }

    public class ApiPagedConversationParticipant
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }

        [DeserializeAs(Name = "email")]
        public string Email { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }
    }
}
