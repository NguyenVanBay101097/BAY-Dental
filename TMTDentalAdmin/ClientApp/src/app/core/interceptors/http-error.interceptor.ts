import { Injectable } from '@angular/core';
import {
    HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse
} from '@angular/common/http';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map, catchError, finalize } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AppLoadingService } from '@shared/app-loading.service';
import { Router } from '@angular/router';


@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

    constructor(private notificationService: NotificationService, private loadingService: AppLoadingService,
        private router: Router) { }

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        this.loadingService.setLoading(true);
        return next.handle(req).pipe(
          map((event: HttpEvent<any>) => {
                if (event instanceof HttpResponse) {
                }
                return event;
            }),
          catchError((errorResponse: HttpErrorResponse) => {
                if (errorResponse.status === 401) {
                    this.router.navigate(['login']);
                }

                let message;
                if (errorResponse instanceof HttpErrorResponse) {
                    // Server Error
                    const error = errorResponse.error;
                    if (error) {
                        message = error.data ? error.data.message : error.message;
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

                return throwError(errorResponse);
            }),
            finalize(() => this.loadingService.setLoading(false)));
    }
}
