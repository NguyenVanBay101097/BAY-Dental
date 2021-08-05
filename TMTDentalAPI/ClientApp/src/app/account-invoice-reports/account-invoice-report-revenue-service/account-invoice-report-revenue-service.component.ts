import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { Observable, of, zip } from "rxjs";
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { saveAs } from '@progress/kendo-file-saver';
import { AccountInvoiceReportService, RevenueServiceReportPar } from '../account-invoice-report.service';
import { RevenueManageService } from '../account-invoice-report-revenue-manage/revenue-manage.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { IntlService } from '@progress/kendo-angular-intl';
@Component({
  selector: 'app-account-invoice-report-revenue-service',
  templateUrl: './account-invoice-report-revenue-service.component.html',
  styleUrls: ['./account-invoice-report-revenue-service.component.css']
})
export class AccountInvoiceReportRevenueServiceComponent implements OnInit {
  filter = new RevenueServiceReportPar();
  companies: CompanySimple[] = [];
  listProduct: ProductSimple[] = [];
  allDataInvoice: any;
  allDataInvoiceExport: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;


  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("pro", { static: true }) productVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  constructor(
    private companyService: CompanyService,
    private accInvService: AccountInvoiceReportService,
    private revenueManageService: RevenueManageService,
    private productService: ProductService,
    private printService: PrintService,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadAllData();
    this.loadCompanies();
    this.FilterCombobox();
    this.loadProducts();
  }

  loadAllData(){
    var val = Object.assign({}, this.filter) as RevenueServiceReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    val.productId = val.productId || '';
    this.accInvService.getRevenueServiceReport(val).subscribe(res => {
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

      this.productVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.productVC.loading = true)),
        switchMap(value => this.searchProduct$(value)
        )
      )
      .subscribe((res) => {
        this.listProduct = res;
        this.productVC.loading = false;
      });
  }

  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.skip = 0;
  }

  searchProduct$(search?) {
    var val = new ProductFilter();
    val.limit = 20;
    val.offset = 0;
    val.type2 = 'service,medicine';
    val.search = search || '';
    return this.productService.autocomplete2(val);
  }

  loadProducts() {
    this.searchProduct$().subscribe(res => {
      this.listProduct = res;
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
    sheet.name = 'BaoCaoDoanhThu_TheoDV';
    sheet.rows.splice(0, 1, { cells: [{
      value:"BÁO CÁO DOANH THU THEO DỊCH VỤ",
      textAlign: "center"
    }], type: 'header' });

    sheet.rows.splice(1, 0, { cells: [{
      value: `Từ ngày ${this.filter.dateFrom ? this.intlService.formatDate(this.filter.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.filter.dateTo ? this.intlService.formatDate(this.filter.dateTo, 'dd/MM/yyyy') : '...'}`,
      textAlign: "center"
    }], type: 'header' });
    args.preventDefault();
    const data = this.allDataInvoiceExport.data;
    this.revenueManageService.emitChange({
       data : data,
       args : args,
       filter : this.filter,
       title: 'Doanh thu theo dịch vụ'
    })

    rows.forEach(row => {
      if (row.type === "data"){
        row.cells[0].value = "Dịch vụ & thuốc: "+row.cells[0].value;
        row.cells[1].value = "Tổng doanh thu   "+row.cells[1].value;
      }
    });
  }

  onSelectProduct(e) {
    this.filter.productId = e? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  printReport(){
    var val = Object.assign({}, this.filter) as RevenueServiceReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.accInvService.getPrintRevenueServiceReport(val).subscribe(result=>this.printService.printHtml(result));
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as RevenueServiceReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.productId = val.productId || '';
    this.loading = true;
    this.accInvService.getRevenueServiceReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaodoanhthu_theoDV";

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
