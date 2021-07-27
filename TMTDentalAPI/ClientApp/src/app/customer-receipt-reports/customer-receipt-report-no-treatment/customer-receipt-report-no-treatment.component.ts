import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Workbook, WorkbookSheet, WorkbookSheetColumn, WorkbookSheetRow, WorkbookSheetRowCell } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Observable, of, Subject, zip } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { CustomerReceiptReportBasic, CustomerReceiptReportFilter, CustomerReceiptReportService } from '../customer-receipt-report.service';
import { saveAs } from '@progress/kendo-file-saver';

@Component({
  selector: 'app-customer-receipt-report-no-treatment',
  templateUrl: './customer-receipt-report-no-treatment.component.html',
  styleUrls: ['./customer-receipt-report-no-treatment.component.css']
})
export class CustomerReceiptReportNoTreatmentComponent implements OnInit {
  loading = false;
  limit = 20;
  skip = 0;
  total: number;
  gridData: GridDataResult;
  listCompany: CompanySimple[] = [];
  listEmployee: EmployeeSimple[] = [];
  customerReceipts: any[] = [];
  searchUpdate = new Subject<string>();
  search: string;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  public today: Date = new Date(new Date().toDateString());
  isExamination: string;
  isNotTreatment: string;
  companyId: string;
  employeeId: string;
  isAdvanced: boolean;

  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;
  @ViewChild("employeeCbx", { static: true }) employeeCbx: ComboBoxComponent;


  constructor(
    private customerReceiptReportService: CustomerReceiptReportService,
    private intlService: IntlService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
  ) { }

  ngOnInit() {
    this.dateFrom = this.today;
    this.dateTo = this.today;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataApi();
      });

    this.loadCompanies();
    this.loadEmployees();

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap((value) => this.searchCompany(value)
      )
    )
      .subscribe((x) => {
        this.listCompany = x.items;
        this.companyCbx.loading = false;
      });

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap((value) => this.searchEmployee(value)
      )
    )
      .subscribe((rs: any[]) => {
        this.listEmployee = rs;
        this.employeeCbx.loading = false;
      });

    this.loadDataApi();

  }

  loadDataApi() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isNoTreatment = 'true';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = 'done';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getPaged(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.customerReceipts = res.data;
        this.gridData = res;
        this.total = res.total;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataApi();
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadDataApi();
  }

  onSelectEmployee(e) {
    this.employeeId = e ? e.id : null;
    this.skip = 0;
    this.loadDataApi();
  }

  loadEmployees() {
    this.searchEmployee().subscribe(res => {
      this.listEmployee = res;
    });
  }

  searchEmployee(search?) {
    var val = new EmployeePaged();
    val.search = search || '';
    val.active = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  searchCompany(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany().subscribe(res => {
      this.listCompany = res.items;
    });
  }

  toggleShow() {
    this.isAdvanced = this.isAdvanced == false ? true : false;
  }

  getExamination(value) {
    switch (value) {
      case true:
        return 'Tái khám';
      case false:
        return 'Khám mới';
    }
  }

  getMinute(value) {
    return `${value} phút`;
  }

  getState(value) {
    switch (value) {
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Chờ khám';
    }
  }

  getTreatment(value) {
    switch (value) {
      case 'true':
        return 'Không điều trị';
      case 'false':
        return 'Có điều trị';
    }
  }



  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public onExcelExport(args: any) {
    args.preventDefault();
    var data = this.customerReceipts;
    const workbook = args.workbook;
    var sheet = args.workbook.sheets[0];
    sheet.mergedCells = ["A1:G1", "A2:G2"];
    sheet.frozenRows = 3;
    sheet.name = 'BaoCaoTiepNhan_KhongDieuTri'

    sheet.rows.splice(0, 0, {
      cells: [{
        value: "BÁO CÁO TIẾP NHẬN KHÔNG ĐIỀU TRỊ",
        textAlign: "left",
        fgColor: { argb: 'FFFFAA00' },
      }], type: 'header'
    });

    sheet.rows.splice(1, 0, {
      cells: [{
        value: `Từ ngày ${this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.dateTo ? this.intlService.formatDate(this.dateTo, 'dd/MM/yyyy') : '...'}`,
        textAlign: "left"

      }], type: 'header'
    });

    const rows = workbook.sheets[0].rows;

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[2].cells[0];

    // add the detail header
    rows.splice(4, 0, {
      cells: [
        Object.assign({}, headerOptions, { value: 'Ngày tiếp nhận'}),
        Object.assign({}, headerOptions, { value: 'Khách hàng' }),
        Object.assign({}, headerOptions, { value: 'Dịch vụ' }),
        Object.assign({}, headerOptions, { value: 'Bác sĩ' }),
        Object.assign({}, headerOptions, { value: 'Giờ tiếp nhận' }),
        Object.assign({}, headerOptions, { value: 'Thời gian phục vụ' }),
        Object.assign({}, headerOptions, { value: 'Lý do không phục vụ' })
      ]
    });

    for (let idx = data.length - 1; idx >= 0; idx--) {
      var dataIndex = Object.assign({}, data[idx]);
      rows.splice(idx + 4, 0, {
        cells: [
          { value: new Date(dataIndex.dateWaiting), format: "dd/MM/yyyy" },
          { value: dataIndex.partner.name },
          { value: dataIndex.products },
          { value: dataIndex.doctorName },
          { value: new Date(dataIndex.dateWaiting), format: "HH:mm" },
          { value: dataIndex.minuteTotal },
          { value: dataIndex.reason }
        ]
      });
    }

    debugger;
    new Workbook(workbook).toDataURL().then((dataUrl: string) => {
      // https://www.telerik.com/kendo-angular-ui/components/filesaver/
      saveAs(dataUrl, `BaoCaoTiepNhan_KhongDieuTri.xlsx`);
    });

  }

}

