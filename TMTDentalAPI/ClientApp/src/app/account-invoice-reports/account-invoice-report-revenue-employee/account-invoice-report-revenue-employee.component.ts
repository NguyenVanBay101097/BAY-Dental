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
import { AccountInvoiceReportService, RevenueEmployeeReportDisplay, RevenueEmployeeReportPaged, RevenueReportDetailPaged } from '../account-invoice-report.service';
import { RevenueManageService } from '../account-invoice-report-revenue-manage/revenue-manage.service';


@Component({
  selector: 'app-account-invoice-report-revenue-employee',
  templateUrl: './account-invoice-report-revenue-employee.component.html',
  styleUrls: ['./account-invoice-report-revenue-employee.component.css']
})
export class AccountInvoiceReportRevenueEmployeeComponent implements OnInit {
  empFilter = 'EmployeeId';
  filter = new RevenueEmployeeReportPaged();
  companies: CompanySimple[] = [];
  listEmployee: EmployeeSimple[] = [];
  allDataInvoice: any;
  gridData: GridDataResult;
  loading = false;

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  @ViewChild("emp", { static: true }) empVC: ComboBoxComponent;

  constructor(
    private companyService: CompanyService,
    private accInvService: AccountInvoiceReportService,
    private revenueManageService: RevenueManageService,
    private employeeService: EmployeeService,

  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadReport();
    this.loadCompanies();
    this.FilterCombobox();
    this.loadEmployees();
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

      this.empVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.empVC.loading = true)),
        switchMap((value) => this.searchEmployee$(value)
        )
      )
      .subscribe((x) => {
        this.listEmployee = x.items;
        this.empVC.loading = false;
      });

  }

  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.filter.limit = 20;
    this.filter.offset = 0;
    this.filter.employeeGroup = this.empFilter == 'EmployeeId' ? true: false;
  }

  loadEmployees() {
    this.searchEmployee$().subscribe(res => {
      this.listEmployee = res.items;
    });
  }

  searchEmployee$(search?) {
    var val = new EmployeePaged();
    val.search = search || '';
    val.active = true;
    return this.employeeService.getEmployeePaged(val);
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
    var val = Object.assign({}, this.filter) as RevenueEmployeeReportPaged;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenueEmployeeReportPaged(val).pipe(
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
    this.loadReport();
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

  public allData = (): any => {
    var val = Object.assign({}, this.filter) as RevenueEmployeeReportPaged;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.limit = 0;

    const observable = this.accInvService.getRevenueEmployeeReportPaged(val).pipe(
      map(res => {
        res.items.forEach((acc: RevenueEmployeeReportDisplay) => {
          acc.employeeName = acc.employeeName || 'Không xác định';
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
       filter : this.filter,
       employeeFilter: this.empFilter
    })
  }

  
  onSelectEmployee(e) {
    this.filter.employeeId = e? e.id : null;
    this.filter.offset = 0;
    this.loadReport();
  }

  onChangeEmployeeFilter() {
    this.filter.employeeGroup = this.empFilter == 'EmployeeId'? true: false;
    this.loadReport();
  }

}
