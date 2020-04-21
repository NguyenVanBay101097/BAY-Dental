using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class BaseHandleErrorVM
    {
        public bool success { get; set; }

        public string message { get; set; }
    }
}
