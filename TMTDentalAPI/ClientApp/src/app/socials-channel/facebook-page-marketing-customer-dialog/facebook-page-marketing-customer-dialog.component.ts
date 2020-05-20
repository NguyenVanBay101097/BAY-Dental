import { Component, OnInit, ViewChild } from '@angular/core';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, tap, switchMap } from 'rxjs/operators';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';

@Component({
  selector: 'app-facebook-page-marketing-customer-dialog',
  templateUrl: './facebook-page-marketing-customer-dialog.component.html',
  styleUrls: ['./facebook-page-marketing-customer-dialog.component.css']
})
export class FacebookPageMarketingCustomerDialogComponent implements OnInit {
  customerId: string;
  profile: any = {};
  formGroup: FormGroup;
  title = 'Người tương tác';

  filteredPartners: PartnerSimple[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  constructor(
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private partnerService: PartnerService,
    private notificationService: NotificationService,
    private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: null
    });

    setTimeout(() => {
      this.loadDataFromApi(this.customerId);
      this.loadFilteredPartners();

      this.partnerCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.partnerCbx.loading = true)),
        switchMap(value => this.searchPartners(value))
      ).subscribe(result => {
        this.filteredPartners = result;
        this.partnerCbx.loading = false;
      });
    });
  }

  loadFilteredPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadDataFromApi(id) {
    this.facebookUserProfilesService.get(id).subscribe((res: any) => {
      this.profile = res;
      this.formGroup.patchValue(res);
      if (res.partner) {
        this.filteredPartners = _.unionBy(this.filteredPartners, [res.partner], 'id');
      }
    });
  }

  onSave() {
    var value = this.formGroup.value;
    value.partnerId = value.partner ? value.partner.id : null;

    this.facebookUserProfilesService.update(this.customerId, value).subscribe(() => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      this.activeModal.close(true);
    }, err => {
      console.log(err);
      this.activeModal.close(true);
    })
  }
}
