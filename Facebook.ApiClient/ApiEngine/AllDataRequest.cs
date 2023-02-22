using Facebook.ApiClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.ApiClient.ApiEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class AllDataRequest : PagedRequest
    {
        internal AllDataRequest(string requestUrl, ApiClient client, bool legacy = false, int limit = 50) : base(requestUrl, client, legacy, limit)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetAllData<T>(int limit = int.MaxValue) where T : class, new()
        {
            var pagedRequest = (IPagedRequest)ApiRequest.Create(ApiRequest.RequestType.Paged, RequestUrl, Client, Legacy);

            //add paged api request limit. This will added as limit parameter to the request
            pagedRequest.AddPageLimit(base.Limit);

            //Once request is ready then execute it to do API call & fetch data
            var result = new List<T>();

            //following will do API call & fetch data & then map it to class AdAccount
            var pagedRequestResponse = pagedRequest.ExecutePage<T>();

            //check for exceptions
            var exceptions = pagedRequestResponse.GetExceptions().ToList();
            if (exceptions.Any())
            {
                Console.WriteLine("Encountered excecption while getting data from API. Details as following.");
                foreach (var exception in exceptions)
                {
                    Console.WriteLine("exception : {0}", exception.ToString());
                }

                return new List<T>();
            }

            //For paged API requests, first check if data is available or not
            if (pagedRequestResponse.IsDataAvailable() && result.Count < limit)
            {
                //if API data is available then add it to the result list.
                result.AddRange(pagedRequestResponse.GetResultData());

                //try to fetch next page data from API using helper method IsNextPageDataAvailable(). 
                //Loop continues until all pages are fetched from API
                while (pagedRequestResponse.IsNextPageDataAvailable())
                {
                    pagedRequestResponse = pagedRequestResponse.GetNextPageData(Legacy);

                    //check for exceptions
                    exceptions = pagedRequestResponse.GetExceptions().ToList();
                    if (exceptions.Any())
                    {
                        Console.WriteLine("Encountered excecption while getting data from API. Details as following.");
                        foreach (var exception in exceptions)
                        {
                            Console.WriteLine("exception : {0}", exception.ToString());
                        }

                        return new List<T>();
                    }

                    result.AddRange(pagedRequestResponse.GetResultData());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="enableException"></param>
        /// <returns></returns>
        public IList<T> GetAllData<T>(int limit, bool enableException = false) where T : class, new()
        {
            var pagedRequest = (IPagedRequest)ApiRequest.Create(ApiRequest.RequestType.Paged, RequestUrl, Client, Legacy);

            //add paged api request limit. This will added as limit parameter to the request
            pagedRequest.AddPageLimit(base.Limit);

            //Once request is ready then execute it to do API call & fetch data
            var result = new List<T>();

            //following will do API call & fetch data & then map it to class AdAccount
            var pagedRequestResponse = pagedRequest.ExecutePage<T>();

            //check for exceptions
            var exceptions = pagedRequestResponse.GetExceptions().ToList();
            if (exceptions.Any())
            {
                if (enableException)
                {
                    throw exceptions.FirstOrDefault();
                }

                //Console.WriteLine("Encountered excecption while getting data from API. Details as following.");
                //foreach (var exception in exceptions)
                //{
                //    Console.WriteLine("exception : {0}", exception.ToString());
                //}

                return new List<T>();
            }

            //For paged API requests, first check if data is available or not
            if (pagedRequestResponse.IsDataAvailable() && result.Count < limit)
            {
                //if API data is available then add it to the result list.
                result.AddRange(pagedRequestResponse.GetResultData());

                //try to fetch next page data from API using helper method IsNextPageDataAvailable(). 
                //Loop continues until all pages are fetched from API
                while (pagedRequestResponse.IsNextPageDataAvailable() || pagedRequestResponse.IsNextPageCursorAvailable())
                {
                    pagedRequestResponse = pagedRequestResponse.GetNextPageData(FullRequestUrl, base.Limit, Legacy);

                    //check for exceptions
                    exceptions = pagedRequestResponse.GetExceptions().ToList();
                    if (exceptions.Any())
                    {
                        Console.WriteLine("Encountered excecption while getting data from API. Details as following.");
                        foreach (var exception in exceptions)
                        {
                            Console.WriteLine("exception : {0}", exception.ToString());
                        }

                        return new List<T>();
                    }

                    result.AddRange(pagedRequestResponse.GetResultData());

                    if (pagedRequestResponse.GetResultData().Count() < base.Limit)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
