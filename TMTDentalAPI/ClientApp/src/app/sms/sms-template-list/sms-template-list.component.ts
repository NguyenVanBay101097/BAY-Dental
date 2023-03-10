import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplatePaged, SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-template-list',
  templateUrl: './sms-template-list.component.html',
  styleUrls: ['./sms-template-list.component.css'],
  host: {
    class: "o_action o_view_controller",
  },
})
export class SmsTemplateListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string = '';
  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private smsTemplateService: SmsTemplateService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

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
    val.search = this.search || '';
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
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  pageChange(event): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'md', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm tin nhắn mẫu';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }

  editItem(dataItem) {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'md', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Sửa tin nhắn mẫu";
    modalRef.componentInstance.id = dataItem.id;
    modalRef.componentInstance.templateTypeTab = dataItem.type;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    });
  }

  deleteItem(dataItem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Xóa tin nhắn mẫu";
    modalRef.componentInstance.body = "Bạn có muốn xóa tin nhắn mẫu không";
    modalRef.result.then(() => {
      this.smsTemplateService.delete(dataItem.id).subscribe(res => {
        this.notify("Xóa tin nhắn mẫu thành công", true);
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
      case "partner":
        return "Chúc mừng sinh nhật";
      case "appointment":
        return "Nhắc lịch hẹn";
      case "saleOrderLine":
        return "Chăm sóc sau điều trị";
      case "saleOrder":
        return "Cám ơn";
      case "partnerCampaign":
        return "Chiến dịch khác";
    }
  }
}
