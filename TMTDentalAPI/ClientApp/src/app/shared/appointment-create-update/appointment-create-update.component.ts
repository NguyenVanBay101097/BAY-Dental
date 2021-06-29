import { UserPaged, UserService } from './../../users/user.service';
import { UserCuDialogComponent } from './../../users/user-cu-dialog/user-cu-dialog.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { PartnerBasic, PartnerDisplay, PartnerSimple, PartnerPaged, PartnerCategorySimple, PartnerSimpleInfo } from 'src/app/partners/partner-simple';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';
import { EmployeePaged, EmployeeSimple, EmployeeBasic } from 'src/app/employees/employee';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { EmployeeService } from 'src/app/employees/employee.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerSearchDialogComponent } from 'src/app/partners/partner-search-dialog/partner-search-dialog.component';
import { Router } from '@angular/router';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { UserSimple } from 'src/app/users/user-simple';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnersService } from '../services/partners.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { NotifyService } from '../services/notify.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-appointment-create-update',
  templateUrl: './appointment-create-update.component.html',
  styleUrls: ['./appointment-create-update.component.css']
})

export class AppointmentCreateUpdateComponent implements OnInit {
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('serviceMultiSelect', { static: true }) serviceMultiSelect: MultiSelectComponent
  customerSimpleFilter: PartnerSimple[] = [];
  userSimpleFilter: UserSimple[] = [];
  filteredServices: ProductSimple[] = [];
  filteredEmployees: EmployeeBasic[] = [];
  appointId: string;
  type: string = "create";
  timeExpecteds: any[] = [
    {
      name: '0 phút', value: 0
    },
    {
      name: '30 phút', value: 30
    },
    {
      name: '45 phút', value: 45
    },
    {
      name: '60 phút', value: 60
    },
    {
      name: '75 phút', value: 75
    },
    {
      name: '90 phút', value: 90
    },
    {
      name: '105 phút', value: 105
    },
    {
      name: '120 phút', value: 120
    },
  ]
  states: any[] = [
    {name: 'Đang hẹn', value:'confirmed'},
    {name: 'Chờ khám', value:'waiting'},
    {name: 'Đang khám', value:'examination'},
    {name: 'Hoàn thành', value:'done'},
    {name: 'Hủy hẹn', value:'cancel'},
    {name: 'Đã đến', value:'arrived'}
  ]
  statesReceive: any[] = [
    {name: 'Chờ khám', value:'waiting'},
    {name: 'Đang khám', value:'examination'},
    {name: 'Hoàn thành', value:'done'}
  ]
  defaultVal: any;
  formGroup: FormGroup;
  dotKhamId: any;
  public steps: any = { hour: 1, minute: 30 };
  hourList: number[] = [];
  minuteList: number[] = [0, 30];
  timeList: string[] = [];
  timeSource: string[] = [];

