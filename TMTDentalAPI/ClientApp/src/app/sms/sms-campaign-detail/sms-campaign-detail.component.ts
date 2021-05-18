import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsMessagePaged, SmsMessageService } from '../sms-message.service';

@Component({
  selector: 'app-sms-campaign-detail',
  templateUrl: './sms-campaign-detail.component.html',
  styleUrls: ['./sms-campaign-detail.component.css']
})
export class SmsCampaignDetailComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  campaignId: string;
  typeSend = "waiting"; // waiting: chờ gửi, sent: đã gửi
  selectedIds: any = [];
  
  constructor(
    private route: ActivatedRoute, 
    private smsMessageService: SmsMessageService, 
    private smsCampaignService: SmsCampaignService, 
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.campaignId = params.get('id');
      this.loadSmsMessagePaged();
      this.getCampaign();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.loadSmsMessagePaged();
      });
  }

  loadSmsMessagePaged() {
    this.loading = true;
    var val = new SmsMessagePaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search || "";
    val.campaignId = this.campaignId;
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
          console.log(res);
          
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  pageChange(event): void {
    this.offset = event.offset;
    this.loadSmsMessagePaged();
  }

  getCampaign() {
    this.smsCampaignService.get(this.campaignId)
    .subscribe(
      (res) => {
        console.log(res);
      },
      (err) => {
        console.log(err);
      }
    );
  }

  changeTypeSend() {
    
  }

  cancelSend() {

  }
}
