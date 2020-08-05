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
  selector: 'app-partner-info',
  templateUrl: './partner-info.component.html',
  styleUrls: ['./partner-info.component.css']
})
export class PartnerInfoComponent implements OnInit {
  @Output() createNewEvent = new EventEmitter<any>();
  constructor(private service: PartnerService, private modalService: NgbModal,
    private notificationService: NotificationService) { }

  @Input() public partnerId: string; // ID của khách hàng
  // partnerId: string;
  partner = new PartnerDisplay();

  address: string;

  @Output() updateChange = new EventEmitter<any>();

  ngOnInit() {
    if (this.partnerId) {
      this.getPartnerInfo();
    }
  }

  getPartnerInfo() {
    var addArray = new Array<string>();
    this.service.getPartner(this.partnerId).subscribe(
      rs => {
        this.partner = rs as PartnerDisplay;
        if (rs.street !== null) {
          addArray.push(rs.street);
        }
        if (rs.ward && rs.ward.name) {
          addArray.push(rs.ward.name);
        }
        if (rs.district && rs.district.name) {
          addArray.push(rs.district.name);
        }
        if (rs.city && rs.city.name) {
          addArray.push(rs.city.name);
        }
        console.log(addArray);
        this.address = addArray.join(', ');
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

  getHistories() {
    if (this.partner.histories) {
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
        case 'other':
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
    this.service.getDefaultRegisterPayment(id).subscribe(result => {
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
