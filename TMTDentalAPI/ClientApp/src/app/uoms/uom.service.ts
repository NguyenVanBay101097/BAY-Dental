import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UoMCategoryBasic } from '../uom-categories/uom-category.service';
import { Observable } from 'rxjs';

export class UoMPaged {
  search: string;
  limit: number;
  offset: number;
}

export class UoMBasic {
  id: string;
  name: string;
  uomType: string;
  categoryName: string;
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

  getPaged(value): Observable<UoMPaging> {
    return this.http.get<UoMPaging>(this.base_api + this.apiUrl, { params: value });
  }

  get(id): Observable<UoMDisplay> {
    return this.http.get<UoMDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }


}
