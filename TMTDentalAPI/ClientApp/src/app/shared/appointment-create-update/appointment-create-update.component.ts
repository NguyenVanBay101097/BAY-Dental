import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { AppointmentBasic } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerSearchDialogComponent } from 'src/app/partners/partner-search-dialog/partner-search-dialog.component';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { UserSimple } from 'src/app/users/user-simple';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { NotifyService } from '../services/notify.service';
import { PartnersService } from '../services/partners.service';
import { PrintService } from '../services/print.service';
import { UserCuDialogComponent } from './../../users/user-cu-dialog/user-cu-dialog.component';
import { UserPaged, UserService } from './../../users/user.service';

@Component({
  selector: 'app-appointment-create-update',
  templateUrl: './appointment-create-update.component.html',
  styleUrls: ['./appointment-create-update.component.css']
})

export class AppointmentCreateUpdateComponent implements OnInit {
  @Input() dateTime: any;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('serviceMultiSelect', { static: true }) serviceMultiSelect: MultiSelectComponent
  customerSimpleFilter: PartnerSimple[] = [];
  userSimpleFilter: UserSimple[] = [];
  filteredServices: ProductSimple[] = [];
  filteredEmployees: EmployeeBasic[] = [];
  appointId: string;
  title: string;
  type: string = "receive_update";
  showIsNotExamination = false;

