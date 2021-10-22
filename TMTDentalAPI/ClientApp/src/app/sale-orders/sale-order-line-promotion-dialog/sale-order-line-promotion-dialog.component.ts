import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleCouponProgramPaged, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { ServiceCardCardFilter, ServiceCardCardService } from 'src/app/service-card-cards/service-card-card.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
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
export class SaleOrderLinePromotionDialogComponent implements OnInit, OnDestroy {

  title = "Ưu đãi Dịch vụ";
  autoPromotions = [];
  servicePreferenceCards = [
    { id: 1, name: 'Chỉnh nha giảm 20%', amount: 200000 },
    { id: 1, name: 'Cạo vôi răng 30%', amount: 300000 },
    { id: 1, name: 'Điều trị tủy răng vĩnh viễn 15%', amount: 400000 },
  ];
  @Input() saleOrderLine: SaleOrderLineDisplay = null;

  private updateSubject = new Subject<any>();

  isChange = false;
  code = '';
  isDiscountLine = false;

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
    private checkPermissionService: CheckPermissionService,
    private serviceCardsService: ServiceCardCardService
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
      this.loadServiceCards();
    }, 0);
    this.isDiscountLine = this.checkPermissionService.check(["Basic.SaleOrder.DiscountLine"]);
  }

  ngOnDestroy(): void {
    this.updateSubject.unsubscribe();
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
    this.promotionService.getPromotionBySaleOrderLine(this.saleOrderLine.productId, this.saleOrderLine.orderPartnerId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }

  getAmountToApply() {
    return this.saleOrderLine.priceUnit * this.saleOrderLine.productUOMQty;
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

  loadServiceCards() {
    let val = new ServiceCardCardFilter();
    val.partnerId = this.saleOrderLine.orderPartnerId;
    val.productId = this.saleOrderLine.productId;
    val.state = 'in_use';
    this.serviceCardsService.getServiceCardCards(val).subscribe((res: any) => {
      console.log(res);
      this.servicePreferenceCards = res;
    }, (error) => { console.log(error) });
  }

  getNameCard(item){
    var discount = "";
    if(item.productPricelistItem)
    {
      discount = item.productPricelistItem.computePrice = "percentage" ? (item.productPricelistItem.percentPrice + "%") : ((item.productPricelistItem.fixedAmountPrice ?? 0) + "VNĐ");
    }

    return "Giảm " +  discount + "";
  }

  applyServiceCard(item) {

  }

  getAppliedCard(item) {
    return false;
  }

  onDeleteServiceCard(item) {

  }

}
