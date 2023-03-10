import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { AppointmentService } from '../appointment.service';

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
    private partnerService: PartnerService,
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
    this.appointmentService.get(this.id).subscribe((result: any) => {
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
    val.search = search;
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

