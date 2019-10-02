import { PartnerSimple } from '../partners/partner-simple';
import { LocaleDateTimeFormats } from '@progress/kendo-angular-intl';
import { EmployeeSimple } from '../employees/employee';

export class AppointmentBasic {
    id: string;
    name: string;
    date: string;
    userId: string;
    user: ApplicationUserSimple;
    state: string;
    partnerId: string;
    partner: PartnerSimple;
    doctorId: string;
    doctor: EmployeeSimple;
    note: string;
}

export class AppointmentDisplay extends AppointmentBasic {
    companyId: string;
    hasDotKhamRef: boolean;
}

export class ApplicationUserSimple {
    id: string;
    name: string;
}

export class ApplicationUserPaged {
    offset: number;
    limit: number;
    searchNameUserName: string;
}

export class ApplicationUserDisplay {
    id: string;
    name: string;
    userName: string;
    passWord: string;
    Email: string;
    companyId: string;
}

export class PagedResult2<T>{
    offset: number;
    limit: number;
    totalItems: number;
    items: T[];
}

export class AppointmentPaged {
    offset: number;
    limit: number;
    searchCustomer: string;//by appointment name
    searchDoctor: string;
    searchAppointment: string;
    dateTimeFrom: string;
    dateTimeTo: string;
    state: string;
}

export class SchedulerConfig {
    slotFillWeek: number;//độ dài của 1 event trên ô chứa nó
    slotFillDay: number;//độ dài của 1 event trên ô chứa nó
    startDisplay: string;//HH:mm
    endDisplay: string;//HH:mm
    workDayStart: string;//HH:mm
    workDayEnd: string;//HH:mm
    workWeekStart: number;
    workWeekEnd: number;
    selectedDate: Date;
    eventHeight: string;
    numberOfDays: number;
    slotDivisions: number;
    slotDuration: number;
    indexViewNum: number;
}

export class AppointmentDefaultGet {
    dotKhamId: string;
}

export class AppointmentPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: AppointmentBasic[];
}

export class AppointmentPatch {
    id: string;
    state: string;
    date: string;
}
