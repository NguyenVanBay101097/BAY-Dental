import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PrintService } from 'src/app/print.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
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
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal,
    private agentService: AgentService, private router: Router,
    private intlService: IntlService,
    private notifyService: NotifyService,
    private printService: PrintService) { }

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
    this.loadDataFromApi();
  }

}
