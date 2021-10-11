import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AccountCommonPartnerReportService, ReportPartnerAdvance, ReportPartnerAdvanceFilter } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-advance-list-report',
  templateUrl: './partner-advance-list-report.component.html',
  styleUrls: ['./partner-advance-list-report.component.css']
})
export class PartnerAdvanceListReportComponent implements OnInit {
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  dateFrom: Date;
  dateTo: Date;
  companies: CompanySimple[] = [];
  companyId: string;
  items: ReportPartnerAdvance[] = [];

  search: string = '';
  searchUpdate = new Subject<string>();

  public aggregates: any[] = [
    { field: 'end', aggregate: 'sum' },
  ];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;

  constructor(
    private intlService: IntlService,
    private companyService: CompanyService,
    private reportService: AccountCommonPartnerReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
    this.loadCompanies();
    this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value))
      )
      .subscribe((x) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });
    this.loadCompanies();

  }

  loadDataFromApi() {
    var val = new ReportPartnerAdvanceFilter();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.search = this.search ? this.search : '';
    val.companyId = this.companyId || '';

    this.reportService.reportPartnerAdvance(val).subscribe(res => {
      this.items = res;
      this.loadItems();
    }, err => {
      console.log(err);
    })
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  searchChangeDate(value: any) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.loadDataFromApi();
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    this.loadDataFromApi();
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }

  onExcelExport(args: any) {

  }

  exportExcel() {
    var val = new ReportPartnerAdvanceFilter();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.search = this.search ? this.search : '';
    val.companyId = this.companyId || '';
    this.reportService.exportReportPartnerAdvanceExcel(val).subscribe((rs) => {
      let filename = "BaoCaoTamUng";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  printReport() {
    var val = new ReportPartnerAdvanceFilter();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.search = this.search ? this.search : '';
    val.companyId = this.companyId || '';
    this.reportService.getReportPartnerAdvancePdf(val).subscribe(result => {
      this.loading = false;
      let filename ="BaoCaoTamUng";

      let newBlob = new Blob([result], {
        type:
          "application/pdf",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

  

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems();
  }

  sum(field) : any{
    if(this.items.length == 0 ) 
    {
     return 0;
    } else {
      var res =  aggregateBy(this.items, [ { aggregate: "sum", field: field }]);
      return res[field].sum;
    }
  }
}
