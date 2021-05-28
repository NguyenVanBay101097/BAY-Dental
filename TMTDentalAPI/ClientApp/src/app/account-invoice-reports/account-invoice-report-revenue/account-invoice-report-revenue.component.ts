import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ExcelExportData, Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { debounce } from 'lodash';
import * as moment from 'moment';
import { from, Observable, zip } from 'rxjs';
import { debounceTime, delay, map, switchMap, tap } from 'rxjs/operators';
import { AccountInvoiceDisplay } from 'src/app/account-invoices/account-invoice.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { AccountInvoiceReportDetailPaged, AccountInvoiceReportDisplay, AccountInvoiceReportPaged, AccountInvoiceReportService } from '../account-invoice-report.service';
import { saveAs } from '@progress/kendo-file-saver';

@Component({
  selector: 'app-account-invoice-report-revenue',
  templateUrl: './account-invoice-report-revenue.component.html',
  styleUrls: ['./account-invoice-report-revenue.component.css']
})
export class AccountInvoiceReportRevenueComponent implements OnInit {

  filter = new AccountInvoiceReportPaged();
  empFilter = 'EmployeeId';
  companies: CompanySimple[] = [];
  listEmployee: EmployeeSimple[] = [];
  listProduct: ProductSimple[] = [];
  allDataInvoice: any;

  gridData: GridDataResult;
  loading = false;

  @ViewChild("emp", { static: true }) empVC: ComboBoxComponent;
  @ViewChild("pro", { static: true }) productVC: ComboBoxComponent;

  @ViewChild("grid", { static: true }) public grid: GridComponent;

  constructor(
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private productService: ProductService,
    private accInvService: AccountInvoiceReportService
  ) {

  }

  ngOnInit() {
    this.initFilterData();
    this.loadReport();
    this.loadCompanies();
    this.loadEmployees();
    this.loadProducts();

    this.FilterCombobox();
  }

  FilterCombobox() {
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
    this.filter.dateFrom = new Date(y, m, 1);
    this.filter.dateTo = new Date(y, m + 1, 0);
    this.filter.companyId = 'all';
    this.filter.limit = 20;
    this.filter.offset = 0;
    this.filter.groupBy = 'InvoiceDate';
  }

  loadCompanies() {
    var val = new CompanyPaged();
    val.active = true;
    this.companyService.getPaged(val).subscribe(res => {
      this.companies = res.items;
    });
  }

  loadReport() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.groupBy = val.groupBy == 'emp-ass' ? this.empFilter : val.groupBy;
    this.loading = true;
    this.accInvService.getRevenueReportPaged(val).pipe(
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

  searchEmployee$(search?) {
    var val = new EmployeePaged();
    val.search = search || '';
    val.active = true;
    return this.employeeService.getEmployeePaged(val);
  }

  loadEmployees() {
    this.searchEmployee$().subscribe(res => {
      this.listEmployee = res.items;
    });
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

  getTitle() {
    switch (this.filter.groupBy) {
      case 'InvoiceDate':
        return 'Doanh thu theo thời gian';
      case 'InvoiceDate':
        return 'Doanh thu theo dịch vụ và thuốc';
      case 'InvoiceDate':
        return 'Doanh thu theo nhân viên';
      default:
        return "";
    }
  }

  onSearchDateChange(e) {
    this.filter.dateFrom = e.dateFrom;
    this.filter.dateTo = e.dateTo;
    this.loadReport();
  }

  sumPriceSubTotal() {
    if (!this.gridData) return 0;
    return this.gridData.data.reduce((total, cur) => {
      return total + cur.priceSubTotal;
    }, 0);
  }

  onSelectEmployee(e) {
    if (!e) {
      this.filter.search = '';
    } else {
      this.filter.search = e.name;
    }

    this.loadReport();
  }

  onSelectProduct(e) {
    if (!e) {
      this.filter.search = '';
    } else {
      this.filter.search = e.name;
    }

    this.loadReport();
  }

  onChangeGroupBy() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = new Date(y, m, 1);
    this.filter.dateTo = new Date(y, m + 1, 0);
    this.filter.companyId = 'all';
    this.filter.limit = 20;
    this.filter.offset = 0;
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

  allData = (): any => {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.groupBy = val.groupBy == 'emp-ass' ? this.empFilter : val.groupBy;
    val.limit = 0;
    val.search = '';

    const observable = this.accInvService.getRevenueReportPaged(val);
    observable.subscribe((result) => {
      this.allDataInvoice = result;
    });

    return observable;
  }
  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public onExcelExport(args: any): void {
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    args.preventDefault();
    this.loading = true;

    const observables = [];
    const workbook = args.workbook;
    const rows = workbook.sheets[0].rows;

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[0].cells[0];

    const data = this.allDataInvoice.items;

    var val = Object.assign({}, this.filter) as AccountInvoiceReportDetailPaged;
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.limit = 0;
    val.search = '';
    // Fetch the data for all details
    for (let idx = 0; idx < data.length; idx++) {
      var dataIndex= data[idx];
      switch (this.filter.groupBy) {
        case 'InvoiceDate':
          val.date = moment(dataIndex.invoiceDate).format('YYYY/MM/DD')
          break;
        case 'ProductId':
          val.productId = dataIndex.productId;
          break;
        case 'EmployeeId':
          val.productId = dataIndex.employeeId;
          break;
        case 'AssistantId':
          val.productId = dataIndex.assistantId;
          break;
        default:
          break;
    }
      observables.push(this.accInvService.getRevenueReportDetailPaged(val));
    }

    zip.apply(Observable, observables).subscribe((result: any[][]) => {
      // add the detail data to the generated master sheet rows
      // loop backwards in order to avoid changing the rows index
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const lines = (<any>result[idx]).items;

        // add the detail data
        for (let productIdx = lines.length - 1; productIdx >= 0; productIdx--) {
          const product = lines[productIdx];
          rows.splice(idx + 2, 0, { cells: [{}, 
            { value: moment(product.invoiceDate).format('DD/MM/YYYY') },
           { value: product.invoiceOrigin },
            { value: product.partnerName },
            { value: product.employeeName || product.assistantName},
            { value: product.productName },
            { value: product.priceSubTotal.toLocaleString('vi') }
          ] });
        }

        // add the detail header
        rows.splice(idx + 2, 0, {
          cells: [
            {},
            Object.assign({}, headerOptions, { value: 'Ngày thanh toán' }),
            Object.assign({}, headerOptions, { value: 'Số phiếu' }),
            Object.assign({}, headerOptions, { value: 'Khách hàng' }),
            Object.assign({}, headerOptions, { value: 'Bác sĩ/Phụ tá' }),
            Object.assign({}, headerOptions, { value: 'Dịch vụ' }),
            Object.assign({}, headerOptions, { value: 'Thanh toán' })
          ]
        });
      }

      // create a Workbook and save the generated data URL
      // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/Workbook/
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, 'Categories.xlsx');
        this.loading = false;
      });
    });

  }
}
