using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AdvisorySimple
    {
        public Guid Id { get; set; }
        public ApplicationUserSimple User { get; set; }
    }
}
