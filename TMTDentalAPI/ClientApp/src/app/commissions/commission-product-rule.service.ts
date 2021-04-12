import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProductBasic3 } from '../products/product.service';

export class CommissionProductRule {
  categName: string;
  categId: string;
  percentAdvisory: number;
  percentDoctor: number;
  percentAssistant: number;
  product: ProductBasic3;
  productId: string;
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
}
