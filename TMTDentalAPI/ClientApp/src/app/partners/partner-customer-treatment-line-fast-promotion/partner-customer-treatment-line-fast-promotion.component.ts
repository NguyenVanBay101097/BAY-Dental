import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { SaleOrderPromotionSave, SaleOrderPromotionService } from 'src/app/sale-orders/sale-order-promotion.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
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

  private updateSubject = new Subject<any>();

  isChange = false;
  errorMsg: string;

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

  ngOnDestroy(): void {
    this.updateSubject.unsubscribe();
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

  getUpdateSJ() {
    return this.updateSubject.asObservable();
  }

  loadDefaultPromotion() {
    this.promotionService.getPromotionBySaleOrderLine(this.saleOrderLine.productId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }


  getAmountToApply() {

    return this.saleOrderLine.priceUnit * this.saleOrderLine.productUOMQty;

  }

  popPromotion(item: SaleOrderPromotionSave) {
    var i = this.saleOrderLine.promotions.findIndex(x => x == item);
    this.saleOrderLine.promotions.splice(i, 1);

    this.notificationService.notify('success', 'Thành công!');
    this.updateSubject.next(this.saleOrderLine);
    this.isChange = true;
  }

  pushPromotion(type, program = null) {
    var ob = new Subject<any>();
    var amount = 0;
    ob.subscribe(res => {
      var exist = this.saleOrderLine.promotions.find(x => (x.type == type && type == 'discount') || (program && x.saleCouponProgramId == program.id));
      if (exist) {
        exist.type == 'code_usage_program' ? this.errorMsg = 'Chương trình khuyến mãi đã được áp dụng cho dịch vụ này' : this.notificationService.notify('error', 'Ưu đãi đã được áp dụng cho dịch vụ này');
        return;
      }

      this.saleOrderLine.promotions.push({
        amount: amount,
        type: type,
        discountType:  type == 'discount'? this.form.discountType : program.discountType,
        discountPercent: type == 'discount'? this.form.discountPercent : program.discountPercentage,
        discountFixed: type == 'discount'?  this.form.discountFixed : program.discountFixedAmount,
        saleCouponProgramId: program ? program.id : null,
        name: type == 'discount' ? 'Giảm tiền' : program.name
      } as SaleOrderPromotionSave);
      this.errorMsg = '';

      this.notificationService.notify('success', 'Thành công!');
      this.isChange = true;
      this.updateSubject.next(this.saleOrderLine);
    });

    switch (type) {
      case 'discount':

        var price_reduce = this.form.discountType == this.discountTypeDict["%"] ? this.saleOrderLine.priceUnit * (1 - this.form.discountPercent / 100) : this.saleOrderLine.priceUnit - this.form.discountFixed;
        amount = (this.saleOrderLine.priceUnit - price_reduce) * this.saleOrderLine.productUOMQty;
        ob.next();
        break;
      case 'code_usage_program':
        this.promotionService.getPromotionUsageCode(this.form.code, this.saleOrderLine.productId).subscribe((result) => {
          if (result && !result.success) {
            this.errorMsg = result.error;
            return;
          }
          let res = result.saleCouponProgram;
          amount = res.discountType == this.discountTypeDict["%"] ? res.discountPercentage * this.getAmountToApply() / 100 : res.discountFixedAmount;
          program = res;
          ob.next();
        });
        break;
      case 'promotion_program':
        this.promotionService.get(program.id).subscribe((res: SaleCouponProgramDisplay) => {
          amount = res.discountType == this.discountTypeDict["%"] ? res.discountPercentage * this.getAmountToApply() / 100 : res.discountFixedAmount;
          program = res;
          ob.next();
        });
        break;
      default:
        break;
    }
  }


  onApplyCoupon() {
    if (this.form.code.trim() == '') {
      this.errorMsg = 'Vui lòng nhập mã khuyến mãi';
      return;
    }
    this.pushPromotion('code_usage_program');
  }

  applyPromotion(item) {
    this.pushPromotion('promotion_program', item);
  }

  applyDiscount() {
    //push ưu đãi xong parent phân bổ
    this.pushPromotion('discount');
  }

  onDeletePromotion(item) {

    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = "Xóa ưu đãi";
    modalRef.componentInstance.body = `Bạn có muốn xóa ưu đãi ${item.name}?`
    modalRef.result.then(() => {
      this.popPromotion(item);
    }, () => {
    });
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
    return this.saleOrderLine ? amount/this.saleOrderLine.productUOMQty : 0;
  }

}
