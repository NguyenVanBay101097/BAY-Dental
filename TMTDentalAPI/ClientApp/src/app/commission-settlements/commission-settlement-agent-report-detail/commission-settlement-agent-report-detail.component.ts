import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { AgentService } from 'src/app/agents/agent.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ToothDiagnosisSave } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';
import { CommissionSettlementFilterReport, CommissionSettlementsService } from '../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-agent-report-detail',
  templateUrl: './commission-settlement-agent-report-detail.component.html',
  styleUrls: ['./commission-settlement-agent-report-detail.component.css']
})
export class CommissionSettlementAgentReportDetailComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  search: string = '';
  skip = 0;
  limit = 20;
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  agentList: any[] = [];
  agentId: string = '';
  agentTypes : any[] = [
    {id:'partner',name:'Đối tác'},
    {id: 'customer', name: 'Khách hàng'},
    {id: 'employee', name: 'Nhân viên'}
  ];
  agentType: string = '';
  commissionType: string = '';
  loading = false;
  pagerSettings: any;
  IscommissionDisplay: boolean = false;
  @ViewChild('agentCbx', {static: true}) agentCbx: ComboBoxComponent;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig, 
    private commissionSettlementService: CommissionSettlementsService,
    private intlService: IntlService, private agentService: AgentService
  ) { 
      this.pagerSettings = config.pagerSettings;
    }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
    this.agentCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.agentCbx.loading = true)),
      switchMap(value => this.searchAgent(value))
    ).subscribe(result => {
      this.agentList = result.items;
      this.agentCbx.loading = false;
    })
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadAgentList();
    this.loadDataFromApi();
  }

  onSearchDateChange(e){
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.loadDataFromApi();
  }

  loadDataFromApi(){
    this.loading = true;
    var val = new CommissionSettlementFilterReport();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search ? this.search : '';
    val.agentId = this.agentId ? this.agentId : '';
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = this.dateFrom ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.groupBy = 'agent';
    val.classify = this.agentType;
    val.commissionDisplay = this.IscommissionDisplay == false ? 'greater_than_zero' : '';
    this.commissionSettlementService.getReportDetail(val).pipe(
      map((res: any) => <GridDataResult>{
        data: res.items,
        total: res.totalItems
      }))
      .subscribe(response => {
        this.gridData = response;
        this.loading = false;
      },err => {
        console.log(err);
        this.loading = false;
      })
  }

  checkCommissionDisplay(event) {
    this.IscommissionDisplay = event.target.checked;
    this.loadDataFromApi();
  }

  loadAgentList(){
    this.searchAgent().subscribe(result => {
      this.agentList = result.items;
    })
  }

  searchAgent(q?: string){
    var val = {
      offset: 0,
      limit: 0,
      search: q || ''
    };
    return this.agentService.getPaged(val);
  }

  onSelectAgentType(e){
    this.agentType = e ? e.id : '';
    this.loadDataFromApi();
  }

  onSelectAgent(e){
    this.agentId = e ? e.id : '';
    this.loadDataFromApi();
  }

  pageChange(e: PageChangeEvent){
    this.skip = e.skip;
    this.limit = e.take;
    this.loadDataFromApi();
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

  exportExcel(){
    var val = new CommissionSettlementFilterReport();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search ? this.search : '';
    val.agentId = this.agentId ? this.agentId : '';
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.dateFrom ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.groupBy = 'agent';
    val.classify = this.agentType || '';
    val.commissionDisplay = this.IscommissionDisplay == false ? 'greater_than_zero' : 'equals_zero';
    this.commissionSettlementService.excelCommissionDetailExport(val).subscribe((result:any) => {
      let filename = "CTHoaHongNguoiGioiThieu";
      let newBlob = new Blob([result], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

}
