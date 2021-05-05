import { Component, Input, OnDestroy, OnInit } from '@angular/core';
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
export class SaleOrderLinePromotionDialogComponent implements OnInit , OnDestroy {

  title = "Ưu đãi Dịch vụ";
  autoPromotions = [];
 @Input() saleOrderLine: SaleOrderLineDisplay = null;

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

  ngOnDestroy(): void {
    this.updateSubject.unsubscribe();
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

  onApplyCouponSuccess() {
    this.notificationService.notify('success', 'Thành công!');
    this.updateSubject.next(true);
    this.isChange = true;
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
