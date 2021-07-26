import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { CustomerReceiptReportFilter, CustomerReceiptReportService } from '../customer-receipt-report.service';

@Component({
  selector: 'app-customer-receipt-report-overview',
  templateUrl: './customer-receipt-report-overview.component.html',
  styleUrls: ['./customer-receipt-report-overview.component.css']
})
export class CustomerReceiptReportOverviewComponent implements OnInit {
  loading = false;
  limit = 20;
  skip = 0;
  total: number;
  gridData: GridDataResult;
  listCompany: CompanySimple[] = [];
  listEmployee: EmployeeSimple[] = [];
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

  // Pie
  public pieDataExamination: any[] = [];
  public pieDataNoTreatment: any[] = [];

  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;
  @ViewChild("employeeCbx", { static: true }) employeeCbx: ComboBoxComponent;

  filterExamination: any[] = [
    { value: 'true', text: 'Tái khám' },
    { value: 'false', text: 'Khám mới' },
  ];

  filterNoTreatment: any[] = [
    { value: 'true', text: 'Có điều trị' },
    { value: 'false', text: 'Không điều trị' },
  ];

  filterState: any[] = [
    { value: 'waiting', text: 'Chờ khám' },
    { value: 'examination', text: 'Đang khám' },
    { value: 'done', text: 'Hoàn thành' },
  ];

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


    this.loadDataExamination();
    this.loadDataNotreatment();
    this.loadDataApi();

  }

  loadDataApi() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = this.state || '';
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

  loadDataExamination() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = this.state || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getCountCustomerReceipt(val).subscribe(
      (res: any[]) => {
        this.pieDataExamination = [];
        this.loadExaminationItems(res);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataNotreatment() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = this.state || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getCountCustomerReceiptNoTreatment(val).subscribe(
      (res: any[]) => {
        this.pieDataNoTreatment = [];
        this.loadNoTreatmentItems(res);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadExaminationItems(items: any[]): void {
    for (let i = 0; i < items.length; i++) {
      this.pieDataExamination.push({ category: items[i].name, value: items[i].countCustomerReceipt, percentage: (items[i].countCustomerReceipt / items[i].totalCustomerReceipt * 100).toFixed(2) })
    };
  }

  loadNoTreatmentItems(items: any[]): void {
    for (let i = 0; i < items.length; i++) {
      this.pieDataNoTreatment.push({ category: items[i].name, value: items[i].countCustomerReceipt, percentage: (items[i].countCustomerReceipt / items[i].totalCustomerReceipt * 100).toFixed(2) })
    };
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

  onChangeExamination(e) {
    var value = e ? e.value : null;
    if (value) {
      this.isExamination = value;
    } else {
      this.isExamination = null;
    }
    this.skip = 0;
    this.loadDataApi();
  }

  onChangeNotTreatment(e) {
    var value = e ? e.value : null;
    if (value) {
      this.isNotTreatment = value;
    } else {
      this.isNotTreatment = null;
    }
    this.skip = 0;
    this.loadDataApi();
  }

  onChangeState(e) {
    var value = e ? e.value : null;
    if (value) {
      this.state = value;
    } else {
      this.state = null;
    }
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

}
