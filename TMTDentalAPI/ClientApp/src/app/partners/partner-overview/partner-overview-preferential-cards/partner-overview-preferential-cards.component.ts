import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ServiceCardCardsPreferentialCuDialogComponent } from 'src/app/service-card-cards/service-card-cards-preferential-cu-dialog/service-card-cards-preferential-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import * as moment from 'moment';

@Component({
  selector: 'app-partner-overview-preferential-cards',
  templateUrl: './partner-overview-preferential-cards.component.html',
  styleUrls: ['./partner-overview-preferential-cards.component.css']
})
export class PartnerOverviewPreferentialCardsComponent implements OnInit {
  @Input() partnerId: string;
  @Input() preferentialCards: any;
  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
  ) { }

  ngOnInit(): void {
  }

  createCard(){
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Tạo thẻ ưu đãi dịch vụ";
    modalRef.componentInstance.partnerId = this.partnerId;
    modalRef.result.then(result => {
      this.notifyService.notify('success', 'Lưu thành công');
    }, () => { });
  }

  editCard(item){
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Chỉnh sửa thẻ " + item.barcode;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(result => {
      this.notifyService.notify('success', 'Lưu thành công');
    }, () => { });
  }

  getState(state){
    switch (state){
      case 'draft':
        return 'Nháp';
      case 'in_use':
        return 'Đã kích hoạt';
      case 'locked':
        return 'Tạm dừng';
      case 'cancelled':
        return 'Hủy thẻ'; 
      default:
        return '';   
    }
  }

  getExpiry(item){
    let period = item?.cardType.period ? (item.cardType.period == 'year' ? 'Năm' : 'Tháng') : '';
    let nbrPeriod = item?.cardType.nbrPeriod || '';
    let activatedDate = item.activatedDate ? moment(item.activatedDate).format('DD/MM/YYYY') : '';
    let expiredDate = item.expiredDate ? moment(item.expiredDate).format('DD/MM/YYYY') : '';
    return nbrPeriod + ' ' + period + ' ('+ activatedDate + ' - ' + expiredDate + ') '; 
  }
}
