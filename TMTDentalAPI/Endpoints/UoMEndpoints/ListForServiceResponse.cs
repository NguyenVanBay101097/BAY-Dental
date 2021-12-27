using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.UoMEndpoints
{
    public class ListForServiceResponse
    {
        public IEnumerable<UoMSimple> Uoms { get; set; } = new List<UoMSimple>();
    }
}
