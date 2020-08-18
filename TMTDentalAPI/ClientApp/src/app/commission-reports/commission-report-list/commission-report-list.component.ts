import { UserPaged, UserService } from './../../users/user.service';
import { UserSimple } from './../../users/user-simple';
import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { IntlService } from '@progress/kendo-angular-intl';
import { CommissionReportsService, CommissionReport, ReportFilterCommission } from '../commission-reports.service';
import { map, debounceTime, tap, switchMap } from 'rxjs/operators';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { CompanyBasic, CompanyService, CompanyPaged } from 'src/app/companies/company.service';

@Component({
  selector: 'app-commission-report-list',
  templateUrl: './commission-report-list.component.html',
  styleUrls: ['./commission-report-list.component.css']
})
export class CommissionReportListComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  gridData: GridDataResult;
  reportResults: CommissionReport[] = [];
  limit = 20;
  skip = 0;
  dateFrom: Date;
  formGroup: FormGroup;
  dateTo: Date;
  searchUpdate = new Subject<string>();
  
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  filteredUsers: UserSimple[] = [];
  
  constructor(private commissionReportService: CommissionReportsService,
    private fb: FormBuilder,
    private intl: IntlService,private userService: UserService,) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateFrom: this.monthStart,
      dateTo: this.monthEnd,
      company: null,
      user:null
    });

    this.loadDataFromApi();

    this.loadUsers();

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });
      
  }

  loadDataFromApi() {   
    var formValue = this.formGroup.value;
    var val = new ReportFilterCommission();
    val.dateFrom = formValue.dateFrom ? this.intl.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = formValue.dateTo ? this.intl.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.companyId = formValue.company ? formValue.company.id : null;
    val.userId = formValue.user ? formValue.user.id : null;
    
    this.loading = true;
    this.commissionReportService.getReport(val).subscribe(result => {
      this.reportResults = result;       
      this.loading = false;
    }, () => {
      this.loading = false;
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

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
    });
  }

  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter || '';
    return this.userService.autocompleteSimple(val);
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
    
  }
  
}