  states: any[] = [
    { value: 'confirmed', text: '??ang h???n' },
    { value: 'done', text: '???? ?????n' },
    { value: 'cancel', text: 'H???y h???n' },
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
  colors = [{ name: '', code: '#2395FF' },
  { name: '', code: '#28A745' },
  { name: '', code: '#FFC107' },
  { name: '', code: '#EB3B5B' },
  { name: '', code: '#B5076B' },
  { name: '', code: '#A70000' },
  { name: '', code: '#034A93' },
  { name: '', code: '#42526E' },
  { name: '', code: '#0C9AB2' },
  { name: '', code: '#FF8900' },
  { name: '', code: '#00875A' },
  { name: '', code: '#EB2E94' }
  ];
  codeColorSelected: number = 0;
  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private partnerService: PartnerService,
    private intlService: IntlService,
    private userService: UserService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private odataPartnerService: PartnersService,
    private productService: ProductService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
    private printService: PrintService,
    private sessionInfoStorageService: SessionInfoStorageService,
    private authService: AuthService,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      partnerAge: null,
      partnerPhone: null,
      partnerTags: this.fb.array([]),
      user: [null],
      dateObj: [null, Validators.required],
      timeObj: [null, Validators.required],
      note: null,
      companyId: null,
      doctor: null,
      timeExpected: 30,
      state: 'confirmed',
      reason: null,
      saleOrderId: null,
      services: [],
      isRepeatCustomer: false,
      isNotExamination: false,
      showReason: false,
      color: ''
    })

    setTimeout(() => {
      this.hourList = _.range(0, 24);
      // this.minuteList = _.range(0, 60, 5);
      this.timeSource = this.TimeInit();
      this.timeList = this.timeSource;

      if (this.appointId) {
        this.loadAppointmentToForm();
        this.title = 'C???p nh???t l???ch h???n';
      } else {
        this.defaultGet();
        this.title = '?????t l???ch h???n';
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

  searchService(q?: string) {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0
    val.search = q || '';
    val.type2 = "service";
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

  get stateControl() {
    return this.formGroup.get('state').value;
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
    val.active = true;
    val.companyId = this.authService.userInfo.companyId;
    return this.employeeService.getEmployeePaged(val);
  }

  onSavePrint() {
    this.submitted = true;
    if (!this.formGroup.valid)
      return false;
    var appoint = this.dataSave();
    this.appointmentService.create(appoint)
      .pipe(
        mergeMap((rs: any) => {
          return this.appointmentService.get(rs.id);
        })).subscribe((res: any) => {
          var basic = this.getBasic(res);
          this.activeModal.close(basic);
          this.notify("success", "L??u th??nh c??ng");
          this.appointId = res.id;
          this.onPrint();
        },
          er => {
            this.submitted = false;
          },
        )
  }

  dataSave() {
    var appoint = this.formGroup.getRawValue();
    appoint.partnerId = appoint.partner ? appoint.partner.id : null;
    appoint.doctorId = appoint.doctor ? appoint.doctor.id : null;
    var apptDate = this.intlService.formatDate(appoint.dateObj, 'yyyy-MM-dd');
    var appTime = this.intlService.formatDate(appoint.timeObj, 'HH:mm');;
    appoint.date = `${apptDate}T${appTime}`;
    appoint.timeExpected = appoint.timeExpected || 0;
    appoint.color = this.codeColorSelected;
    if (this.state != 'cancel') {
      appoint.reason = null;
    }
    return appoint;
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }
    if (this.codeColorSelected == undefined)
      return;
    var appoint = this.dataSave();

    if (this.appointId) {
      this.appointmentService.update(this.appointId, appoint).subscribe(
        () => {
          appoint.id = this.appointId;
          var basic = this.getBasic(appoint);
          this.activeModal.close(basic);
          this.notify("success", "L??u th??nh c??ng");

        },
        er => {
          this.submitted = false;
        },
      )
    } else {
      this.appointmentService.create(appoint)
        .pipe(
          mergeMap((rs: any) => {
            return this.appointmentService.get(rs.id);
          })).subscribe(res => {
            var basic = this.getBasic(res);
            this.activeModal.close(basic);
            this.notify("success", "L??u th??nh c??ng");
          },
            er => {
              this.submitted = false;
            },
          )
    }
  }

  getBasic(res) {
    var basic = new AppointmentBasic();
    basic.id = this.appointId ? this.appointId : res.id;
    basic.doctorId = res.doctor ? res.doctorId : null;
    basic.doctorName = res.doctor ? res.doctor.name : '';
    basic.partnerId = res.partnerId;
    basic.partnerName = res.partner.name;
    basic.partnerPhone = res.partner.phone;
    basic.date = res.date ? res.date : null;
    basic.note = res.note;
    basic.time = res.time;
    basic.state = res.state;
    basic.color = res.color;
    return basic;
  }

  onDelete() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'md', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'X??a l???ch h???n';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a l???ch h???n n??y?';
    modalRef.result.then(() => {
      this.appointmentService.removeAppointment(this.appointId).subscribe(() => {
        this.notify("success", "X??a th??nh c??ng");
        this.activeModal.close({ id: this.appointId, isDetele: true });
      })

    });
  }

  onDuplicate() {
    this.appointId = null;
    this.title = '?????t l???ch h???n'
    var item = this.formGroup.value;
    var res = this.fb.group({
      name: null,
      partner: item.partner ? item.partner : null,
      partnerAge: item.partner ? item.partner.age : null,
      partnerPhone: item.partner ? item.partner.phone : null,
      partnerTags: this.fb.array([]),
      user: item.user ? item.user : null,
      dateObj: [null, Validators.required],
      timeObj: [null, Validators.required],
      note: item.note ? item.note : null,
      companyId: item.companyId ? item.companyId : null,
      doctor: item.doctor ? item.doctor : null,
      timeExpected: 30,
      state: 'confirmed',
      reason: null,
      saleOrderId: null,
      services: item.services ? item.services : [],
      isRepeatCustomer: false,
      isNotExamination: false,
      showReason: false
    })

    this.formGroup.patchValue(res);
    let date = new Date();
    this.formGroup.get('dateObj').patchValue(date);
    this.formGroup.get('timeObj').patchValue(date);
  }

  onCreateNewAppointment() {
    this.appointId = null;
    this.title = '?????t l???ch h???n'
    this.defaultGet();
  }

  onChange() {
    if (this.appointId) {
      if (this.stateControl == 'cancel') {
        this.formGroup.get("reason").setValidators([Validators.minLength(0), Validators.required]);
        this.formGroup.get("reason").updateValueAndValidity();
      } else {
        this.formGroup.get('reason').clearValidators();
        this.formGroup.get('reason').updateValueAndValidity();
        this.formGroup.get('reason').setValue(null);
      }
    }
  }

  eventCheck(value) {
    if (value == true && this.f.isRepeatCustomer.value == false) {
      this.f.reason.setValidators(Validators.required);
      this.f.reason.updateValueAndValidity();
    }
    else {
      this.f.reason.clearValidators();
      this.f.reason.updateValueAndValidity();
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
    let modalRef = this.modalService.open(PartnerSearchDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'T??m kh??ch h??ng';
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
    let modalRef = this.modalService.open(UserCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m b??c s??';

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
    this.searchCustomers().subscribe(
      rs => {
        this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, rs, 'id');
      }
    )
  }

  filterChangeCombobox() {
    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.partnerCbx.loading = true),
      switchMap(val =>
        this.searchCustomers(val)
      )
    ).subscribe(
      rs => {
        this.customerSimpleFilter = rs;
        this.partnerCbx.loading = false;
      }
    )

    this.doctorCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.doctorCbx.loading = true),
      switchMap(val => this.searchEmployees(val))
    ).subscribe(
      rs => {
        this.filteredEmployees = rs.items;
        this.doctorCbx.loading = false;
      }
    )
  }

  searchCustomers(search?: string) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.customer = true;
    partnerPaged.limit = 20;
    partnerPaged.search = search || '';

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
          this.formGroup.get('dateObj').patchValue(date);
          this.formGroup.get('timeObj').patchValue(date);
          this.codeColorSelected = rs.color || 0;
          // this.formGroup.get('apptHour').patchValue(date.getHours());
          // this.formGroup.get('apptMinute').patchValue(date.getMinutes());

          // var appoint = this.formGroup.value;
          //console.log(appoint);

          if (rs.partner) {
            this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
            this.onChangePartner();
          }

          if (rs.doctor) {
            this.filteredEmployees = _.unionBy(this.filteredEmployees, [rs.doctor], 'id');
          }

          if (this.stateControl == 'cancel') {
            this.formGroup.get("reason").setValidators([Validators.minLength(0), Validators.required]);
            this.formGroup.get("reason").updateValueAndValidity();
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
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a kh??ch h??ng';
    modalRef.componentInstance.id = this.partner.id;

    modalRef.result.then(() => {
    }, () => {
    });
  }

  quickCreateCustomerModal() {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m kh??ch h??ng';

    modalRef.result.then(result => {
      if (result && result.id) {
        // var newPartner = new PartnerSimple();
        // newPartner.id = result.id;
        // newPartner.name = result.name;
        // newPartner.displayName = result.displayName;
        this.notifyService.notify("success", "L??u th??nh c??ng");
        this.customerSimpleFilter.push(result as PartnerSimple);
        this.formGroup.patchValue({ partner: result });
        this.onChangePartner();
      }
    })
  }

  onChangePartner() {
    if (this.partner) {
      this.partnerService.getPartner(this.partner.id).subscribe(rs => {
        this.formGroup.get('partnerAge').patchValue(rs.age);
        this.formGroup.get('partnerPhone').patchValue(rs.phone);
      });
    } else {
      this.getCustomerList();
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

        let date = this.dateTime ? new Date(this.dateTime) : new Date(rs.date);
        this.formGroup.get('dateObj').patchValue(date);
        this.formGroup.get('timeObj').patchValue(date);
        this.formGroup.get('timeExpected').patchValue(30);

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
    if (this.state == 'cancel') {
      this.formGroup.get("reason").setValidators([Validators.minLength(0), Validators.required]);
      this.formGroup.get("reason").updateValueAndValidity();
    } else {
      this.formGroup.get('reason').clearValidators();
      this.formGroup.get('reason').updateValueAndValidity();
    }
  }

  onPrint() {
    this.appointmentService.print(this.appointId).subscribe((res: any) => {
      this.printService.printHtml(res.html);
    });
  }

  clickColor(color) {
    if (this.codeColorSelected == color)
      this.codeColorSelected = undefined;
    else
      this.codeColorSelected = color;
  }
}
