import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import {SmsCampaignPaged, SmsCampaignService} from '../sms-campaign.service';
import { SmsSmsPaged, SmsSmsService } from '../sms-sms.service';

@Component({
  selector: 'app-sms-statisticaddd',
  templateUrl: './sms-statistic.component.html',
  styleUrls: ['./sms-statistic.component.css'],
})
export class SmsStatisticComponent implements OnInit {
  @ViewChild('campaignCbx', { static: false }) campaignCbx: ComboBoxComponent;
  gridData: GridDataResult;
  dateFrom: Date;
  dateTo: Date;
  filteredState: any[] = [
    { name: 'Gửi thất bại', value: 'fails' },
    { name: 'Gửi Thành công', value: 'success' },
    { name: 'Đang gửi', value: 'sending' }
  ]
  campaignData: any[];
  state: string;
  limit: number = 20;
  skip: number = 0;
  search: string;
  searchUpdate = new Subject<string>();
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private smsSmsService: SmsSmsService,
    private intlService: IntlService,
    private smsCampaignService: SmsCampaignService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.campaignCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.campaignCbx.loading = true)),
      switchMap(value => this.searchCampaign(value))
    ).subscribe((result: any) => {
      this.campaignData = result ? result.items : [];
      this.campaignCbx.loading = false;
    });

    setTimeout(() => {
      this.loadCampaign();
    });
  }

  loadDataFromApi() {
    var val = new SmsSmsPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.state || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    this.smsSmsService.getPaged(val)
      .pipe(map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))).subscribe(res => {
        this.gridData = res;
      })
  }

  onChangeState(state) {
    if (state) {
      this.state = state.value;
      this.skip = 0;
      this.loadDataFromApi();
    }
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onChangeCampaign(event) {
    this.skip = 0;
    this.loadDataFromApi();
  }

  searchCampaign(q?: string) {
    var val = new SmsCampaignPaged();
    val.search = q || '';
    return this.smsCampaignService.getPaged(val);
  }
  loadCampaign() {
    this.searchCampaign().subscribe(
      (result:any) => {
        this.campaignData = result ? result.items : [];
      }
    )
  }

}
