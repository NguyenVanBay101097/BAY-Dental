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
import { AccountInvoiceReportService, RevenueEmployeeReportDisplay, RevenueEmployeeReportPar } from '../account-invoice-report.service';
import { RevenueManageService } from '../account-invoice-report-revenue-manage/revenue-manage.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { IntlService } from '@progress/kendo-angular-intl';


@Component({
  selector: 'app-account-invoice-report-revenue-employee',
  templateUrl: './account-invoice-report-revenue-employee.component.html',
  styleUrls: ['./account-invoice-report-revenue-employee.component.css']
})
export class AccountInvoiceReportRevenueEmployeeComponent implements OnInit {
  empFilter = 'employee';
  filter = new RevenueEmployeeReportPar();
  companies: CompanySimple[] = [];
  listEmployee: EmployeeSimple[] = [];
  allDataInvoice: any;
  allDataInvoiceExport: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  @ViewChild("emp", { static: true }) empVC: ComboBoxComponent;

  constructor(
    private companyService: CompanyService,
    private accInvService: AccountInvoiceReportService,
    private revenueManageService: RevenueManageService,
    private employeeService: EmployeeService,
    private printService: PrintService,
    private intlService: IntlService

  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadAllData();
    this.loadCompanies();
    this.FilterCombobox();
    this.loadEmployees();
  }

  loadAllData(){
    var val = Object.assign({}, this.filter) as RevenueEmployeeReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.groupById = val.groupById || '';
    this.loading = true;
    this.accInvService.getRevenueEmployeeReport(val).subscribe(res => {
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
    this.skip = 0;
    this.filter.groupBy = this.empFilter;
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
      acc.employeeName = acc.employeeName || 'Không xác định';
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
    sheet.name = 'BaoCaoDoanhThu_TheoNV';
    sheet.rows.splice(0, 1, { cells: [{
      value:"BÁO CÁO DOANH THU THEO NHÂN VIÊN",
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
       employeeFilter: this.empFilter,
       title: 'Doanh thu theo nhân viên'
    })
    rows.forEach(row => {
      if (row.type === "data"){
        row.cells[0].value = "Nhân viên: "+row.cells[0].value;
        row.cells[1].value = "Tổng doanh thu   "+row.cells[1].value;
      }
    });
  }

  
  onSelectEmployee(e) {
    this.filter.groupById = e? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  onChangeEmployeeFilter() {
    this.filter.groupBy = this.empFilter ;
    this.skip = 0;
    this.loadAllData();
  }

  printReport(){
    var val = Object.assign({}, this.filter) as RevenueEmployeeReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.groupById = val.groupById || '';
    this.accInvService.getPrintRevenueEmployeeReport(val).subscribe(result => {
      this.printService.printHtml(result);
    })
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as RevenueEmployeeReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.groupById = val.groupById || '';
    this.loading = true;
    this.accInvService.getRevenueEmployeeReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaodoanhthu_theoNV";

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
