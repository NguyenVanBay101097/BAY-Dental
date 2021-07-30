import { NotifyService } from 'src/app/shared/services/notify.service';
import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SmsAccountSettingDialogComponent } from '../sms-account-setting-dialog/sms-account-setting-dialog.component';
import { SmsAccountService, SmsAccountPaged } from '../sms-account.service';

@Component({
  selector: 'app-sms-account-list',
  templateUrl: './sms-account-list.component.html',
  styleUrls: ['./sms-account-list.component.css']
})
export class SmsAccountListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  title = 'Danh sách Brandname';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private smsAccountService: SmsAccountService,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SmsAccountPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.smsAccountService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  createItem() {
    let modalRef = this.modalService.open(SmsAccountSettingDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm Brandname';
    modalRef.result.then(() => {
      this.loadDataFromApi();
      this.notifyService.notify('success','Lưu thành công');
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    let modalRef = this.modalService.open(SmsAccountSettingDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật Brandname: ' + item.brandName || 'BrandName';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
      this.notifyService.notify('success','Lưu thành công');
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa Brandname';
    modalRef.componentInstance.body = 'Bạn có chắc chắn xóa Brandname hay không?';
    modalRef.result.then(() => {
      this.smsAccountService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
        this.notificationService.show({
          content: 'Xóa Brandname thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
