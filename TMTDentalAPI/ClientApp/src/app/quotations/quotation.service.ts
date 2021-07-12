import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PagedResult2 } from '../core/paged-result-2';
import { EmployeeSimple } from '../employees/employee';
import { PartnerSimple } from '../partners/partner-simple';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';
import { ToothBasic } from '../teeth/tooth.service';
import { UserSimple } from '../users/user-simple';

// import { PagedResult2 } from '../paged-result-2';

export class QuotationLineDisplay {
  productId: string;
  id: string;
  product: any;
  qty: number;
  discount: number;
  discountType: string;
  subPrice: number;
  diagnostic: string;
  toothCategoryId: string;
  employee: any;
  employeeId: string;
  assistant: any;
  assistantId: string;
  counselor: any;
  counselorId: string;
  advisoryId: string;
  toothCategory: any;
  toothIds: any[];
  name: string;
  amount: number;
  teeth: any;
  toothType: string;
  promotions: QuotationPromotionBasic[];
  amountDiscountTotal: number;
  amountPromotionToOrder: number;
  amountPromotionToOrderLine: number;
}
export class QuotationLineSave {
  productId: string;
  qty: number;
  discount: number;
  discountType: string;
  subPrice: number;
  diagnostic: number;
  toothCategoryId: number;
  toothIds: any[];
  companyId: string;
}

export class PaymentQuotationDisplay {
  id: string;
  payment: number;
  discountPercentType: string;
  amount: number;
  date: Date;
}
export class QuotationsBasic {
  id: string;
  name: string;
  partner: PartnerSimple;
  user: UserSimple;
  dateQuotation: string;
  dateApplies: number;
  dateEndQuotation: string;
  note: string;
  totalAmount: number;
  state: string;
  toothType: string;
}

export class QuotationPromotionBasic {
  id: string;
  name: string;
  saleCouponProgramId: string;
  amount: number;
  type: string;
}

export class SaleOrderSimple {
  id: string;
  name: string;
}
export class QuotationsDisplay {
  /**
   *
   */
  constructor() {
    this.subPrice = this.listPrice;
  }
  id: string;
  name: string;
  partner: PartnerSimple;
  partnerId: string;
  user: UserSimple;
  userId: string;
  employee: EmployeeSimple;
  employeeId: string;
  dateQuotation: Date;
  dateApplies: number;
  subPrice: number;
  listPrice: number;
  dateEndQuotation: Date;
  note: string;
  totalAmount: number;
  state: string;
  lines: QuotationLineDisplay[];
  payments: PaymentQuotationDisplay[];
  companyId: string;
  orders: SaleOrderSimple[];
  promotions: QuotationPromotionBasic[];
  totalAmountDiscount: number;
}

export class QuotationSimple {
  id: string;
  name: string;
}

export class QuotationsPaged {
  dateFrom: string;
  dateTo: string;
  search: string;
  limit: number;
  partnerId: string;
  offset: number;
}
@Injectable({
  providedIn: 'root'
})
export class QuotationService {
  apiUrl = 'api/Quotations';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<QuotationsBasic>> {
    return this.http.get<PagedResult2<QuotationsBasic>>(this.baseApi + this.apiUrl, { params: val });
  }

