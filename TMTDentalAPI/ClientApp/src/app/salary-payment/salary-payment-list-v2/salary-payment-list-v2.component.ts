import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { load } from '@progress/kendo-angular-intl';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AccountPaymentPaged, AccountPaymentSave, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';
import { SalaryPaymentDialogV2Component } from '../salary-payment-dialog-v2/salary-payment-dialog-v2.component';
import { SalaryPaymentFormComponent } from '../salary-payment-form/salary-payment-form.component';

@Component({
  selector: 'app-salary-payment-list-v2',
  templateUrl: './salary-payment-list-v2.component.html',
  styleUrls: ['./salary-payment-list-v2.component.css']
})
export class SalaryPaymentListV2Component implements OnInit {
  type: string;
  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  constructor(
    private accountPaymentService: AccountPaymentService,
    private modalService: NgbModal, private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  computeState(state) {
    switch (state) {
      case "posted":
        return "Đã xác nhận";
      case "draft":
        return "Nháp";
      case "cancel":
        return "Đã hủy";
      default:
        break;
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountPaymentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search ? this.search : '';
    val.partnerType = "employee";

    this.accountPaymentService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
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
    let modalRef = this.modalService.open(SalaryPaymentDialogV2Component, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Phiếu tạm ứng';
    modalRef.result.then((result) => {
      this.loadDataFromApi();
      if (result && result.print) {
        this.printItem(result.id);
      }
    }, () => {
    });
  }

  printItem(id) {
    this.accountPaymentService.getPrint(id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  editItem(item) {
    let modalRef = this.modalService.open(SalaryPaymentDialogV2Component, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Sửa: Phiếu tạm ứng/chi lương";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      (result) => {
        this.loadDataFromApi();
        if (result && result.print) {
          this.printItem(item.id);
        }
      },
      () => { }
    );
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xóa: Phiếu tạm ứng, chi lương";
    modalRef.componentInstance.body = `Bạn có chắc chắn muốn xóa phiếu ${item.name}?`;
    modalRef.result.then(
      () => {
        this.accountPaymentService.unlink([item.id]).subscribe(
          () => {
            this.loadDataFromApi();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => { }
    );
  }
}
