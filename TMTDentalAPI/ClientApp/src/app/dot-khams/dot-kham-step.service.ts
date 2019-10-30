import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class DotKhamStepAssignDotKhamVM {
    ids: string[];
    dotKhamId: string;
}

@Injectable()
export class DotKhamStepService {
    apiUrl = 'api/DotKhamSteps';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    assignDotKham(val: DotKhamStepAssignDotKhamVM) {
        return this.http.post(this.baseApi + this.apiUrl + `/AssignDotKham`, val);
    }

    toggleIsDone(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + `/ToggleIsDone`, ids);
    }
}