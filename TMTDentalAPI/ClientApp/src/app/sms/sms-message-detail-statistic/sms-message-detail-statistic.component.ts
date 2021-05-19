import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsComfirmDialogComponent } from '../sms-comfirm-dialog/sms-comfirm-dialog.component';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';
import { SmsMessageDetailPaged, SmsMessageDetailService } from '../sms-message-detail.service';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-message-detail-statistic',
  templateUrl: './sms-message-detail-statistic.component.html',
  styleUrls: ['./sms-message-detail-statistic.component.css']
})
export class SmsMessageDetailStatisticComponent implements OnInit {
  title: string;
  gridData: any;
  filteredSMSAccount: any[];
  filteredTemplate: any[];
  skip: number = 0;
  listMessageDetails: any[];
  smsCamapignId: string;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  loading = false;
  filterStatus = [
    { name: "Thành công", value: "success" },
    { name: "Thất bại", value: "fails" },
  ];
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  state: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private smsMessageDetailService: SmsMessageDetailService,
    private modalService: NgbModal,
    private activatedRoute: ActivatedRoute,
    private notificationService: NotificationService,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.smsCamapignId = this.activatedRoute.snapshot.queryParams.id;
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SmsMessageDetailPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.smsCampaignId = this.smsCamapignId;
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    val.state = this.state ? this.state : '';
    this.smsMessageDetailService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
      this.loading = false;
      if (res.data) {
        this.listMessageDetails = res.data;
      }
    }, err => {
      console.log(err);
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

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("chưa chọn khách hàng", false);
    }
    else {
      var smsMessageDetailIds = [];
      this.selectedIds.forEach(id => {
        var item = this.listMessageDetails.find(x => x.id == id);
        if (item && item.state != 'success') {
          smsMessageDetailIds.push(id);
        }
      });
      var modalRef = this.modalService.open(SmsComfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Xác nhận gửi lại tin nhắn";
      modalRef.componentInstance.bodyContent = 'Bạn chắc chắn muốn gửi lại tin nhắn chúc mừng sinh nhật?';
      modalRef.componentInstance.bodyNote = 'Lưu ý: Hệ thống chỉ gửi lại những tin nhắn đã thất bại';
      modalRef.result.then(
        result => {
          this.smsMessageDetailService.ReSend(smsMessageDetailIds).subscribe(
            () => {
              this.loadDataFromApi();
              this.notify("Gửi tin nhắn thành công", true);
            }
          )
        }
      )
    }

  }

  onStatusChange(event) {
    this.skip = 0;
    this.state = event.value;
    this.loadDataFromApi();
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }
}
