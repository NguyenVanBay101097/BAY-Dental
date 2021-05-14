import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { QuotationLineService } from '../quotation-line.service';
import { QuotationPromotionService } from '../quotation-promotion.service';
import { QuotationLineDisplay } from '../quotation.service';

@Component({
  selector: 'app-quotation-line-promotion-dialog',
  templateUrl: './quotation-line-promotion-dialog.component.html',
  styleUrls: ['./quotation-line-promotion-dialog.component.css']
})
export class QuotationLinePromotionDialogComponent implements OnInit {
  @Input() quotationLine: QuotationLineDisplay = null;
  title = "Ưu đãi Dịch vụ";
  isChange: boolean = false;
  autoPromotions = [];
  code = '';
  private updateSubject = new Subject<any>();

  constructor(
    public activeModal: NgbActiveModal,
    private notificationService: NotifyService,
    private modelService: NgbModal,
    private promotionService: SaleCouponProgramService,
    private quotationLineService: QuotationLineService,
    private quotationPromotionService: QuotationPromotionService
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
    this.promotionService.getPromotionBySaleOrderLine(this.quotationLine.productId).subscribe((res: any) => {
      this.autoPromotions = res;
    });
  }

  getAmountToApply() {
    return this.quotationLine.subPrice * this.quotationLine.qty;
  }

  onApplyCouponSuccess() {
    this.notificationService.notify('success', 'Thành công!');
    this.updateSubject.next(true);
    this.isChange = true;
  }

  applyPromotion(item) {
    var val = {
      id: this.quotationLine.id,
      saleProgramId: item.id,
    };

    var apply$ = this.quotationLineService.applyPromotion(val);
    apply$.subscribe((res) => {
      this.notificationService.notify('success', 'Thành công!');
      this.updateSubject.next(true);
      this.isChange = true;
    });
  }

  applyDiscount(value) {
    var val = {
      id: this.quotationLine.id,
      discountType: value.discountType,
      discountPercent: value.discountPercent,
      discountFixed: value.discountFixed,
    };
    var apply$ = this.quotationLineService.applyDiscountOnQuotationLine(val);
    apply$.subscribe((res) => {
      this.notificationService.notify('success', 'Thành công!');
      this.isChange = true;
      this.updateSubject.next(true);
    });
  }

  onDeletePromotion(item) {
    this.quotationPromotionService.removePromotion([item.id]).subscribe(res => {
      this.notificationService.notify('success', 'Thành công!');
      this.updateSubject.next(true);
      this.isChange = true;
    })
  }

  getListPromotion(type): any[] {
    return this.quotationLine.promotions.filter(x => x.type == type);
  }

  onClose() {
    this.activeModal.close(this.isChange ? true : false);
  }

  getApplied(item) {
    var index = this.quotationLine.promotions.findIndex(x => x.saleCouponProgramId == item.id);
    return this.quotationLine.promotions[index];
  }

  getPriceUnitPromotion(amount) {
    return this.quotationLine ? amount / this.quotationLine.qty : 0;
  }
}
