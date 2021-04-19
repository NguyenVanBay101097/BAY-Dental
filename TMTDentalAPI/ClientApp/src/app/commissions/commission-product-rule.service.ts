import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCategoryBasic } from '../product-categories/product-category.service';
import { ProductBasic3 } from '../products/product.service';

export class CommissionProductRule {
  id: string;
  categ: ProductCategoryBasic;
  categId: string;
  percentAdvisory: number;
  percentDoctor: number;
  percentAssistant: number;
  product: ProductBasic3;
  productId: string;
  companyId: string;
  appliedOn: string;
}

@Injectable({
  providedIn: 'root'
})

export class CommissionProductRuleService {
  apiUrl = 'api/CommissionProductRules';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getForCommission(commisionId: string) {
    return this.http.get<CommissionProductRule[]>(this.baseApi + this.apiUrl + '/GetForCommission', { params: { commissionId: commisionId } });
  }

  save(vals: any): Observable<CommissionProductRule[]> {
    return this.http.post<CommissionProductRule[]>(this.baseApi + this.apiUrl, vals);
  }
}
