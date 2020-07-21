import { Component, OnInit, Input } from '@angular/core';
import { ReportTCare, ReportTCareItem, TcareReportService } from '../tcare-report.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-tcare-report-detail',
  templateUrl: './tcare-report-detail.component.html',
  styleUrls: ['./tcare-report-detail.component.css']
})
export class TCareReportDetailComponent implements OnInit {
  @Input() public item: ReportTCare;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: ReportTCareItem[];
  loading = false;
  constructor(private reportService: TcareReportService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    console.log(this.item);
    this.reportService.getDetail(this.item).subscribe(res => {
      this.details = res;
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }

}
