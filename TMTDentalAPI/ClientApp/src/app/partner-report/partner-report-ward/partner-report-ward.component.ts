import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PartnerReportLocationDistrict, PartnerReportLocationWard, PartnerService } from 'src/app/partners/partner.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-report-ward',
  templateUrl: './partner-report-ward.component.html',
  styleUrls: ['./partner-report-ward.component.css']
})
export class PartnerReportWardComponent implements OnInit {
  @Input() public item: PartnerReportLocationDistrict;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  gridData: GridDataResult;
  details: PartnerReportLocationWard[];
  loading = false;

  constructor(
    private partnerService: PartnerService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.partnerService.reportLocationWard(this.item).subscribe(res => {
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
    this.limit = event.take;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }
}


