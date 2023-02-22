using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductOnChangeUOM
    {
        public Guid? UOMId { get; set; }

        public Guid? UOMPOId { get; set; }
    }

    public class ProductOnChangeUOMResponse
    {
        public UoMBasic UOM { get; set; }

        public UoMBasic UOMPO { get; set; }
    }
}
