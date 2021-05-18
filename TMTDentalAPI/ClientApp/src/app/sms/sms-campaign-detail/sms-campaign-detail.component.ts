import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
  formGroup: FormGroup
  campaign: any;
  campaignId: string;
  isEdit: boolean = false;
  state = "waiting"; // waiting: chờ gửi, success: đã gửi
  selectedIds: any = [];

  get f() { return this.formGroup.controls; }

  constructor(
    private route: ActivatedRoute,
    private smsMessageService: SmsMessageService,
    private smsCampaignService: SmsCampaignService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      typeDate: "unlimited", // unlimited: vô thời hạn, period: khoảng thời gian
      startDateObj: new Date(),
      endDateObj: new Date(),
      limitMessage: [0, Validators.required],
      stateCheck: true
    });

    this.route.paramMap.subscribe(params => {
      this.campaignId = params.get('id');
      this.loadDataFromApi();
      this.getSmsCampaign();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SmsMessagePaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search || "";
    val.campaignId = this.campaignId;
    val.state = this.state;
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
    this.loadDataFromApi();
  }

  getSmsCampaign() {
    this.smsCampaignService.get(this.campaignId)
      .subscribe(
        (res: any) => {
          if (res.state == "running") {
            this.formGroup.get('stateCheck').setValue(true);
          } else {
            this.formGroup.get('stateCheck').setValue(false);
          }
          this.formGroup.patchValue(res);
        },
        (err) => {
          console.log(err);
        }
      );
  }

  changeState() {
    this.loadDataFromApi();
  }

  cancelSend() {

  }

  getValueFormControl(key) {
    return this.formGroup.get(key).value;
  }
}
