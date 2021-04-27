import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { NotificationService } from "@progress/kendo-angular-notification";
import { SaleOrderLineService } from "src/app/core/services/sale-order-line.service";
import { SaleOrderService } from "src/app/core/services/sale-order.service";
import {
  SaleCouponProgramPaged,
  SaleCouponProgramService,
} from "src/app/sale-coupon-promotion/sale-coupon-program.service";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { NotifyService } from "src/app/shared/services/notify.service";
import { setTimeout } from "timers";
import {
  SaleOrderPromotionPaged,
  SaleOrderPromotionService,
} from "../sale-order-promotion.service";

@Component({
  selector: "app-sale-order-promotion-dialog",
  templateUrl: "./sale-order-promotion-dialog.component.html",
  styleUrls: ["./sale-order-promotion-dialog.component.css"],
})
export class SaleOrderPromotionDialogComponent implements OnInit {
  title = "Ưu đãi phiếu điều trị";
  form = {
    discountFixed: 0,
    discountPercent: 0,
    discountType: "percentage", //percentage
    code: null,
  };

  salerOrderId = null;
  salerOrderLineId = null;

  discountTypeDict = {
    VNĐ: "fixed",
    "%": "percentage",
  };

  autoPromotions = [];
  appliedPromotions = [];

  constructor(
    public activeModal: NgbActiveModal,
    private promotionService: SaleCouponProgramService,
    private saleOrderPromotionService: SaleOrderPromotionService,
    private saleOrderSevice: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService,
    private notificationService: NotifyService,
    private modelService: NgbModal
  ) {}

  ngOnInit() {
    setTimeout(() => {

      this.loadDefaultPromotion();
    this.loadAllPromotionApplied();
    }, 300);
  }

  loadDefaultPromotion() {
    var val = new SaleCouponProgramPaged();
    val.limit = 0;
    val.offset = 0;
    val.programType = "promotion_program";
    val.promoCodeUsage = "no_code_needed";
    val.discountApplyOn = this.salerOrderId? "on_order" : 'specific_products';
    this.promotionService.getPaged(val).subscribe((res) => {
      this.autoPromotions = res.items;
    });
  }

  loadAllPromotionApplied() {
    var val = new SaleOrderPromotionPaged();
    val.limit = 0;
    val.offset = 0;
    val.saleOrderId = this.salerOrderId ? this.salerOrderId : "";
    val.saleOrderLineId = this.salerOrderLineId ? this.salerOrderLineId : "";
    this.saleOrderPromotionService.getPaged(val).subscribe((res) => {
      this.appliedPromotions = res.items;
    });
  }

  onChangeDiscount(val) {
    if(!val.target.value || val.target.value == "") {
      this.form.discountFixed = 0;
      this.form.discountPercent = 0;
    }
  }

  onChangeDiscountType(value) {
    this.form.discountFixed = 0;
    this.form.discountPercent = 0;
  }

  applyCoupon(){
    if(this.form.code.trim() == '') return;

      var val = {
        id: this.salerOrderId,
        couponCode: this.form.code,
      };
      this.saleOrderSevice.applyCouponOnOrder(val).subscribe((res) => {
        this.notificationService.notify('success', 'Thành công!');
        this.loadAllPromotionApplied();

      });
  }

  applyPromotion(item) {
      var val = {
        id: this.salerOrderId ? this.salerOrderId : this.salerOrderLineId,
        saleProgramId: item.id,
      }; 

      var apply$ = this.salerOrderId ? this.saleOrderSevice.applyPromotion(val) : this.saleOrderLineService.applyPromotion(val);
      apply$.subscribe((res) => {
      this.notificationService.notify('success', 'Thành công!');
      this.loadAllPromotionApplied();
    });
  }

  applyDiscount() {
    if (this.form.discountPercent > 0 || this.form.discountFixed > 0) {
      var val = {
        id: this.salerOrderId ? this.salerOrderId : this.salerOrderLineId,
        discountType: this.form.discountType,
        DiscountPercent: this.form.discountPercent,
        DiscountFixed: this.form.discountFixed,
      };
        var apply$ = this.salerOrderId ? this.saleOrderSevice.applyDiscountOnOrder(val) : this.saleOrderLineService.applyDiscountOnOrderLine(val);
      apply$.subscribe((res) => {
        this.notificationService.notify('success', 'Thành công!');
        this.loadAllPromotionApplied();
      });
    } 
  }

  onDeletePromotion(item) {
    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
      modalRef.componentInstance.title = "Xóa ưu đãi";
      modalRef.componentInstance.body = `Bạn có muốn xóa ưu đãi ${item.name}?`
      modalRef.result.then(() => {
        this.saleOrderPromotionService.removePromotion([item.id]).subscribe(res => {
          this.notificationService.notify('success', 'Thành công!');
          this.loadAllPromotionApplied();
        }) 
      }, () => {
      });
   
  }

  getListPromotion(type): any[]{
  return this.appliedPromotions.filter(x=> x.type == type)
  }

  getApplied(item) {
    var listApllied = this.getListPromotion('promotion_program');
    var index = listApllied.findIndex(x=> item.discountLineProductId == x.productId);
    return listApllied[index];
  }
}
