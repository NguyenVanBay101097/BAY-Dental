import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { UserSimple } from '../users/user-simple';

export class ApplicationRolePaged {
    search: string;
    limit: number;
    offset: number;
}

export class ApplicationRoleBasic {
    id: string;
    name: string;
}

export class ApplicationRoleDisplay {
    id: string;
    name: string;
    functions: string[];
    users: UserSimple[];
}

export class ApplicationRolePaging {
    limit: number;
    offset: number;
    totalItems: number;
    items: ApplicationRoleBasic[];
}

@Injectable()
export class RoleService {
    apiUrl = 'api/ApplicationRoles';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<ApplicationRolePaging> {
        return this.http.get<ApplicationRolePaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id: string): Observable<ApplicationRoleDisplay> {
        return this.http.get<ApplicationRoleDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: ApplicationRoleDisplay) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: ApplicationRoleDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    permissionTree() {
        return this.http.get(this.baseApi + this.apiUrl + "/PermissionTree");
    }
}