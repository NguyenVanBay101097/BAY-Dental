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

  public allData = (): any => {

    var newData = [];
    this.items.forEach(acc => {
      var s = Object.assign({}, acc);
      newData.push(s);
    });
    newData.forEach((acc: ReportPartnerDebitRes) => {
      acc.begin = acc.begin.toLocaleString('vi') as any;
      acc.end = acc.end.toLocaleString('vi') as any;
      acc.debit = acc.debit.toLocaleString('vi') as any;
      acc.credit = acc.credit.toLocaleString('vi') as any;
      return acc;
    });
    const observable = of(newData).pipe(
      map(res => {
        return {
          data: res,
          total: res.length
        }
      })
    );;

    observable.pipe(
    ).subscribe((result) => {
    });

    return observable;

  }

  public onExcelExport(args: any) {
    args.preventDefault();
    var data = this.items;

    const observables = [];
    const workbook = args.workbook;

    const rows = workbook.sheets[0].rows;

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[0].cells[0];
    rows.forEach((row, index) => {
      if (row.type === "data") {
        row.cells[3].textAlign = 'right';
        row.cells[4].textAlign = 'right';
        row.cells[5].textAlign = 'right';
        row.cells[6].textAlign = 'right';
      }
    });

    if (data.length == 0) {
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, 'BaoCaoCongNoKhachHang.xlsx');
      });
    }

    
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
      var listDetailHeaderIndex = [];
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const lines = (<any>result[idx]);

        // add the detail data
        for (let productIdx = lines.length - 1; productIdx >= 0; productIdx--) {
          const line = lines[productIdx] as ReportPartnerDebitDetailRes;
          rows.splice(idx + 2, 0, {
            cells: [
              {},
              { value: moment(line.date).format('DD/MM/YYYY') },
              { value: line.invoiceOrigin },
              { value:  line.begin.toLocaleString('vi'), textAlign: 'right'},
              { value:  line.debit.toLocaleString('vi'), textAlign: 'right'},
              { value: line.credit.toLocaleString('vi'), textAlign: 'right'},
              { value: line.end.toLocaleString('vi'), textAlign: 'right' }
            ]
          });
        }

        // add the detail header
        listDetailHeaderIndex.push(idx + 2);
        rows.splice(idx + 2, 0, {
          cells: [
            {},
            Object.assign({}, headerOptions, { value: 'Ngày', background: '#aabbcc', width: 20 }),
            Object.assign({}, headerOptions, { value: 'Số phiếu', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Nợ đầu kì', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Phát sinh', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Thanh toán', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Nợ cuối kì', background: '#aabbcc', width: 200 })
          ]
        });
      }
      var a = workbook;
      delete a.sheets[0].columns[1].width;
      a.sheets[0].columns[1].autoWidth = true;
      a.sheets[0].columns[2] = {
        width: 120
      };
      a.sheets[0].columns[3] = {
        width: 200
      };
      a.sheets[0].columns[4] = {
        width: 200
      };
      a.sheets[0].columns[5] = {
        width: 200
      };
      a.sheets[0].columns[6] = {
        width: 150
      };
    
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
