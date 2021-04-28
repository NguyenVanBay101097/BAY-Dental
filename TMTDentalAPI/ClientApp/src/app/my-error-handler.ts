import { ErrorHandler, Injectable } from '@angular/core';
import { NotificationService } from '@progress/kendo-angular-notification';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class MyErrorHandler implements ErrorHandler {

    constructor() { }

    handleError(error) {
        debugger;
        console.log(error);
    }
}