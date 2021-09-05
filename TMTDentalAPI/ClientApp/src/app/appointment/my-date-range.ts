export class MyDateRange {
    start: Date;
    end: Date;
    constructor(start = new Date(), end = new Date()) {
        this.start = start;
        this.end = end;
    }
}