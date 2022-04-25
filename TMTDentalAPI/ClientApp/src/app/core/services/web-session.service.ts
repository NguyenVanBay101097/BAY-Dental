import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class WebSessionService {
    apiUrl = 'Web/Session';
    private loadedSubject: BehaviorSubject<any>;

    public $loaded: Observable<any>;

    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { 
        this.loadedSubject = new BehaviorSubject<any>(null);
        this.$loaded = this.loadedSubject.asObservable();
    }

    getSessionInfo() {
        return this.http.get(this.baseApi + this.apiUrl + "/GetSessionInfo")
        .pipe(
            mergeMap((result: any) => {
                localStorage.setItem('session_info', JSON.stringify(result));
                this.loadedSubject.next(result);
                return of(result);
            })
        );
    }

    getCurrentUserInfo() {
        return this.http.get(this.baseApi + this.apiUrl + "/GetCurrentUserInfo");
    }
}
