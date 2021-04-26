import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { NotificationService } from "@progress/kendo-angular-notification";
import { SaleOrderService } from "src/app/core/services/sale-order.service";
import {
  SaleCouponProgramPaged,
  SaleCouponProgramService,
} from "src/app/sale-coupon-promotion/sale-coupon-program.service";
import { NotifyService } from "src/app/shared/services/notify.service";
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
    discountType: "fixed", //percentage
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
    private notificationService: NotifyService
  ) {}

  ngOnInit() {
    this.loadDefaultPromotion();
    this.loadAllPromotionApplied();
  }

  loadDefaultPromotion() {
    var val = new SaleCouponProgramPaged();
    val.limit = 0;
    val.offset = 0;
    val.programType = "promotion_program";
    val.promoCodeUsage = "no_code_needed";
    val.discountApplyOn = "on_order";
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

  applyDiscount() {
    if (this.form.discountPercent > 0 || this.form.discountFixed > 0) {
      var val = {
        id: this.salerOrderId,
        discountType: this.form.discountType,
        DiscountPercent: this.form.discountPercent,
        DiscountFixed: this.form.discountFixed,
      };
      this.saleOrderSevice.applyDiscountOnOrder(val).subscribe((res) => {
        console.log(res);
      });
    } 
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
}
