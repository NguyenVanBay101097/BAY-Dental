import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AgentService, CommissionAgentDetailItemFilter } from '../agent.service';

@Component({
  selector: 'app-agent-commmission-form-detail-item',
  templateUrl: './agent-commmission-form-detail-item.component.html',
  styleUrls: ['./agent-commmission-form-detail-item.component.css']
})
export class AgentCommmissionFormDetailItemComponent implements OnInit {
  @Input() public partnerId: string;
  @Input() public dateFrom: Date;
  @Input() public dateTo: Date;

  agentId: string;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute,
    private agentService: AgentService,
    private intlService: IntlService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.agentId = this.route.parent.snapshot.paramMap.get('id');

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CommissionAgentDetailItemFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.agentId = this.agentId;
    val.partnerId = this.partnerId;
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.agentService.getCommissionAgentDetailItem(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }
}
