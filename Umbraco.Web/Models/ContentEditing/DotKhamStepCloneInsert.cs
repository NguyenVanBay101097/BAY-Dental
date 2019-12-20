using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepCloneInsert
    {
        public Guid Id { get; set; }

        /// <summary>
        /// up: tren
        /// down: duoi
        /// </summary>
        public string CloneInsert { get; set; }
    }
}
