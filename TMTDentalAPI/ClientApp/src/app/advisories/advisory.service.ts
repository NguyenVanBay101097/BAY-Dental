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

export class AdvisoryToothAdvise {
  customerId?: string;
  companyId?: string;
}

export class AdvisoryToothAdviseDisplay {
  toothIds: any[];
}

export class AdvisoryLine {
  date: Date;
  id: string;
  name: string;
  productName: string;
  doctorName: string;
  type: string;
}

export class AdvisoryLinePaged {
  offset: number;
  limit: number;
  advisoryId: string;
}

export class AdvisoryLinePagedResult {
  offset: number;
  limit: number;
  totalItems: number;
  items: AdvisoryLine[];
}

export class CreateFromAdvisoryInput {
  customerId: string;
  ids: string[];
}

@Injectable({
  providedIn: 'root'
})
export class AdvisoryService {
  apiUrl = 'api/advisories';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<AdvisoryPagedResult> {
    return this.http.get<AdvisoryPagedResult>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
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
    return this.http.put<AdvisorySave>(this.baseApi + this.apiUrl + "/" + id, val);
  }

  remove(id) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

  getPrint(customerId, ids) {
    return this.http.get(this.baseApi + this.apiUrl + "/GetPrint", { params: new HttpParams({ fromObject: { customerId: customerId, ids: ids } }) });
  }

  getToothAdvise(val?: AdvisoryToothAdvise) {
    return this.http.post(this.baseApi + this.apiUrl + "/" + "getToothAdvise", val);
  }

  getAdvisoryLinePaged(val: any): Observable<AdvisoryLinePagedResult> {
    return this.http.get<AdvisoryLinePagedResult>(this.baseApi + this.apiUrl + "/GetAdvisoryLines", { params: new HttpParams({ fromObject: val }) });
  }

  createQuotations(val:CreateFromAdvisoryInput){
    return this.http.post(this.baseApi + this.apiUrl + "/" + "CreateQuotation", val);
  }

  createSaleOrder(val:CreateFromAdvisoryInput){
    return this.http.post(this.baseApi + this.apiUrl + "/" + "CreateSaleOrder", val);
  }
  
}
