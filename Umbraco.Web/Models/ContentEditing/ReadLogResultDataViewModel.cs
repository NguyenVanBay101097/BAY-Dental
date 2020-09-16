using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ReadLogResultDataViewModel
    {
        public string UserId { get; set; }

        public string VerifyDate { get; set; }

        public int VerifyType { get; set; }

        public int VerifyState { get; set; }

        public int WorkCode { get; set; }
    }

    public class ResultSyncDataViewModel
    {
        public int? IsSuccess { get; set; }
        public int? IsError { get; set; }

        public ICollection<ResponeseDataViewModel> ResponeseDataViewModel { get; set; } = new List<ResponeseDataViewModel>();
    }

    public class ResponeseDataViewModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ReadLogResultDataViewModel LogReponseData { get; set; }
    }
}
