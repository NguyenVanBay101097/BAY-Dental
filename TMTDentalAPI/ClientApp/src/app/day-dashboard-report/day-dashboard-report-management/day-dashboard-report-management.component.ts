import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookService, DataInvoiceFilter } from 'src/app/cash-book/cash-book.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';
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
  dateFrom: Date;
  dateTo: Date;
  services : any[] = [];
  dataInvoices : any[] = [];
  companyId: string;

  constructor(
    private authService : AuthService,
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private companyService: CompanyService,
    private dayDashboardReportService: DayDashboardReportService,
    private saleReportService: SaleReportService,
    private router: Router
  ) { }

  ngOnInit() {
    this.dateFrom = this.date;
    this.dateTo = this.date;
    this.loadCompanies();
    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap((value) => this.searchCompany$(value)
      )
    )
      .subscribe((x) => {
        this.listCompany = x.items;
        this.companyCbx.loading = false;
      });
      
      this.loadAllData();
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

  loadAllData(){
    this.loadDataServiceApi();
    this.loadDataInvoiceApi();
  }

  loadDataServiceApi() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    val.state = 'draft';
    this.saleReportService.getReportService(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.services = res.data;    
    }, err => {
      console.log(err);
    })
  }

  loadDataInvoiceApi() {
    var gridPaged = new DataInvoiceFilter();
    gridPaged.companyId = this.authService.userInfo.companyId;
    gridPaged.resultSelection = 'all';
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;

    this.cashBookService.getDataInvoices(gridPaged).subscribe(
        (res : any) => {
          this.dataInvoices = res;
        },
        (err) => {
        }
      );
  }

  onSelectCompany(e){
    this.companyId = e ? e.id : null;
    this.loadAllData();
  }
  
  onChangeDate(value: any) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;    
    this.loadAllData();
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
