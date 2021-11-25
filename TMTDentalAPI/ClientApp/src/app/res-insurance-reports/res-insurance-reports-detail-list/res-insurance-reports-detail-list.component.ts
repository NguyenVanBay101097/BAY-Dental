import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { InsuranceReportDetailFilter } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-reports-detail-list',
  templateUrl: './res-insurance-reports-detail-list.component.html',
  styleUrls: ['./res-insurance-reports-detail-list.component.css']
})
export class ResInsuranceReportsDetailListComponent implements OnInit {
  @Input() dateFrom: string;
  @Input() dateTo: string;
  @Input() partnerId: string;
  @Input() companyId: string;
  gridData: GridDataResult;
  allDataGrid: any;
  limit = 20;
  offset = 0;
  pagerSettings: any;

  constructor(
    private resInsuranceReportService: ResInsuranceReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit(): void {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    let val = new InsuranceReportDetailFilter();
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    val.partnerId = this.partnerId || '';
    val.companyId = this.companyId || '';
    this.resInsuranceReportService.getDetailReports(val).subscribe((res: any) => {
      this.allDataGrid = res;
      this.loadReport();
    })
  }

  loadReport() {
    this.gridData = <GridDataResult>{
      total: this.allDataGrid.length,
      data: this.allDataGrid.slice(this.offset, this.offset + this.limit)
    };
  }

  onPageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.limit = event.take;
    this.loadReport();
  }
}
