import { Injectable } from '@angular/core';
import {
    HttpHandler,
    HttpProgressEvent,
    HttpInterceptor,
    HttpSentEvent,
    HttpHeaderResponse,
    HttpUserEvent,
    HttpRequest,
    HttpResponse,
    HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';
export const CACHE_KEY = 'X-No-Cache';

@Injectable()
export class NoCacheInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const httpRequest = req.clone({
            headers: new HttpHeaders({
                'Cache-Control': 'no-cache',
                'Pragma': 'no-cache',
                'Expires': 'Sat, 01 Jan 2000 00:00:00 GMT'
            })
        });

        return next.handle(httpRequest);
    }
}