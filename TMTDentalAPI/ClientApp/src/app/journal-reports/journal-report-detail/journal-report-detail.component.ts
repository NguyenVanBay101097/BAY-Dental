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

  @Input() public item: JournalReportDetailPaged; //Id journal
  gridView: GridDataResult;
  loading = false;

  skip = 0;
  pageSize = 20;

  public total: any;
  public aggregates: any[] = [
    { field: 'debit', aggregate: 'sum' },
    { field: 'credit', aggregate: 'sum' },
    { field: 'balance', aggregate: 'sum' }
  ];
  constructor(private service: JournalReportService) { }

  ngOnInit() {
    this.getMoveLines();
  }

  getMoveLines() {
    this.loading = true;
    this.service.getDetail(this.item).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1,
        total: rs1.length
      }))
    ).subscribe(rs2 => {
      this.total = aggregateBy(rs2.data, this.aggregates);
      this.gridView = rs2;
      this.loading = false;
    }, er => {
      this.loading = true;
    }
    );
  }

}
