import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { QuotationLineService } from '../quotation-line.service';
import { QuotationPromotionService } from '../quotation-promotion.service';
import { QuotationLineDisplay } from '../quotation.service';

@Component({
  selector: 'app-quotation-line-promotion-dialog',
  templateUrl: './quotation-line-promotion-dialog.component.html',
  styleUrls: ['./quotation-line-promotion-dialog.component.css']
})
export class QuotationLinePromotionDialogComponent implements OnInit {
  @Input() quotationLine: QuotationLineDisplay = null;
  @Input() partnerId: string;
  title = "Ưu đãi Dịch vụ";
  isChange: boolean = false;
  autoPromotions = [];
  // code = '';
  private updateSubject = new Subject<any>();
  
  private btnDiscountSubject = new Subject<any>();
  private btnPromoCodeSubject = new Subject<any>();
  private btnPromoNoCodeSubject = new Subject<any>();
  private btnDeletePromoSubject = new Subject<any>();

  constructor(
    public activeModal: NgbActiveModal,
    private notificationService: NotifyService,
    private modelService: NgbModal,
    private promotionService: SaleCouponProgramService,
    private quotationLineService: QuotationLineService,
    private quotationPromotionService: QuotationPromotionService
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
    }, 300);
  }

  ngOnDestroy(): void {
    this.updateSubject.unsubscribe();
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
    this.promotionService.getPromotionBySaleOrderLine(this.quotationLine.productId, this.partnerId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }

  getAmountToApply() {
    return this.quotationLine.subPrice * this.quotationLine.qty;
  }

  onApplyCouponSuccess(data) {
    this.btnPromoCodeSubject.next(data);
  }

  applyPromotion(item) {
    this.btnPromoNoCodeSubject.next(item);
  }

  applyDiscount(value) {
    this.btnDiscountSubject.next(value);
  }

  onDeletePromotion(item) {
    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa khuyến mãi?';
    modalRef.result.then(() => {
      this.btnDeletePromoSubject.next(item);

    });
  }

  getListPromotion(type): any[] {
    return this.quotationLine.promotions.filter(x => x.type == type);
  }

  onClose() {
    this.activeModal.close(this.isChange ? true : false);
  }

  getApplied(item) {
    var index = this.quotationLine.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return this.quotationLine.promotions[index];
  }

  getPriceUnitPromotion(amount) {
    return this.quotationLine ? amount / this.quotationLine.qty : 0;
  }
}
