import { Component, Input, OnInit } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { GridDataResult } from "@progress/kendo-angular-grid";
import { NotificationService } from "@progress/kendo-angular-notification";
import { Subject } from "rxjs";
import { debounceTime, distinctUntilChanged, map } from "rxjs/operators";
import { PartnerSimple } from "src/app/partners/partner-simple";
import {
  PartnerFilter,
  PartnerService,
} from "src/app/partners/partner.service";
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
  phones_List: string[] = [];
  phoneSearch: string = "";
  loading: boolean = false;
  partners_List: PartnerSimple[] = [];

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
      return;
    }

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

  togglePopover(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      if (this.phones) {
        this.phones_List = this.phones.split(",");
      }
      if (this.phones_List.length) {
        this.phoneSearch = this.phones_List[0];
        this.loadDataFromApi();
      }
      popover.open();
    }
  }

  checkPhoneSearch(phone) {
    if (phone.length == 10) {
      this.loadDataFromApi();
    }
  }

  createPartner(popover) {
    popover.close();
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
        popover.open();
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
        },
        (err) => {}
      );
  }
}
