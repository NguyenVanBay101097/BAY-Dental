import { Injectable } from '@angular/core';
import {
    HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse
} from '@angular/common/http';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map, catchError, finalize } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AppLoadingService } from './shared/app-loading.service';


@Injectable()
export class HttpHandleErrorInterceptor implements HttpInterceptor {

    constructor(private notificationService: NotificationService, private router: Router, private loadingService: AppLoadingService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        this.loadingService.setLoading(true);
        return next.handle(req).pipe(
            map((event: HttpEvent<any>) => {
                if (event instanceof HttpResponse) {
                }
                return event;
            }),
            catchError((error: HttpErrorResponse) => {
                if (error.status === 401) {
                    this.router.navigate(['login']);
                }

                let message = '';
                if (error.error) {
                    if (error.error.error) {
                        message = error.error.error;
                    } else if (error.error.errors) {
                        Object.keys(error.error.errors).forEach(keyError => {
                            message += `${keyError}: ${error.error.errors[keyError]} `;
                        });
                    } else {
                        message = error.error.title;
                    }
                } else {
                    message = error.message;
                }

                this.notificationService.show({
                    content: message,
                    hideAfter: 3000,
                    position: { horizontal: 'right', vertical: 'bottom' },
                    animation: { type: 'fade', duration: 400 },
                    type: { style: 'error', icon: true }
                });

                return throwError(error);
            }),
            finalize(() => this.loadingService.setLoading(false)));
    }
}
