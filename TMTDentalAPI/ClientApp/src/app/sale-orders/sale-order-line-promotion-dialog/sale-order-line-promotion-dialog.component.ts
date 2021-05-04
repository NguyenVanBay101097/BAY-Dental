import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramPaged, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { SaleOrderDisplay } from '../sale-order-display';
import { SaleOrderLineDisplay } from '../sale-order-line-display';
import { SaleOrderPromotionService } from '../sale-order-promotion.service';

@Component({
  selector: 'app-sale-order-line-promotion-dialog',
  templateUrl: './sale-order-line-promotion-dialog.component.html',
  styleUrls: ['./sale-order-line-promotion-dialog.component.css']
})
export class SaleOrderLinePromotionDialogComponent implements OnInit {

  title = "Ưu đãi phiếu điều trị";
  saleOrderLine: SaleOrderLineDisplay = null;//input


// input
  autoPromotions = [];

  private updateSubject = new Subject<any>();

  isChange = false;
  code = '';


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


  getUpdateSJ() {
    return this.updateSubject.asObservable();
  }

  loadDefaultPromotion() {
    var val = new SaleCouponProgramPaged();
    val.limit = 0;
    val.offset = 0;
    val.programType = "promotion_program";
    val.promoCodeUsage = "no_code_needed";
    val.discountApplyOn = 'specific_products';
    val.productId = this.saleOrderLine ? this.saleOrderLine.productId : '';
    
    this.promotionService.getPaged(val).subscribe((res) => {
      this.autoPromotions = res.items;
    });
  }


  getAmountToApply() {

      return this.saleOrderLine.priceUnit * this.saleOrderLine.productUOMQty;
 
  }

  // pushAppliedPromotion(type, program = null) { // val: code or programId
  //   var amount = 0;

  //   switch (type) {
  //     case 'discount':
  //       amount = this.form.discountType == this.discountTypeDict["%"] ? this.form.discountPercent * this.getAmountToApply() / 100 : this.form.discountFixed;
  //       break;
  //   case 'code_usage_program':
  //     this.promotionService.getByCode(this.form.code).subscribe((res) => {
  //       amount = res.discountType == this.discountTypeDict["%"] ? res.discountPercentage * this.getAmountToApply() / 100 : res.discountFixedAmount;
  //     });
  //     break;
  //     case 'promotion_program':
  //       this.promotionService.get(program.id).subscribe((res: SaleCouponProgramDisplay) => {
  //       amount = res.discountType == this.discountTypeDict["%"] ? res.discountPercentage * this.getAmountToApply() / 100 : res.discountFixedAmount;
  //       });
  //       break;
  //     default:
  //       break;
  //   }
  //   this.saleOrder.promotions.push({
  //     amount: amount,
  //     type: type,
  //     discountType: this.form.discountType,
  //     discountPercent: this.form.discountPercent,
  //     discountFixed: this.form.discountFixed,
  //   } as SaleOrderPromotionSave);
  //   this.notificationService.notify('success', 'Thành công!');
  //   this.isChange = true;
  // }

  applyPromotionManual() {
      if (this.code.trim() == '') return;

      var val = {
        id: this.saleOrderLine.id,
        couponCode: this.code,
      };
      this.saleOrderSevice.applyCouponOnOrder(val).subscribe((res) => {
        this.notificationService.notify('success', 'Thành công!');
        this.updateSubject.next(true);
        this.isChange = true;
      });
  }

  applyPromotion(item) {
      var val = {
        id:this.saleOrderLine.id,
        saleProgramId: item.id,
      };

      var apply$ = this.saleOrderLineService.applyPromotion(val);
      apply$.subscribe((res) => {
        this.notificationService.notify('success', 'Thành công!');
        this.updateSubject.next(true);
        this.isChange = true;

      });
  }

  applyDiscount(value) {
      var val = {
        id: this.saleOrderLine.id,
        discountType: value.discountType,
        discountPercent: value.discountPercent,
        discountFixed: value.discountFixed,
      };
      var apply$ = this.saleOrderLineService.applyDiscountOnOrderLine(val);
      apply$.subscribe((res) => {
        this.notificationService.notify('success', 'Thành công!');
        this.isChange = true;
        this.updateSubject.next(true);
      });
  }

  onDeletePromotion(item) {

      let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
      modalRef.componentInstance.title = "Xóa ưu đãi";
      modalRef.componentInstance.body = `Bạn có muốn xóa ưu đãi ${item.name}?`
      modalRef.result.then(() => {
        this.saleOrderPromotionService.removePromotion([item.id]).subscribe(res => {
          this.notificationService.notify('success', 'Thành công!');
          this.updateSubject.next(true);
          this.isChange = true;

        })
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
    return  this.saleOrderLine.promotions[index];
  }

}
