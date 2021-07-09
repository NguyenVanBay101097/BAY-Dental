import { AuthService } from './../../auth/auth.service';
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
import { CustomerReceiptRequest, DashboardReportService } from 'src/app/core/services/dashboard-report.service';

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
  id: string;
  title: string;
  showIsNoTreatment = false;
  defaultData: any;
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
    private dashboardReportService : DashboardReportService,
    private partnerService: PartnerService,
    private authService: AuthService,
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
      partner: [null, Validators.required],
      partnerAge: null,
      partnerPhone: null,
      dateObj: [null, Validators.required],
      note: null,
      companyId: null,
      doctor: null,
      timeExpected: '30',
      state: 'waiting',
      reason: null,
      products: [],
      isRepeatCustomer: false,
      isNoTreatment: false
    })

    setTimeout(() => {
      this.hourList = _.range(0, 24);
      this.timeSource = this.TimeInit();
      this.timeList = this.timeSource;

      if (this.id) {
        this.loadDataFromApi();
      } else {
        this.loadDefault();
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

  get stateControl() {
    return this.formGroup.get('state').value;
  }

  onSave() {
    
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }
    
    var receipt = this.formGroup.value;
    receipt.partnerId = receipt.partner ? receipt.partner.id : null;
    receipt.doctorId = receipt.doctor ? receipt.doctor.id : null;
    receipt.dateWaiting = this.intlService.formatDate(receipt.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    receipt.dateExamination = this.stateControl == 'examination' ? this.intlService.formatDate(new Date(), 'yyyy-MM-ddTHH:mm:ss')  : null;
    receipt.dateDone =  this.stateControl == 'done' ? this.intlService.formatDate(new Date(), 'yyyy-MM-ddTHH:mm:ss')  : null;
    receipt.timeExpected = Number.parseInt(receipt.timeExpected); 
    receipt.companyId = this.authService.userInfo.companyId;
    if (this.id) {   
      this.customerReceiptService.update(this.id, receipt).subscribe(
        () => {
          this.activeModal.close();
        },
        er => {
          this.errorService.show(er);
          this.submitted = false;
        },
      )  
    } else {    
      this.customerReceiptService.create(receipt).subscribe(
        res => {
          this.activeModal.close();
        },
        er => {
          this.errorService.show(er);
          this.submitted = false;
        },
      )
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
  }

  checkedRepeatCustomer(value){
    if (value == true && this.isNoTreatment){
      this.formGroup.get('isNoTreatment').setValue(false);
      this.formGroup.get('reason').setValue(null);
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

  loadDefault(){
    if(this.appointId){
      this.formGroup.patchValue(this.defaultData);

      if (this.defaultData.partner) {
        this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [this.defaultData.partner], 'id');
        this.onChangePartner();
      }

      if (this.defaultData.doctor) {
        this.filteredEmployees = _.unionBy(this.filteredEmployees, [this.defaultData.doctor], 'id');
      }

    }

    let date = new Date();
    this.formGroup.get('dateObj').patchValue(date);
  }

  loadDataFromApi() {
    if (this.id) {
      this.customerReceiptService.get(this.id).subscribe(
        (rs: any) => {        
          this.formGroup.patchValue(rs);  
          let date = new Date(rs.dateWaiting);           
          this.formGroup.get('dateObj').patchValue(date);
          
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

  onChangeState(state){
    if(state != 'done'){
      this.formGroup.get('isNoTreatment').setValue(false);
      this.formGroup.get('reason').setValue(null);
      this.f.reason.clearValidators();
      this.f.reason.updateValueAndValidity();
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

  get isRepeatCustomer() {
    return this.formGroup.get('isRepeatCustomer').value;
  }


  get isNoTreatment() {
    return this.formGroup.get('isNoTreatment').value;
  }

  updateCustomerModal() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khách hàng';
    modalRef.componentInstance.id = this.partner.id;

    modalRef.result.then(() => {
    }, () => {
    });
  }

  onSaveToAppoint(){
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }
    debugger
    var receipt = this.formGroup.value;
    var res = new CustomerReceiptRequest();
    res.partnerId = receipt.partner ? receipt.partner.id : null;
    res.doctorId = receipt.doctor ? receipt.doctor.id : null;
    res.dateWaiting = this.intlService.formatDate(receipt.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    res.timeExpected = Number.parseInt(receipt.timeExpected); 
    res.companyId = this.authService.userInfo.companyId;
    res.products = receipt.products;
    res.isRepeatCustomer = this.isRepeatCustomer;
    res.note = receipt.note;
    res.appointmentId = this.appointId;
    this.dashboardReportService.createCustomerReceiptToAppointment(res).subscribe(
      () => {
        this.activeModal.close();
      },
      er => {
        this.notify('error',er);
        this.submitted = false;
      },
    )

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
      this.partnerService.getCustomerInfo(this.partner.id).subscribe((rs: any) => {
        this.formGroup.get('partnerAge').patchValue(rs.age);
        this.formGroup.get('partnerPhone').patchValue(rs.phone);
      });
    }
  }

  


}
