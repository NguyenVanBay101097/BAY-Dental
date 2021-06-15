import { PartnerSimple } from './../partners/partner-simple';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class AgentPaged {
  limit: number;
  offset: number;
  search: string;
}

export class CommissionAgentFilter {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  companyId: string;
}

export class CommissionAgentDetailFilter {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  agentId: string;
}

export class CommissionAgentDetailItemFilter {
  limit: number;
  offset: number;
  dateFrom: string;
  dateTo: string;
  agentId: string;
  partnerId: string;
}

export class CommissionAgentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: CommissionAgentResult[];
}

export class CommissionAgentResult {
  agent: AgentBasic;
  amountTotal: number;
  amountCommissionTotal: number;
}

export class CommissionAgentDetailPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: CommissionAgentDetailResult[];
}

export class CommissionAgentDetailItemPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: CommissionAgentDetailItemResult[];
}

export class CommissionAgentDetailResult {
  partner: PartnerSimple;
  amountTotal: number;
  amountCommissionTotal: number;
}

export class CommissionAgentDetailItemResult {
  date: Date;
  orderName: string;
  productName: string;
  amount: number;
}

export class AgentBasic {
  id: string;
  name: string;
  phone: string;
}

export class AgentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: AgentBasic[];
}

export class AgentDisplay {
  id: string;
  name: string;
  gender: string
  birthYear: number;
  birthMonth: number;
  birthDay: number;
  jobTitle: string;
  Phone: string;
  email: string;
  address: string;
}

export class AgentSave {
  name: string;
  gender: string
  birthYear: number;
  birthMonth: number;
  birthDay: number;
  jobTitle: string;
  Phone: string;
  email: string;
  address: string;
}

export class TotalAmountAgentFilter {
  agentId: string;
  companyId: string;
  partnerId: string;
}


@Injectable({
  providedIn: 'root'
})

export class AgentService {
  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/Agents";

  getPaged(val): Observable<AgentPagging> {
    return this.http.get<AgentPagging>(this.base_api + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  getCommissionAgent(val): Observable<CommissionAgentPagging> {
    return this.http.get<CommissionAgentPagging>(this.base_api + this.apiUrl + '/GetCommissionAgent', { params: new HttpParams({ fromObject: val }) });
  }

  getCommissionAgentDetail(val): Observable<CommissionAgentDetailPagging> {
    return this.http.get<CommissionAgentDetailPagging>(this.base_api + this.apiUrl + '/GetCommissionAgentDetail', { params: new HttpParams({ fromObject: val }) });
  }

  getCommissionAgentDetailItem(val): Observable<CommissionAgentDetailItemPagging> {
    return this.http.get<CommissionAgentDetailItemPagging>(this.base_api + this.apiUrl + '/GetCommissionAgentDetailItem', { params: new HttpParams({ fromObject: val }) });
  }


  getAmountCommissionAgentToTal(val) {
    return this.http.get(this.base_api + this.apiUrl + '/GetCommissionAmountAgentTotal', { params: new HttpParams({ fromObject: val }) });
  }

  getAmountBalanceCommissionAgentForPartner(val) {
    return this.http.get<number>(this.base_api + this.apiUrl + '/GetAmountBalanceCommissionAgentForPartner', { params: new HttpParams({ fromObject: val }) });
  }

  get(id): Observable<AgentDisplay> {
    return this.http.get<AgentDisplay>(this.base_api + this.apiUrl + "/" + id);
  }

  create(val): Observable<AgentDisplay> {
    return this.http.post<AgentDisplay>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + "/" + id);
  }
}
