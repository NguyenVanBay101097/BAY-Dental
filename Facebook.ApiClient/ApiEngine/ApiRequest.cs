using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Enumerations.ApiEngine;
using Facebook.ApiClient.Exceptions;
using Facebook.ApiClient.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestRequest = RestSharp.RestRequest;

namespace Facebook.ApiClient.ApiEngine
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Facebook API requests
    /// </summary>
    public class ApiRequest : IApiRequest
    {
        private Stopwatch _apiTimer;

        protected bool Legacy { get; set; }

        /// <summary>
        /// Instance of RestClient 
        /// </summary>
        protected IRestClient RestClient { get; private set; }

        /// <summary>
        /// Instance of RestRequest
        /// </summary>
        protected IRestRequest RestRequest { get; private set; }

        /// <summary>
        /// <see cref="ApiEngine.ApiClient"/> to use to execute API request
        /// </summary>
        public ApiClient Client { get; protected set; }

        /// <summary>
        /// API request uri
        /// </summary>
        public string RequestUrl { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullRequestUrl
        {
            get
            {
                if (RequestUrl.StartsWith("https://graph.facebook.com"))
                {
                    return RequestUrl;
                }

                var baseUrl = Legacy ? FacebookApiRequestUrls.GRAPH_REQUEST_BASE_URL_LEGACY : FacebookApiRequestUrls.GRAPH_REQUEST_BASE_URL;
                return $"{baseUrl}/{RequestUrl}";
            }
        }


        /// <summary>
        /// Type of API request
        /// </summary>
        public enum RequestType
        {
            /// <summary>
            /// GET API request
            /// </summary>
            Get,

            /// <summary>
            /// POST API request
            /// </summary>
            Post,

            /// <summary>
            /// Paged GET API request
            /// </summary>
            Paged,
            /// <summary>
            /// All GET API request
            /// </summary>
            GetAll,
        }

        /// <summary>
        /// Initialize instance of <see cref="ApiRequest"/>
        /// </summary>
        protected ApiRequest(string requestUrl, ApiClient client, ApiRequestHttpMethod method, bool legacy = false)
        {
            Legacy = legacy;

            RequestUrl = requestUrl;
            Client = client;

            RestClient = new RestClient(legacy ? FacebookApiRequestUrls.GRAPH_REQUEST_BASE_URL_LEGACY : FacebookApiRequestUrls.GRAPH_REQUEST_BASE_URL);
            RestRequest = new RestRequest(requestUrl, (Method)method);

            SetFacebookRequestParameters();
        }

        /// <summary>
        /// Create new api request of given type
        /// </summary>
        /// <param name="type">Type of the request</param>
        /// <param name="url">Request url</param>
        /// <param name="client">Instance of <see cref="ApiClient"/> for this request</param>
        /// <param name="legacy"></param>
        /// <param name="limit"></param>
        /// <returns>Instance of API request </returns>
        public static IApiRequest Create(RequestType type, string url, ApiClient client, bool legacy = false, int limit = 50)
        {
            switch (type)
            {
                case RequestType.Get:
                    return new GetRequest(url, client, legacy);
                case RequestType.Post:
                    return new PostRequest(url, client, legacy);
                case RequestType.Paged:
                    return new PagedRequest(url, client, legacy, limit);
                case RequestType.GetAll:
                    return new AllDataRequest(url, client, legacy, limit);
            }

            throw new NotImplementedException();
        }

        #region Methods to add different request parameters

        /// <inheritdoc />
        public void AddUrlSegment(string name, string value)
        {
            RestRequest.AddUrlSegment(name, value);
        }

        /// <inheritdoc />
        public void AddHttpHeader(string name, string value)
        {
            RestRequest.AddHeader(name, value);
        }

        /// <inheritdoc />
        public void AddCookie(string name, string value)
        {
            RestRequest.AddCookie(name, value);
        }

        #endregion

        private void SetFacebookRequestParameters()
        {
            RestRequest.AddParameter(FacebookApiRequestParameters.API_VERSION, Client.Version, ParameterType.UrlSegment);
            RestRequest.AddParameter(FacebookApiRequestParameters.ACCESS_TOKEN, Client.AccessToken, ParameterType.QueryString);
        }

        /// <summary>
        /// Get instance of Newtonsoft.JsonSerializer with correct settings
        /// </summary>
        /// <returns>Object of type Newtonsoft.JsonSerializer</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected JsonSerializer GetJsonSerializer()
        {
            var jsonSerializer = new JsonSerializer
            {
                CheckAdditionalContent = true,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.Default,
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            return jsonSerializer;
        }

        /// <summary>
        /// Check if there is any api exception in received resonse. If yes then return list of api exceptions
        /// </summary>
        /// <param name="response">Received api response</param>
        /// <returns>List of api exceptions from received response</returns>
        protected static IEnumerable<FacebookOAuthException> GetExceptionsFromResponse(IRestResponse response)
        {
            IList<FacebookOAuthException> exceptions = new List<FacebookOAuthException>();

            if (response.StatusCode != HttpStatusCode.BadRequest) return exceptions;

            var responseHeaders =
                response.Headers.ToDictionary(e => e.Name);

            var parsedException = JObject.Parse(response.Content);
            var exceptionCode = parsedException["error"]?["code"] != null
                ? int.Parse(parsedException["error"]["code"].ToString(), CultureInfo.CurrentCulture)
                : 200;

            exceptions.Add(new FacebookOAuthException(exceptionCode, parsedException["error"]?["message"].ToString())
            {
                StatusCode = response.StatusCode,
                FBTraceId =
                    responseHeaders.ContainsKey(FacebookApiResponseHeaders.X_FB_TRACE_ID)
                        ? responseHeaders[FacebookApiResponseHeaders.X_FB_TRACE_ID].Value.ToString()
                        : string.Empty,
                FBRev =
                    responseHeaders.ContainsKey(FacebookApiResponseHeaders.X_FB_REV)
                        ? responseHeaders[FacebookApiResponseHeaders.X_FB_REV].Value.ToString()
                        : string.Empty,
                FBDebug =
                    responseHeaders.ContainsKey(FacebookApiResponseHeaders.X_FB_DEBUG)
                        ? responseHeaders[FacebookApiResponseHeaders.X_FB_DEBUG].Value.ToString()
                        : string.Empty,
                RawExceptionString = response.Content,
                ErrorUserMessage = parsedException["error"]?["error_user_message"]?.ToString(),
                ErrorUserTitle = parsedException["error"]?["error_user_title"]?.ToString(),
                SubCode = parsedException["error"]?["subcode"] != null
                    ? int.Parse(parsedException["error"]["subcode"].ToString())
                    : 0
            });

            return exceptions;
        }

        /// <summary>
        /// Initialize &amp; start <see cref="_apiTimer"/>
        /// </summary>
        protected void StartTimer()
        {
            if (_apiTimer == null)
                _apiTimer = new Stopwatch();

            if (_apiTimer.IsRunning)
            {
                _apiTimer.Stop();
                _apiTimer.Reset();
            }

            _apiTimer.Start();
        }

        /// <summary>
        /// Stop <see cref="_apiTimer"/>
        /// </summary>
        protected void StopTimer()
        {
            _apiTimer.Stop();
        }

        /// <inheritdoc />
        public TimeSpan GetElapsedTime()
        {
            return _apiTimer.Elapsed;
        }
    }
}
