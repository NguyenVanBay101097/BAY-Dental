using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EmployeeInfo
    {
        public Guid IdNV { get; set; }
        public string TenNV { get; set; }
        public string MaNV { get; set; }
        public decimal? Luong1Thang { get; set; }
        public int SoNgayNghi1Thang { get; set; }
        public int SoGioLam1Ngay { get; set; }
    }

    public class TimeKeepingOfEmp
    {
        public int SoNgayCong { get; set; }
        public int SoNgayNghi { get; set; }
        public int NgayNghiKhongLuong { get; set; }
        public int NgayLamThem { get; set; }
        public int SoGioTangCa { get; set; }
    }

    public class SalaryDetail
    {
        public decimal? LuongCoBan { get; set; }
        public decimal? LuongTangCa { get; set; }
        public decimal? LuongLamThem { get; set; }
        public decimal? PhuCapXacDinh { get; set; }
        public decimal? Thuong { get; set; }
        public decimal? PhuCapLeTet { get; set; }
        public decimal? HoaHong { get; set; }
        public decimal? Phat { get; set; }
        public decimal? TamUng { get; set; }
    }

    public class EmployeeSalaryPrint
    {
        public EmployeeInfo EmployeeInfo { get; set; }
        public TimeKeepingOfEmp TimeKeepingOfEmp { get; set; }
        public SalaryDetail SalaryDetail { get; set; }
    }
}
