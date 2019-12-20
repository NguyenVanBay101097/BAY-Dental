import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { AppointmentService } from '../appointment.service';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { PartnerBasic, PartnerDisplay, PartnerSimple, PartnerPaged, PartnerCategorySimple } from 'src/app/partners/partner-simple';
import { AppointmentDisplay, ApplicationUserSimple, ApplicationUserPaged, ApplicationUserDisplay, AppointmentPatch, AppointmentDefaultGet } from '../appointment';
import { PartnerCreateUpdateComponent } from 'src/app/partners/partner-create-update/partner-create-update.component';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { EmployeeService } from 'src/app/employees/employee.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerSearchDialogComponent } from 'src/app/partners/partner-search-dialog/partner-search-dialog.component';
import { Router } from '@angular/router';

class DatePickerLimit {
  min: Date;
  max: Date;
}
@Component({
  selector: 'app-appointment-create-update',
  templateUrl: './appointment-create-update.component.html',
  styleUrls: ['./appointment-create-update.component.css']
})

export class AppointmentCreateUpdateComponent implements OnInit {

  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  isChange = false;
  customerSimpleFilter: PartnerSimple[] = [];
  doctorSimpleFilter: EmployeeSimple[] = [];
  stateFilter = [{ text: 'Đang hẹn', value: 'confirmed', disabled: false }, { text: 'Đang chờ', value: 'waiting', disabled: false }, { text: 'Hoàn tất', value: 'done', disabled: false },
  { text: 'Quá hạn', value: 'expired', disabled: true }, { text: 'Đã hủy', value: 'cancel', disabled: false }];

  appointId: string;
  dotKhamId: string;
  state: string;

  appointState: string;
  timeConfig: Date;
  userIdDefault: string;

  dateLimit: DatePickerLimit = new DatePickerLimit();

  skip: number = 0;
  limit: number = 20;

  windowOpened: boolean = false;

  appointName: string;

  // companyIdDefault: string;

  constructor(private fb: FormBuilder, private service: AppointmentService, private employeeService: EmployeeService,
    private partnerService: PartnerService, private intlService: IntlService,
    private notificationService: NotificationService, public activeModal: NgbActiveModal, private modalService: NgbModal,
    private router: Router) { }

  formCreate: FormGroup;

