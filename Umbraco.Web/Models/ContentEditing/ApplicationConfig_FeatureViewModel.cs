using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PermissionTreeFunctionOpVM
    {
        public string Name { get; set; }

        public string Permission { get; set; }
    }

    public class PermissionTreeFunctionVM
    {
        public string Name { get; set; }

        public string Permission { get; set; }

        public List<PermissionTreeFunctionOpVM> Ops { get; set; } = new List<PermissionTreeFunctionOpVM>();
    }

    public class PermissionTreeViewModel
    {
        public string Name { get; set; }
        public List<PermissionTreeFunctionVM> Functions { get; set; } = new List<PermissionTreeFunctionVM>();
    }
}
