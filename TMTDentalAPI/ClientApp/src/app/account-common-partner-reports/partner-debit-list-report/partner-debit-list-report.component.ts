import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { Observable, of, Subject, zip } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { AccountCommonPartnerReportService, ReportPartnerDebitDetailReq, ReportPartnerDebitDetailRes, ReportPartnerDebitReq, ReportPartnerDebitRes } from '../account-common-partner-report.service';
import { saveAs } from '@progress/kendo-file-saver';
import * as moment from 'moment';
@Component({
  selector: 'app-partner-debit-list-report',
  templateUrl: './partner-debit-list-report.component.html',
  styleUrls: ['./partner-debit-list-report.component.css'],
   host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerDebitListReportComponent implements OnInit {

  loading = false;
  items: ReportPartnerDebitRes[] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
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

  constructor(private reportService: AccountCommonPartnerReportService,
    private intlService: IntlService,
    private companyService: CompanyService) { }

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
        this.loadDataFromApi();
      });

      this.loadDataFromApi();
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
    this.loadDataFromApi();
  }
  
  searchChangeDate(value: any) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;    
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ReportPartnerDebitReq();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.search = this.search ? this.search : '';
    val.companyId = this.companyId || '';

    this.reportService.ReportPartnerDebit(val).subscribe(res => {
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
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }

  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public onExcelExport(args: any) {
    args.preventDefault();
    var data = this.items;

    const observables = [];
    const workbook = args.workbook;
    var sheet = args.workbook.sheets[0];
    sheet.mergedCells = ["A1:G1", "A2:G2"];
    sheet.frozenRows = 3;
    sheet.name = 'BaoCaoCongNo_KH'

    sheet.rows.splice(0, 0, { cells: [{
      value:"BÁO CÁO CÔNG NỢ KHÁCH HÀNG",
      textAlign: "center"
    }], type: 'header' });

    sheet.rows.splice(1, 0, { cells: [{
      value: `Từ ngày ${this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.dateTo ? this.intlService.formatDate(this.dateTo, 'dd/MM/yyyy') : '...'}`,
      textAlign: "center"
    }], type: 'header' });


    const rows = workbook.sheets[0].rows;

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[2].cells[0];

    // Fetch the data for all details
    for (let idx = 0; idx < data.length; idx++) {
      var val = new ReportPartnerDebitDetailReq();
      var dataIndex = Object.assign({},data[idx]);
    val.fromDate = dataIndex.dateFrom ? this.intlService.formatDate(new Date(dataIndex.dateFrom), 'yyyy-MM-dd') : null;
    val.toDate = dataIndex.dateTo ? this.intlService.formatDate(new Date(dataIndex.dateTo), 'yyyy-MM-dd') : null;
    val.partnerId = dataIndex.partnerId || '';
    val.companyId = dataIndex.companyId || '';
      observables.push(this.reportService.ReportPartnerDebitDetail(val));
    }

    zip.apply(Observable, observables).subscribe((result: any[][]) => {
      // add the detail data to the generated master sheet rows
      // loop backwards in order to avoid changing the rows index
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const lines = (<any>result[idx]);

        // add the detail data
        for (let productIdx = lines.length - 1; productIdx >= 0; productIdx--) {
          const line = lines[productIdx] as ReportPartnerDebitDetailRes;
          rows.splice(idx + 4, 0, {
            cells: [
              {},
              { value: new Date(line.date), format: "dd/MM/yyyy" },
              { value: line.invoiceOrigin },
              { value: line.begin, format: "#,##0"},
              { value: line.debit, format: "#,##0"},
              { value: line.credit, format: "#,##0"},
              { value: line.end, format: "#,##0" }
            ]
          });
        }

        // add the detail header
        rows.splice(idx + 4, 0, {
          cells: [
            {},
            Object.assign({}, headerOptions, { value: 'Ngày' }),
            Object.assign({}, headerOptions, { value: 'Số phiếu' }),
            Object.assign({}, headerOptions, { value: 'Nợ đầu kì' }),
            Object.assign({}, headerOptions, { value: 'Phát sinh' }),
            Object.assign({}, headerOptions, { value: 'Thanh toán' }),
            Object.assign({}, headerOptions, { value: 'Nợ cuối kì'})
          ]
        });
      }
    
      debugger;
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, `BaoCaoCongNoKhachHang.xlsx`);
      });
      // create a Workbook and save the generated data URL
      // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/Workbook/

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

}
