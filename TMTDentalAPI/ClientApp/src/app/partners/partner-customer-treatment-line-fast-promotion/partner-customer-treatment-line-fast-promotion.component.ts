import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-partner-customer-treatment-line-fast-promotion',
  templateUrl: './partner-customer-treatment-line-fast-promotion.component.html',
  styleUrls: ['./partner-customer-treatment-line-fast-promotion.component.css']
})
export class PartnerCustomerTreatmentLineFastPromotionComponent implements OnInit {

  title = "Ưu đãi Dịch vụ";
  autoPromotions = [];
  allPromotions = [];
  form = {
    discountFixed: 0,
    discountPercent: 0,
    discountType: "percentage", //percentage
    code: '',
  };

  discountTypeDict = {
    VNĐ: "fixed",
    "%": "percentage",
  };
  @Input() saleOrderLine: SaleOrderLineDisplay = null;

  private discountSubject = new Subject<any>();

  private promotionSubject = new Subject<any>();
  private deleteSubject = new Subject<any>();


  isChange = false;

  constructor(
    public activeModal: NgbActiveModal,
    private promotionService: SaleCouponProgramService,
    private notificationService: NotifyService,
  ) { }

  ngOnInit() {
    // setTimeout(() => {
    //   this.loadDefaultPromotion();
    // }, 300);
    this.autoPromotions = this.allPromotions.filter(x => x.promoCodeUsage == 'no_code_needed');
  }

  onChangeDiscount(val) {
    if (!val.target.value || val.target.value == "") {
      this.form.discountFixed = 0;
      this.form.discountPercent = 0;
    }
  }

  onChangeDiscountType(value) {
    this.form.discountFixed = 0;
    this.form.discountPercent = 0;
  }

  getDiscountSJ() {
    return this.discountSubject.asObservable();
  }

  getPromotionSJ() {
    return this.promotionSubject.asObservable();
  }

  getDeleteSJ() {
    return this.deleteSubject.asObservable();
  }

  loadDefaultPromotion() {
    this.promotionService.getPromotionBySaleOrderLine(this.saleOrderLine.productId, this.saleOrderLine.orderPartnerId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }


  getAmountToApply() {
    return this.saleOrderLine.priceUnit * this.saleOrderLine.productUOMQty;
  }

  onApplyCoupon() {
    if (this.form.code.trim() == '') {
      this.notificationService.notify('error', 'Nhập mã khuyến mãi');
      return;
    }

    var exist = this.allPromotions.find(x => x.promoCode == this.form.code);
    if(!exist) {
      this.notificationService.notify('error', 'Mã khuyến mãi không chính xác');
      return;
    }
    this.promotionSubject.next(exist);
  }

  applyPromotion(item) {
    this.promotionService.get(item.id).subscribe((res: SaleCouponProgramDisplay) => {
      this.promotionSubject.next(res);
    });
  }

  applyDiscount() {
    this.discountSubject.next(this.form);
  }

  onDeletePromotion(item) {
    this.deleteSubject.next(item);
  }


  onClose() {

    this.activeModal.close(this.isChange ? true : false);
  }

  sumPromotion() {
    return this.saleOrderLine.promotions.reduce((total, cur) => { return total + cur.amount }, 0);
  }

  getListPromotion(type): any[] {
    return this.saleOrderLine.promotions.filter(x => x.type == type);
  }

  getApplied(item) {// item is salecouponprogram
    var index = this.saleOrderLine.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return this.saleOrderLine.promotions[index];
  }

  getPriceUnitPromotion(amount) {
    return this.saleOrderLine ? amount / this.saleOrderLine.productUOMQty : 0;
  }

}
