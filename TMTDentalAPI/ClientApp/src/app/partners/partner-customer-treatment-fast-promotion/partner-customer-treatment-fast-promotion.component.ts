import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, observable, Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { SaleOrderDisplay } from 'src/app/sale-orders/sale-order-display';
import { SaleOrderPromotionSave, SaleOrderPromotionService } from 'src/app/sale-orders/sale-order-promotion.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
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
  autoPromotions = [];

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
    this.promotionService.getPromotionBySaleOrder().subscribe((res: any) => {
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

  popPromotion(item: SaleOrderPromotionSave) {
    var i = this.saleOrder.promotions.findIndex(x=> x == item);
    this.saleOrder.promotions.splice(i, 1);

    this.notificationService.notify('success', 'Thành công!');
    this.updateSubject.next(this.saleOrder);
    this.isChange = true;
  }

  pushPromotion(type, program = null) {
    var ob = new Subject<any>();
    var amount = 0;
    ob.subscribe(res => {
      var exist = this.saleOrder.promotions.find(x=> (x.type == type && type == 'discount') || (program && x.saleCouponProgramId == program.id));
      if(exist){
        exist.type == 'code_usage_program'?this.errorMsg='Chương trình khuyến mãi đã được áp dụng cho đơn hàng này' : this.notificationService.notify('error', 'Ưu đãi đã được áp dụng cho đơn hàng này');
        return;
      }

      this.saleOrder.promotions.push({
        amount: amount,
        type: type,
        discountType:  type == 'discount'? this.form.discountType : program.discountType,
        discountPercent: type == 'discount'? this.form.discountPercent : program.discountPercentage,
        discountFixed: type == 'discount'?  this.form.discountFixed : program.discountFixedAmount,
        saleCouponProgramId : program? program.id : null,
        name: type == 'discount' ? 'Giảm tiền' : program.name
      } as SaleOrderPromotionSave );
      this.errorMsg = '';
  
      this.notificationService.notify('success', 'Thành công!');
      this.isChange = true;
      this.updateSubject.next(this.saleOrder);
    });

    switch (type) {
      case 'discount':
        amount = this.form.discountType == this.discountTypeDict["%"] ? this.form.discountPercent * this.getAmountToApply() / 100 : this.form.discountFixed;
        ob.next();
        break;
      case 'code_usage_program':
        this.promotionService.getPromotionUsageCode(this.form.code).subscribe((result) => {
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
      this.errorMsg = 'Nhập mã khuyến mãi';
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
