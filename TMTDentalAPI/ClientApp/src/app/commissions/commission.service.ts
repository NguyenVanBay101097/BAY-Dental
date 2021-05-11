import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductSimple } from '../products/product-simple';
import { ProductCategoryBasic } from '../product-categories/product-category.service';

export class Commission {
  id: string;
  name: string;
}

export class CommissionPaged {
  offset: number;
  limit: number;
  search: string;
}

export class CommissionPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: any[];
}

export class CommissionProductRule {
  appliedOn: string;
  categ: ProductCategoryBasic;
  categId: string;
  percentAdvisory: number;
  percentDoctor: number;
  percentAssistant: number;
  product: ProductSimple;
  productId: string;
}

export class CommissionProductRuleDisplay {
  appliedOn: string;
  categ: ProductCategoryBasic;
  categId: string;
  percentAdvisory: number;
  percentDoctor: number;
  percentAssistant: number;
  product: ProductSimple;
  productId: string;
  companyId: string;
}

@Injectable({
  providedIn: 'root'
})
export class CommissionService {
  apiUrl = 'api/commissions';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<CommissionPaging> {
    return this.http.get<CommissionPaging>(this.baseApi + this.apiUrl, { params: val });
  }

  create(category: Commission) {
    return this.http.post(this.baseApi + this.apiUrl, category);
  }

  update(id: string, category: Commission) {
    return this.http.put(this.baseApi + this.apiUrl + '/' + id, category);
  }

  get(id: string) {
    return this.http.get<Commission>(this.baseApi + this.apiUrl + '/' + id);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }

  autocomplete(val: CommissionPaged): Observable<Commission[]> {
    return this.http.post<Commission[]>(this.baseApi + this.apiUrl + '/autocomplete', val);
  }

}
