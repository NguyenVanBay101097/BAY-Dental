import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PartnerReportLocationCity, PartnerReportLocationDistrict, PartnerService } from 'src/app/partners/partner.service';
import { aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-partner-report-district',
  templateUrl: './partner-report-district.component.html',
  styleUrls: ['./partner-report-district.component.css']
})

export class PartnerReportDistrictComponent implements OnInit {
  @Input() public item: PartnerReportLocationCity;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: PartnerReportLocationDistrict[];
  loading = false;

  public total: any;
  public aggregates: any[] = [
    { field: 'total', aggregate: 'sum' },
  ];

  constructor(private partnerService: PartnerService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.partnerService.reportLocationDistrict(this.item).subscribe(res => {
      this.details = res;
      this.total = aggregateBy(this.details, this.aggregates);
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

