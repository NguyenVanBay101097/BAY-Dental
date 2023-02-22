import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { SalaryPaymentFormComponent } from '../salary-payment-form/salary-payment-form.component';
import { SalaryPaymentPaged, SalaryPaymentService } from '../salary-payment.service';

@Component({
  selector: 'app-salary-payment-list',
  templateUrl: './salary-payment-list.component.html',
  styleUrls: ['./salary-payment-list.component.css']
})
export class SalaryPaymentListComponent implements OnInit {
  type: string;
  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;

  // permission
  canSalaryPaymentCreate = this.checkPermissionService.check(["Salary.SalaryPayment.Create"]);
  canSalaryPaymentDelete = this.checkPermissionService.check(["Salary.SalaryPayment.Delete"]);

  constructor(
    private modalService: NgbModal,
    private salaryPaymentService: SalaryPaymentService,
    private printService: PrintService,
    private checkPermissionService: CheckPermissionService,
    private notifyService: NotifyService,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadData();
      });

    this.loadData();
  }

  loadData() {
    var paged = new SalaryPaymentPaged();
    paged.limit = this.limit;
    paged.offset = this.skip;
    paged.search = this.search ? this.search : '';
    paged.companyId = this.authService.userInfo.companyId;
    this.loading = true;
    this.salaryPaymentService.getPaged(paged).pipe(
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
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadData();
  }

  createItem() {
    let modalRef = this.modalService.open(SalaryPaymentFormComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Phiếu chi tạm ứng';
    modalRef.componentInstance.type = 'advance';
    modalRef.result.then((result) => {
      this.loadData();
      if (result.print && result.id) {
        this.printItem(result.id);
      }
    }, () => {
    });
  }

  // printItem(id) {
  //   this.salaryPaymentService.getPrint([id]).subscribe(
  //     result => {
  //       if (result && result['html']) {
  //         this.printService.printHtml(result['html']);
  //       } else {
  //         alert('Có lỗi xảy ra, thử lại sau');
  //       }
  //     }
  //   )
  // }

  printItem(id) {
    this.salaryPaymentService.getPrint([id]).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }

  editItem(item) {
    let modalRef = this.modalService.open(SalaryPaymentFormComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = (item.type == "advance" ? "Phiếu chi tạm ứng" : "Phiếu chi lương");
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      (result: any) => {
        this.notifyService.notify('success', 'Lưu thành công')
        this.loadData();
        if (result && result.print) {
          this.printItem(item.id);
        }
      },
      () => { }
    );
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xóa: " + (item.type == "advance" ? "phiếu chi tạm ứng" : "phiếu chi lương");
    modalRef.result.then(
      () => {
        this.salaryPaymentService.delete(item.id).subscribe(
          () => {
            this.loadData();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => { }
    );
  }

  stateGet(value) {
    switch (value) {
      case "waiting":
        return "Chờ xác nhận";
      case "done":
        return "Đã xác nhận";
    }
  }

  getType(value) {
    switch (value) {
      case "advance":
        return "Tạm ứng";
      case "salary":
        return "Chi lương";
    }
  }
}
