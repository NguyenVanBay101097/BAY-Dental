import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class ToothCategoryBasic {
    id: string;
    name: string;
}

@Injectable({ providedIn: 'root' })
export class ToothCategoryService {
    apiUrl = 'api/toothcategories';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getAll(): Observable<ToothCategoryBasic[]> {
        return this.http.get<ToothCategoryBasic[]>(this.baseApi + this.apiUrl + '/getAll');
    }

    getDefaultCategory(): Observable<ToothCategoryBasic> {
        return this.http.get<ToothCategoryBasic>(this.baseApi + this.apiUrl + '/GetDefaultCategory');
    }
}