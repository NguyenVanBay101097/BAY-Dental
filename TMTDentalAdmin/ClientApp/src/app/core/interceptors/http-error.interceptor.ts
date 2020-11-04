import { Injectable } from '@angular/core';
import {
    HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse
} from '@angular/common/http';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map, catchError, finalize } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AppLoadingService } from '@shared/app-loading.service';


@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

    constructor(private notificationService: NotificationService, private loadingService: AppLoadingService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        this.loadingService.setLoading(true);
        return next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.message) {
                    this.notificationService.show({
                        content: error.message,
                        hideAfter: 3000,
                        position: { horizontal: 'center', vertical: 'top' },
                        animation: { type: 'fade', duration: 400 },
                        type: { style: 'error', icon: true }
                    });
                }

                this.loadingService.setLoading(false);
                return throwError(error);
            }))
            .pipe(map<HttpEvent<any>, any>((evt: HttpEvent<any>) => {
                if (evt instanceof HttpResponse) {
                    this.loadingService.setLoading(false);
                }
                return evt;
            }));
    }
}
