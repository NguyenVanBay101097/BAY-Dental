import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EmployeeSimpleContact } from '../employees/employee';
import { PartnerBasic } from '../partners/partner-simple';
import { AccountJournalBasic } from '../res-partner-banks/res-partner-bank.service';
import { ToaThuocDisplay, ToaThuocLineDisplay, ToaThuocVM } from '../toa-thuocs/toa-thuoc.service';

export class PrecscriptionPaymentPaged {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  state: string;
}

export class PrecscriptionPaymentVM {
  id: string;
  name: string;
  orderDate: string;
  employeeName: string;
  partnerName: string;
  saleOrderId: string;
  saleOrderName: string;
  amount: number;
  state: string;
}

export class PrecscriptionPaymentBasic {
  id: string;
  name: string;
}

export class PrescriptionPaymentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: PrecscriptionPaymentVM[];
}

export class PrecscriptPaymentDisplay {
  id: string;
  name: string;
  companyId: string;
  orderDate: string;
  note: string;
  employeeId: string;
  employee: EmployeeSimpleContact
  toaThuocId: string;
  toaThuoc: ToaThuocDisplay;
  journalId: string;
  journal: AccountJournalBasic;
  partnerId: string;
  partner: PartnerBasic;
  amount: string;
  state: string;
  medicineOrderLines: PrecscriptPaymentLineDisplay[];
}

export class PrecscriptionPaymentReport {
  constructor() {
    this.amountTotal = 0;
    this.medicineOrderCount = 0;
  }
  amountTotal: number;
  medicineOrderCount: number;
}

export class PrecscriptPaymentLineDisplay {
  id: string;
  toaThuocLineId: string;
  toaThuocLine: ToaThuocLineDisplay;
  quantity: number;
  price: number;
  amountTotal: number;
  productId: string;
}

export class PrecsriptionPaymentSave {
  orderDate: string;
  journalId: string;
  toaThuocId: string;
  employeeId: string;
  partnerId: string;
  companyId: string;
  note: string;
  amount: number;
  state: string;
  MedicineOrderLines: PrecsriptionPaymentLineSave[];
}

export class PrecsriptionPaymentLineSave {
  id: string;
  toaThuocLineId: string;
  quantity: number;
  price: number;
  amountTotal: number;
}

@Injectable({
  providedIn: 'root'
})
export class MedicineOrderService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/MedicineOrders";
  apiPrintUrl = "MedicineOrder";

  getPaged(val): Observable<PrescriptionPaymentPagging> {
    return this.http.get<PrescriptionPaymentPagging>(this.base_api + this.apiUrl, { params: val });
  }

  create(val): Observable<PrecscriptionPaymentVM> {
    return this.http.post<PrecscriptionPaymentVM>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val)
  }

  cancelPayment(ids) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionCancel', ids)
  }

  confirmPayment(val): Observable<PrecscriptionPaymentBasic> {
    return this.http.post<PrecscriptionPaymentBasic>(this.base_api + this.apiUrl + "/ActionPayment", val);
  }

  getDefault(id): Observable<PrecscriptPaymentDisplay> {
    return this.http.post<PrecscriptPaymentDisplay>(this.base_api + this.apiUrl + '/DefaultGet', { toaThuocId: id })
  }

  getDisplay(id): Observable<PrecscriptPaymentDisplay> {
    return this.http.get<PrecscriptPaymentDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  getReport(val): Observable<PrecscriptionPaymentReport> {
    return this.http.post<PrecscriptionPaymentReport>(this.base_api + this.apiUrl + '/GetReport', val);
  }

  // getPrint(id: string) {
  //   return this.http.get(this.base_api + this.apiUrl + "/" + id + '/GetPrint');
  // }

  getPrint(id: string) {
    return this.http.get(this.base_api + this.apiPrintUrl + "/Print" + `?id=${id}`, { responseType: 'text' });
  }
}
