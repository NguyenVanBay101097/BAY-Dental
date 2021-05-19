import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { SaleOrderPromotionSave, SaleOrderPromotionService } from 'src/app/sale-orders/sale-order-promotion.service';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-partner-customer-treatment-line-fast-promotion',
  templateUrl: './partner-customer-treatment-line-fast-promotion.component.html',
  styleUrls: ['./partner-customer-treatment-line-fast-promotion.component.css']
})
export class PartnerCustomerTreatmentLineFastPromotionComponent implements OnInit {

  title = "Ưu đãi Dịch vụ";
  autoPromotions = [];
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
    private saleOrderPromotionService: SaleOrderPromotionService,
    private saleOrderSevice: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService,
    private notificationService: NotifyService,
    private modelService: NgbModal
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
    }, 300);
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
    this.promotionService.getPromotionBySaleOrderLine(this.saleOrderLine.productId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }


  getAmountToApply() {
    return this.saleOrderLine.priceUnit * this.saleOrderLine.productUOMQty;
  }

  onApplyCoupon() {
    if (this.form.code.trim() == '') {
      // this.errorMsg = 'Nhập mã khuyến mãi';
      return;
    }
    this.promotionService.getPromotionUsageCode(this.form.code, this.saleOrderLine.productId).subscribe((result) => {
      if (result && !result.success) {
        // this.errorMsg = result.error;
        return;
      }
      this.promotionSubject.next(result.saleCouponProgram);
    });
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
