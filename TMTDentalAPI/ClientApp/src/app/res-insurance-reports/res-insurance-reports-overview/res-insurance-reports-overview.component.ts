import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';
import { InsuranceReportFilter } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-reports-overview',
  templateUrl: './res-insurance-reports-overview.component.html',
  styleUrls: ['./res-insurance-reports-overview.component.css']
})
export class ResInsuranceReportsOverviewComponent implements OnInit, AfterViewInit {
  @ViewChild('companyCbx', { static: false }) companyCbx: ComboBoxComponent;
  allDataGrid: any;
  gridData: GridDataResult;
  limit: number = 20;
  offset: number = 0;
  pagerSettings: any;
  dateFrom: Date;
  dateTo: Date;
  companyId: string;
  search: string;
  searchUpdate = new Subject<string>();
  filteredCompanies: any[];
  sumBegin: number = 0;
  sumEnd: number = 0;
  sumDebit: number = 0;
  sumCredit: number = 0;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private companyService: CompanyService,
    private resInsuranceReportService: ResInsuranceReportService,
    private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadCompany();
    this.loadDataFromApi();
    this.onSearchUpdate();
  }

  ngAfterViewInit(): void {
    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.companyCbx.loading = true),
      switchMap(val => this.searchCompany(val.toString().toLowerCase()))
    ).subscribe(rs => {
      this.filteredCompanies = rs.items;
      this.companyCbx.loading = false;
    });
  }

  onSearchUpdate(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    let val = new InsuranceReportFilter();
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    val.search = this.search || '';
    val.companyId = this.companyId || '';
    this.resInsuranceReportService.getSummaryReports(val).subscribe(res => {
      this.allDataGrid = res;
      this.loadReport();
    }, (error) => console.log(error))
  }

  loadReport() {
    this.gridData = <GridDataResult>{
      total: this.allDataGrid.length,
      data: this.allDataGrid.slice(this.offset, this.offset + this.limit)
    };

    const result = aggregateBy(this.allDataGrid, [
      { aggregate: "sum", field: "begin" },
      { aggregate: "sum", field: "credit" },
      { aggregate: "sum", field: "debit" },
      { aggregate: "sum", field: "end" },
    ]);

    this.sumBegin = result.begin ? result.begin.sum : 0;
    this.sumEnd = result.end ? result.end.sum : 0;
    this.sumDebit = result.debit ? result.debit.sum : 0;
    this.sumCredit = result.credit ? result.credit.sum : 0;
  }

  searchCompany(search?: string) {
    var params = new CompanyPaged();
    params.limit = 20;
    params.offset = 0;
    params.search = search || '';
    params.active = true;
    return this.companyService.getPaged(params);
  }

  loadCompany(): void {
    this.searchCompany().subscribe(result => {
      this.filteredCompanies = result.items;
    });
  }

  onPageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.limit = event.take;
    this.loadReport();
  }

  changeCompany(e): void {
    this.companyId = e ? e : '';
    this.loadDataFromApi();
  }

  onSearchDateChange(e): void {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
    this.loadDataFromApi();
  }

  exportExcel() {
    let val;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    // this.accInvService.exportRevenueTimeReportExcel(val).subscribe((rs) => {
    //   let filename = "BaoCaoCongNoBaoHiem";
    //   let newBlob = new Blob([rs], {
    //     type:
    //       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //   });

    //   let data = window.URL.createObjectURL(newBlob);
    //   let link = document.createElement("a");
    //   link.href = data;
    //   link.download = filename;
    //   link.click();
    //   setTimeout(() => {
    //     // For Firefox it is necessary to delay revoking the ObjectURL
    //     window.URL.revokeObjectURL(data);
    //   }, 100);
    // });
  }

  getFilter(){
    let val = new InsuranceReportFilter();
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    val.search = this.search || '';
    val.companyId = this.companyId || '';
    return val;
  }

  printReport(){
    var val = this.getFilter();
    this.resInsuranceReportService.printGetSummary(val).subscribe(result => 
      this.printService.printHtml(result));
  }

  onExportPDF(){
    var val = this.getFilter();
    this.resInsuranceReportService.getSummaryPdf(val).subscribe(result => {
      let filename ="BaoCaoCongNo_BH";

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
}
