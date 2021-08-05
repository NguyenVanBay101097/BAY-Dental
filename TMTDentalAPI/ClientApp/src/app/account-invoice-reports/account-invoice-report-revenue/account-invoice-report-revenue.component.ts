import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { from, Observable, of, zip } from "rxjs";
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { saveAs } from '@progress/kendo-file-saver';
import { AccountInvoiceReportService, RevenueTimeReportPar } from '../account-invoice-report.service';
import { RevenueManageService } from '../account-invoice-report-revenue-manage/revenue-manage.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { IntlService } from '@progress/kendo-angular-intl';
@Component({
  selector: 'app-account-invoice-report-revenue',
  templateUrl: './account-invoice-report-revenue.component.html',
  styleUrls: ['./account-invoice-report-revenue.component.css']
})
export class AccountInvoiceReportRevenueComponent implements OnInit {

  filter = new RevenueTimeReportPar();
  companies: CompanySimple[] = [];
  allDataInvoice: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;

  constructor(
    private companyService: CompanyService,
    private accInvService: AccountInvoiceReportService,
    private revenueManageService: RevenueManageService,
    private printService: PrintService,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadCompanies();
    this.FilterCombobox();
    this.loadAllData();
  }

  loadAllData() {
    var val = Object.assign({}, this.filter) as RevenueTimeReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenueTimeReport(val).subscribe(res => {
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
    return this.companyService.getPaged(val);
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

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : null;
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
      acc.date = acc.invoiceDate;
      acc.invoiceDate = acc.invoiceDate ? moment(acc.invoiceDate).format('DD/MM/YYYY') : '';
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
    );;

    observable.pipe(
    ).subscribe((result) => {
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
    sheet.name = 'BaoCaoDoanhThu_TheoTG';
    sheet.rows.splice(0, 1, { cells: [{
      value:"BÁO CÁO DOANH THU THEO THỜI GIAN",
      textAlign: "center"
    }], type: 'header' });

    sheet.rows.splice(1, 0, { cells: [{
      value: `Từ ngày ${this.filter.dateFrom ? this.intlService.formatDate(this.filter.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.filter.dateTo ? this.intlService.formatDate(this.filter.dateTo, 'dd/MM/yyyy') : '...'}`,
      textAlign: "center"
    }], type: 'header' });
    
    args.preventDefault();
    const data = this.allDataInvoice;
    this.revenueManageService.emitChange({
      data: data,
      args: args,
      filter: this.filter,
      title:'Doanh thu theo thời gian'
    })

    rows.forEach(row => {
      if (row.type === "data"){
        row.cells[0].value = "Ngày "+row.cells[0].value;
        row.cells[1].value = "Tổng doanh thu   "+row.cells[1].value;
      }
    });
    
  }

  printReport(){
    var val = Object.assign({}, this.filter) as RevenueTimeReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.accInvService.getPrintRevenueTimeReport(val).subscribe(result =>{
      this.printService.printHtml(result);
    });
    
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as RevenueTimeReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenueTimeReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaodoanhthu_theoTG";

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
