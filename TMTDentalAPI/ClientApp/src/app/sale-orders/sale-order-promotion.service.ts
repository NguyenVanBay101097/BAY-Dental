import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class SaleOrderPromotionPaged {
  offset : number;
  limit : number;
  saleOrderId : string;
  saleOrderLineId: string;
}

export class SaleOrderPromotionSave {
name: string;
amount: number;
saleCouponProgramId: string;
saleOrderId: string;
saleOrderLineId: string;
discountType: string;
discountPercent: number;
discountFixed: number;
type: string;

}

export class HistoryPromotionRequest {
  offSet: number;
  limit: number;
  dateFrom: string;
  dateTo: string;
  searchOrder?: string;
  searchOrderLine?: string;
  saleCouponProgramId: string;
}

@Injectable({
  providedIn: 'root'
})
export class SaleOrderPromotionService {

  apiUrl = 'api/SaleOrderPromotions'
  constructor(
    private http: HttpClient, @Inject('BASE_API') private baseApi: string
  ) { }

  getPaged(val: any) :Observable<PagedResult2<any>>{
    return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl, {params: new HttpParams({fromObject: val})} );
  }

 removePromotion(ids: string[]) {
  return this.http.post(this.baseApi + this.apiUrl + '/RemovePromotion', ids );
  }

  GetHistoryPromotionByCouponProgram(val):Observable<PagedResult2<any>>{
    return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl + "/GetHistoryPromotionByCouponProgram",{params: new HttpParams({fromObject: val})});
  }

  ExportExcelFile(val){
    return this.http.post(this.baseApi + this.apiUrl + '/ExportExcelFile', val, { responseType: "blob" } );
  }
}
