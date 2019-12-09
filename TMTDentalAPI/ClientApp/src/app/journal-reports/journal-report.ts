import { StringFilterCellComponent } from '@progress/kendo-angular-grid';

export class JournalReportPaged {
    dateFrom: string;
    dateTo: string;
    search: string;
    groupBy: string;
    filter: string;
}

export class JournalReport {
    debitSum: number;
    creditSum: number;
    balanceSum: number;
    journalId: string;
    name: string;
    groupBy: string;
}

export class JournalReportDetailPaged {
    journalId: string;
    groupBy: string;
    dateFrom: string;
    dateTo: string;
}