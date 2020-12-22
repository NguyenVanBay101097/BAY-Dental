import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class LaboBiteJointPaged {
  search: string;
  limit: number;
  offset: number;
}

export class LaboBiteJointDisplay{
  id: string;
  name: string;
}

export class LaboBiteJointSave{
  name: string;
}

export class LaboBiteJointBasic {
  id: string;
  name: string;
}

export class LaboBiteJointPageResult {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  items: LaboBiteJointBasic[];
}

export class ImportExcelBaseViewModel {
  fileBase64: string;
}

export class LaboBiteJointPageSimple{
  search?: string;
  limit?: number;
  offset?: number;
}

@Injectable({
  providedIn: 'root'
})

export class LaboBiteJointService {
  apiUrl = "api/LaboBiteJoints";
  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) { }

  getPaged(val: any): Observable<LaboBiteJointPageResult> {
    return this.http.get<LaboBiteJointPageResult>(this.baseApi + this.apiUrl, {
      params: val,
    });
  }

  get(id: string): Observable<LaboBiteJointDisplay> {
    return this.http.get<LaboBiteJointDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }
  create(product: LaboBiteJointSave) {
    return this.http.post(this.baseApi + this.apiUrl, product);
  }

  update(id: string, product: LaboBiteJointSave) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, product);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

  ImportExcel(val: ImportExcelBaseViewModel) {
    return this.http.post(this.baseApi + this.apiUrl + "/ImportExcel", val);
  }

  autoComplete(val: LaboBiteJointPageSimple) {
    return this.http.post(this.baseApi + this.apiUrl + "/AutoComplete", val);
  }
}
