import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SmsMessagePaged, SmsMessageService } from '../sms-message.service';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-message-statistic',
  templateUrl: './sms-message-statistic.component.html',
  styleUrls: ['./sms-message-statistic.component.css']
})
export class SmsMessageStatisticComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private smsMessageService: SmsMessageService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SmsMessagePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    this.smsMessageService.getPaged(val).pipe(
      map(
        (response: any) =>
          <GridDataResult>{
            data: response.items,
            total: response.totalItems,
          }
      )
    )
      .subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  pageChange(event): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }
}
