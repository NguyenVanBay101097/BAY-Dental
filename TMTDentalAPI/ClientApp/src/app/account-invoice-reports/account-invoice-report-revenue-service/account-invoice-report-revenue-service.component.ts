import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { Observable, zip } from "rxjs";
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { saveAs } from '@progress/kendo-file-saver';
import { AccountInvoiceReportService, RevenueServiceReportPaged } from '../account-invoice-report.service';
import { RevenueManageService } from '../account-invoice-report-revenue-manage/revenue-manage.service';

@Component({
  selector: 'app-account-invoice-report-revenue-service',
  templateUrl: './account-invoice-report-revenue-service.component.html',
  styleUrls: ['./account-invoice-report-revenue-service.component.css']
})
export class AccountInvoiceReportRevenueServiceComponent implements OnInit {
  filter = new RevenueServiceReportPaged();
  companies: CompanySimple[] = [];
  listProduct: ProductSimple[] = [];
  allDataInvoice: any;
  gridData: GridDataResult;
  loading = false;

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("pro", { static: true }) productVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  constructor(
    private companyService: CompanyService,
    private accInvService: AccountInvoiceReportService,
    private revenueManageService: RevenueManageService,
    private productService: ProductService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadReport();
    this.loadCompanies();
    this.FilterCombobox();
    this.loadProducts();
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
    this.filter.limit = 20;
    this.filter.offset = 0;
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
    var val = Object.assign({}, this.filter) as RevenueServiceReportPaged;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenueServiceReportPaged(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  onSearchDateChange(e) {
    this.filter.dateFrom = e.dateFrom;
    this.filter.dateTo = e.dateTo;
    this.filter.offset = 0;
    this.filter.limit = 20;
    this.loadReport();
  }

  sumPriceSubTotal() {
    if (!this.gridData) return 0;
    return this.gridData.data.reduce((total, cur) => {
      return total + cur.priceSubTotal;
    }, 0);
  }

  onSelectCompany(e){
    this.filter.companyId = e? e.id : null;
    this.filter.offset = 0;
    this.loadReport();
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

  public allData = (): any => {
    var val = Object.assign({}, this.filter) as RevenueServiceReportPaged;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.limit = 0;

    const observable = this.accInvService.getRevenueServiceReportPaged(val).pipe(
      map(res => {
        res.items.forEach((acc: any) => {
          acc.priceSubTotal = acc.priceSubTotal.toLocaleString('vi') as any;
        });
        return {
          data: res.items,
          total: res.totalItems
        }
      })
    );

    observable.pipe(
    ).subscribe((result) => {
      this.allDataInvoice = result;
    });

    return observable;

  }
  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public onExcelExport(args: any): void {
    args.preventDefault();
    const data = this.allDataInvoice.data;
    this.revenueManageService.emitChange({
       data : data,
       args : args,
       filter : this.filter
    })
  }

  onSelectProduct(e) {
    this.filter.productId = e? e.id : null;
    this.filter.offset = 0;
    this.loadReport();
  }


}