  submitted = false;
  private btnDeleteSubject = new Subject<any>();

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private partnerService: PartnerService,
    private intlService: IntlService,
    private userService: UserService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private errorService: AppSharedShowErrorService,
    private odataPartnerService: PartnersService,
    private productService: ProductService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService
   ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      partnerAge: null,
      partnerPhone: null,
      partnerTags: this.fb.array([]),
      user: [null],
      apptDate: [null, Validators.required],
      appTime: ['00:00',Validators.required],
      note: null,
      companyId: null,
      doctor: null,
      timeExpected: '30',
      state: 'confirmed',
      reason: null,
      saleOrderId: null,
      services: [],
      isRepeatCustomer:false
    })

    setTimeout(() => {
      this.hourList = _.range(0, 24);
      // this.minuteList = _.range(0, 60, 5);
      this.timeSource = this.TimeInit();
      this.timeList = this.timeSource;

      if (this.appointId) {
        this.loadAppointmentToForm();
      } else {
        this.defaultGet();
      }

      this.loadEmployees();
      this.getCustomerList();
      this.filterChangeCombobox();
      this.filterChangeMultiselect();
      this.loadService();
    });
  }

  filterChangeMultiselect() {
    this.serviceMultiSelect.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.serviceMultiSelect.loading = true)),
        switchMap((value) => this.searchService(value))
      )
      .subscribe((result) => {
        this.filteredServices = result;
        this.serviceMultiSelect.loading = false;
      });
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result.items, 'id');
    });
  }

  loadService() {
    this.searchService().subscribe(
      result => {
        this.filteredServices = result;
      }
    )
  }

  loadAppointmentToFormByType(){
    if (this.appointId && this.type == 'receive'){
      this.f.name.disable();
      this.f.doctor.setValidators(Validators.required);
      this.f.doctor.updateValueAndValidity();
    }
    if (this.appointId && this.type == 'receive_create'){
      this.f.doctor.setValidators(Validators.required);
      this.f.doctor.updateValueAndValidity();
    }
    if (this.appointId && this.type == 'receive_update'){
      this.f.doctor.disable();
      this.f.name.disable();
      this.f.appTime.disable();
    }
  }

  searchService(q?: string) {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0
    val.search = q || '';
    val.type = "service";
    return this.productService.autocomplete2(val);
  }

  TimeInit() {
    var times = new Array();; // time array
    var tt = 0; // start time
    var x = 30; //minutes interval
    //var ap = ['AM', 'PM']; // AM-PM

    for (var i = 0; tt < 24 * 60; i++) {
      var hh = Math.floor(tt / 60); // getting hours of day in 0-24 format
      var mm = (tt % 60); // getting minutes of the hour in 0-55 format
      times[i] = ("0" + hh).slice(-2) + ':' + ("0" + mm).slice(-2);
      tt = tt + x;
    }
    return times;
  }




  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
    return this.employeeService.getEmployeePaged(val);
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var appoint = this.formGroup.value;
    appoint.partnerId = appoint.partner ? appoint.partner.id : null;
    appoint.doctorId = appoint.doctor ? appoint.doctor.id : null;
    var apptDate = this.intlService.formatDate(appoint.apptDate, 'yyyy-MM-dd');
    var appTime = appoint.appTime;
    appoint.date = `${apptDate}T00:00:00`;
    appoint.time = appTime;
    appoint.timeExpected = Number.parseInt(appoint.timeExpected);

    
    if (this.state != 'cancel') {
      appoint.reason = null;
    }

    if (this.appointId) {
      if(this.type == 'receive'){
        appoint.state = 'arrived';
      }
      if(this.type == 'receive_update'){

      }
      this.appointmentService.update(this.appointId, appoint).subscribe(
        () => {
          appoint.id = this.appointId;
          this.activeModal.close(appoint);
        },
        er => {
          this.errorService.show(er);
          this.submitted = false;
        },
      )
    } else {
      if(this.type == 'receive_create'){
        appoint.state = 'waiting';
      }
      else {
        appoint.state = 'confirmed';
      }
      this.appointmentService.create(appoint).subscribe(
        res => {
          this.activeModal.close(res);
        },
        er => {
          this.errorService.show(er);
          this.submitted = false;
        },
      )
    }
  }

  onDelete(){
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa lịch hẹn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.appointmentService.removeAppointment(this.appointId).subscribe(()=>{
        this.notify("success","Xóa thành công");
        this.activeModal.close(true);
        this.btnDeleteSubject.next();
      })
      
    });
  }

  onDuplicate(){
    this.appointId = null;
  }

  onChange(value){
    if(this.appointId && this.type == 'create'){
      if (value == 'cancel'){
        this.f.reason.setValidators(Validators.required);
        this.f.reason.updateValueAndValidity();
      }
      else{
        this.f.reason.clearValidators();
        this.f.reason.updateValueAndValidity();
      }
    }
    if(this.appointId && this.type == 'receive_update'){

    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  searchCustomerDialog() {
    let modalRef = this.modalService.open(PartnerSearchDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tìm khách hàng';
    modalRef.componentInstance.domain = { customer: true };

    modalRef.result.then(result => {
      if (result.length) {
        var p = result[0].dataItem;
        this.formGroup.get('partner').patchValue(p);
        this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [p], 'id');
      }
    }, () => {
    });
  }

  createDoctorDialog() {
    let modalRef = this.modalService.open(UserCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bác sĩ';

    modalRef.result.then(result => {
      var p = new UserSimple();
      p.id = result.id;
      p.name = result.name;
      this.formGroup.get('user').patchValue(p);
      this.userSimpleFilter = _.unionBy(this.userSimpleFilter, [p], 'id');
    }, () => {
    });
  }

  getCustomerList() {
    var partnerPaged = new PartnerPaged();
    partnerPaged.employee = false;
    partnerPaged.customer = true;
    partnerPaged.supplier = false;
    partnerPaged.limit = 10;
    partnerPaged.offset = 0;
    this.partnerService.autocompletePartner(partnerPaged).subscribe(
      rs => {
        this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, rs, 'id');
      }
    )
  }

  filterChangeCombobox() {
    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.partnerCbx.loading = true),
      switchMap(val => this.searchCustomers(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.customerSimpleFilter = rs;
        this.partnerCbx.loading = false;
      }
    )

    this.doctorCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.doctorCbx.loading = true),
      switchMap(val => this.searchEmployees(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.filteredEmployees = rs.items;
        this.doctorCbx.loading = false;
      }
    )
  }

  searchCustomers(search) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.customer = true;
    if (search) {
      partnerPaged.search = search.toLowerCase();
    }

    return this.partnerService.autocompletePartner(partnerPaged);
  }

  searchUsers(search) {
    var userPaged = new UserPaged();
    if (search) {
      userPaged.search = search.toLowerCase();
    }
    return this.userService.autocompleteSimple(userPaged);
  }

  getUserList() {
    var userPn = new UserPaged;
    this.userService.autocompleteSimple(userPn).subscribe(
      rs => {
        this.userSimpleFilter = rs;
      });
  }

  getBtnDeleteObs() {
    return this.btnDeleteSubject.asObservable();
  }

  loadAppointmentToForm() {
    if (this.appointId != null) {
      this.appointmentService.get(this.appointId).subscribe(
        (rs: any) => {
          this.formGroup.patchValue(rs);
          let date = new Date(rs.date);

          this.formGroup.get('apptDate').patchValue(date);
          this.formGroup.get('appTime').patchValue(rs.time);
          // this.formGroup.get('apptHour').patchValue(date.getHours());
          // this.formGroup.get('apptMinute').patchValue(date.getMinutes());

          var appoint = this.formGroup.value;
          //console.log(appoint);

          if (rs.partner) {
            this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
            this.onChangePartner();
          }

          if (rs.doctor) {
            this.filteredEmployees = _.unionBy(this.filteredEmployees, [rs.doctor], 'id');
          }
        },
        er => {
          console.log(er);
        }
      )
    }

  }

  get partner() {
    return this.formGroup.get('partner').value;
  }

  get partnerAge() {
    return this.formGroup.get('partnerAge').value;
  }

  get partnerPhone() {
    return this.formGroup.get('partnerPhone').value;
  }

  get state() {
    return this.formGroup.get('state').value;
  }

  updateCustomerModal() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khách hàng';
    modalRef.componentInstance.id = this.partner.id;

    modalRef.result.then(() => {
    }, () => {
    });
  }

  quickCreateCustomerModal() {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';

    modalRef.result.then(result => {
      if (result && result.id) {
        var newPartner = new PartnerSimple();
        newPartner.id = result.id;
        newPartner.name = result.name;
        newPartner.displayName = result.displayName;
        this.customerSimpleFilter.push(newPartner);
        this.formGroup.get('partner').setValue(newPartner);
        this.onChangePartner();
      }
    })
  }

  onChangePartner() {
    if (this.partner) {
      var expand: any = {
        $expand: 'Tags'
      };
      this.odataPartnerService.get(this.partner.id, expand).subscribe(rs => {
        this.formGroup.get('partnerAge').patchValue(rs.Age);
        this.formGroup.get('partnerPhone').patchValue(rs.Phone);
        this.tags.clear();
        rs.Tags.forEach(tag => {
          var g = this.fb.group(tag);
          this.tags.push(g);
        });
      });
    }
  }

  get tags() {
    return this.formGroup.get('partnerTags') as FormArray;
  }


  partnercatolories(tags) {
    return tags.map(x => x.name).join(', ');
  }

  defaultGet() {
    var val = this.defaultVal || {};
    if (this.dotKhamId) {
      val.DotKhamId = this.dotKhamId;
    }
    this.appointmentService.defaultGet(val).subscribe(
      (rs: any) => {
        this.formGroup.patchValue(rs);

        let date = new Date(rs.date);
        this.formGroup.get('apptDate').patchValue(date);
        this.formGroup.get('appTime').patchValue('07:00');
        this.formGroup.get('timeExpected').patchValue('30');

        if (rs.partner) {
          this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
          this.onChangePartner();
        }

        if (rs.user) {
          this.userSimpleFilter = _.unionBy(this.userSimpleFilter, [rs.user], 'id');
        }
      }
    )
  }

  onChangeState() {
    event.stopPropagation();
    if (this.state == 'cancel') {
      this.formGroup.get("reason").setValidators([Validators.minLength(0), Validators.required]);
      this.formGroup.get("reason").updateValueAndValidity();
    } else {
      this.formGroup.get('reason').clearValidators();
      this.formGroup.get('reason').updateValueAndValidity();
    }
  }
}
