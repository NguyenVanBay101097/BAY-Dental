import { Injectable } from '@angular/core';

import { Observable, of, ReplaySubject, BehaviorSubject } from 'rxjs';
import { tap, delay, catchError, retry, switchMap, map } from 'rxjs/operators';
import { AuthResource, LoginTokenResult, LoginUserInfo, LoginViewModel, LoggedInViewModel, ForgotPasswordViewModel, RefreshViewModel, RefreshResponseViewModel, UserViewModel } from './auth.resource';
import { LoginForm } from './login-form';
import { HttpErrorResponse, HttpClient } from '@angular/common/http';
import { debug } from 'util';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private currentUserSubject: BehaviorSubject<UserViewModel>;
    public currentUser: Observable<UserViewModel>;

    constructor(private authResource: AuthResource, public jwtHelper: JwtHelperService, private http: HttpClient) {
        this.currentUserSubject = new BehaviorSubject<UserViewModel>(JSON.parse(localStorage.getItem('user_info')));
        this.currentUser = this.currentUserSubject.asObservable();
    }
    isLoggedIn = false;

    // store the URL so we can redirect after logging in
    redirectUrl: string;

    login(loginForm: LoginForm) {
        var val = new LoginViewModel();
        val.userName = loginForm.username;
        val.password = loginForm.password;
        val.rememberMe = loginForm.remember_me;
        return this.authResource
            .performLogin(val)
            .pipe(
                map((result: LoggedInViewModel) => {
                    if (result.succeeded) {
                        localStorage.setItem('access_token', result.token);
                        localStorage.setItem('user_info', JSON.stringify(result.user));
                        localStorage.setItem('refresh_token', result.refreshToken);
                        this.isLoggedIn = true;
                        this.currentUserSubject.next(result.user);
                    }
                    return result;
                })
            );
    }

    forgotPassword(val) {
        return this.authResource.forgotPassword(val);
    }

    resetPassword(val) {
        return this.authResource.resetPassword(val);
    }

    getGroups() {
        return this.authResource.getGroups();
    }

    refresh(): Observable<RefreshResponseViewModel> {
        var refreshToken = this.getRefreshToken();
        var val = new RefreshViewModel();
        val.refreshToken = refreshToken;
        const refreshObservable = this.authResource.performRefresh(val);

        const refreshSubject = new ReplaySubject<RefreshResponseViewModel>(1);
        refreshSubject.subscribe((r: RefreshResponseViewModel) => {
            this.setAccessToken(r.accessToken);
            this.setRefreshToken(r.refreshToken);
        }, (err) => {
            this.handleAuthenticationError(err);
        });

        refreshObservable.subscribe(refreshSubject);
        return refreshSubject;
    }

    private handleAuthenticationError(err: any) {
        // TODO: Only for authentication error codes
        this.setAccessToken(null);
        this.setRefreshToken(null);
    }

    private setAccessToken(accessToken: string) {
        if (!accessToken) {
            localStorage.removeItem('access_token');
        } else {
            localStorage.setItem('access_token', accessToken);
        }
    }

    private setRefreshToken(refreshToken: string) {
        if (!refreshToken) {
            localStorage.removeItem('refresh_token');
        } else {
            localStorage.setItem('refresh_token', refreshToken);
        }
    }

    changePassword(val) {
        return this.authResource.changePassword(val);
    }

    getRefreshToken() {
        return localStorage.getItem('refresh_token');
    }

    logout(): void {
        localStorage.removeItem('access_token');
        localStorage.removeItem('user_info');
        this.isLoggedIn = false;
        this.currentUserSubject.next(null);
    }

    getAuthorizationToken() {
        return localStorage.getItem('access_token');
    }

    get userInfo() {
        return JSON.parse(localStorage.getItem('user_info'));
    }

    isAuthenticated() {
        return !this.jwtHelper.isTokenExpired();
    }
}
