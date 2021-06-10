import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { CashBookCuDialogComponent } from '../../shared/cash-book-cu-dialog/cash-book-cu-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PrintService } from 'src/app/shared/services/print.service';
import { PhieuThuChiPaged, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';

@Component({
  selector: 'app-cash-book-tab-page-re-pa',
  templateUrl: './cash-book-tab-page-re-pa.component.html',
  styleUrls: ['./cash-book-tab-page-re-pa.component.css']
})
export class CashBookTabPageRePaComponent implements OnInit {
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  type: string;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;

  // permission 
  canPhieuThuChiCreate = this.checkPermissionService.check(["Account.PhieuThuChi.Create"]);
  canPhieuThuChiUpdate = this.checkPermissionService.check(["Account.PhieuThuChi.Update"]);
  canPhieuThuChiDelete = this.checkPermissionService.check(["Account.PhieuThuChi.Delete"]);

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private route: ActivatedRoute, 
    private modalService: NgbModal, 
    private intlService: IntlService, 
    private authService: AuthService, 
    private phieuThuChiService: PhieuThuChiService,
    private notificationService: NotificationService,
    private printService: PrintService, 
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.skip = 0;
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  getType(type) {
    if (type == "thu") {
      return "Phiếu thu";
    } else {
      return "Phiếu chi";
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
      return "";
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var paged = new PhieuThuChiPaged();
    paged.companyId = this.authService.userInfo.companyId;
    paged.limit = this.limit;
    paged.offset = this.skip;
    paged.type = this.type;
    paged.accountType = "other" || '';
    paged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : '';
    paged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : '';
    paged.search = this.search || '';

    this.loading = true;
    this.phieuThuChiService.getPaged(paged).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
      this.loading = false;
    }, (err) => {
      console.log(err);
      this.loading = false;
    })
  }

  searchChangeDate(value) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  createItem() {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.type = this.type;
    modalRef.result.then((res: any) => {
      this.loadDataFromApi();
      if (res && res.print) {
        this.printPhieu(res.id);
      }
    }, (err) => { });
  }

  printPhieu(id: string) {
    this.phieuThuChiService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }

  editItem(item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.type = this.type;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then((res) => {
      this.loadDataFromApi();
      if (res && res.print) {
        this.printPhieu(item.id);
      }
    }, (err) => { });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = `Xóa ${this.getType(this.type).toLowerCase()}`;
      modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa ${this.getType(this.type).toLowerCase()}?`;
      modalRef.result.then((res) => {
        this.phieuThuChiService.delete(item.id).subscribe(() => {
          this.notificationService.show({
            content: "Xóa thành công",
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
    var paged = new PhieuThuChiPaged();
    paged.companyId = this.authService.userInfo.companyId;
    paged.limit = this.limit;
    paged.offset = this.skip;
    paged.type = this.type;
    paged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : '';
    paged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : '';
    paged.search = this.search || '';

    this.phieuThuChiService.exportExcelFile(paged).subscribe((res) => {
      let filename = this.type == "thu" ? "PhieuThu" : "PhieuChi";

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

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
