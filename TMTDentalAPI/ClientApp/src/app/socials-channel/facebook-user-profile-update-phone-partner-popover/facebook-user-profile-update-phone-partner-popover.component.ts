import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbModal, NgbPopover } from "@ng-bootstrap/ng-bootstrap";
import { NotificationService } from "@progress/kendo-angular-notification";
import { PartnerSimple } from "src/app/partners/partner-simple";
import { PartnerFilter, PartnerService } from "src/app/partners/partner.service";
import { FacebookUserProfilesService } from "src/app/socials-channel/facebook-user-profiles.service";
import { observe, generate } from 'fast-json-patch';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { FacebookUserProfilesODataService } from 'src/app/shared/services/facebook-user-profiles.service';

@Component({
  selector: 'app-facebook-user-profile-update-phone-partner-popover',
  templateUrl: './facebook-user-profile-update-phone-partner-popover.component.html',
  styleUrls: ['./facebook-user-profile-update-phone-partner-popover.component.css']
})
export class FacebookUserProfileUpdatePhonePartnerPopoverComponent implements OnInit {
  @Input() phones;
  @Input() customerId;
  @Input() partnerName;
  @Output() reloadCustomerList = new EventEmitter();
  @ViewChild('popover', { static: true }) public popover: NgbPopover;
  phoneSearch: string = "";
  phoneSearch_invalid: boolean = false;
  phones_List: string[] = [];
  partners_List: PartnerSimple[] = [];
  show_partners_List: boolean = false;
  loading: boolean = false;

  @Input() data: any;
  searchResults: any = [];

  formGroup: FormGroup;
  observer: any;
  document: any;
  @Output() valueChange = new EventEmitter<any>();
  @Output() createPartnerClick = new EventEmitter<any>();

  constructor(
    private partnerService: PartnerService,
    private modalService: NgbModal,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private notificationService: NotificationService,
    private fb: FormBuilder,
    private partnersService: PartnersService,
    private facebookUserProfilesODataService: FacebookUserProfilesODataService
  ) { }

  ngOnInit() { }

  searchPartners(phone) {
    if (phone && phone.length >= 10) {
      this.loading = true;
      this.partnersService.getViewByPhone(phone).subscribe(result => {
        this.searchResults = result.data;
        this.loading = false;
      }, (err) => {
        this.loading = false;
      });
    }
  }

  selectPartner(partner: any) {
    this.document.partnerId = partner.Id;
    var patch = generate(this.observer);
    this.facebookUserProfilesODataService.patch(this.document.Id, patch).subscribe(() => {
      this.document = partner;
      this.valueChange.emit(this.document);
      this.popover.close();
    });
  }

  createPartner() {
    this.createPartnerClick.emit({
      phone: this.document.Phone,
      id: this.document.Id,
    });
    this.popover.close();
  }

  togglePopover() {
    if (this.popover.isOpen()) {
      this.popover.close();
    } else {
      this.document = Object.assign({}, this.data);
      this.observer = observe(this.document);
      this.popover.open({ dataItem: document });
      this.searchPartners(this.document.Phone);
    }
  }

  checkPhoneSearch(phone) {
    if (phone.length >= 10) {
      this.searchPartners(phone);
    }
  }

  onSave() {
    var patch = generate(this.observer);
    this.facebookUserProfilesODataService.patch(this.document.Id, patch).subscribe(() => {
      this.valueChange.emit(this.document);
      this.popover.close();
    });
  }

  // createPartner() {
  //   this.show_partners_List = false;
  //   this.popover.close();
  //   const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, {
  //     scrollable: true,
  //     size: "xl",
  //     windowClass: "o_technical_modal",
  //     keyboard: false,
  //     backdrop: "static",
  //   });
  //   modalRef.componentInstance.title = "Thêm khách hàng";
  //   modalRef.result.then(
  //     () => {
  //       this.searchPartners();
  //       this.popover.open();
  //     },
  //     (err) => {}
  //   );
  // }

  connectPartner(partnerId) {
    this.facebookUserProfilesService.update(this.customerId, { partnerId: partnerId })
      .subscribe(
        () => {
          this.notificationService.show({
            content: "Lưu thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.show_partners_List = false;
          this.popover.close();
          this.reloadCustomerList.emit();
        },
        (err) => { }
      );
  }
}

