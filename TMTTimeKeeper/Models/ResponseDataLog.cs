using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class LogReponseData
    {
        public string userId { get; set; }
        public string verifyDate { get; set; }
        public int verifyType { get; set; }
        public int verifyState { get; set; }
        public int workCode { get; set; }
    }

    public class ResponeseDataViewModel
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public LogReponseData logReponseData { get; set; }
    }

    public class ResponseDataLogViewModel
    {
        public int isSuccess { get; set; }
        public int isError { get; set; }
        public List<ResponeseDataViewModel> responeseDataViewModel { get; set; }
    }
}
