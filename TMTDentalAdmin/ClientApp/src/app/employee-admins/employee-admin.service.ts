import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';


export class EmployeeAdminBasic {
    id: string;
    name: string;
}

export class EmployeeAdminSave {
    name: string;
}

export class EmployeeAdminPaged {
    search: string;
    limit: number;
    offset: number;
}

export class EmployeeAdminPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: EmployeeAdminBasic[];
}

@Injectable()
export class EmployeeAdminService {
    apiUrl = 'api/employeeAdmins';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<EmployeeAdminPaging> {
        return this.http.get<EmployeeAdminPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id);
    }

    create(val: EmployeeAdminSave) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/' + id + '/Update', val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }
}