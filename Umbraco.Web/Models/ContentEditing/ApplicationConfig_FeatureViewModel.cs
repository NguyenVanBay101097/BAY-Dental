using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PermissionTreeViewModel
    {
        public string Name { get; set; }

        public string Permission { get; set; }

        public List<PermissionTreeViewModel> Children { get; set; } = new List<PermissionTreeViewModel>();

        public string Groups { get; set; }
    }
}
