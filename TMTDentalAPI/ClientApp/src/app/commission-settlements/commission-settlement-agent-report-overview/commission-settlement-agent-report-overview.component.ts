import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CommissionSettlementOverviewFilter, CommissionSettlementsService } from '../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-agent-report-overview',
  templateUrl: './commission-settlement-agent-report-overview.component.html',
  styleUrls: ['./commission-settlement-agent-report-overview.component.css']
})
export class CommissionSettlementAgentReportOverviewComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  search: string = '';
  searchUpdate = new Subject<string>();
  limit = 20;
  skip = 0;
  items: any;
  agentTypes : any[] = [
    {id:'partner',name:'Đối tác'},
    {id: 'customer', name: 'Khách hàng'},
    {id: 'employee', name: 'Nhân viên'}
  ];
  agentType: string = '';
  gridData: GridDataResult;
  pagerSettings: any;
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(@Inject(PAGER_GRID_CONFIG) config: PageGridConfig, 
  private commissionSettlementService: CommissionSettlementsService,
  private intlService: IntlService,) { 
    this.pagerSettings = config.pagerSettings;
  }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.getItems();
      });
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();
  }

  onSearchDateChange(e){
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.loadDataFromApi();
  }

  loadDataFromApi(){
    var val = new CommissionSettlementOverviewFilter();
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    val.groupBy = 'agent';
    val.classify = this.agentType;
    this.loading = true;
    this.commissionSettlementService.getAgentOverview(val).subscribe(result => {
      this.items = result;
      this.getItems();
    })
  }

  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.limit = event.take;
    this.getItems();
  }

  onSelectAgentType(event){
    this.agentType = event ? event.id : '';
    this.getItems();
  }

  getAgentDetail(item){

  }

  getAgentType(type){
    switch(type) {
      case 'customer':
        return 'Khách hàng';
      case 'employee':
        return 'Nhân viên';
      case 'partner':
        return 'Đối tác';
      default:
        return '';
    }
  }

  getItems(){
    this.loading = true;
    var items = this.items;
    if (this.search != '')
      items = items.filter(x => x.name.toUpperCase().indexOf(this.search.toUpperCase()) !== -1)
    if (this.agentType != '')
      items = items.filter(x => x.classify == this.agentType);
    this.gridData = {
      data: items.slice(this.skip, this.skip + this.limit),
      total: items.length
    };
    this.loading = false;
  }

  actionPayment(item){

  }
}
