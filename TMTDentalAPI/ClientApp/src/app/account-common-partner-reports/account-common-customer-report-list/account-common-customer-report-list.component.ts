import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';
import { AccountCommonPartnerReportItem, AccountCommonPartnerReportSearch, AccountCommonPartnerReportService } from '../account-common-partner-report.service';

@Component({
  selector: 'app-account-common-customer-report-list',
  templateUrl: './account-common-customer-report-list.component.html',
  styleUrls: ['./account-common-customer-report-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class AccountCommonCustomerReportListComponent implements OnInit {

  loading = false;
  items: AccountCommonPartnerReportItem[] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0; 
  pagerSettings: any;
  dateFrom: Date;
  dateTo: Date;
  resultSelection: string;
  public total: any;
  companies: CompanySimple[] = [];
  companyId: string;

  search: string;
  searchUpdate = new Subject<string>();
  public aggregates: any[] = [
    { field: 'end', aggregate: 'sum' },
  ];

  constructor(private reportService: AccountCommonPartnerReportService, private intlService: IntlService,
    private route: ActivatedRoute,
    private companyService: CompanyService,
    private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

      this.route.queryParamMap.subscribe(params => {
        this.resultSelection = params.get('result_selection');
        this.loadDataFromApi();
      });
      this.loadCompanies();
      this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });

  }

 
  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
   return  this.companyService.getPaged(val);
  } 

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  onSelectCompany(e){
    this.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadDataFromApi();
  }
  
  searchChangeDate(value: any) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  getParamReport() {
    var val = new AccountCommonPartnerReportSearch();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.partnerId = null;
    val.search = this.search ? this.search : '';
    val.resultSelection = this.resultSelection;
    val.companyId = this.companyId || '';
    val.display = "";
    return val;
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountCommonPartnerReportSearch();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.partnerId = null;
    val.search = this.search ? this.search : '';
    val.resultSelection = this.resultSelection;
    val.companyId = this.companyId || '';
    val.display = "";

    this.reportService.getSummary(val).subscribe(res => {
      this.items = res;
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems(); 
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }

  onExport() {
    var val = new AccountCommonPartnerReportSearch();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.partnerId = null;
    val.search = this.search ? this.search : '';
    val.resultSelection = this.resultSelection;
    val.companyId = this.companyId || '';
    val.display = "";

    this.reportService.exportExcelFile(val).subscribe((res: any) => {
      const filename = this.resultSelection == 'customer'? `BaoCaoCongNoKhachHang` : 'BaoCaoCongNoNCC';
      const newBlob = new Blob([res], {
        type:
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });

      const data = window.URL.createObjectURL(newBlob);
      const link = document.createElement('a');
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    });
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

  onExportPDF(){
    var val = this.getParamReport();
    this.reportService.getSummaryPdf(val).subscribe(result => {
      this.loading = false;
      let filename ="BaoCaoCongNo_KH";

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

  printReport(){
    var val = this.getParamReport();
    this.reportService.printGetSummary(val).subscribe(result => this.printService.printHtml(result));
  }

}
