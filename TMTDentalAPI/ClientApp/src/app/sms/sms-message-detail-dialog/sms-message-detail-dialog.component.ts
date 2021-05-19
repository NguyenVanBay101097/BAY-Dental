import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SmsMessageDetailPaged, SmsMessageDetailService } from '../sms-message-detail.service';

@Component({
  selector: 'app-sms-message-detail-dialog',
  templateUrl: './sms-message-detail-dialog.component.html',
  styleUrls: ['./sms-message-detail-dialog.component.css']
})
export class SmsMessageDetailDialogComponent implements OnInit {

  title: string;
  state: string;
  limit: number = 20;
  offset: number = 0;
  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;

  constructor(
    private smsMessageDetailService: SmsMessageDetailService, 
    public activeModal: NgbActiveModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.offset = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    var val = new SmsMessageDetailPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search || '';
    val.state = this.state || '';
    // val.smsCampaignId = this.smsCampaignId || '';
    this.smsMessageDetailService.getPagedStatistic(val)
      .pipe(map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))).subscribe(res => {
        this.gridData = res;
      })
  }

  pageChange(event): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  onChangeState(state) {
    if (state) {
      this.state = state.value;
      this.offset = 0;
      this.loadDataFromApi();
    }
  }
}
