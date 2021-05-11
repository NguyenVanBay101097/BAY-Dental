import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { NotificationService } from "@progress/kendo-angular-notification";
import { Subject } from "rxjs";
import { SaleOrderLineService } from "src/app/core/services/sale-order-line.service";
import { SaleOrderService } from "src/app/core/services/sale-order.service";
import {
  SaleCouponProgramDisplay,
  SaleCouponProgramPaged,
  SaleCouponProgramService,
} from "src/app/sale-coupon-promotion/sale-coupon-program.service";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { NotifyService } from "src/app/shared/services/notify.service";
import { setTimeout } from "timers";
import { SaleOrderDisplay } from "../sale-order-display";
import { SaleOrderLineDisplay } from "../sale-order-line-display";
import {
  SaleOrderPromotionPaged,
  SaleOrderPromotionSave,
  SaleOrderPromotionService,
} from "../sale-order-promotion.service";

@Component({
  selector: "app-sale-order-promotion-dialog",
  templateUrl: "./sale-order-promotion-dialog.component.html",
  styleUrls: ["./sale-order-promotion-dialog.component.css"],
})
export class SaleOrderPromotionDialogComponent implements OnInit {

  title = "Ưu đãi phiếu điều trị";
  saleOrder: SaleOrderDisplay;//input
  // input
  autoPromotions = [];
  private updateSubject = new Subject<any>();
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
    // }, 0);
    this.loadDefaultPromotion();
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

  onApplyCouponSuccess() {
    this.notificationService.notify('success', 'Thành công!');
    this.updateSubject.next(true);
    this.isChange = true;
  }

  applyPromotion(item) {
    var val = {
      id: this.saleOrder.id,
      saleProgramId: item.id,
    };

    var apply$ = this.saleOrder ? this.saleOrderSevice.applyPromotion(val) : this.saleOrderLineService.applyPromotion(val);
    apply$.subscribe((res) => {
      this.notificationService.notify('success', 'Thành công!');
      this.updateSubject.next(true);
      this.isChange = true;

    });
  }

  applyDiscount(value) {
    var val = {
      id: this.saleOrder.id,
      discountType: value.discountType,
      discountPercent: value.discountPercent,
      discountFixed: value.discountFixed,
    };
    var apply$ = this.saleOrder ? this.saleOrderSevice.applyDiscountOnOrder(val) : this.saleOrderLineService.applyDiscountOnOrderLine(val);
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
