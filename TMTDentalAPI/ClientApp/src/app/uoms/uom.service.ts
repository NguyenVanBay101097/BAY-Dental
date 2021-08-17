import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { UoMCategoryBasic } from '../uom-categories/uom-category.service';
import { Observable } from 'rxjs';

export class UoMPaged {
  search: string;
  limit: number;
  categoryId: string;
  productId: string;
  offset: number;
}

export class UoMBasic {
  id: string;
  name: string;
  uomType: string;
  categoryName: string;
  categoryId: string;
  active: boolean;
}

export class UoMDisplay {
  id: string;
  name: string;
  uomType: string;
  categoryId: string;
  factor: number;
  factorInv: number;
  rounding: number;
  category: UoMCategoryBasic;
  active: boolean;
}

export class UoMPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: UoMBasic[];
}



@Injectable({
  providedIn: 'root'
})
export class UomService {

  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }
  private readonly apiUrl = "api/UoMs"

  create(value): Observable<UoMCategoryBasic> {
    return this.http.post<UoMCategoryBasic>(this.base_api + this.apiUrl, value);
  }

  update(id, value) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, value);
  }

  getPaged(val: any) {
    return this.http.get(this.base_api + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id): Observable<UoMDisplay> {
    return this.http.get<UoMDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  autocomplete(val: UoMPaged): Observable<UoMBasic[]> {
    return this.http.post<UoMBasic[]>(this.base_api + this.apiUrl + '/autocomplete', val);
  }

  importExcel(val: any) {
    return this.http.post(this.base_api + this.apiUrl + '/ImportExcel', val);
  }
}
