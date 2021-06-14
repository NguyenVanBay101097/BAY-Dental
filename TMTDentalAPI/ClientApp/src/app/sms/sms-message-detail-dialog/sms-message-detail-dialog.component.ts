import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SmsComfirmDialogComponent } from '../sms-comfirm-dialog/sms-comfirm-dialog.component';
import { SmsMessageDetailPaged, SmsMessageDetailService } from '../sms-message-detail.service';

@Component({
  selector: 'app-sms-message-detail-dialog',
  templateUrl: './sms-message-detail-dialog.component.html',
  styleUrls: ['./sms-message-detail-dialog.component.css']
})
export class SmsMessageDetailDialogComponent implements OnInit {

  id: string;
  title: string;
  state: string;
  limit: number = 20;
  offset: number = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  listMessageDetails: any[];
  filteredState: any[] = [
    { name: 'Gửi thất bại', value: 'fails' },
    { name: 'Gửi Thành công', value: 'success' }
  ];
  selectedIds: string[] = [];

  constructor(
    private smsMessageDetailService: SmsMessageDetailService, 
    public activeModal: NgbActiveModal,
    private notificationService: NotificationService, 
    private modalService: NgbModal
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
    this.loading = true;
    var val = new SmsMessageDetailPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search || '';
    val.state = this.state || '';
    val.smsMessageId = this.id || '';
    val.state = this.state ? this.state : '';
    this.smsMessageDetailService.getPagedStatistic(val)
      .pipe(map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))).subscribe(res => {
        this.gridData = res;
        this.loading = false;
        if (res.data) {
          this.listMessageDetails = res.data;
        }
      })
  }

  pageChange(event): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  onChangeState(state) {
    this.state = state ? state.value : "";
    this.offset = 0;
    this.loadDataFromApi();
  }

  stranslateCodeResponse(code) {
    switch (code) {
      case "" || null:
        return "Gửi thành công";
      case "sms_server":
        return "Lỗi hệ thống đối tác";
      case "sms_acc":
        return "Lỗi tài khoản của bạn";
      case "sms_credit":
        return "Số dư không đủ";
      case "sms_blacklist":
        return "Tin nhắn chứa các ký tự đặc biệt";
      case "sms_template":
        return "Mẫu tin nhắn chưa đúng";
      default:
        return "Lý do khác";
    }
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

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("Chưa chọn khách hàng", false);
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
}
