import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SmsCampaignService } from '../sms-campaign.service';
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
  campaign: any;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public yesterday: Date = new Date(new Date(new Date().setDate(new Date().getDate() - 1)).toDateString());
  public today: Date = new Date();

  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private saleOrderService: SaleOrderService,
    private smsCampaignService: SmsCampaignService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.dateFrom = this.yesterday;
    this.dateTo = this.today;
    this.loadDataFromApi();
    setTimeout(() => {
      this.loadDefaultCampaignThanksCustomer();
    }, 1000);
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
    val.companyId = this.authService.userInfo.companyId;
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
  loadDefaultCampaignThanksCustomer() {
    this.smsCampaignService.getDefaultThanksCustomer().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      })
  }
  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("Bạn phải chọn khách hàng trước khi gửi tin", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "sm", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tin nhắn Cảm ơn";
      modalRef.componentInstance.resIds = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.resModel = 'sale-order';
      modalRef.componentInstance.templateTypeTab = "saleOrder";
      modalRef.componentInstance.campaign = this.campaign;
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
