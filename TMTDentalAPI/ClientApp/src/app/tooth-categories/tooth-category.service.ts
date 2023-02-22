import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

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