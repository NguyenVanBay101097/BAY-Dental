import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SalaryPaymentBindingDirective } from 'src/app/shared/directives/salary-payment-binding.directive';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { SalaryPaymentFormComponent } from '../salary-payment-form/salary-payment-form.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { SalaryPaymentPaged, SalaryPaymentService } from '../salary-payment.service';
import { PrintService } from 'src/app/shared/services/print.service';

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
  loading = false;

  constructor(
    private modalService: NgbModal,
    private salaryPaymentService: SalaryPaymentService,
    private printService: PrintService
  ) { }

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

  printItem(id) {
    this.salaryPaymentService.getPrint([id]).subscribe(
      result => {
        if (result && result['html']) {
          this.printService.printHtml(result['html']);
        } else {
          alert('Có lỗi xảy ra, thử lại sau');
        }
      }
    )
  }

  editItem(item) {
    let modalRef = this.modalService.open(SalaryPaymentFormComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = (item.type == "advance" ? "Phiếu chi tạm ứng" : "Phiếu chi lương");
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      (result: any) => {
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
