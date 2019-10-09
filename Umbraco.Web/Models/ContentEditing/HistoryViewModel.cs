using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HistorySimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    
    public class HistoryPaged
    {
        public HistoryPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }
}
