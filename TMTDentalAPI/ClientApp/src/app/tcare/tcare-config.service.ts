import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class TCareConfigDisplay {
id: string;
jobCampaignHour: number;
jobCampaignMinute: number;
jobMessagingMinute: number;
jobMessageMinute: number;
}

@Injectable({
    providedIn: 'root'
  })

export class TCareConfigService {
    constructor(
        @Inject('BASE_API') private base_api: string,
        private http: HttpClient
      ) {}

      private readonly apiUrl = 'api/TCareConfigs';
      
      getFirst() {
          return this.http.get<TCareConfigDisplay> (this.base_api + this.apiUrl);
      }

      update(id, val) {
        return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
      }
}