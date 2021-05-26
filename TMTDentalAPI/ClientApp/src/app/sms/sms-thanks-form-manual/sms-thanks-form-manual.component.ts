import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';

@Component({
  selector: 'app-sms-thanks-form-manual',
  templateUrl: './sms-thanks-form-manual.component.html',
  styleUrls: ['./sms-thanks-form-manual.component.css']
})
export class SmsThanksFormManualComponent implements OnInit {
  formGroup: FormGroup;
  filteredTemplate: any[];
  gridData: any;
  skip: number = 0;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public yesterday: Date = new Date(new Date(new Date().setDate(new Date().getDate() - 1)).toDateString());
  public today: Date = new Date();

  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.dateFrom = this.yesterday;
    this.dateTo = this.today;
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
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.dateOrderFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateOrderTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    val.state = "done";
    this.saleOrderService.getSaleOrderForSms(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
    }, err => {
      console.log(err);
    }
    )
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
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tạo tin gửi";
      modalRef.componentInstance.ids = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.isThanksCustomer = true;
      modalRef.componentInstance.templateTypeTab = "thanks";
      modalRef.result.then(
        result => {

        }
      )
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
}
