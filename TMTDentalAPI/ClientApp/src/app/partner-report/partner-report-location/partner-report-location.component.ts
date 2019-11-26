import { Component, OnInit } from '@angular/core';
import { PartnerService, PartnerReportLocationCity, PartnerReportLocationCitySearch } from 'src/app/partners/partner.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-partner-report-location',
  templateUrl: './partner-report-location.component.html',
  styleUrls: ['./partner-report-location.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerReportLocationComponent implements OnInit {

  loading = false;
  items: PartnerReportLocationCity[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;

  cityCode: string;
  districtCode: string;
  wardCode: string;

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
    var val = new PartnerReportLocationCitySearch();
    val.cityCode = this.cityCode;
    val.districtCode = this.districtCode;
    val.wardCode = this.wardCode;
    this.partnerService.reportLocationCity(val).subscribe(res => {
      this.items = res;
      this.total = aggregateBy(this.items, this.aggregates);
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  onFilterChange(data) {
    this.cityCode = data.city ? data.city.code : null;
    this.wardCode = data.ward ? data.ward.code : null;
    this.districtCode = data.district ? data.district.code : null;
    this.loadDataFromApi();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }
}
