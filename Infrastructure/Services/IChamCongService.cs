﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IChamCongService : IBaseService<ChamCong>
    {
        Task CreateListChamcongs(IEnumerable<ChamCong> val);
        //Task<PagedResult2<EmployeeDisplay>> GetByEmployeePaged(employeePaged val);
        Task<string> GetStatus(ChamCong val);
        Task<decimal> GetStandardWorkHour();
        ChamCongTinhCong TinhSoCongGioCong(ChamCong cc, IEnumerable<AttendanceInterval> attendanceIntervals, decimal hoursPerDay);
        Task<ChamCongImportResponse> ImportExcel(PartnerImportExcelViewModel val);
        Task<ChamCong> GetLastChamCong(employeePaged val);
        Task CheckChamCong(IEnumerable<ChamCong> vals);
        Task<PagedResult2<ChamCongDisplay>> GetPaged(ChamCongPaged val);
        Task<ChamCongDefaultGetResult> DefaultGet(ChamCongDefaultGetPost val);
    }
}
