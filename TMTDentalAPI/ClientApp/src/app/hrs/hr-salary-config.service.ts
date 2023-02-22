import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class HrSalaryConfigDisplay {
    id: string;
    defaultGlobalLeaveTypeId: string;
    companyId: string;
    defaultGlobalLeaveType: any;
}

export class HrSalaryConfigSave {
    defaultGlobalLeaveTypeId: string;
}

@Injectable()
export class HrSalaryConfigService {
    apiUrl = 'api/HrSalaryConfigs';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    get(): Observable<HrSalaryConfigDisplay> {
        return this.http.get<HrSalaryConfigDisplay>(this.baseApi + this.apiUrl);
    }

    create(val: HrSalaryConfigSave): Observable<HrSalaryConfigDisplay> {
        return this.http.post<HrSalaryConfigDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: HrSalaryConfigSave) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }

}
