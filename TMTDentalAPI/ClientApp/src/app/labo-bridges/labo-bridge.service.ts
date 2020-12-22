import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class LaboBridgePaged {
  search: string;
  limit: number;
  offset: number;
}

export class LaboBridgeDisplay{
  id: string;
  name: string;
}

export class LaboBridgeSave{
  name: string;
}

export class LaboBridgeBasic {
  id: string;
  name: string;
}

export class LaboBridgePageResult {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  items: LaboBridgeBasic[];
}

export class ImportExcelBaseViewModel {
  fileBase64: string;
}

export class LaboBridgePageSimple{
  search?: string;
  limit?: number;
  offset?: number;
}

@Injectable({
  providedIn: 'root'
})

export class LaboBridgeService {
  apiUrl = "api/LaboBridges";
  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) { }

  getPaged(val: any): Observable<LaboBridgePageResult> {
    return this.http.get<LaboBridgePageResult>(this.baseApi + this.apiUrl, {
      params: val,
    });
  }

  get(id: string): Observable<LaboBridgeDisplay> {
    return this.http.get<LaboBridgeDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }
  create(product: LaboBridgeSave) {
    return this.http.post(this.baseApi + this.apiUrl, product);
  }

  update(id: string, product: LaboBridgeSave) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, product);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

  ImportExcel(val: ImportExcelBaseViewModel) {
    return this.http.post(this.baseApi + this.apiUrl + "/ImportExcel", val);
  }

  autoComplete(val: LaboBridgePageSimple) {
    return this.http.post(this.baseApi + this.apiUrl + "/AutoComplete", val);
  }
}
