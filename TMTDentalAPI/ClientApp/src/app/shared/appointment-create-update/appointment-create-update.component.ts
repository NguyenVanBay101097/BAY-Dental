import { UserPaged, UserService } from './../../users/user.service';
import { UserCuDialogComponent } from './../../users/user-cu-dialog/user-cu-dialog.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { PartnerBasic, PartnerDisplay, PartnerSimple, PartnerPaged, PartnerCategorySimple } from 'src/app/partners/partner-simple';
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
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { UserSimple } from 'src/app/users/user-simple';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';

@Component({
  selector: 'app-appointment-create-update',
  templateUrl: './appointment-create-update.component.html',
  styleUrls: ['./appointment-create-update.component.css']
})

export class AppointmentCreateUpdateComponent implements OnInit {
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  customerSimpleFilter: PartnerSimple[] = [];
  userSimpleFilter: UserSimple[] = [];
  appointId: string;
  defaultVal: any;
  formGroup: FormGroup;
  partnerSend: any;
  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private partnerService: PartnerService,
    private intlService: IntlService,
    private userService: UserService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      user: [null],
      dateObj: [null, Validators.required],
      note: null,
      companyId: null,
      state: 'confirmed',
    })



    setTimeout(() => {
      if (this.appointId) {
        this.loadAppointmentToForm();
      } else {
        this.defaultGet();
      }

      this.getUserList();
      this.getCustomerList();
      this.filterChangeCombobox();
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var appoint = this.formGroup.value;
    appoint.partnerId = appoint.partner ? appoint.partner.id : null;
    appoint.userId = appoint.user ? appoint.user.id : null;
    appoint.date = this.intlService.formatDate(appoint.dateObj, 'yyyy-MM-ddTHH:mm:ss');

    if (this.appointId) {
      this.appointmentService.update(this.appointId, appoint).subscribe(
        () => {
          appoint.id = this.appointId;
          this.activeModal.close(appoint);
        },
        er => {
          this.errorService.show(er);
        },
      )
    } else {
      this.appointmentService.create(appoint).subscribe(
        res => {
          this.activeModal.close(res);
        },
        er => {
          this.errorService.show(er);
        },
      )
    }
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

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.userCbx.loading = true),
      switchMap(val => this.searchUsers(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.userSimpleFilter = rs;
        this.userCbx.loading = false;
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

  loadAppointmentToForm() {
    if (this.appointId != null) {
      this.appointmentService.get(this.appointId).subscribe(
        (rs: any) => {
          this.formGroup.patchValue(rs);
          let date = new Date(rs.date);
          this.formGroup.get('dateObj').patchValue(date);

          if (rs.partner) {
            this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
          }

          if (rs.user) {
            this.userSimpleFilter = _.unionBy(this.userSimpleFilter, [rs.user], 'id');
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
        this.customerSimpleFilter.push(newPartner);
        this.formGroup.get('partner').setValue(newPartner);
      }
    }, er => {
      this.errorService.show(er);
    })
  }

  defaultGet() {
    var val = this.defaultVal || {};
    this.appointmentService.defaultGet(val).subscribe(
      (rs: any) => {
        if (this.partnerSend)
          rs.partner = this.partnerSend;
        this.formGroup.patchValue(rs);

        let date = new Date(rs.date);
        this.formGroup.get('dateObj').patchValue(date);

        if (rs.partner) {
          this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [rs.partner], 'id');
        }

        if (rs.user) {
          this.userSimpleFilter = _.unionBy(this.userSimpleFilter, [rs.user], 'id');
        }
      }
    )
  }
}
