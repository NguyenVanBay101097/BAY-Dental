using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToothViewModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string ViTriHam { get; set; }
        public string Position { get; set; }

    }
}
