import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { CustomerReceiptService } from 'src/app/customer-receipt/customer-receipt.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerSearchDialogComponent } from 'src/app/partners/partner-search-dialog/partner-search-dialog.component';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { UserCuDialogComponent } from 'src/app/users/user-cu-dialog/user-cu-dialog.component';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnersService } from '../services/partners.service';
import { AppSharedShowErrorService } from '../shared-show-error.service';

@Component({
  selector: 'app-customer-receip-create-update',
  templateUrl: './customer-receip-create-update.component.html',
  styleUrls: ['./customer-receip-create-update.component.css']
})
export class CustomerReceipCreateUpdateComponent implements OnInit {

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('serviceMultiSelect', { static: true }) serviceMultiSelect: MultiSelectComponent
  customerSimpleFilter: PartnerSimple[] = [];
  userSimpleFilter: UserSimple[] = [];
  filteredServices: ProductSimple[] = [];
  filteredEmployees: EmployeeBasic[] = [];
  appointId: string;
  receiptId: string;
  type: string = "receipt";
  showIsNoTreatment = false;
  value: any;
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
  statesReceipt: any[] = [
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
    private notificationService: NotificationService,
    private customerReceiptService: CustomerReceiptService
   ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      partnerAge: null,
      partnerPhone: null,
      partnerTags: this.fb.array([]),
      user: [null],
      note: null,
      companyId: null,
      doctor: null,
      timeExpected: '30',
      state: 'confirmed',
      reason: null,
      saleOrderId: null,
      services: [],
      isRepeatCustomer:false,
      isNoTreatment: false
    })

    setTimeout(() => {
      this.hourList = _.range(0, 24);
      this.timeSource = this.TimeInit();
      this.timeList = this.timeSource;

      if (this.receiptId) {
        this.loadDataFromApi();
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
    console.log(this.value);
    
    this.submitted = true;

    if (!this.formGroup.valid || this.value == undefined) {
      return false;
    }
    var receipt = this.formGroup.getRawValue();
    receipt.partnerId = receipt.partner ? receipt.partner.id : null;
    receipt.doctorId = receipt.doctor ? receipt.doctor.id : null;
    receipt.timeExpected = Number.parseInt(receipt.timeExpected);
    if (this.receiptId) {
      if (receipt.state == 'waitting'){
        receipt.dateWaitting = this.value;
      }
      if (receipt.state == 'examination'){
        receipt.dateExamination = this.value;
      }
      this.customerReceiptService.updateState(this.receiptId, receipt).subscribe(
        () => {
          this.activeModal.close(receipt);
        },
        er => {
          this.errorService.show(er);
          this.submitted = false;
        },
      )
      // if(this.type == 'create_receipt'){
      //   appoint.state = 'waitting';
      //   appoint.dateWaitting = this.value;
      //   console.log(appoint.dateWaitting);
        
      //   this.customerReceiptService.update(this.appointId, appoint).subscribe(
      //     () => {
      //       this.activeModal.close(appoint);
      //     },
      //     er => {
      //       this.errorService.show(er);
      //       this.submitted = false;
      //     },
      //   )
      // }
      // if(this.type == 'receipt_update'){
      //   this.customerReceiptService.updateState(this.appointId,appoint).subscribe(
      //     () => {
      //       if (appoint.state == 'done'){
      //         this.f.timeExpected.disable();
      //         this.f.isRepeatCustomer.disable();
      //         this.f.services.disable();
      //         this.f.note.disable();
      //         this.f.state.disable();
      //         this.f.reason.disable();
      //       }
      //       else{
      //         this.activeModal.close(appoint);
      //       }
      //     },
      //     er => {
      //       this.errorService.show(er);
      //       this.submitted = false;
      //     },
      //   )
      // }
      
    } else {
      receipt.state = 'create_receipt';
      receipt.dateWaitting = this.value;
      this.customerReceiptService.create(receipt).subscribe(
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
    if(this.receiptId){
      if(value == 'waiting'){
        this.value = new Date(Date.now());
      }
      if(value == 'done'){
        this.value = null;
        this.showIsNoTreatment = true;
      }
      else{
        this.showIsNoTreatment = false;
      }
    }
  }

  eventCheck(value){
    if (value == true && this.f.isRepeatCustomer.value == false){
      this.f.reason.setValidators(Validators.required);
      this.f.reason.updateValueAndValidity();
    }
    else{
      this.f.reason.clearValidators();
      this.f.reason.updateValueAndValidity();
    }
    console.log(value);
    
    console.log(this.f.isNoTreatment.value);
    
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

  loadDataFromApi() {
    if (this.receiptId) {
      this.customerReceiptService.get(this.receiptId).subscribe(
        (rs: any) => {
          this.formGroup.patchValue(rs);
          if (rs.state == 'waitting'){
            this.value = new Date(rs.dateWaitting);
          }
          if (rs.state == 'examination'){
            this.value = new Date(rs.dateExamination);
          }

          if (rs.state == 'done'){
            this.value = new Date(rs.dateExamination);
          }
          
          if (rs.partner) {
            this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
            this.onChangePartner();
          }

          if (rs.doctor) {
            this.filteredEmployees = _.unionBy(this.filteredEmployees, [rs.doctor], 'id');
          }
          if (rs.state == 'waitting' || rs.state == 'examination'){
            this.f.doctor.disable();
            this.f.partner.disable();
          }
          if (rs.state == 'done'){
            this.f.doctor.disable();
            this.f.partner.disable();
            this.f.timeExpected.disable();
            this.f.isRepeatCustomer.disable();
            this.f.services.disable();
            this.f.note.disable();
            this.f.state.disable();
            this.f.isNoTreatment.disable();
            this.f.reason.disable();
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

        // let date = new Date(rs.date);
        // this.formGroup.get('apptDate').patchValue(date);
        // this.formGroup.get('appTime').patchValue('07:00');
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

}
