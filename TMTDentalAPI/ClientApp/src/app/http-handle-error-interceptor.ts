import { Injectable } from '@angular/core';
import {
    HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse
} from '@angular/common/http';
import { map, catchError, finalize, retry, switchMap } from 'rxjs/operators';
import { throwError, BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { AppLoadingService } from './shared/app-loading.service';
import { AuthService } from './auth/auth.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Injectable()
export class HttpHandleErrorInterceptor implements HttpInterceptor {

    constructor(private loadingService: AppLoadingService, private authService: AuthService,
        private router: Router, private notificationService: NotificationService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler) {
        this.loadingService.setLoading(true);
        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                this.loadingService.setLoading(false);
                let message;
                if (error.status === 410) {
                    this.authService.logout();
                    this.router.navigate(['/auth/expired']);
                    return throwError(error);
                }
                if (error.status === 503) {
                    this.authService.logout();
                    this.router.navigate(['/auth/notavaliable']);
                    return throwError(error);
                }

                if (error instanceof HttpErrorResponse) {
                    // Server Error
                    message = error.message;
                    if (error.error) {
                        message = error.error.message;
                    }

                    if (message) {
                        this.notificationService.show({
                            content: message,
                            hideAfter: 3000,
                            position: { horizontal: 'center', vertical: 'top' },
                            animation: { type: 'fade', duration: 400 },
                            type: { style: 'error', icon: true }
                        });
                    }
                } else {
                    // Client Error
                    if (!navigator.onLine) {
                        message = 'No Internet Connection';
                    } else {
                        message = 'Client error';
                    }
                }

                return throwError(error);
            }))
            .pipe(map<HttpEvent<any>, any>((evt: HttpEvent<any>) => {
                if (evt instanceof HttpResponse) {
                    this.loadingService.setLoading(false);
                }
                return evt;
            }));
    }

    addAuthenticationToken(request) {
        // Get access token from Local Storage
        const accessToken = this.authService.getAuthorizationToken();

        // If access token is null this means that user is not logged in
        // And we return the original request
        if (!accessToken) {
            return request;
        }
        // We clone the request, because the original request is immutable
        return request.clone({
            setHeaders: {
                Authorization: 'Bearer ' + accessToken
            }
        });
    }
}
