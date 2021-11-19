import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
// import { debug } from 'util';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, Observable, of, ReplaySubject } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import { WebSessionService } from '../core/services/web-session.service';
import { AuthResource, LoggedInViewModel, LoginViewModel, RefreshResponseViewModel, RefreshViewModel, UserViewModel } from './auth.resource';
import { LoginForm } from './login-form';
import { environment } from '../../environments/environment';


export class UserInfo {
    avatar: string;
    name: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private currentUserSubject: BehaviorSubject<UserViewModel>;

    public currentUser: Observable<UserViewModel>;

    constructor(private authResource: AuthResource, public jwtHelper: JwtHelperService, private http: HttpClient, @Inject('BASE_API') private baseApi: string,
        private webSessionService: WebSessionService
    ) {
        var user_info = localStorage.getItem('user_info');
        if (typeof user_info != 'string') {
            localStorage.removeItem('user_info');
        }
        this.currentUserSubject = new BehaviorSubject<UserViewModel>(null);

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
                mergeMap((result: LoggedInViewModel) => {
                    if (result.succeeded) {
                        localStorage.setItem('access_token', result.token);
                        localStorage.setItem('user_info', JSON.stringify(result.user));
                        localStorage.setItem('refresh_token', result.refreshToken);
                        localStorage.setItem('app_version', environment.version + '');
                        this.isLoggedIn = true;
                        this.currentUserSubject.next(result.user);
                        return this.webSessionService.getSessionInfo();
                    } else {
                        return of(result);
                    }
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
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('user_info');
        localStorage.removeItem('session_info');
        this.isLoggedIn = false;
        this.currentUserSubject.next(null);
    }

    getAuthorizationToken() {
        return localStorage.getItem('access_token');
    }

    get userInfo() {
        try {
            var a = JSON.parse(localStorage.getItem('user_info'));
            return a;
        } catch (e) {
            return null; // error in the above string (in this case, yes)!
        }
    }

    isAuthenticated() {
        const token = localStorage.getItem('access_token');
        if (!token) {
            return false;
        }

        return !this.jwtHelper.isTokenExpired(token);
    }

    getUserInfo(id): Observable<UserInfo> {
        return this.http.get<UserInfo>(this.baseApi + 'api/Account/GetUserInfo/' + id);
    }
}
