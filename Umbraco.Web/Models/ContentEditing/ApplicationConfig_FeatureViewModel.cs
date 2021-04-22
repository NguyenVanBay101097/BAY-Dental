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

    public class PermissionTreeV2GroupViewModel
    {
        public string Name { get; set; }

        public List<PermissionTreeV2FunctionViewModel> Functions { get; set; } = new List<PermissionTreeV2FunctionViewModel>();
    }

    public class PermissionTreeV2FunctionViewModel
    {
        public string Name { get; set; }

        public List<PermissionTreeV2OpViewModel> Ops { get; set; } = new List<PermissionTreeV2OpViewModel>();
    }

    public class PermissionTreeV2OpViewModel
    {
        public string Name { get; set; }

        public string Permission { get; set; }

        public bool Expand { get; set; }

        public bool Checked { get; set; }
    }
}
