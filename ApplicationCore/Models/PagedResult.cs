using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ApplicationCore.Models
{
    /// <summary>
    /// Represents a paged result for a model collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "pagedCollection", Namespace = "")]
    public class PagedResult<T>
    {
        public PagedResult(long totalItems, long pageNumber, long pageSize)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;

            if (pageSize > 0)
            {
                TotalPages = (long)Math.Ceiling(totalItems / (decimal)pageSize);
            }
            else
            {
                TotalPages = 1;
            }
        }

        [DataMember(Name = "pageNumber")]
        public long PageNumber { get; private set; }

        [DataMember(Name = "pageSize")]
        public long PageSize { get; private set; }

        [DataMember(Name = "totalPages")]
        public long TotalPages { get; private set; }

        [DataMember(Name = "totalItems")]
        public long TotalItems { get; private set; }

        [DataMember(Name = "items")]
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Calculates the skip size based on the paged parameters specified
        /// </summary>
        /// <remarks>
        /// Returns 0 if the page number or page size is zero
        /// </remarks>
        public int GetSkipSize()
        {
            if (PageNumber > 0 && PageSize > 0)
            {
                return Convert.ToInt32((PageNumber - 1) * PageSize);
            }
            return 0;
        }
    }

    /// <summary>
    /// Represents a paged result for a model collection with offset limit
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "pagedCollection", Namespace = "")]
    public class PagedResult2<T>
    {
        public PagedResult2(long totalItems, long offset, long limit)
        {
            TotalItems = totalItems;
            Offset = offset;
            Limit = limit;
        }

        [DataMember(Name = "offset")]
        public long Offset { get; private set; }

        [DataMember(Name = "limit")]
        public long Limit { get; private set; }

        [DataMember(Name = "totalItems")]
        public long TotalItems { get; private set; }

        [DataMember(Name = "items")]
        public IEnumerable<T> Items { get; set; }
    }
}
