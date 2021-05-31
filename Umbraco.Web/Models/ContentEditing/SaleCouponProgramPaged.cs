using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponProgramPaged
    {
        public SaleCouponProgramPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public string ProgramType { get; set; }


        public bool? Active { get; set; }

        /// <summary>
        /// waiting
        /// running
        /// paused
        /// expired
        /// </summary>
        public string Status { get; set; }

        public IEnumerable<Guid> Ids { get; set; }

        public DateTime? RuleDateFromBegin { get; set; }

        public DateTime? RuleDateFromEnd { get; set; }
    }

    public class SaleCouponProgramResponse
    {
        public bool Success { get; set; }
        public SaleCouponProgramDisplay SaleCouponProgram { get; set; }
        public string Error { get; set; }
    }

    public class SaleCouponProgramGetListPagedRequest
    {
        public SaleCouponProgramGetListPagedRequest()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public string ProgramType { get; set; }

        public bool? Active { get; set; }

        /// <summary>
        /// waiting
        /// running
        /// paused
        /// expired
        /// </summary>
        public string Status { get; set; }

        public DateTime? RuleDateFromBegin { get; set; }

        public DateTime? RuleDateFromEnd { get; set; }
    }

    public class SaleCouponProgramGetListPagedResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? RuleDateFrom { get; set; }
        public DateTime? RuleDateTo { get; set; }
        public int? MaximumUseNumber { get; set; }
        public bool Active { get; set; }
        public bool IsPaused { get; set; }
        public string StatusDisplay
        {
            get
            {
                if (!Active)
                    return "Lưu nháp";
                var now = DateTime.Today;
                if (now > RuleDateTo)
                    return "Hết hạn";
                if (IsPaused)
                    return "Tạm ngừng";
                if (now >= RuleDateFrom && now <= RuleDateTo)
                    return "Đang chạy";

                return "Chưa chạy";
            }
        }
        public decimal AmountTotal { get; set; }
    }
}
