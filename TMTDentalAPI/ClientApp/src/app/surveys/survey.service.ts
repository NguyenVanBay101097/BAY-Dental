import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EmployeeSimple } from '../employees/employee';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';

export class SurveyAssignmentPaged {
  limit: number;
  offset: number;
  search: string;
  status: string;
  dateFrom: string;
  dateTo: string;
  employeeId: string;
  IsGetScore: boolean;
}

export class SurveyAssignmentBasic {
  id: string;
  employeeName: string;
  employee: EmployeeSimple;
  saleOrderId: string;
  saleOrder: SaleOrderBasic;
  status: string;
  completeDate: string;
  userInputScore: number;
  userInputMaxScore: number;
  partnerName: string;
  partnerRef: string;
  partnerId: string;
  partnerPhone: string;
  partnerGender: string;
  partnerGenderDisplay: string;
  age: string;
  partnerBirthYear: string;
  partnerCategoriesDisplay: string;
}

export class SurveyAssignmentPagging {
  limit: number;
  offset: number;
  employeeId: string;
  totalItems: number;
  items: SurveyAssignmentBasic[];
}

export class SurveyAssignmentDefaultGet {
  saleOrderId: string;
  partnerName: string;
  employeeId: string;
  employee: EmployeeSimple;
  partnerRef: string;
  partnerId: string;
  partnerPhone: string;
  saleOrderName: string;
  dateOrder: string;
  status: string;
}

export class SurveyAssignmentGetCountVM {
  status: string;
  dateFrom: string;
  employeeId: string; 
  dateTo: string;
}

export class SurveyAssignmentDefaultGetPar {
  constructor() {
    this.IsRandomAssign = false;
  }
  IsRandomAssign: boolean
}

@Injectable({
  providedIn: 'root'
})
export class SurveyService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyAssignments"

  getPaged(val: any): Observable<SurveyAssignmentPagging> {
    return this.http.get<SurveyAssignmentPagging>(this.base_api + this.apiUrl, { params: val });
  }

  defaultGetList(val): Observable<SurveyAssignmentDefaultGet[]> {
    return this.http.post<SurveyAssignmentDefaultGet[]>(this.base_api + this.apiUrl + '/DefaultGetList', val);
  }

  createListAssign(vals) {
    return this.http.post(this.base_api + this.apiUrl + '/CreateList', vals);
  }

  getSumary(val): Observable<number> {
    return this.http.post<number>(this.base_api + this.apiUrl + '/GetSummary', val);
  }

  actionContact(ids: string[]) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionContact', ids);
  }

  actionDone(val: any) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionDone', val);
  }

  actionCancel(ids: string[]) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionCancel', ids);
  }

  updateAssignment(id: string, val: any) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

}
