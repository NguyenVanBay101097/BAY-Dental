import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SalaryPaymentService } from 'src/app/shared/services/salary-payment.service';
import { SalaryPaymentBindingDirective } from 'src/app/shared/directives/salary-payment-binding.directive';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { SalaryPaymentFormComponent } from '../salary-payment-form/salary-payment-form.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-salary-payment-list',
  templateUrl: './salary-payment-list.component.html',
  styleUrls: ['./salary-payment-list.component.css']
})
export class SalaryPaymentListComponent implements OnInit {
  type: string;
  search: string;
  searchUpdate = new Subject<string>();
  loading = false;
  limit = 20;
  skip = 0;
  gridData: GridDataResult;

  @ViewChild(SalaryPaymentBindingDirective, { static: true }) dataBinding: SalaryPaymentBindingDirective;

  gridFilter: CompositeFilterDescriptor = {
    logic: "and",
    filters: []
  };
  gridSort = [];
  advanceFilter: any = {
    params: {},
    expand : "Employee,Journal",
  };

  constructor(
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private salaryPaymentService: SalaryPaymentService,
    private router: Router
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.dataBinding.filter = this.generateFilter();       
        this.refreshData();
      });
  }

  updateFilter() {
    this.gridFilter = this.generateFilter();
  }



  refreshData() {
    this.dataBinding.rebind();
  }

  generateFilter() {
    var filter: CompositeFilterDescriptor = {
      logic: "and",
      filters: []
    };

    if (this.search) {
      filter.filters.push({
        logic: "or",
        filters: [
          { field: "Name", operator: "contains", value: this.search }
        ]
       
      });
    }
    return filter;
  }

  createItem() {
    let modalRef = this.modalService.open(SalaryPaymentFormComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Phiếu tạm ứng';
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    });
  }

  editItem(item) {
    let modalRef = this.modalService.open(SalaryPaymentFormComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Sửa: " + (item.Type == "advance" ? "phiếu tạm ứng" : "phiếu chi lương");
    modalRef.componentInstance.id = item.Id;
    modalRef.result.then(
      () => {
        this.refreshData();
      },
      () => { }
    );
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xóa: " + (item.Type == "advance" ? "phiếu tạm ứng" : "phiếu chi lương");
    modalRef.result.then(
      () => {
        this.salaryPaymentService.delete(item.Id).subscribe(
          () => {
            this.refreshData();
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
      case "waitting":
        return "Chờ xác nhận";
      case "posted":
        return "Xác nhận";
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
