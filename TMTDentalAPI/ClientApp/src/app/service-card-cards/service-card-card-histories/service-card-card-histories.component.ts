import { Component, OnInit, Input, Inject } from '@angular/core';
import { ServiceCardCardService } from '../service-card-card.service';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-service-card-card-histories',
  templateUrl: './service-card-card-histories.component.html',
  styleUrls: ['./service-card-card-histories.component.css']
})
export class ServiceCardCardHistoriesComponent implements OnInit {
  @Input() public item: any;
  skip = 0;
  pageSize = 20;
  pagerSettings: any;
  gridData: any;
  loading = false;
  gridView: GridDataResult;

  public total: any;
  public aggregates: any[] = [
    { field: 'amount', aggregate: 'sum' },
  ];

  constructor(private cardService: ServiceCardCardService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.cardService.getHistories(this.item.id).subscribe(res => {
      this.gridData = res;
      this.total = aggregateBy(this.gridData, this.aggregates);
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.loadItems();
  }

  private loadItems(): void {
    this.gridView = {
      data: this.gridData.slice(this.skip, this.skip + this.pageSize),
      total: this.gridData.length
    };
  }
}

