import { Injectable, Inject } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FacebookUserProfileDisplay } from './facebook-user-profile-display';

@Injectable({
    providedIn: 'root'
})

export class FacebookUserProfileService {
    apiUrl = 'api/FacebookUserProfiles';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    get(id: string): Observable<FacebookUserProfileDisplay> {
        return this.http.get<FacebookUserProfileDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }
}
