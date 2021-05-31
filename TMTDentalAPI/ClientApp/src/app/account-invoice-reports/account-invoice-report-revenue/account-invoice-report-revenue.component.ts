import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ExcelExportData, Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { debounce } from 'lodash';
import * as moment from 'moment';
import { from, Observable, of, zip } from 'rxjs';
import { debounceTime, delay, map, switchMap, tap } from 'rxjs/operators';
import { AccountInvoiceDisplay } from 'src/app/account-invoices/account-invoice.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { AccountInvoiceReportDetailPaged, AccountInvoiceReportDisplay, AccountInvoiceReportPaged, AccountInvoiceReportService } from '../account-invoice-report.service';
import { saveAs } from '@progress/kendo-file-saver';
import { State } from "@progress/kendo-data-query";

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

  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;

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
    this.filter.groupBy = this.filter.groupBy || 'InvoiceDate';
    this.empFilter = 'EmployeeId';

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
      case 'ProductId':
        return 'Doanh thu theo dịch vụ và thuốc';
      case 'emp-ass':
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
    this.initFilterData();
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

  public allData = (): any => {
    from([]);
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.groupBy = val.groupBy == 'emp-ass' ? this.empFilter : val.groupBy;
    val.limit = 0;
    val.search = '';

    const observable = this.accInvService.getRevenueReportPaged(val).pipe(
      map(res => {
        res.items.forEach((acc: any) => {
          acc.date = acc.invoiceDate;
          acc.invoiceDate = acc.invoiceDate ? moment(acc.invoiceDate).format('DD/MM/YYYY') : '';
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

  exportsimple() {

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

    const data = this.allDataInvoice.data;

    var val = Object.assign({}, this.filter) as AccountInvoiceReportDetailPaged;
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.limit = 0;
    val.search = '';
    val.groupBy = val.groupBy == 'emp-ass' ? this.empFilter : val.groupBy;

    // Fetch the data for all details
    for (let idx = 0; idx < data.length; idx++) {
      var dataIndex = data[idx];
      switch (this.filter.groupBy) {
        case 'InvoiceDate':
          val.date = moment(dataIndex.date).format('YYYY/MM/DD')
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
      var listDetailHeaderIndex = [];
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const lines = (<any>result[idx]).items;

        // add the detail data
        for (let productIdx = lines.length - 1; productIdx >= 0; productIdx--) {
          const line = lines[productIdx];
          rows.splice(idx + 2, 0, {
            cells: [
              {},
              { value: moment(line.invoiceDate).format('DD/MM/YYYY') },
              { value: line.invoiceOrigin },
              { value: line.partnerName },
              { value: line.employeeName || line.assistantName },
              { value: line.productName },
              { value: line.priceSubTotal.toLocaleString('vi') }
            ]
          });
        }

        // add the detail header
        listDetailHeaderIndex.push(idx + 2);
        rows.splice(idx + 2, 0, {
          cells: [
            {},
            Object.assign({}, headerOptions, { value: 'Ngày thanh toán', background: '#aabbcc', width: 20 }),
            Object.assign({}, headerOptions, { value: 'Số phiếu', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Khách hàng', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Bác sĩ/Phụ tá', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Dịch vụ', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Thanh toán', background: '#aabbcc', width: 200 })
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
      a.sheets[0].columns[5] = {
        width: 150
      };
      rows.forEach((row, index) => {
        //colspan
        if (row.type === 'header' || row.type == "footer") {
          row.cells[1].colSpan = 6;
          if (row.type === 'header') {
            rows[index + 1].cells[1].colSpan = 6;
          }
        }
        //làm màu
        if (row.type === "header") {
          row.cells.forEach((cell) => {
            cell.background = "#aabbcc";
          });
        }
      });
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, 'baocaodoanhthu.xlsx');
        this.loading = false;
      });
      // create a Workbook and save the generated data URL
      // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/Workbook/

    });

  }
}
