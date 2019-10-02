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

  constructor(private fb: FormBuilder, private window: WindowRef, private service: AppointmentService, private employeeService: EmployeeService,
    private partnerService: PartnerService, private windowService: WindowService,
    private intlService: IntlService, private notificationService: NotificationService) { }

  formCreate: FormGroup;

  ngOnInit() {
    this.formCreate = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      user: null,
      userId: null,
      date: null,
      dateObj: null,
      note: null,
      companyId: null,
      // dotKhamId: this.dotKhamId,
      doctor: null,
      doctorId: null,
      state: null
    })
    if (this.appointId) {
      this.loadAppointmentToForm();
    } else {
      if (this.timeConfig != null) {
        this.formCreate.get('dateObj').setValue(this.timeConfig);
      }
      this.defaultGet();
    }

    this.getDoctorList();
    this.getCustomerList();
    this.filterChangeCombobox();
    this.setDateLimit();

  }

  setDateLimit() {
    this.dateLimit.min = new Date();
    this.dateLimit.max = new Date(this.dateLimit.min.getFullYear() + 1, this.dateLimit.min.getMonth(), this.dateLimit.min.getDate());
  }

  createNewAppointment() {
    var appoint = this.formCreate.value;
    appoint.partnerId = appoint.partner ? appoint.partner.id : null;
    appoint.userId = appoint.user ? appoint.user.id : null;
    appoint.doctorId = appoint.doctor ? appoint.doctor.id : null;
    appoint.date = this.intlService.formatDate(appoint.dateObj, 'g', 'en-US');
    var today = new Date();
    if (appoint.dateObj as Date >= today) {
      console.log(10);
      appoint.state = "confirmed";
    }
    this.service.createUpdateAppointment(appoint, this.appointId).subscribe(
      rs => {
        console.log(rs);
        this.isChange = true;
        this.closeWindow(rs);
      },
      er => {
        console.log(er);
      },
    )
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

  searchCustomers(search) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.employee = false;
    partnerPaged.customer = true;
    partnerPaged.supplier = false;
    partnerPaged.limit = this.limit;
    partnerPaged.offset = this.skip;
    if (search != null) {
      partnerPaged.searchNameRef = search.toLowerCase();
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
    empPaged.position = "assistant";
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.employeeService.getEmployeeSimpleList(empPaged);
  }

  getDoctorList() {
    var empPn = new EmployeePaged;
    empPn.position = "doctor";
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
          this.appointState = rs.state;
          let date = this.intlService.parseDate(rs.date);
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
          this.state = this.formCreate.get('state').value;
          // this.availableEdit(rs.state.toString());
          console.log(rs.state);
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
        this.window.close(true);
      }
    )
  }

  closeWindow(rs) {
    if (this.isChange) {
      if (rs) {
        this.window.close(rs);
      } else {
        this.window.close(true);
      }
    } else {
      this.window.close();
    }
  }

  quickCreateCustomer() {
    const windowRef: WindowRef = this.windowService.open(
      {
        title: 'Thông tin khách hàng',
        content: PartnerCreateUpdateComponent,
        minWidth: 250,
        width: 920
      });
    this.windowOpened = true;
    const instance = windowRef.content.instance;
    instance.queryCustomer = true;

    windowRef.result.subscribe(
      (result) => {
        this.windowOpened = false;
        console.log(result instanceof WindowCloseResult);
        if (!(result instanceof WindowCloseResult)) {
          var newPartner = new PartnerSimple();
          newPartner.id = result['id'];
          newPartner.name = result['name'];
          this.customerSimpleFilter.push(newPartner);
          this.formCreate.get('partner').setValue(newPartner);
        }
      }
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
            let date = this.intlService.parseDate(rs.date);
            this.formCreate.get('dateObj').patchValue(date);
          }
          // if (rs.user) {
          //   this.employeeSimpleFilter = _.unionBy(this.employeeSimpleFilter, [rs.user], 'id');
          // }
        }
      )
    }
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
        return 'Kết thúc';
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
