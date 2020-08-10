import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class ToothBasic {
    id: string;
    name: string;
    category: object;
}

export class ToothDisplay extends ToothBasic {
    viTriHam: string;
    position: string;
}

export class ToothPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: ToothBasic[];
}

export class ToothFilter {
    categoryId: string;
}

@Injectable({ providedIn: 'root' })
export class ToothService {
    apiUrl = 'api/teeth';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getAllBasic(val: ToothFilter): Observable<ToothDisplay[]> {
        return this.http.post<ToothDisplay[]>(this.baseApi + this.apiUrl + '/getAllDisplay', val);
    }
}