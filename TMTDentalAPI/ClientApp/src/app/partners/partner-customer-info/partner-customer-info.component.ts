import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PartnerService } from '../partner.service';
import { PartnerDisplay, PartnerCategorySimple } from '../partner-simple';
import { HistorySimple } from 'src/app/history/history';
import { PartnerCategoryBasic } from 'src/app/partner-categories/partner-category.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PartnerCreateUpdateComponent } from '../partner-create-update/partner-create-update.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';

@Component({
  selector: 'app-partner-customer-info',
  templateUrl: './partner-customer-info.component.html',
  styleUrls: ['./partner-customer-info.component.css']
})

export class PartnerCustomerInfoComponent implements OnInit {

  constructor(private partnerService: PartnerService, private modalService: NgbModal,
    private notificationService: NotificationService) { }

  @Input() partnerId: string; // ID của khách hàng
  partner = new PartnerDisplay();
  @Output() updateChange = new EventEmitter<any>();

  ngOnInit() {
    if (this.partnerId) {
      this.getPartnerInfo();
    }
  }

  getPartnerInfo() {
    this.partnerService.getPartner(this.partnerId).subscribe(rs => {
      console.log(rs);
      this.partner = rs as PartnerDisplay;
    });
  }

  getGender(g: string) {
    if (g) {
      switch (g.toLowerCase()) {
        case 'female':
          return 'Nữ';
        case 'male':
          return 'Nam';
        default:
          return 'Khác';
      }
    }
  }

  getAge(y: number) {
    var today = new Date();
    return today.getFullYear() - y;
  }

  getAddress(partner) {
    var list = [];
    if (partner.street) {
      list.push(partner.street);
    }

    if (partner.ward && partner.ward.name) {
      list.push(partner.ward.name);
    }

    if (partner.district && partner.district.name) {
      list.push(partner.district.name);
    }

    if (partner.city && partner.city.name) {
      list.push(partner.city.name);
    }

    return list.join(', ');
  }

  getHistories(partner) {
    if (partner.histories) {
      var arr = new Array<string>();
      this.partner.histories.forEach(e => {
        arr.push(e.name);
      });
      return arr.join(', ');
    }
  }

  getCategories() {
    if (this.partner.categories) {
      var arr = new Array<string>();
      this.partner.categories.forEach(e => {
        arr.push(e.name);
      });
      return arr.join(', ');
    }
  }

  getReferral() {
    if (this.partner.source) {
      var s = this.partner.source;
      switch (s.type.toLowerCase()) {
        case 'normal':
          return this.partner.source.name;
        case 'referral':
          return this.partner.referralUser.name;
        case 'ads':
          return 'Quảng cáo';
        case 'friend':
          return 'Bạn bè';
      }
    }
  }

  openModal(id) {
    const modalRef = this.modalService.open(PartnerCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.cusId = id;
    modalRef.result.then(
      rs => {
        this.getPartnerInfo();
        this.updateChange.emit(null);
      },
      er => { }
    )
  }

  registerPayment(id: string) {
    this.partnerService.getDefaultRegisterPayment(id).subscribe(result => {
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh toán';
      modalRef.componentInstance.defaultVal = result;
      modalRef.result.then(() => {
        this.notificationService.show({
          content: 'Thanh toán thành công',
          hideAfter: 3000,
          position: { horizontal: 'right', vertical: 'bottom' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.updateChange.emit(null);
      }, () => {
      });
    });
  }

}

