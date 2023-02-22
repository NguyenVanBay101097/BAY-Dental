import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
@Injectable()
export class RevenueManageService {
    // Observable string sources
    private onExportEvent = new Subject<any>();
    // Observable string streams
    changeEmitted$ = this.onExportEvent.asObservable();
    // Service message commands
    emitChange(change: any) {
        this.onExportEvent.next(change);
    }
}