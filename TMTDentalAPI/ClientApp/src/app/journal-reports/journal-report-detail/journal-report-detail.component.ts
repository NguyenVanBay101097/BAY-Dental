import { Component, OnInit, Input } from '@angular/core';
import { JournalReportService } from '../journal-report.service';
import { map } from 'rxjs/operators';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { JournalReport, JournalReportDetailPaged } from '../journal-report';
import { aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-journal-report-detail',
  templateUrl: './journal-report-detail.component.html',
  styleUrls: ['./journal-report-detail.component.css']
})
export class JournalReportDetailComponent implements OnInit {
  @Input() public items: any = []; //Id journal
  constructor(private service: JournalReportService) { }

  ngOnInit() {
  }
}
