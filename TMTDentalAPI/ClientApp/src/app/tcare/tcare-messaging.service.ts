import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class TCareMessagingBasic
{
  id: string;
  scenarioName: string;
  campaignName: string;
  scheduleDate: string;
  partnerTotal: number;
  messageTotal: number;
  messageSentTotal: number;
  messageExceptionTotal: number;
  state: string;
}

export class TCareMessagingPaged {
  limit: number;
  offset: number;
  dateFrom: string;
  dateTo: string;
  tCareScenarioId: string;
}

export class TCareMessagingPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: TCareMessagingBasic[];
}

@Injectable({
  providedIn: 'root'
})
export class TcareMessagingService {

  constructor(
    @Inject('BASE_API') private base_api: string,
    private http: HttpClient
  ) {}

  private readonly apiUrl = "api/TCareMessagings"

  getPaged(val: any) {
    return this.http.get(this.base_api + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }
}
