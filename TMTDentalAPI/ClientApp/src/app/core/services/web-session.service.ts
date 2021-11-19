import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class WebSessionService {
    apiUrl = 'Web/Session';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getSessionInfo() {
        return this.http.get(this.baseApi + this.apiUrl + "/GetSessionInfo");
    }

    getCurrentUserInfo() {
        return this.http.get(this.baseApi + this.apiUrl + "/GetCurrentUserInfo");
    }
}
