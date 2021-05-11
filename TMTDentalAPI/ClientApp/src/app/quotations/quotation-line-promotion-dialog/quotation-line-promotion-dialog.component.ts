import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { QuotationLineDisplay } from '../quotation.service';

@Component({
  selector: 'app-quotation-line-promotion-dialog',
  templateUrl: './quotation-line-promotion-dialog.component.html',
  styleUrls: ['./quotation-line-promotion-dialog.component.css']
})
export class QuotationLinePromotionDialogComponent implements OnInit {
  title = "Ưu đãi Dịch vụ";
  isChange: boolean = false;
  @Input() quotationLine: QuotationLineDisplay = null;

  constructor(
    public activeModal: NgbActiveModal,

  ) { }

  ngOnInit() {
  }

  applyDiscount(value) {
    var val = {
      id: this.quotationLine.id,
      discountType: value.discountType,
      discount: value.discount,
    };
    // var apply$ = this.saleOrderLineService.applyDiscountOnOrderLine(val);
    // apply$.subscribe((res) => {
    //   this.notificationService.notify('success', 'Thành công!');
    //   this.isChange = true;
    //   this.updateSubject.next(true);
    // });
}

onDeletePromotion(item) {

  // let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
  // modalRef.componentInstance.title = "Xóa ưu đãi";
  // modalRef.componentInstance.body = `Bạn có muốn xóa ưu đãi ${item.name}?`
  // modalRef.result.then(() => {
  //   this.saleOrderPromotionService.removePromotion([item.id]).subscribe(res => {
  //     this.notificationService.notify('success', 'Thành công!');
  //     this.updateSubject.next(true);
  //     this.isChange = true;

  //   })
  // }, () => {
  // });
}

  getListPromotion(type): any[] {
    return this.quotationLine.promotions.filter(x => x.type == type);
  }

  getPriceUnitPromotion(amount) {
    return this.quotationLine ? amount/this.quotationLine.qty : 0;
  }
  
  onClose() {
    this.activeModal.close(this.isChange ? true : false);
  }
}
