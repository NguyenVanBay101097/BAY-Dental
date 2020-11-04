using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// Năm sinh
        /// </summary>
        public int? BirthYear { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }
        public string Birth
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(BirthDay.ToString()))
                    list.Add(BirthDay.ToString());
                if (!string.IsNullOrEmpty(BirthMonth.ToString()))
                    list.Add(BirthMonth.ToString());
                if (!string.IsNullOrEmpty(BirthYear.ToString()))
                    list.Add(BirthYear.ToString());
                return string.Join("/", list);
            }
            set { }
        }
        /// <summary>
        /// Tên tỉnh/thành phố
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public string DistrictName { get; set; }


        /// <summary>
        /// Tên phường xã
        /// </summary>
        public string WardName { get; set; }
        public string Phone { get; set; }
        public string Address
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(Street))
                    list.Add(Street);
                if (!string.IsNullOrEmpty(WardName))
                    list.Add(WardName);
                if (!string.IsNullOrEmpty(DistrictName))
                    list.Add(DistrictName);
                if (!string.IsNullOrEmpty(CityName))
                    list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }
    }

    public class PartnerSimpleContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
