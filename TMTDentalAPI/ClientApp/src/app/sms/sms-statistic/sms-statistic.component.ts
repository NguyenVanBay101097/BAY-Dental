import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { SmsCampaignPaged, SmsCampaignService } from '../sms-campaign.service';
import { SmsMessageDetailPaged, SmsMessageDetailService } from '../sms-message-detail.service';

@Component({
  selector: 'app-sms-statistic',
  templateUrl: './sms-statistic.component.html',
  styleUrls: ['./sms-statistic.component.css'],
})
export class SmsStatisticComponent implements OnInit {
  gridData: GridDataResult;
  dateFrom: Date;
  dateTo: Date;
  filteredState: any[] = [
    { name: 'Đang gửi', value: 'outgoing' },
    { name: 'Hủy', value: 'canceled' },
    { name: 'Thất bại', value: 'error' },
    { name: 'Thành công', value: 'sent' }
  ]
  campaignData: any[] = [];
  state: string;
  limit: number = 20;
  skip: number = 0;
  search: string;
  smsCampaignId: string;
  searchUpdate = new Subject<string>();
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  @ViewChild('campaignCbx', { static: true }) campaignCbx: ComboBoxComponent;

  constructor(
    private intlService: IntlService,
    private smsCampaignService: SmsCampaignService,
    private smsMessageDetailService: SmsMessageDetailService
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

    this.loadCampaign();
  }

  loadDataFromApi() {
    var val = new SmsMessageDetailPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.state || '';
    val.smsCampaignId = this.smsCampaignId || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    this.smsMessageDetailService.getPagedStatistic(val)
      .pipe(map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))).subscribe(res => {
        this.gridData = res;
      })
  }

  onChangeState(state) {
    state ? this.state = state.value : this.state = '';
    this.skip = 0;
    this.loadDataFromApi();
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
    this.smsCampaignId = event ? event.id : '';
    this.skip = 0;
    this.loadDataFromApi();
  }

  searchCampaign(q?: string) {
    var val = new SmsCampaignPaged();
    val.search = q || '';
    val.limit = 20;
    val.offset = 0;
    return this.smsCampaignService.getPaged(val);
  }

  loadCampaign() {
    this.searchCampaign().subscribe(
      (result: any) => {
        this.campaignData = result ? result.items : [];
      }
    )
  }

  stranslateCodeResponse(code) {
    switch (code) {
      case "100":
        return "Gửi thành công";
      case "99":
        return "Lỗi không xác định , thử lại sau";
      case "101":
        return "Đăng nhập thất bại (api key hoặc secrect key không đúng)";
      case "102":
        return "Tài khoản đã bị khóa";
      case "103":
        return "Số dư tài khoản không đủ dể gửi tin";
      case "104":
        return "Brandname không tồn tại hoặc đã bị hủy";
      case "118":
        return "Loại tin nhắn không hợp lệ";
      default:
        return "Lý do khác";
    }
  }
}


