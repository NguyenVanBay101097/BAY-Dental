import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PrintService } from 'src/app/print.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AgentService, CommissionAgentFilter } from '../agent.service';

@Component({
  selector: 'app-agent-commission-list',
  templateUrl: './agent-commission-list.component.html',
  styleUrls: ['./agent-commission-list.component.css']
})
export class AgentCommissionListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private route: ActivatedRoute, private modalService: NgbModal,
    private agentService: AgentService, private router: Router,
    private intlService: IntlService,
    private notifyService: NotifyService,
    private printService: PrintService) { }

  ngOnInit() {

    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd; 

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
    var val = new CommissionAgentFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.agentService.getCommissionAgent(val).pipe(
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
    this.loadDataFromApi();
  }

  clickItem(item) {
    if (item && item.dataItem) {
      var id = item.dataItem.id;
    }
  }

}
