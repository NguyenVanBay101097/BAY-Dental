import { Injectable } from '@angular/core';

import { Observable, of } from 'rxjs';
import { tap, delay, catchError, retry, switchMap, map } from 'rxjs/operators';
import { AuthResource, LoginTokenResult, LoginUserInfo, LoginViewModel, LoggedInViewModel, ForgotPasswordViewModel } from './auth.resource';
import { LoginForm } from './login-form';
import { HttpErrorResponse } from '@angular/common/http';
import { debug } from 'util';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(private authResource: AuthResource, public jwtHelper: JwtHelperService) { }
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
                    console.log(result);
                    if (result.succeeded) {
                        localStorage.setItem('access_token', result.token);
                        localStorage.setItem('user_info', JSON.stringify(result.user));
                        this.isLoggedIn = true;
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

    changePassword(val) {
        return this.authResource.changePassword(val);
    }

    logout(): void {
        localStorage.removeItem('access_token');
        localStorage.removeItem('user_info');
        this.isLoggedIn = false;
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
