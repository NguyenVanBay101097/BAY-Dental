import { Component, OnInit, ViewChild } from '@angular/core';
import { CommissionSettlementsService, CommissionSettlementReport, CommissionSettlementReportOutput } from '../commission-settlements.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { EmployeeService } from 'src/app/employees/employee.service';
import { EmployeeSimple, EmployeePaged } from 'src/app/employees/employee';
import { CompanyService, CompanySimple, CompanyPaged } from 'src/app/companies/company.service';
import { Subject } from 'rxjs';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-commission-settlement-report-list',
  templateUrl: './commission-settlement-report-list.component.html',
  styleUrls: ['./commission-settlement-report-list.component.css']
})
export class CommissionSettlementReportListComponent implements OnInit {
  loading = false;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  formGroup: FormGroup;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  searchUpdate = new Subject<string>();
  reportResults: CommissionSettlementReportOutput[] = [];

  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[] = [];

  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  filteredCompanies: CompanySimple[] = [];
  
  constructor(
    private commissionSettlementReportsService: CommissionSettlementsService,
    private fb: FormBuilder,
    private intl: IntlService,
    private employeeService: EmployeeService,
    private companyService: CompanyService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateFrom: this.monthStart,
      dateTo: this.monthEnd,
      employee: null,
      company: null
    });

    this.loadEmployees();
    this.loadCompanies();

    this.loadDataFromApi();

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result;
      this.employeeCbx.loading = false;
    });

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap(value => this.searchCompanies(value))
    ).subscribe(result => {
      this.filteredCompanies = result.items;
      this.companyCbx.loading = false;
    });
  }

  getDateFrom(){
    var formValue = this.formGroup.value;
    return formValue.dateFrom ? this.intl.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
  }

  getDateTo(){
    var formValue = this.formGroup.value;
    return formValue.dateTo ? this.intl.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
  }

  loadDataFromApi() {   
    var formValue = this.formGroup.value;
    var val = new CommissionSettlementReport();
    val.dateFrom = formValue.dateFrom ? this.intl.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = formValue.dateTo ? this.intl.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.companyId = formValue.company ? formValue.company.id : null;
    val.employeeId = formValue.employee ? formValue.employee.id : null;
    
    this.loading = true;
    this.commissionSettlementReportsService.getReport(val).subscribe(result => {
      this.reportResults = result;   
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result, 'id');
    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadCompanies() {
    this.searchCompanies().subscribe(result => {
      this.filteredCompanies = _.unionBy(this.filteredCompanies, result.items, 'id');
    });
  }

  searchCompanies(filter?: string) {
    var val = new CompanyPaged();
    val.search = filter || '';
    return this.companyService.getPaged(val);
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  getFilter() {
    debugger;
    var formValue = this.formGroup.value;
    var val = new CommissionSettlementReport();
    val.dateFrom = formValue.dateFrom ? this.intl.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = formValue.dateTo ? this.intl.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.companyId = formValue.company ? formValue.company.id : null;
    val.employeeId = formValue.employee ? formValue.employee.id : null;
    return val;
  }
}
