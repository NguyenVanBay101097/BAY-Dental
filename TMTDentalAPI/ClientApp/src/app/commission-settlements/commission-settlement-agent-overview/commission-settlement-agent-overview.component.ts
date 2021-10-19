import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CommissionSettlementOverviewFilter, CommissionSettlementsService } from '../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-agent-overview',
  templateUrl: './commission-settlement-agent-overview.component.html',
  styleUrls: ['./commission-settlement-agent-overview.component.css']
})
export class CommissionSettlementAgentOverviewComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  search: string = '';
  searchUpdate = new Subject<string>();
  limit = 20;
  skip = 0;
  items: any;
  agentTypes : any[] = [
    {id:'doi_tac',name:'Đối tác'},
    {id: 'khach_hang', name: 'Khách hàng'}
  ];
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
        this.loadDataFromApi();
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
    this.commissionSettlementService.getAgentOverview(val).subscribe(result => {
      console.log(result);
      
      this.items = result;
      this.getItems();
    })
  }

  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.limit = event.take;
    this.items();
  }

  getAgentDetail(item){

  }

  getAgentType(item){

  }

  getItems(){
    var items = this.items.filter(x => x.name.toUpperCase().indexOf(this.search.toUpperCase()) !== -1);
    this.gridData = {
      data: items.slice(this.skip, this.skip + this.limit),
      total: items.length
    };
  }

  actionPayment(item){

  }

}
