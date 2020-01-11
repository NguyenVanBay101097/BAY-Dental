import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpErrorResponse } from '@angular/common/http';
import { SharedErrorDialogComponent } from './shared-error-dialog/shared-error-dialog.component';

@Injectable()
export class AppSharedShowErrorService {
    constructor(private modalService: NgbModal) {
    }

    show(error: HttpErrorResponse) {
        var body = error.error;
        let modalRef = this.modalService.open(SharedErrorDialogComponent, { windowClass: 'o_technical_modal' });
        if (body.error) {
            modalRef.componentInstance.body = body.error;
        } else {
        }
    }
}