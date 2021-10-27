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
import { CheckPermissionService } from "src/app/shared/check-permission.service";
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
  promotions: any[] = [];
  // input
  autoPromotions = [];
  private updateSubject = new Subject<any>();
  isChange = false;
  isDiscountOrder = false;
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
    private modelService: NgbModal,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    // setTimeout(() => {
    //   this.loadDefaultPromotion();
    // }, 0);
    this.loadDefaultPromotion();
    this.isDiscountOrder = this.checkPermissionService.check(["Basic.SaleOrder.DiscountOrder"]);
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
    if (this.saleOrder.partner){
      this.promotionService.getPromotionBySaleOrder(this.saleOrder.partner.id).subscribe((res: any) => {
        this.autoPromotions = res;
      });
    }
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
    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa khuyến mãi?';
    modalRef.result.then(() => {
      this.btnDeletePromoSubject.next(item);

    });
  }

  onClose() {
    this.activeModal.dismiss();
  }

  sumPromotion() {
    return this.promotions.reduce((total, cur) => { return total + cur.amount }, 0);
  }

  getListPromotion(type): any[] {
    return this.promotions.filter(x => x.type == type);
  }

  getApplied(item) {// item is salecouponprogram
    var index = this.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return this.promotions[index];
  }

}
