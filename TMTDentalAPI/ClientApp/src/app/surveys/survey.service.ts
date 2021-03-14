import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../employee-categories/emp-category';
import { EmployeeSimple } from '../employees/employee';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';
import { SaleOrderDisplay, SaleOrderDisplayVm } from '../sale-orders/sale-order-display';
import { SurveyCallContentDisplay } from './survey-callcontent.service';
import { SurveyUserInputDisplay } from './survey-userinput.service';

export class SurveyAssignmentPaged {
  limit: number;
  offset: number;
  search: string;
  status: string;
  dateFrom: string;
  dateTo: string;
  employeeId: string;
  IsGetScore: boolean;
  userId: string;
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
  assignDate: any;
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

export class SurveyAssignmentDisplay {
  id: string;
  partner: SurveyAssignmentDisplayPartner;
  saleOrder: SurveyAssignmentDisplaySaleOrder;
  saleLines: SurveyAssignmentDisplaySaleOrderLine[];
  dotKhams: SurveyAssignmentDisplayDotKham[];
  userInputId: string;
  userInput: SurveyUserInputDisplay;
  status: string;
  callContents: SurveyAssignmentDisplayCallContent[];
}

export class SurveyAssignmentDisplayPartner {
  avatar: string;
  name: string;
  ref: string;
  date: string;
  gender: string;
  genderDisplay: string;
  dateOfBirth: string;
  jobTitle: string;
  histories: string[];
  phone: string;
  email: string;
  address: string;
  categories: string[];
  age: string;
}

export class SurveyAssignmentDisplaySaleOrder {
  name: string;
  dateOrder: string;
  state: string;
  stateDisplay: string;
  amountTotal: number;
}

export class SurveyAssignmentDisplaySaleOrderLine {
  productName: string;
  employeeName: string;
  productUOMQty: number;
  teeth: string[];
  diagnostic: string;
}

export class SurveyAssignmentDisplayDotKham {
  id: string;
  date: string;
  reason: number;
  doctorName: string[];
  lines: SurveyAssignmentDisplayDotKhamLine[];
}

export class SurveyAssignmentDisplayDotKhamLine {
  nameStep: string;
  productName: string;
  note: number;
  teeth: string[];
}

export class SurveyAssignmentDisplayCallContent {
  id: string;
  name: string;
}

export class AssignmentActionDone {
  id: string;
  surveyUserInput: any;
}

export class SurveyAssignmentGetSummaryFilter {
  dateFrom: string;
  employeeId: string;
  dateTo: string;
  userId: string;
}

export class SurveyAssignmentDefaultGetPar {
  constructor() {
    this.isRandomAssign = false;
  }
  isRandomAssign: boolean
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
}

export class SurveyAssignmentUpdateEmployee {
  id: string;
  employeeId: string;
}

@Injectable({
  providedIn: 'root'
})
export class SurveyAssignmentService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyAssignments"

  getPaged(val: any): Observable<SurveyAssignmentPagging> {
    return this.http.get<SurveyAssignmentPagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id: string): Observable<SurveyAssignmentDisplay> {
    return this.http.get<SurveyAssignmentDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  defaultGetList(val) {
    return this.http.post(this.base_api + this.apiUrl + '/DefaultGetList', val);
  }

  createListAssign(vals) {
    return this.http.post(this.base_api + this.apiUrl + '/CreateList', vals);
  }

  getSumary(val) {
    return this.http.post(this.base_api + this.apiUrl + '/GetSummary', val);
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

  updateAssignment(val: any) {
    return this.http.post(this.base_api + this.apiUrl + '/UpdateEmployee', val);
  }

}
