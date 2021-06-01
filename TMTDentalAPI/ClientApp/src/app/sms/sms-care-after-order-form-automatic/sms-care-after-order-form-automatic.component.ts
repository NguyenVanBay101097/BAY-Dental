import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SmsCareAfterOrderFormAutomaticDialogComponent } from '../sms-care-after-order-form-automatic-dialog/sms-care-after-order-form-automatic-dialog.component';
import { SmsConfigService } from '../sms-config.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';

@Component({
  selector: 'app-sms-care-after-order-form-automatic',
  templateUrl: './sms-care-after-order-form-automatic.component.html',
  styleUrls: ['./sms-care-after-order-form-automatic.component.css']
})
export class SmsCareAfterOrderFormAutomaticComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  states: string = "false,true";
  state: string;
  filterStatus = [
    { name: "Kích hoạt", value: "active" },
    { name: "Không kích hoạt", value: "inactive" }
  ];

  constructor(
    private modalService: NgbModal,
    private smsConfigService: SmsConfigService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.loadDataFromApi();
      });
  }

  onStatusChange(event) {
    this.state = event ? event.value : '';
    this.offset = 0;
    this.loadDataFromApi();
  }

  editItem(item) {
    var modalRef = this.modalService.open(SmsCareAfterOrderFormAutomaticDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
    modalRef.componentInstance.title = "Thiết lập tin nhắn gửi tự động";
    modalRef.componentInstance.templateTypeTab = "care_after_order";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      result => {
        this.loadDataFromApi();
      }
    )
  }

  deleteItem(item) {
    var modalRef = this.modalService.open(ConfirmDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
    modalRef.componentInstance.title = "Xóa thiết lập gửi tự động";
    modalRef.componentInstance.body = "Bạn có muốn xóa thiết lập gửi tin tự động không?";
    modalRef.result.then(
      result => {
        this.smsConfigService.delete(item.id).subscribe(
          () => {

          }
        )
      }
    )
  }

  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: this.limit,
      offset: this.offset,
      search: this.search,
      type: 'care-after-order',
      states: this.states,
    }
    this.smsConfigService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
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

  setupAutomaic() {
    var modalRef = this.modalService.open(SmsCareAfterOrderFormAutomaticDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
    modalRef.componentInstance.title = "Thiết lập tin nhắn gửi tự động";
    modalRef.componentInstance.templateTypeTab = "care_after_order";
    modalRef.result.then(
      result => {
        this.loadDataFromApi();
      }
    )
  }

  pageChange(event): void {
    this.offset = event.skip;
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
