import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
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

  private btnDiscountSubject = new Subject<any>();
  private btnPromoCodeSubject = new Subject<any>();
  private btnPromoNoCodeSubject = new Subject<any>();
  private btnDeletePromoSubject = new Subject<any>();

  constructor(
    public activeModal: NgbActiveModal,
    private modelService: NgbModal,
    private promotionService: SaleCouponProgramService,
  ) { }
  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
    }, 300);
  }

  getUpdateSJ() {
    return this.updateSubject.asObservable();
  }

  getBtnDiscountObs() {
    return this.btnDiscountSubject.asObservable();
  }

  getBtnPromoCodeObs() {
    return this.btnPromoCodeSubject.asObservable();
  }

  getBtnPromoNoCodeObs() {
    return this.btnPromoNoCodeSubject.asObservable();
  }

  getBtnDeletePromoObs() {
    return this.btnDeletePromoSubject.asObservable();
  }

  loadDefaultPromotion() {
    this.promotionService.getPromotionBySaleOrder(this.quotation.partner.id).subscribe((res: any) => {
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
    this.btnDiscountSubject.next(value);
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
    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa khuyến mãi?';
    modalRef.result.then(() => {
      this.btnDeletePromoSubject.next(item);

    });
  }

  applyPromotion(item) {
    this.btnPromoNoCodeSubject.next(item);
  }

  onApplyCouponSuccess(data) {
    this.btnPromoCodeSubject.next(data);
  }
  getApplied(item) {
    var index = this.quotation.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return index >= 0 ? this.quotation.promotions[index] : null;
  }

}
