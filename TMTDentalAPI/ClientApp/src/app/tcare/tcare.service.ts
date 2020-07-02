import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export class TCareCampaignSave {
  name: string;
  graphXml: string;
}

export class TCareCampaignBasic {
  id: string;
  name: string;
  xml: string;
}

export class TCareCampaignPaged {
  search: string;
  limit: number;
  offset: number;
}

export class TCareCampaignDisplay {
  id: string;
  name: string;
  graphXml: string;
  state: string;
  sheduleStart: string;
  recurringJobId: string;
  active: boolean;
}

export class TCareScenarioDisplay {
  id: string;
  name: string;
  campaigns: TCareCampaignDisplay[];
}



export class TCarePropertySave {
  name: string;
  type: string;
  value: string;
}

export class Tag {
  id: string;
  name: string;
}

export class TCareReadMessage {
  name: string;
  tags: Tag[];
}


export class TCareRuleSave {
  type: string;
  campaignid: string;
  properties: TCarePropertySave[];
}

export class TCareRuleBasic {
  id: string;
  type: string;
}

export class TCareMessageSave {
  tCareCampaignId: string;
}

export class TCareMessageDisplay {
  methodType: string;
  intervalType: string;
  intervalNumber: number;
  sheduleDate: any;
  content: string;
  channelType: string;
  channelSocialId: string;
  tCareCampaignId: string;
}

export class TCareRuleCondition {
  type: string;
  name: string;
  value: string;
  displayValue: string;
  op: string; //toán tử (bằng : eq; không bằng: neq; chứa : contains; Không chứa: not_contains; Nhỏ hơn hoặc bằng: lte; Lớn hơn hoặc bằng: gte)
  //truyển kiểu String
}

export class TCareScenarioPaged {
  search: string;
  limit: number;
  offset: number;
}


export class TCareRule {
  logic: string;
  conditions: TCareRuleCondition[];
}

@Injectable({
  providedIn: 'root'
})
export class TcareService {

  constructor(
    @Inject('BASE_API') private base_api: string,
    private http: HttpClient
  ) { }
  private readonly apiUrlCampaign = "api/TCareCampaigns"
  private readonly apiUrlScenario = "api/TCareScenarios"
  //TCareCampaign

  nameCreate(val): Observable<TCareCampaignBasic> {
    return this.http.post<TCareCampaignBasic>(this.base_api + this.apiUrlCampaign + '/NameCreate', val);
  }

  update(id, value) {
    return this.http.put(this.base_api + this.apiUrlCampaign + '/' + id, value);
  }

  getPaged(val: any) {
    return this.http.get(this.base_api + this.apiUrlCampaign, { params: new HttpParams({ fromObject: val }) });
  }

  get(id): Observable<TCareCampaignDisplay> {
    return this.http.get<TCareCampaignDisplay>(this.base_api + this.apiUrlCampaign + '/' + id);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrlCampaign + '/' + id);
  }

  actionStartCampaign(val) {
    return this.http.post(this.base_api + this.apiUrlCampaign + "/ActionStartCampaign", val)
  }

  actionStopCampaign(val) {
    return this.http.post(this.base_api + this.apiUrlCampaign + "/ActionStopCampaign", val)
  }

  actionSetSheduleStartCampaign(val) {
    return this.http.post(this.base_api + this.apiUrlCampaign + '/ActionSetSheduleStartCampaign', val);
  }

  autocomplete(val: TCareCampaignPaged): Observable<TCareCampaignBasic[]> {
    return this.http.post<TCareCampaignBasic[]>(this.base_api + this.apiUrlCampaign + '/autocomplete', val);
  }

  getScenario(id): Observable<TCareScenarioDisplay> {
    return this.http.get<TCareScenarioDisplay>(this.base_api + this.apiUrlScenario + '/' + id);
  }

  getPagedScenario(params) {
    return this.http.get(this.base_api + this.apiUrlScenario, { params: params });
  }

  deleteScenario(id) {
    return this.http.delete(this.base_api + this.apiUrlScenario + '/' + id);
  }

  createScenario(val): Observable<TCareScenarioDisplay> {
    return this.http.post<TCareScenarioDisplay>(this.base_api + this.apiUrlScenario, val);
  }

  updateScenario(id, val) {
    return this.http.put(this.base_api + this.apiUrlScenario + "/" + id, val);
  }
}
