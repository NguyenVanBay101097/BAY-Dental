import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { WorkbookSheetRowCell } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { RevenueManageService } from '../account-invoice-report-revenue-manage/revenue-manage.service';
import { AccountInvoiceReportService, RevenuePartnerReportPar } from '../account-invoice-report.service';

@Component({
  selector: 'app-account-invoice-report-revenue-partner',
  templateUrl: './account-invoice-report-revenue-partner.component.html',
  styleUrls: ['./account-invoice-report-revenue-partner.component.css']
})
export class AccountInvoiceReportRevenuePartnerComponent implements OnInit {
  filter = new RevenuePartnerReportPar();
  companies: CompanySimple[] = [];
  allDataInvoice: any;
  allDataInvoiceExport: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  searchUpdate = new Subject<string>();


  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  constructor(
    private companyPartner: CompanyService,
    private accInvService: AccountInvoiceReportService,
    private revenueManagePartner: RevenueManageService,
    private printPartner: PrintService,
    private intlPartner: IntlService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadAllData();
    this.loadCompanies();
    this.FilterCombobox();
    this.searchChange();
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadAllData();
      });
  }


  loadAllData(){
    var val = Object.assign({}, this.filter) as RevenuePartnerReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenuePartnerReport(val).subscribe(res => {
      this.allDataInvoice = res;
      this.loading = false;
      this.loadReport();
    },
      err => {
        this.loading = false;
      });
  }

  FilterCombobox() {
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

  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.skip = 0;
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
   return  this.companyPartner.getPaged(val);
  } 

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  loadReport() {
    this.gridData = <GridDataResult>{
      total: this.allDataInvoice.length,
      data: this.allDataInvoice.slice(this.skip, this.skip + this.limit)
    };
  }


  onSearchDateChange(e) {
    this.filter.dateFrom = e.dateFrom;
    this.filter.dateTo = e.dateTo;
    this.skip = 0;
    this.loadAllData();
  }

  sumPriceSubTotal() {
    if (!this.allDataInvoice) return 0;
    return this.allDataInvoice.reduce((total, cur) => {
      return total + cur.priceSubTotal;
    }, 0);
  }

  onSelectCompany(e){
    this.filter.companyId = e? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  pageChange(e) {
    this.skip = e.skip;
    this.loadReport();
  }

  public allData = (): any => {
    var newData = [];
    this.allDataInvoice.forEach(acc => {
      var s = Object.assign({}, acc);
      newData.push(s);
    });
    newData.forEach(acc => {
      acc.priceSubTotal = acc.priceSubTotal.toLocaleString('vi') as any;
      return acc;
    });
    const observable = of(newData).pipe(
      map(res => {
        return {
          data: res,
          total: res.length
        }
      })
    );

    observable.pipe(
    ).subscribe((result) => {
      this.allDataInvoiceExport = result;
    });

    return observable;

  }
  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public onExcelExport(args: any): void {
    const observables = [];
    const workbook = args.workbook;
    var sheet = args.workbook.sheets[0];
    var rows = sheet.rows;
    sheet.mergedCells = ["A1:H1", "A2:H2"];
    sheet.frozenRows = 3;
    sheet.name = 'BaoCaoDoanhThu_TheoKH';
    sheet.rows.splice(0, 1, { cells: [{
      value:"BÁO CÁO DOANH THU THEO KHÁCH HÀNG",
      textAlign: "center"
    }], type: 'header' });

    sheet.rows.splice(1, 0, { cells: [{
      value: `Từ ngày ${this.filter.dateFrom ? this.intlPartner.formatDate(this.filter.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.filter.dateTo ? this.intlPartner.formatDate(this.filter.dateTo, 'dd/MM/yyyy') : '...'}`,
      textAlign: "center"
    }], type: 'header' });
    args.preventDefault();
    const data = this.allDataInvoiceExport.data;
    this.revenueManagePartner.emitChange({
       data : data,
       args : args,
       filter : this.filter,
       title: 'BaoCaoDoanhThu_TheoKH'
    })

    rows.forEach(row => {
      if (row.type === "data"){
        row.cells[0].value = "Khách hàng: "+row.cells[0].value;
        (row.cells[0] as WorkbookSheetRowCell).bold = true;
        (row.cells[0] as WorkbookSheetRowCell).color = "#60aed0";
        row.cells[1].value = "Tổng doanh thu   "+row.cells[1].value;
      }
    });
  }

  printReport(){
    var val = Object.assign({}, this.filter) as RevenuePartnerReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.search = val.search || '';
    this.accInvService.getPrintRevenuePartnerReport(val).subscribe(result=>this.printPartner.printHtml(result));
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as RevenuePartnerReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.search = val.search || '';
    this.loading = true;
    this.accInvService.getRevenuePartnerReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaodoanhthu_theoKH";

      let newBlob = new Blob([res], {
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
    });
  }

}