  create(val: any) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val: any) {
    return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
  }

  get(id: string): Observable<QuotationsDisplay> {
    return this.http.get(this.baseApi + this.apiUrl + '/' + id)
      .pipe(
        map(
          (response: any) =>
            <QuotationsDisplay>{
              id: response.id,
              name: response.name,
              partner: response.partner,
              partnerId: response.partnerId,
              employee: response.employee,
              employeeId: response.employeeId,
              dateQuotation: new Date(response.dateQuotation),
              dateApplies: response.dateApplies,
              subPrice: response.subPrice,
              listPrice: response.listPrice,
              dateEndQuotation: new Date(response.dateEndQuotation),
              note: response.note,
              totalAmount: response.totalAmount,
              state: response.state,
              lines: response.lines.map(x => <QuotationLineDisplay>{
                productId: x.productId,
                id: x.id,
                product: x.product,
                qty: x.qty,
                subPrice: x.subPrice,
                diagnostic: x.diagnostic,
                toothCategoryId: x.toothCategoryId,
                employee: x.employee,
                employeeId: x.employeeId,
                assistant: x.assistant,
                assistantId: x.assistantId,
                counselor: x.counselor,
                counselorId: x.counselorId,
                toothCategory: x.toothCategory,
                toothIds: x.toothIds,
                name: x.name,
                amount: x.amount,
                teeth: x.teeth,
                toothType: x.toothType,
                promotions: x.promotions,
                amountDiscountTotal: x.amountDiscountTotal,
                amountPromotionToOrder: x.amountPromotionToOrder,
                amountPromotionToOrderLine: x.amountPromotionToOrderLine,
              }),
              payments: response.payments.map(x => <PaymentQuotationDisplay>{
                amount: x.amount,
                date: new Date(x.date),
                discountPercentType: x.discountPercentType,
                payment: x.payment,
                id: x.id
              }),
              companyId: response.companyId,
              orders: response.orders.map(x => <SaleOrderSimple>{
                id: x.id,
                name: x.name,
              }),
              promotions: response.promotions.map(x => <QuotationPromotionBasic>{
                id: x.id,
                name: x.name,
                saleCouponProgramId: x.saleCouponProgramId,
                type: x.type,
                amount: x.amount
              }),
              totalAmountDiscount: response.totalAmountDiscount
            }
        )
      );
  }

  printQuotation(id: string) {
    return this.http.get(this.baseApi + 'Quotation' + '/Print' + `?id=${id}`, { responseType: 'text' });
  }

  defaultGet(partnerId: string): Observable<QuotationsDisplay> {
    return this.http.get<QuotationsDisplay>(this.baseApi + this.apiUrl + '/GetDefault/' + partnerId)
      .pipe(
        map(
          (response: any) =>
            <QuotationsDisplay>{
              id: response.id,
              name: response.name,
              partner: response.partner,
              partnerId: response.partnerId,
              employee: response.employee,
              employeeId: response.employeeId,
              dateQuotation: new Date(response.dateQuotation),
              dateApplies: response.dateApplies,
              subPrice: response.subPrice,
              listPrice: response.listPrice,
              dateEndQuotation: new Date(response.dateEndQuotation),
              note: response.note,
              totalAmount: response.totalAmount,
              state: response.state,
              lines: response.lines.map(x => <QuotationLineDisplay>{
                productId: x.productId,
                id: x.id,
                product: x.product,
                qty: x.qty,
                subPrice: x.subPrice,
                diagnostic: x.diagnostic,
                toothCategoryId: x.toothCategoryId,
                employee: x.employee,
                employeeId: x.employeeId,
                assistant: x.assistant,
                assistantId: x.assistantId,
                counselor: x.counselor,
                counselorId: x.counselorId,
                toothCategory: x.toothCategory,
                toothIds: x.toothIds,
                name: x.name,
                amount: x.amount,
                teeth: x.teeth,
                toothType: x.toothType,
                promotions: x.promotions,
                amountDiscountTotal: x.amountDiscountTotal,
                amountPromotionToOrder: x.amountPromotionToOrder,
                amountPromotionToOrderLine: x.amountPromotionToOrderLine,
              }),
              payments: response.payments.map(x => <PaymentQuotationDisplay>{
                amount: x.amount,
                date: new Date(x.date),
                discountPercentType: x.discountPercentType,
                payment: x.payment
              }),
              companyId: response.companyId,
              // orders: any[],
              promotions: []
            }
        )
      );
  }

  delete(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
  }

  createSaleOrderByQuotation(id: string) {
    return this.http.get(`${this.baseApi + this.apiUrl}/${id}/CreateSaleOrderByQuotation`);
  }

  applyDiscountOnQuotation(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/ApplyDiscountOnQuotation', val);
  }

  applyDiscountOnQuotationLine(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/ApplyDiscountOnQuotationLine', val);
  }

  applyCouponOnQuotation(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/ApplyPromotionUsageCode', val);
  }

  applyPromotion(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/ApplyPromotion', val);
  }
}
