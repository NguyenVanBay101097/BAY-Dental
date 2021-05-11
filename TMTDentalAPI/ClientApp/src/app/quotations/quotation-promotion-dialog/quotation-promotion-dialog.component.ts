import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { QuotationsDisplay } from '../quotation.service';

@Component({
  selector: 'app-quotation-promotion-dialog',
  templateUrl: './quotation-promotion-dialog.component.html',
  styleUrls: ['./quotation-promotion-dialog.component.css']
})
export class QuotationPromotionDialogComponent implements OnInit {
  title = "Ưu đãi phiếu điều trị";
  isChange: boolean = false;
  quotation: QuotationsDisplay = null;
  private updateSubject = new Subject<any>();
  autoPromotions = [];

  constructor(
    public activeModal: NgbActiveModal,
    private notificationService: NotifyService,
    private modelService: NgbModal,
    private promotionService: SaleCouponProgramService
  ) { }
  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
    }, 300);
  }

  loadDefaultPromotion() {
    this.promotionService.getPromotionBySaleOrder().subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }
  onClose() {
    this.activeModal.close(this.isChange ? true : false);
  }

  getListPromotion(type): any[] {
    return this.quotation.promotions.filter(x => x.type == type);
  }

  applyDiscount(value) {
    var val = {
      id: this.quotation.id,
      discountType: value.discountType,
      discountPercent: value.discountPercent,
      discountFixed: value.discountFixed,
    };
    // var apply$ = this.quotation ? this.quotationSevice.applyDiscountOnOrder(val) : this.quotationLineService.applyDiscountOnOrderLine(val);
    // apply$.subscribe((res) => {
    //   this.notificationService.notify('success', 'Thành công!');
    //   this.isChange = true;
    //   this.updateSubject.next(true);
    // });
  }

  getAmountToApply() {
    if (this.quotation) {
      return this.quotation.lines.reduce((total, cur) => {
        return total + cur.subPrice * cur.qty;
      }, 0);
    }
    return 0;
  }

  onDeletePromotion(item) {
    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = "Xóa ưu đãi";
    modalRef.componentInstance.body = `Bạn có muốn xóa ưu đãi ${item.name}?`
    // modalRef.result.then(() => {
    //   this.saleOrderPromotionService.removePromotion([item.id]).subscribe(res => {
    //     this.notificationService.notify('success', 'Thành công!');
    //     this.updateSubject.next(true);
    //     this.isChange = true;

    //   })
    // }, () => {
    // });
  }

  applyPromotion(item) {
    var val = {
      id: this.quotation.id,
      saleProgramId: item.id,
    };

    // var apply$ = this.quotation ? this.quotationSevice.applyPromotion(val) : this.quotationLineService.applyPromotion(val);
    // apply$.subscribe((res) => {
    //   this.notificationService.notify('success', 'Thành công!');
    //   this.updateSubject.next(true);
    //   this.isChange = true;

    // });
  }

  onApplyCouponSuccess() {
    this.notificationService.notify('success', 'Thành công!');
    this.updateSubject.next(true);
    this.isChange = true;
  }
  getApplied(item) {// item is salecouponprogram
    var index = this.quotation.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return index >= 0 ? this.quotation.promotions[index] : null;
  }

}