  ngOnInit() {
    this.formCreate = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      user: null,
      userId: null,
      date: null,
      dateObj: [null, Validators.required],
      note: null,
      companyId: null,
      // dotKhamId: this.dotKhamId,
      doctor: null,
      doctorId: null,
      stateObj: null,
      state: 'confirmed',
    })
    if (this.appointId) {
      setTimeout(() => {
        this.loadAppointmentToForm();
      });
    } else {
      setTimeout(() => {
        if (this.timeConfig != null) {
          this.formCreate.get('dateObj').setValue(this.timeConfig);
        }
        this.defaultGet();
      });
    }

    setTimeout(() => {
      this.getDoctorList();
      this.getCustomerList();
      this.filterChangeCombobox();
      this.setDateLimit();
    });
  }

  setDateLimit() {
    this.dateLimit.min = new Date();
    this.dateLimit.max = new Date(this.dateLimit.min.getFullYear() + 1, this.dateLimit.min.getMonth(), this.dateLimit.min.getDate());
  }

  //Create + Update
  createNewAppointment() {
    if (!this.formCreate.valid) {
      return false;
    }

    var appoint = this.formCreate.value;
    appoint.partnerId = appoint.partner ? appoint.partner.id : null;
    appoint.userId = appoint.user ? appoint.user.id : null;
    appoint.doctorId = appoint.doctor ? appoint.doctor.id : null;
    appoint.date = this.intlService.formatDate(appoint.dateObj, 'g', 'en-US');

    this.service.createUpdateAppointment(appoint, this.appointId).subscribe(
      rs => {
        console.log(rs);
        this.isChange = true;
        this.closeModal(rs);
      },
      er => {
        console.log(er);
      },
    )
  }

  searchCustomerDialog() {
    let modalRef = this.modalService.open(PartnerSearchDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tìm khách hàng';
    modalRef.componentInstance.domain = { customer: true };

    modalRef.result.then(result => {
      if (result.length) {
        var p = result[0].dataItem;
        this.formCreate.get('partner').patchValue(p);
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
    partnerPaged.limit = this.limit;
    partnerPaged.offset = this.skip;
    this.partnerService.autocompletePartner(partnerPaged).subscribe(
      rs => {
        this.customerSimpleFilter = rs as PartnerSimple[];
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
      switchMap(val => this.searchDoctors(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.doctorSimpleFilter = rs;
        this.doctorCbx.loading = false;
      }
    )
  }

  createSaleOrder() {
    if (this.appointId) {
      var partner = this.formCreate.get('partner').value;
      if (partner) {
        this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: partner.id } });
        this.activeModal.close();
      }
    }
  }

  searchCustomers(search) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.employee = false;
    partnerPaged.customer = true;
    partnerPaged.supplier = false;
    partnerPaged.limit = this.limit;
    partnerPaged.offset = this.skip;
    if (search != null) {
      partnerPaged.searchNamePhoneRef = search.toLowerCase();
    }
    return this.partnerService.autocompletePartner(partnerPaged);
  }

  searchDoctors(search) {
    var empPaged = new EmployeePaged();
    empPaged.position = "doctor";
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.employeeService.getEmployeeSimpleList(empPaged);
  }

  searchAssitants(search) {
    var empPaged = new EmployeePaged();
    empPaged.isAssistant = true;
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.employeeService.getEmployeeSimpleList(empPaged);
  }

  getDoctorList() {
    var empPn = new EmployeePaged;
    empPn.isDoctor = true;
    empPn.limit = this.limit;
    empPn.offset = this.skip;
    this.employeeService.getEmployeeSimpleList(empPn).subscribe(
      rs => {
        this.doctorSimpleFilter = rs;
      });
  }

  loadAppointmentToForm() {
    if (this.appointId != null) {
      this.service.getAppointmentInfo(this.appointId).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
          this.formCreate.get('stateObj').setValue(this.stateFilter.filter(x => x.value == rs.state)[0]);
          this.state = rs.state;
          let date = new Date(rs.date);
          this.formCreate.get('dateObj').patchValue(date);
          if (rs.partner) {
            this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
          }
          if (rs.hasDotKhamRef) {
            this.dotKhamId = 'default';
          }
          // if (!this.dotKhamId && this.formCreate.get('dotKhamId').value) {
          //   this.dotKhamId = this.formCreate.get('dotKhamId').value;
          // }
          // this.state = this.formCreate.get('state').value;
          // this.availableEdit(rs.state.toString());
        },
        er => {
          console.log(er);
        }
      )
    }

  }

  // getUser(id: string): ApplicationUserSimple {
  //   var user = new ApplicationUserSimple;
  //   this.partnerService.getUser(id).subscribe(
  //     rs2 => {
  //       user.id = rs2.id;
  //       user.name = rs2.name;
  //       if (this.employeeSimpleFilter.filter(x => x.id == user.id).length == 0) {
  //         this.employeeSimpleFilter.push(user);
  //       }

  //     },
  //     er => {
  //       console.log(er);
  //     }
  //   );
  //   return user;
  // }

  cancelAppointment() {
    this.updateOnly(this.appointId, "cancel");
  }

  doneAppointment() {
    this.updateOnly(this.appointId, "done");
  }

  waitingAppointment() {
    this.updateOnly(this.appointId, "waiting");
  }

  confirmedAppointment() {
    this.updateOnly(this.appointId, "confirmed");
  }

  updateOnly(id, state) {
    var apnPatch = new AppointmentPatch;
    var ar = [];
    apnPatch.id = id;
    apnPatch.state = state;
    for (var p in apnPatch) {
      var o = { op: 'replace', path: '/' + p, value: apnPatch[p] };
      ar.push(o);
    }
    console.log(ar);
    this.service.patch(id, ar).subscribe(
      rs => {
        this.activeModal.close(true);
      }
    )
  }

  closeModal(rs) {
    if (this.isChange) {
      if (rs) {
        this.activeModal.close(rs);
      } else {
        this.activeModal.close(true);
      }
    }
    else {
      this.activeModal.dismiss();
    }
  }

  // closeWindow(rs) {
  //   if (this.isChange) {
  //     if (rs) {
  //       this.window.close(rs);
  //     } else {
  //       this.window.close(true);
  //     }
  //   } else {
  //     this.window.close();
  //   }
  // }

  // quickCreateCustomer() {
  //   const windowRef: WindowRef = this.windowService.open(
  //     {
  //       title: 'Thông tin khách hàng',
  //       content: PartnerCreateUpdateComponent,
  //       minWidth: 250,
  //       width: 920
  //     });
  //   this.windowOpened = true;
  //   const instance = windowRef.content.instance;
  //   instance.queryCustomer = true;

  //   windowRef.result.subscribe(
  //     (result) => {
  //       this.windowOpened = false;
  //       console.log(result instanceof WindowCloseResult);
  //       if (!(result instanceof WindowCloseResult)) {
  //         var newPartner = new PartnerSimple();
  //         newPartner.id = result['id'];
  //         newPartner.name = result['name'];
  //         this.customerSimpleFilter.push(newPartner);
  //         this.formCreate.get('partner').setValue(newPartner);
  //       }
  //     }
  //   )
  // }

  quickCreateCustomerModal() {
    const modalRef = this.modalService.open(PartnerCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.queryCustomer = true;
    modalRef.result.then(
      result => {
        this.windowOpened = false;

        if (result && result.id) {
          console.log(result);
          var newPartner = new PartnerSimple();
          newPartner.id = result.id;
          newPartner.name = result.name;
          this.customerSimpleFilter.push(newPartner);
          this.formCreate.get('partner').setValue(newPartner);
        }
      },
      er => { }
    )
  }

  defaultGet() {
    if (this.appointId == null) {
      var a = new AppointmentDefaultGet();
      if (this.dotKhamId) {
        a.dotKhamId = this.dotKhamId;
      }
      this.service.defaultGet(a).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
          if (this.timeConfig == null) {
            let date = new Date(rs.date);
            this.formCreate.get('dateObj').patchValue(date);
          }
          // if (rs.user) {
          //   this.employeeSimpleFilter = _.unionBy(this.employeeSimpleFilter, [rs.user], 'id');
          // }
        }
      )
    }
  }

  changeState(e) {
    this.state = e.value;
  }

  itemDisabled(itemArgs: { dataItem: any, index: number }) {
    return itemArgs.dataItem.disabled;
  }

  timeAvailble() {
    switch (this.state) {
      case 'confirmed':
        return false;
      case 'waiting':
        return true;
      case 'expired':
        return false;
      case 'cancel':
        return true;
      case 'done':
        return true;
    }
  }

  doctorAvailble() {
    switch (this.state) {
      case 'confirmed':
        return false;
      case 'waiting':
        return false;
      case 'expired':
        return true;
      case 'cancel':
        return true;
      case 'done':
        return true;
    }
  }

  partnerAvailble() {
    switch (this.state) {
      case 'confirmed':
        if (this.dotKhamId) {
          return true;
        } else {
          return false;
        }
      case 'waiting':
        return true;
      case 'expired':
        return true;
      case 'cancel':
        return true;
      case 'done':
        return true;
    }
  }

  getState(state: string) {
    switch (state) {
      case 'done':
        return 'Hoàn tất';
      case 'cancel':
        return 'Đã hủy';
      case 'waiting':
        return 'Đang chờ';
      case 'expired':
        return 'Quá hạn';
      default:
        return 'Đang hẹn';
    }
  }

}
