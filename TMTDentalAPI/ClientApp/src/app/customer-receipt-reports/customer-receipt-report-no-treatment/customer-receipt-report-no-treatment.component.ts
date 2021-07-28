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


  onExcelExport() {
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
    this.customerReceiptReportService.exportExcelReportNoTreatment(val).subscribe((res: any) => {
      let filename = "BaoCaoTiepNhan_KhongDieuTri";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

}

