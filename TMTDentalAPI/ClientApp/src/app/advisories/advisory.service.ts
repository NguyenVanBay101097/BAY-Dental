import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductBasic2 } from '../products/product.service';
import { ToothDisplay } from '../teeth/tooth.service';
import { ToothCategoryBasic } from '../tooth-categories/tooth-category.service';

export class AdvisorySave {
  customerId: string;
  customerSimple: any;
  userId: string;
  user: any;
  date: Date;
  toothCategoryId: string;
  teeth: ToothDisplay[];
  toothDiagnosis: any;
  product: any;
  note: string;
  companyId: string;
}

export class AdvisoryPaged {
  offset: number;
  limit: number;
  search: string;
  dateFrom?: any;
  dateTo?: any;
  customerId: string;
  toothIds: string[];
}

export class AdvisoryPagedResult {
  offset: number;
  limit: number;
  totalItems: number;
  items: any[];
}

export class AdvisoryDisplay {
  id: string;
  customerId: string;
  customer: any;
  userId: string;
  user: any;
  date: Date;
  toothCategoryId: string;
  toothCategory: ToothCategoryBasic;
  teeth: ToothDisplay[];
  toothDiagnosis: any;
  product: any;
  note: string;
  companyId: string;
}

export class AdvisoryDefaultGet {
  customerId: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdvisoryService {
  apiUrl = 'api/advisories';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val:any): Observable<AdvisoryPagedResult> {
    return this.http.get<AdvisoryPagedResult>(this.baseApi + this.apiUrl, {params: new HttpParams({fromObject: val})});
  }

  get(id): Observable<AdvisoryDisplay> {
    return this.http.get<AdvisoryDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }

  getDefault(val: AdvisoryDefaultGet): Observable<AdvisoryDisplay> {
    return this.http.post<AdvisoryDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
  }

  create(val: AdvisorySave): Observable<AdvisorySave> {
    return this.http.post<AdvisorySave>(this.baseApi + this.apiUrl, val);
  }

  update(val: AdvisorySave, id): Observable<AdvisorySave> {
    return this.http.put<AdvisorySave>(this.baseApi + this.apiUrl + "/" +id, val);
  }

  remove(id){
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

  getPrint(customerId) 
  {
    return this.http.get(this.baseApi + this.apiUrl+ "/" +customerId+ "/GetPrint");
  }
}
