import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class TCareMessageTemplatePaged {
    limit: number;
    offset: number;
    search: string;
}

export class TCareMessageTemplatePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: any[];
}

@Injectable({
    providedIn: 'root'
})

export class TCareMessageTemplateService {
    constructor(
        @Inject('BASE_API') private base_api: string,
        private http: HttpClient
    ) { }

    apiUrl = 'api/TCareMessageTemplates';

    getPaged(val: any): Observable<TCareMessageTemplatePaging> {
        return this.http.get<TCareMessageTemplatePaging>(this.base_api + this.apiUrl, { params: val });
    }

    get(id) {
        return this.http.get(this.base_api + this.apiUrl + '/' + id);
    }

    create(val) {
        return this.http.post(this.base_api + this.apiUrl, val);
    }

    update(id, value) {
        return this.http.put(this.base_api + this.apiUrl + '/' + id, value);
    }


    delete(id) {
        return this.http.delete(this.base_api + this.apiUrl + '/' + id);
    }
}
