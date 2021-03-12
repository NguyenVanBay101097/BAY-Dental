import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PagedResult2 } from "../core/paged-result-2";

export class CashBookPaged {
  limit: number;
  offset: number;
  companyId: string;
  search: string;
  resultSelection: string;
  dateFrom: string;
  dateTo: string;
  begin: boolean;
}

export class CashBookSummarySearch {
  companyId: string;
  resultSelection: string;
  dateFrom: string;
  dateTo: string;
}

export class AccountMoveLineCashBookVM {
  date: string;
  name: string;
  ref: string;
  credit: number;
  debit: number;
  partnerName: string;
  journalType: string;
  companyId: string;
}

export class ReportDataResult {
  constructor(
    begin?: number,
    totalAmount?: number,
    totalChi?: number,
    totalThu?: number
  ) {
    this.begin = begin || 0;
    this.totalAmount = totalAmount || 0;
    this.totalChi = totalChi || 0;
    this.totalThu = totalThu || 0;
  }
  begin: number;
  totalAmount: number;
  totalChi: number;
  totalThu: number;
}

export class CashBookDetailFilter {
  companyId: string;
  resultSelection: string;
  dateFrom: string;
  dateTo: string;
  search: string;
  limit: number;
  offset: number;
}

@Injectable({
  providedIn: "root",
})
export class CashBookService {
  apiUrl = "api/CashBooks";

  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) {}

  getMoney(val: CashBookPaged): Observable<PagedResult2<AccountMoveLineCashBookVM>> {
    return this.http.post<PagedResult2<AccountMoveLineCashBookVM>>(
      this.baseApi + this.apiUrl + "/GetMoney",
      val
    );
  }

  getSumary(val: any): Observable<ReportDataResult> {
    return this.http.post<ReportDataResult>(
      this.baseApi + this.apiUrl + "/GetSumary",
      val
    );
  }

  getDetails(val: any) {
    return this.http.post(
      this.baseApi + this.apiUrl + "/GetDetails",
      val
    );
  }

  getTotal(val: any): Observable<number> {
    return this.http.post<number>(
      this.baseApi + this.apiUrl + "/GetTotal",
      val
    );
  }

  exportExcelFile(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/ExportExcelFile", {
      responseType: "blob",
      params: val,
    });
  }

  changeData(){
    return this.http.post(this.baseApi + this.apiUrl + "/ChangeData", null);
  }
}
