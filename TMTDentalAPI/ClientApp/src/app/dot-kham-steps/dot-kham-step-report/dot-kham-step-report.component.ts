import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, tap, switchMap } from 'rxjs/operators';
import { DotKhamStepService, DotKhamStepPaged } from 'src/app/dot-khams/dot-kham-step.service';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';

@Component({
  selector: 'app-dot-kham-step-report',
  templateUrl: './dot-kham-step-report.component.html',
  styleUrls: ['./dot-kham-step-report.component.css'],
  providers: [NgbDropdown]
})
export class DotKhamStepReportComponent implements OnInit {
  @ViewChild('ngbUser', { static: false }) ngbUser: NgbDropdown;
  @ViewChild('ngbPartner', { static: false }) ngbPartner: NgbDropdown;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  userSimpleFilter: EmployeeBasic[] = [];
  partnerSimpleFilter: PartnerSimple[] = [];
  formGroup: FormGroup;
  dateTo: Date;
  search: string;
  userId: string;
  partnerId: string;
  searchUpdate = new Subject<string>();

  constructor(
    private dotKhamStepService: DotKhamStepService,
    private fb: FormBuilder,
    private intl: IntlService,
    private partnerService: PartnerService,
    private userService: UserService,
    private employeeService: EmployeeService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: null,
      user: null
    })
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadData();
      });

    this.filterChangeCombobox();
    this.loadUser();
    this.loadPartner();
    this.loadData();
  }

  filterChangeCombobox() {
    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.partnerCbx.loading = true),
      switchMap(val => this.searchPartner(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.partnerSimpleFilter = rs;
        this.partnerCbx.loading = false;
      }
    )

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.userCbx.loading = true),
      switchMap(val => this.searchUsers(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.userSimpleFilter = rs.items;
        this.userCbx.loading = false;
      }
    )
  }

  loadPartner() {
    this.searchPartner().subscribe(
      result => {
        this.partnerSimpleFilter = result;
      }
    );
  }

  loadUser() {
    this.searchUsers().subscribe(
      result => {
        this.userSimpleFilter = result.items;
      }
    );
  }

  searchPartner(search?) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.customer = true;
    if (search) {
      partnerPaged.search = search.toLowerCase();
    }

    return this.partnerService.autocompletePartner(partnerPaged);
  }

  searchUsers(search?) {
    var userPaged = new EmployeePaged();
    if (search) {
      userPaged.search = search.toLowerCase();
    }
    userPaged.isDoctor = true;

    return this.employeeService.getEmployeePaged(userPaged);
  }

  loadData() {
    var val = new DotKhamStepPaged();
    val.search = this.search ? this.search : '';
    val.limit = this.limit;
    val.offset = this.skip;
    val.doctorId = this.userId ? this.userId : '';
    val.partnerId = this.partnerId || '';
    if (this.dateFrom && this.dateTo) {
      val.dateFrom = this.intl.formatDate(this.dateFrom, "yyyy-MM-ddTHH:mm");
      val.dateTo = this.intl.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    }
    this.dotKhamStepService.dotKhamStepReport(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadData();
  }


  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadData();
  }

  onChangePartner(partner) {
    this.partnerId = partner ? partner.id : '';
    this.loadData();
  }

  getResultPartner() {
    return `Khách hàng: ${this.formGroup.get('partner').value ? this.formGroup.get('partner').value.name : ''}`;
  }

  onChangeUser(user) {
    this.userId = user ? user.id : '';
    this.loadData();
  }

  getResultUser() {
    return `Bác sĩ: ${this.formGroup.get('user').value ? this.formGroup.get('user').value.name : ''}`;
  }

  removeFilterPartner(event) {
    event.stopPropagation();
    this.partnerId = '';
    this.formGroup.get('partner').setValue(null);
    this.loadData();
  }

  removeFilterUser() {
    event.stopPropagation();
    this.userId = null;
    this.formGroup.get('user').setValue(null);
    this.loadData();
  }
}
