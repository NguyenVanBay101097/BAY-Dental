import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { CardCardFilter, CardCardService } from 'src/app/card-cards/card-card.service';
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
  servicePreferenceCards : any[] = [];
  cardCards : any[] = [];
  @Input() saleOrderLine: SaleOrderLineDisplay = null;

  private updateSubject = new Subject<any>();

  isChange = false;
  code = '';
  isDiscountLine = false;

  private btnDiscountSubject = new Subject<any>();
  private btnPromoCodeSubject = new Subject<any>();
  private btnPromoNoCodeSubject = new Subject<any>();
  private btnPromoServiceCardSubject = new Subject<any>();
  private btnPromoCardCardSubject = new Subject<any>();
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
    private serviceCardsService: ServiceCardCardService,
    private cardCardService: CardCardService,
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDefaultPromotion();
      this.loadServiceCards();
      this.loadCardCards();
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
  
  getBtnPromoServiceCardObs() {
    return this.btnPromoServiceCardSubject.asObservable();
  }

  getBtnPromoCardCardObs() {
    return this.btnPromoCardCardSubject.asObservable();
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
    var name = item.saleCouponProgramId ? 'khuyến mãi' : 'ưu đãi';
    let modalRef = this.modelService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa ' + name;
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa ' + name + ' ?';
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

  getListPromotionApplied(): any[] {
    var types = ['promotion_program','service_card_card' , 'card_card'] ;
    return this.saleOrderLine.promotions.filter(x => types.includes(x.type));
  }

  getApplied(item) {// item is salecouponprogram
    var index = this.saleOrderLine.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return this.saleOrderLine.promotions[index];
  }

  getPriceUnitPromotion(amount) {
    return this.saleOrderLine ? amount / this.saleOrderLine.productUOMQty : 0;
  }

  loadServiceCards() {
    this.saleOrderLineService.getListServiceCardCardApplyable(this.saleOrderLine.id).subscribe((res: any) => {
      this.servicePreferenceCards = res;
    }, (error) => { console.log(error) });
  }

  loadCardCards() {  
    this.saleOrderLineService.getListCardCardApplyable(this.saleOrderLine.id).subscribe((res: any) => {
      this.cardCards = res;     
    }, (error) => { console.log(error) });
  }


  getNameCard(item){
    var discount = "";
    if(item.productPricelistItem)
    {
      discount = item.productPricelistItem.computePrice = "percentage" ? (item.productPricelistItem.percentPrice + "%") : ((item.productPricelistItem.fixedAmountPrice ?? 0) + "VNĐ");
    }

    return "Giảm " + " " + discount + " " + "theo thẻ ưu đãi" + " " + item.cardType.name;
  }

  getNameCardCard(item){
    var discount = "";
    if(item.productPricelistItem)
    {
      discount = item.productPricelistItem.computePrice = "percentage" ? (item.productPricelistItem.percentPrice + "%") : ((item.productPricelistItem.fixedAmountPrice ?? 0) + "VNĐ");
    }

    return "Giảm " + " " + discount + " " + "theo thẻ thành viên" + " " + item.type.name;
  }

  applyServiceCard(item) {
    this.btnPromoServiceCardSubject.next(item);
  }

  applyCardCard(item) {
    this.btnPromoCardCardSubject.next(item);
  }

  getAppliedCard(item) {
    var index = this.saleOrderLine.promotions.findIndex(x => x.serviceCardCardId == item.id);
    return this.saleOrderLine.promotions[index];
  }

  getAppliedCardCard(item) {
    var index = this.saleOrderLine.promotions.findIndex(x => x.cardCardId == item.id);
    return this.saleOrderLine.promotions[index];
  }

}
