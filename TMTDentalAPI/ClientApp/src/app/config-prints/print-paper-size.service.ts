import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';


export class PrintPaperSizePaged {
  limit: number;
  offset: number;
  search: string;
}


export class PrintPaperSizeBasic {
  id: string;
  name: string;
}

export class PrintPaperSizePagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: PrintPaperSizeBasic[];
}

export class PrintPaperSizeSave {
  name: string;
  paperFormat: string;
  topMargin: number;
  bottomMargin: number;
  leftMargin: number;
  rightMargin: number;
  companyId: string;
}

export class PrintPaperSizeDisplay {
  id: string;
  name: string;
  paperFormat: string;
  topMargin: number;
  bottomMargin: number;
  leftMargin: number;
  rightMargin: number;
  companyId: string;
}

@Injectable({
  providedIn: 'root'
})

export class PrintPaperSizeService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/PrintPaperSizes";

  getPaged(val): Observable<PrintPaperSizePagging> {
    return this.http.get<PrintPaperSizePagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id): Observable<PrintPaperSizeDisplay> {
    return this.http.get<PrintPaperSizeDisplay>(this.base_api + this.apiUrl + "/" + id);
  }

  create(val): Observable<PrintPaperSizeSave> {
    return this.http.post<PrintPaperSizeSave>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + "/" + id);
  }

}
