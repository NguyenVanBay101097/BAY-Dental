import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { NotificationService } from "@progress/kendo-angular-notification";
import { PartnerSimple } from "src/app/partners/partner-simple";
import { PartnerFilter, PartnerService } from "src/app/partners/partner.service";
import { FacebookUserProfilesService } from "src/app/socials-channel/facebook-user-profiles.service";
import { PartnerCustomerCuDialogComponent } from "../partner-customer-cu-dialog/partner-customer-cu-dialog.component";

@Component({
  selector: "app-partner-phone-popover",
  templateUrl: "./partner-phone-popover.component.html",
  styleUrls: ["./partner-phone-popover.component.css"],
})
export class PartnerPhonePopoverComponent implements OnInit {
  @Input() phones;
  @Input() customerId;
  @Input() partnerName;
  @Output() reloadCustomerList = new EventEmitter();
  @ViewChild('popover', { static: true }) public popover: any;
  phoneSearch: string = "";
  phoneSearch_invalid: boolean = false;
  phones_List: string[] = [];
  partners_List: PartnerSimple[] = [];
  show_partners_List: boolean = false;
  loading: boolean = false;

  constructor(
    private partnerService: PartnerService,
    private modalService: NgbModal,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private notificationService: NotificationService
  ) {}

  ngOnInit() {}

  loadDataFromApi() {
    this.phoneSearch = this.phoneSearch.replace(/\s/g, "");
    if (this.phoneSearch.length != 10 || isNaN(Number(this.phoneSearch))) {
      this.show_partners_List = false;
      this.phoneSearch_invalid = true;
      return;
    }
    this.show_partners_List = true;
    this.phoneSearch_invalid = false;
    
    var val = new PartnerFilter();
    val.customer = true;
    val.search = this.phoneSearch || "";

    this.loading = true;
    this.partnerService.autocomplete2(val).subscribe(
      (res) => {
        this.partners_List = res;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  togglePopover() {
    if (this.popover.isOpen()) {
      this.show_partners_List = false;
      this.popover.close();
    } else {
      if (this.phones) {
        this.phones_List = this.phones.split(",");
      }
      if (this.phones_List.length) {
        this.phoneSearch = this.phones_List[0];
      }
      this.popover.open();
    }
  }

  checkPhoneSearch(phone) {
    if (phone.length == 10) {
      this.loadDataFromApi();
    }
  }

  createPartner() {
    this.show_partners_List = false;
    this.popover.close();
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, {
      scrollable: true,
      size: "xl",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm khách hàng";
    modalRef.result.then(
      () => {
        this.loadDataFromApi();
        this.popover.open();
      },
      (err) => {}
    );
  }

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
        (err) => {}
      );
  }
}
