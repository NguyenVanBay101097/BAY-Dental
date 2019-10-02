using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Nhóm răng: Người lớn, trẻ em
    /// </summary>
    public class ToothCategory: BaseEntity
    {
        public string Name { get; set; }

        public int? Sequence { get; set; }
    }
}
