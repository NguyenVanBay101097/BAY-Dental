import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { AppointmentService } from '../appointment.service';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { AppointmentDisplay, ApplicationUserSimple, ApplicationUserPaged, ApplicationUserDisplay } from '../appointment';
import { map, debounceTime, tap, switchMap } from 'rxjs/operators';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { UserService, UserPaged } from 'src/app/users/user.service';

@Component({
  selector: 'app-appointment-cu-dialog',
  templateUrl: './appointment-cu-dialog.component.html',
  styleUrls: ['./appointment-cu-dialog.component.css']
})

export class AppointmentCuDialogComponent implements OnInit {
  filteredPartners: PartnerSimple[] = [];
  filteredUsers: UserSimple[] = [];

  dotKhamId: string;
  id: string;
  appointmentForm: FormGroup;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;

  constructor(private window: WindowRef, private appointmentService: AppointmentService,
    private partnerService: PartnerService, private windowService: WindowService,
    private userService: UserService,
    private intlService: IntlService, private fb: FormBuilder) { }

  ngOnInit() {
    this.appointmentForm = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      partnerId: null,
      user: null,
      userId: null,
      date: null,
      dateObj: null,
      note: null,
      companyId: null,
      dotKhamId: null,
      state: null
    });
    if (this.id) {
      this.loadRecord();
    } else {
      // this.loadDefault();
    }

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });
  }

  loadRecord() {
    this.appointmentService.getAppointmentInfo(this.id).subscribe(result => {
      this.appointmentForm.patchValue(result);
      let date = this.intlService.parseDate(result.date);
      this.appointmentForm.get('dateObj').patchValue(date);
      if (result.user) {
        this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
      }
      if (result.partner) {
        this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
      }
    });
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.searchNameRef = search;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchUsers(search?: string) {
    var val = new UserPaged();
    val.search = search;
    return this.userService.autocompleteSimple(val);
  }

  onSave() {
    if (!this.appointmentForm.valid) {
      return;
    }
    var val = this.appointmentForm.value;
    val.partnerId = val.partner ? val.partner.id : null;
    val.userId = val.user ? val.user.id : null;
    val.date = this.intlService.formatDate(val.dateObj, 'g', 'en-US');

    if (this.id) {
      this.appointmentService.update(this.id, val).subscribe(() => {
        this.window.close(true);
      });
    } else {
      this.appointmentService.create(val).subscribe(result => {
        this.window.close(result);
      });
    }
  }

  onCancel() {
    this.window.close();
  }

  // loadDefault() {
  //   // var val = new AppointmentDefaultGet();
  //   // val.dotKhamId = this.dotKhamId;
  //   this.appointmentService.defaultGet(null).subscribe(result => {
  //     this.appointmentForm.patchValue(result);
  //     let date = this.intlService.parseDate(result.date);
  //     this.appointmentForm.get('dateObj').patchValue(date);
  //     if (result.user) {
  //       this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
  //     }
  //     if (result.partner) {
  //       this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
  //     }
  //   });
  // }
}

