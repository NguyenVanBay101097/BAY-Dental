import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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
  // advisoryUserId: string;
  // advisoryUser: any;
  // advisoryEmployee: any;
  // advisoryEmployeeId: string;
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
  promotions: any;
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
  dateQuotation: any;
  dateApplies: number;
  subPrice: number;
  listPrice: number;
  dateEndQuotation: string;
  note: string;
  totalAmount: number;
  state: string;
  lines: any[];
  payments: any[];
  companyId: string;
  orders: any[];
  promotions: any[];
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
    return this.http.get<QuotationsDisplay>(this.baseApi + this.apiUrl + '/' + id);
  }

  printQuotation(id: string) {
    return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/Print');
  }

  defaultGet(partnerId: string): Observable<QuotationsDisplay> {
    return this.http.get<QuotationsDisplay>(this.baseApi + this.apiUrl + '/GetDefault/' + partnerId);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
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
