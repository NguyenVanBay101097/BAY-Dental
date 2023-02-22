import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SmsMessageDetailPaged {
    limit: number;
    offset: number;
    search: string;
    smsCampaignId: string;
    dateFrom: string;
    dateTo: string;
    state: string;
    smsMessageId: string;
    companyId: string;
}

export class ReportTotalInput {
    dateFrom: string;
    dateTo: string;
    smsAccountId: string;
    smsCampaignId: string;
    companyId: string;
}

export class ReportCampaignPaged {
    limit: number;
    offset: number;
    search: string;
    dateFrom: string;
    dateTo: string;
    companyId: string;
}

export class ReportSupplierInput {
    smsSupplierCode: string;
}

@Injectable({
    providedIn: 'root'
})
export class SmsMessageDetailService {

    apiUrl: string = 'api/SmsMessageDetails';
    constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

    getPaged(val) {
        return this.http.get(this.base_api + this.apiUrl, { params: val });
    }

    ReSend(ids) {
        return this.http.post(this.base_api + this.apiUrl + '/ReSend', ids);
    }

    getPagedStatistic(val) {
        return this.http.get(this.base_api + this.apiUrl + '/GetPagedStatistic', { params: val });
    }

    getReportTotal(val) {
        return this.http.get(this.base_api + this.apiUrl + '/GetReportTotal', { params: val });
    }

    getReportCampaign(val) {
        return this.http.get(this.base_api + this.apiUrl + '/GetReportCampaign', { params: val });
    }

    getReportSupplier(val) {
        return this.http.get(this.base_api + this.apiUrl + '/GetReportSupplier', { params: val });
    }


    getReportSupplierSumaryChart(val) {
        return this.http.post(this.base_api + this.apiUrl + '/GetReportSupplierSumaryChart', val)
    }
}
