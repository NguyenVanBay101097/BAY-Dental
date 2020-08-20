import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayslipService, HrPayslipPaged } from '../hr-payslip.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { map } from 'rxjs/internal/operators/map';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-hr-payslip-to-pay-list',
  templateUrl: './hr-payslip-to-pay-list.component.html',
  styleUrls: ['./hr-payslip-to-pay-list.component.css']
})
export class HrPayslipToPayListComponent implements OnInit {
  gridData: GridDataResult = {
    data: [],
    total: 0
  };
  limit = 20;
  skip = 0;
  loading = false;
  collectionSize = 0;

  constructor(
    private hrPayslipService: HrPayslipService,
    private modalService: NgbModal,
    private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    const val = new HrPayslipPaged();
    val.limit = this.limit;
    val.offset = this.skip;

    this.hrPayslipService.getPaged(val).pipe(
      map((res: any) => ({
        data: res.items,
        total: res.totalItems,
      } as GridDataResult))
    )
      .subscribe(res => {
        this.gridData = res;
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/hr/payslip-to-pay/create']);
  }

  editItem(dataitem) {
    this.router.navigate(['/hr/payslip-to-pay/edit/' + dataitem.id]);
  }

  deleteItem(dataitem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu lương';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.hrPayslipService.delete(dataitem.id).subscribe(res => {
        this.loadDataFromApi();
      });
    });
  }

}
