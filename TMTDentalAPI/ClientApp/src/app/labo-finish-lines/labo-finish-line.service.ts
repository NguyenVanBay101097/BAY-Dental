import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class LaboFinishLinePaged {
  search: string;
  limit: number;
  offset: number;
}

export class LaboFinishLineDisplay{
  id: string;
  name: string;
}

export class LaboFinishLineSave{
  name: string;
}

export class LaboFinishLineBasic {
  id: string;
  name: string;
}

export class LaboFinishLinePageResult {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  items: LaboFinishLineBasic[];
}

export class ImportExcelBaseViewModel {
  fileBase64: string;
}

@Injectable({
  providedIn: 'root'
})

export class LaboFinishLineService {
  apiUrl = "api/LaboFinishLines";
  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) { }

  getPaged(val: any): Observable<LaboFinishLinePageResult> {
    return this.http.get<LaboFinishLinePageResult>(this.baseApi + this.apiUrl, {
      params: val,
    });
  }

  get(id: string): Observable<LaboFinishLineDisplay> {
    return this.http.get<LaboFinishLineDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }
  create(product: LaboFinishLineSave) {
    return this.http.post(this.baseApi + this.apiUrl, product);
  }

  update(id: string, product: LaboFinishLineSave) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, product);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

  ImportExcel(val: ImportExcelBaseViewModel) {
    return this.http.post(this.baseApi + this.apiUrl + "/ImportExcel", val);
  }
}
