import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PhieuThuChiService, PhieuThuChiPaged } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-cash-book-tab-page-re-pa',
  templateUrl: './cash-book-tab-page-re-pa.component.html',
  styleUrls: ['./cash-book-tab-page-re-pa.component.css']
})
export class CashBookTabPageRePaComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  paymentType: string;
  quickOptionDate: string;
  paged: PhieuThuChiPaged;
  changeDateFirst: boolean = true;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, 
    private modalService: NgbModal, 
    private phieuThuChiService: PhieuThuChiService, 
    private intlService: IntlService, 
    private authService: AuthService, 
    private accountPaymentService: AccountPaymentService,
    private notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.paged = new PhieuThuChiPaged();
    this.paged.companyId = this.authService.userInfo.companyId;
    this.quickOptionDate = "Tháng này"; // Auto Call this.searchChangeDate()
    this.route.queryParamMap.subscribe(params => {
      this.paymentType = params.get('payment-type');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  getType(paymentType) {
    if (paymentType == "inbound") {
      return "Phiếu thu";
    } else {
      return "Phiếu chi";
    }
  }

  convertType(paymentType) {
    if (paymentType == "inbound") {
      return "thu";
    } else {
      return "chi";
    }
  }

  getState(state) {
    if (state == "posted") {
      return "Đã xác nhận";
    } else if (state == "draft") {
      return "Nháp";
    } else if (state == "cancel") {
      return "Đã hủy";
    } else {
      return 'Nháp';
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    if (this.changeDateFirst == true) {
      this.changeDateFirst = false;
      return;
    }
    this.loading = true;
    this.paged.limit = this.limit;
    this.paged.offset = this.skip;
    this.paged.paymentType = this.paymentType;
    this.accountPaymentService.getPaged(this.paged).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
      console.log(res);
      this.loading = false;
    }, (err) => {
      console.log(err);
      this.loading = false;
    })
  }

  searchChangeDate(value) {
    this.paged.paymentDateFrom = value.dateFrom ? this.intlService.formatDate(value.dateFrom, "yyyy-MM-dd") : '';
    this.paged.paymentDateTo = value.dateTo ? this.intlService.formatDate(value.dateTo, "yyyy-MM-ddT23:59") : '';
    this.loadDataFromApi();
  }

  createItem() {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.paymentType = this.paymentType;
    modalRef.result.then((res) => {
      this.loadDataFromApi();
    }, (err) => { });
  }

  editItem(item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.paymentType = this.paymentType;
    modalRef.componentInstance.itemId = item.id;
    modalRef.result.then((res) => {
      this.loadDataFromApi();
    }, (err) => { });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `Xóa ${this.getType(this.paymentType).toLowerCase()}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa ${this.getType(this.paymentType).toLowerCase()}?`;
    modalRef.result.then((res) => {
      this.accountPaymentService.unlink([item.id]).subscribe(() => {
        this.notificationService.show({
          content: "Xóa phiếu thành công",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
        this.loadDataFromApi();
      }, (res) => {
      });
    }, (err) => {
    });
  }

  exportExcelFile() {
    this.phieuThuChiService.exportExcelFile(this.paged).subscribe((res) => {
      let filename = "PhieuThu";
      if (this.paymentType == "outbound") {
        filename = "PhieuChi";
      }

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      console.log(res);

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }
}
