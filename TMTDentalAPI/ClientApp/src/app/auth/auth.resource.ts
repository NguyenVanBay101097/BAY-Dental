import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export class LoginTokenResult {
    access_token: string;
    expires_in: number;
    token_type: string;
}

export class LoginUserInfo {
    name: string;
    given_name: string;
    company_id: string;
    email: string;
}

export class LoginViewModel {
    userName: string;
    password: string;
    rememberMe: boolean;
}

export class RefreshViewModel {
    accessToken: string;
    refreshToken: string;
}

export class RefreshResponseViewModel {
    accessToken: string;
    refreshToken: string;
}

export class LoggedInViewModel {
    succeeded: boolean;
    message: string;
    token: string;
    refreshToken: string;
    configs: object;
    user: UserViewModel;
}

export class UserViewModel {
    id: string;
    userName: string;
    phone: string;
    email: string;
    name: string;
    avatar: string;
}

export class ForgotPasswordViewModel {
    email: string;
}

export class ResetPasswordViewModel {
    email: string;
    password: string;
    confirmPassword: string;
    code: string;
}

export class ForgotPasswordResponse {
    success: boolean;
    message: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthResource {
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
    isLoggedIn = false;

    // store the URL so we can redirect after logging in
    redirectUrl: string;
    performLogin(val: LoginViewModel): Observable<LoggedInViewModel> {
        return this.http.post<LoggedInViewModel>(this.baseApi + 'api/Account/Login', val);
    }

    performRefresh(val: RefreshViewModel): Observable<RefreshResponseViewModel> {
        return this.http.post<RefreshResponseViewModel>(this.baseApi + 'api/Account/Refresh', val);
    }

    forgotPassword(val: ForgotPasswordViewModel): Observable<ForgotPasswordResponse> {
        return this.http.post<ForgotPasswordResponse>(this.baseApi + 'api/Account/ForgotPassword', val);
    }

    resetPassword(val: ResetPasswordViewModel) {
        return this.http.post(this.baseApi + 'api/Account/ResetPassword', val);
    }

    changePassword(val) {
        return this.http.post(this.baseApi + 'api/Account/ChangePassword', val);
    }

    logout(): void {
        this.isLoggedIn = false;
    }
}
