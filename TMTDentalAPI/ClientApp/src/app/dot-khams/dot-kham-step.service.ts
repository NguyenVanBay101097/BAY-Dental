import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DotKhamStepDisplay } from './dot-khams';

export class DotKhamStepPaged {
    offset: number;
    limit: number;
    search: string;
    dateTo: string;
    dateFrom: string;
    userId: string;
    partnerId: string;
    doctorId: string;
}

export class DotKhamStepReport {
    stepName: string;
    date: string;
    doctorName: string;
    serviceName: string;
    assistantName: string;
    partnerName: string;
}

export class DotKhamStepPaggingReport {
    offset: number;
    limit: number;
    totalItems: number;
    items: DotKhamStepReport[];
}


export class DotKhamStepAssignDotKhamVM {
    ids: string[];
    dotKhamId: string;
}

export class DotKhamStepSetDone {
    ids: string[];
    dotKhamId: string;
    isDone: boolean;
}

export class DotKhamStepCloneInsert {
    id: string;
    cloneInsert: string;
}

@Injectable({ providedIn: 'root' })
export class DotKhamStepService {
    apiUrl = 'api/DotKhamSteps';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    assignDotKham(val: DotKhamStepAssignDotKhamVM) {
        return this.http.post(this.baseApi + this.apiUrl + `/AssignDotKham`, val);
    }

    toggleIsDone(val: DotKhamStepSetDone) {
        return this.http.post(this.baseApi + this.apiUrl + `/ToggleIsDone`, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    cloneInsert(val: DotKhamStepCloneInsert): Observable<DotKhamStepDisplay> {
        return this.http.post<DotKhamStepDisplay>(this.baseApi + this.apiUrl + `/CloneInsert`, val);
    }

    patch(id: string, patch: any) {
        return this.http.patch(this.baseApi + this.apiUrl + "/" + id, patch);
    }

    dotKhamStepReport(params): Observable<DotKhamStepPaggingReport> {
        return this.http.get<DotKhamStepPaggingReport>(this.baseApi + this.apiUrl + '/DotKhamStepReport', { params: params });
    }

    updateIsDone(val) {
        return this.http.post(this.baseApi + this.apiUrl + `/UpdateIsDone`, val);
    }
}