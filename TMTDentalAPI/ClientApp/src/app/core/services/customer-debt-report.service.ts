import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class CustomerDebtFilter {
  offset: number;
  limit: number;
  dateFrom: string;
  dateTo: string;
  search: string;
  partnerId: string;
  companyId: string;
}

export class CustomerDebtPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: CustomerDebtResult[];
}

export class AmountCustomerDebtFilter {
 partnerId: string;
 companyId: string;
}

export class CustomerDebtResult {
  id: string;
  name: string;
  paymentName: string;
  paymentDate: Date;
  paymentAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerDebtReportService {
  apiUrl = 'api/ReportCustomerDebts';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getReports(val): Observable<CustomerDebtPagging> {
    return this.http.get<CustomerDebtPagging>(this.baseApi + this.apiUrl + "/GetReports", { params: val });
  }

  getAmountDebtTotal(val){
    return this.http.get(this.baseApi + this.apiUrl  + '/GetAmountDebtTotal' , { params: val });
  }

  exportExcelFile(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/ExportExcelFile", {
      responseType: "blob",
      params: val,
    });
  }

}
