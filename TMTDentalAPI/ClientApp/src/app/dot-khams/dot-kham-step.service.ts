import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class DotKhamStepAssignDotKhamVM {
    ids: string[];
    dotKhamId: string;
}

export class DotKhamStepSetDone {
    ids: string[];
    dotKhamId: string;
    isDone: boolean;
}

@Injectable()
export class DotKhamStepService {
    apiUrl = 'api/DotKhamSteps';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    assignDotKham(val: DotKhamStepAssignDotKhamVM) {
        return this.http.post(this.baseApi + this.apiUrl + `/AssignDotKham`, val);
    }

    toggleIsDone(val: DotKhamStepSetDone) {
        return this.http.post(this.baseApi + this.apiUrl + `/ToggleIsDone`, val);
    }
}