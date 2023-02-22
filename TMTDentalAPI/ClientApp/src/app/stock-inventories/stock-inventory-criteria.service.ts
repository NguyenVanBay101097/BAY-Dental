import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class StockInventoryCriteriaPaged {
  limit: number;
  offset: number;
  search: string;
}

export class StockInventoryCriteriaPaging {
    limit: number;
    offset: number;
    totalItems: number;
    items: StockInventoryCriteriaBasic[];
}

export class StockInventoryCriteriaDisplay {
  id: string;
  name: string;
  note: string;
}

export class StockInventoryCriteriaBasic {
  id: string;
  name: string;
  note: string;
}

export class StockInventoryCriteriaSave {
  name: string;
  note: string;
}

@Injectable({
  providedIn: 'root'
})
export class StockInventoryCriteriaService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/StockInventoryCriterias";

  getPaged(val: any): Observable<StockInventoryCriteriaPaging> {
    return this.http.get<StockInventoryCriteriaPaging>(this.base_api + this.apiUrl,{params: new HttpParams({ fromObject: val }) } );
  }

  get(id): Observable<StockInventoryCriteriaDisplay> {
    return this.http.get<StockInventoryCriteriaDisplay>(this.base_api + this.apiUrl + "/" + id);
  }

  create(val): Observable<StockInventoryCriteriaSave> {
    return this.http.post<StockInventoryCriteriaDisplay>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

}
