import { HttpClient } from "@angular/common/http";
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
  journalId: string;
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
  journalId: string;
}

export class DataInvoiceFilter {
  dateFrom: string;
  dateTo: string;
  companyId: string;
  resultSelection: string;
}

export class CashBookReportFilter {
  dateFrom: string;
  dateTo: string;
  companyId: string;
  groupBy: string;
}

export class CashBookReportItem {
  date: string;
  begin: number;
  totalChi: number;
  totalThu: number;
  totalAmount: number;
}

export class SumaryCashBookFilter {
  dateFrom: string;
  dateTo: string;
  companyId: string;
  resultSelection: string;
  partnerType: string;
  accountCode: string;
}

export class SumaryCashBook {
  type: string;
  credit: number;
  debit: number
  balance: number;
}

@Injectable({
  providedIn: "root",
})
export class CashBookService {
  apiUrl = "api/CashBooks";

  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) { }

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

  getSumaryCashBookReport(val: any) {
    return this.http.post<SumaryCashBook>(this.baseApi + this.apiUrl + "/GetSumaryCashBookReport", val);
  }

  getDetails(val: any) {
    return this.http.post(
      this.baseApi + this.apiUrl + "/GetDetails",
      val
    );
  }

  getDataInvoices(val: any) {
    return this.http.post( this.baseApi + this.apiUrl + "/GetDataInvoices", val);
  }

  getTotal(val: any): Observable<number> {
    return this.http.post<number>(
      this.baseApi + this.apiUrl + "/GetTotal",
      val
    );
  }

  getChartReport(val: any) {
    return this.http.post<CashBookReportItem[]>(this.baseApi + this.apiUrl + "/GetChartReport", val);
  }

  exportExcelFile(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/ExportExcelFile", val, {
      responseType: "blob",
    });
  }

  changeData() {
    return this.http.post(this.baseApi + this.apiUrl + "/ChangeData", null);
  }


}
