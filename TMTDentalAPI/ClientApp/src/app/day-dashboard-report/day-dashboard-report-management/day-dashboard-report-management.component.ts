import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { DayDashboardReportService } from '../day-dashboard-report.service';

@Component({
  selector: 'app-day-dashboard-report-management',
  templateUrl: './day-dashboard-report-management.component.html',
  styleUrls: ['./day-dashboard-report-management.component.css']
})
export class DayDashboardReportManagementComponent implements OnInit {
  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;
  listCompany: CompanySimple[] = [];
  date = new Date();
  dateFrom: any;
  dateTo: any;
  company: any;
  constructor(
    private companyService: CompanyService,
    private dayDashboardReportService: DayDashboardReportService
  ) { }

  ngOnInit() {
    this.dateFrom = this.date;
    this.dateTo = this.date;
    this.loadCompanies();
    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap((value) => this.searchCompany(value)
      )
    )
      .subscribe((x) => {
        this.listCompany = x.items;
        this.companyCbx.loading = false;
      });
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.listCompany = res.items;
    });
  }

  searchCompany(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  onSelectCompany(e) {
    this.company = e;
  }

  onChangeDate(e) {
    this.dateFrom = e;
    this.dateTo = e;
  }

  exportExcelFile() {
    let val;
    this.dayDashboardReportService.exportExcelReport(val).subscribe((res: any) => {
      let filename = "BaoCaoTongQuanNgay";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }
}
