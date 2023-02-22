using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class IRSequenceSave
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int NumberNext { get; set; }

        public int Padding { get; set; }

        public int NumberIncrement { get; set; }

        public string Prefix { get; set; }

    }
}
