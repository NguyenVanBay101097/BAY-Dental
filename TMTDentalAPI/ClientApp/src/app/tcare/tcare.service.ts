import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChannelSocial } from '../socials-channel/facebook-page.service';

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
}

export class TCareCampaignPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: TCareCampaignBasic[];
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
  sheduleDate: string;
  content: string;
  channelType: string;
  channelSocialId: string;
  tCareCampaignId: string;
}

export class TCareRuleCondition {
  typeCondition: string;
  valueCondition: string;
  nameCondition: string;
  flagCondition: string;
}

export class TCareRule {
  logic: string;
  conditions: TCareRuleCondition[];
}

export class AudienceFilterItem {
  type: string;
  name: string;
  formula_type: string;
  formula_value: string;
  formula_display: string;
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
  private readonly apiUrlRules = "api/TCareRules";
  private readonly apiUrlMessage = "api/TCareMessagings"


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

  autocomplete(val: TCareCampaignPaged): Observable<TCareCampaignBasic[]> {
    return this.http.post<TCareCampaignBasic[]>(this.base_api + this.apiUrlCampaign + '/autocomplete', val);
  }

  //TCareRule

  createTCareRule(val: TCareRuleSave): Observable<TCareRuleBasic> {
    return this.http.post<TCareRuleBasic>(this.base_api + this.apiUrlRules, val);
  }

  updateTCareRuleBirthday(id, val: TCareRuleSave) {
    return this.http.put(`${this.base_api + this.apiUrlRules}/${id}/Birthday/`, val);
  }

  getTcareRuleBirthday(id) {
    return this.http.get(this.base_api + this.apiUrlRules + '/' + id);
  }

  deleteTCareRuleBirthday(id) {
    return this.http.delete(this.base_api + this.apiUrlRules + '/' + id);
  }

  //TCareMessage

  createTCareMessage(val: TCareMessageSave): Observable<TCareMessageDisplay> {
    return this.http.post<TCareMessageDisplay>(this.base_api + this.apiUrlMessage, val);
  }

  updateTCareMessage(id, val: TCareMessageSave) {
    return this.http.put(`${this.base_api + this.apiUrlMessage}/${id}`, val);
  }

  getTcareMessage(id): Observable<TCareMessageDisplay> {
    return this.http.get<TCareMessageDisplay>(this.base_api + this.apiUrlMessage + '/' + id);
  }

  deleteTCareMessage(id) {
    return this.http.delete(this.base_api + this.apiUrlMessage + '/' + id);
  }

}
