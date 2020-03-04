import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';

export class MarketingCampaignActivity {
    id: string;
    name: string;
    condition: string;
    daysNoSales: number;
    activityType: string;
    content: string;
    intervalType: string;
    intervalNumber: number;
    triggerType: string;
    everyDayTimeAt: string;
}

export class MarketingCampaign {
    id: string;
    name: string;
    state: string;
    activities: MarketingCampaignActivity[];
}

export class MarketingCampaignPaged {
    limit: number;
    offset: number;
    search: string;
    state: string;
}

@Injectable()
export class MarketingCampaignService {
    apiUrl = 'api/MarketingCampaigns';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<MarketingCampaign>> {
        return this.http.get<PagedResult2<MarketingCampaign>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<MarketingCampaign> {
        return this.http.get<MarketingCampaign>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: MarketingCampaign): Observable<MarketingCampaign> {
        return this.http.post<MarketingCampaign>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: MarketingCampaign) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }
}