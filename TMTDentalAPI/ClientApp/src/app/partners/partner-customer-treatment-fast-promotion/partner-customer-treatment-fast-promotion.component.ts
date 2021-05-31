import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, observable, Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { SaleOrderDisplay } from 'src/app/sale-orders/sale-order-display';
import { SaleOrderPromotionSave, SaleOrderPromotionService } from 'src/app/sale-orders/sale-order-promotion.service';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-partner-customer-treatment-fast-promotion',
  templateUrl: './partner-customer-treatment-fast-promotion.component.html',
  styleUrls: ['./partner-customer-treatment-fast-promotion.component.css']
})
export class PartnerCustomerTreatmentFastPromotionComponent implements OnInit {
  title = "Ưu đãi phiếu điều trị";
  saleOrder: SaleOrderDisplay = null;//input

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


  // input
  allPromotions = [];
  autoPromotions = [];

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
    this.promotionService.getPromotionBySaleOrder(this.saleOrder.partner.id).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }


  getAmountToApply() {
    if (this.saleOrder) {
      return this.saleOrder.orderLines.reduce((total, cur) => {
        return total + cur.priceUnit * cur.productUOMQty;
      }, 0);
    }
    return 0;
  }

  onApplyCoupon() {
    if (this.form.code.trim() == '') {
      this.notificationService.notify('error', 'Nhập mã khuyến mãi');
      return;
    }
    var exist = this.allPromotions.find(x => x.promoCode == this.form.code);
    if (!exist) {
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
    return this.saleOrder.promotions.reduce((total, cur) => { return total + cur.amount }, 0);
  }

  getListPromotion(type): any[] {
    return this.saleOrder.promotions.filter(x => x.type == type);
  }

  getApplied(item) {// item is salecouponprogram
    var index = this.saleOrder.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return this.saleOrder.promotions[index];
  }

}
