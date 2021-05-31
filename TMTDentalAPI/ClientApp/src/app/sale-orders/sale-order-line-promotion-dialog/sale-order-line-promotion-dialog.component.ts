import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramPaged, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { SaleOrderDisplay } from '../sale-order-display';
import { SaleOrderLineDisplay } from '../sale-order-line-display';
import { SaleOrderPromotionService } from '../sale-order-promotion.service';

@Component({
  selector: 'app-sale-order-line-promotion-dialog',
  templateUrl: './sale-order-line-promotion-dialog.component.html',
  styleUrls: ['./sale-order-line-promotion-dialog.component.css']
})
export class SaleOrderLinePromotionDialogComponent implements OnInit , OnDestroy {

  title = "Ưu đãi Dịch vụ";
  autoPromotions = [];
  @Input() saleOrderLine: SaleOrderLineDisplay = null;

  private updateSubject = new Subject<any>();

  isChange = false;
  code = '';

  private btnDiscountSubject = new Subject<any>();
  private btnPromoCodeSubject = new Subject<any>();
  private btnPromoNoCodeSubject = new Subject<any>();
  private btnDeletePromoSubject = new Subject<any>();

  constructor(
    public activeModal: NgbActiveModal,
    private promotionService: SaleCouponProgramService,
    private saleOrderPromotionService: SaleOrderPromotionService,
    private saleOrderSevice: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService,
    private notificationService: NotifyService,
    private modelService: NgbModal
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
    }, 0);
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
    this.promotionService.getPromotionBySaleOrderLine(this.saleOrderLine.productId, this.saleOrderLine.orderPartnerId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }

  getAmountToApply() {
    return this.saleOrderLine.priceUnit * this.saleOrderLine.productUOMQty;
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
    this.btnDeletePromoSubject.next(item);
  }

  onClose() {
    this.activeModal.dismiss();
  }

  sumPromotion() {
    return this.saleOrderLine.promotions.reduce((total, cur) => { return total + cur.amount }, 0);
  }

  getListPromotion(type): any[] {
    return this.saleOrderLine.promotions.filter(x => x.type == type);
  }

  getApplied(item) {// item is salecouponprogram
    var index = this.saleOrderLine.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return  this.saleOrderLine.promotions[index];
  }

  getPriceUnitPromotion(amount) {
    return this.saleOrderLine ? amount/this.saleOrderLine.productUOMQty : 0;
  }

}
