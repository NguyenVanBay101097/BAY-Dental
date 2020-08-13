import { Injectable, EventEmitter } from "@angular/core";
import { Subject } from 'rxjs';
import { AppointmentBasic, AppointmentPaged } from './appointment';
import { AppointmentService } from './appointment.service';
import { map } from 'rxjs/operators';

@Injectable()
export class AppointmentVMService {
    private eventsSource = new Subject<any[]>();
    events$ = this.eventsSource.asObservable();
    filter = new EventEmitter<any>();
    eventEdit = new EventEmitter<any>();
    eventDelete = new EventEmitter<any>();

    private dateRangeSource = new Subject<any>();
    dateRange$ = this.dateRangeSource.asObservable();

    private stateSubject = new Subject<string>();
    state$ = this.stateSubject.asObservable();

    searchSubject = new Subject<string>();
    search$ = this.searchSubject.asObservable();


    private apCreateSource = new Subject<string>();
    apCreate$ = this.apCreateSource.asObservable();

    private refreshSubject = new Subject<boolean>();
    refresh$ = this.refreshSubject.asObservable();

    constructor(private appointmentService: AppointmentService) {

    }

    updateSearch(text) {
        this.searchSubject.next(text);
    }

    refreshData() {
        this.refreshSubject.next(true);
    }

    announceApCreate(id: string) {
        this.apCreateSource.next(id);
    }

    setDateRange(dateFrom, dateTo) {
        this.dateRangeSource.next({ dateFrom, dateTo });
    }

    setState(state) {
        this.stateSubject.next(state);
    }

    query(val: any) {
        this.appointmentService.searchRead(val)
            .pipe(
                map(response => {
                    return response.map(ap => (<any>{
                        id: ap.id,
                        isAllDay: false,
                        start: new Date(ap.date),
                        end: new Date(ap.date),
                        title: ap.partnerName,
                        description: ap.doctorName ? 'BS. ' + ap.doctorName : '',
                        state: ap.state,
                        partnerId: ap.partnerId
                    }));
                })
            )
            .subscribe(result => {
                this.eventsSource.next(result);
            });

    }
}