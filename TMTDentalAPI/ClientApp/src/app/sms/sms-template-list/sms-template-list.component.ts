import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplatePaged, SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-template-list',
  templateUrl: './sms-template-list.component.html',
  styleUrls: ['./sms-template-list.component.css']
})
export class SmsTemplateListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private smsTemplateService: SmsTemplateService
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
    var val = new SmsTemplatePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    this.smsTemplateService.getPaged(val).pipe(
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
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Thêm tin nhắn mẫu';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }

  editItem(dataItem) {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = "Sửa tin nhắn mẫu";
    modalRef.componentInstance.id = dataItem.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    });
  }

  deleteItem(dataItem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = "Xóa tin nhắn mẫu";
    modalRef.componentInstance.body = "Bạn chắc chắn muốn xóa";
    modalRef.result.then(() => {
      this.smsTemplateService.delete(dataItem.id).subscribe(res => {
        this.notify("thành công", true);
        this.loadDataFromApi();
      });
    });
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

  getTemplateType(type) {
    switch (type) {
      case "birthday":
        return "Chúc mừng sinh nhật";
      case "appointment":
        return "Nhắc lịch hẹn";
      case "care_after_order":
        return "Chăm sóc sau điều trị";
      case "thanks":
        return "Cám ơn";
      case "campaign":
        return "Chiến dịch khác";
    }
  }
}
