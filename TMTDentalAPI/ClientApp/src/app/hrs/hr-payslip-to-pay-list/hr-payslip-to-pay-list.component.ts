import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayslipService, HrPayslipPaged } from '../hr-payslip.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { map } from 'rxjs/internal/operators/map';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { IntlService } from '@progress/kendo-angular-intl';

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
  StateFilters = [
    { text: 'tất cả', value: '' },
    { text: 'bản nháp', value: 'draft' },
    { text: 'đang xử lý', value: 'process' },
    { text: 'hoàn thành', value: 'done' }
  ];
  stateSelected: string;
  dateFrom: Date;
  dateTo: Date;
  search: string;

  constructor(
    private hrPayslipService: HrPayslipService,
    private modalService: NgbModal, private intlService: IntlService,
    private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    const val = new HrPayslipPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.search) {
      val.search = this.search;
    }
    if (this.stateSelected) {
      val.state = this.stateSelected;
    }
    if (this.dateFrom && this.dateTo) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'g', 'en-US');
      val.dateTo = this.intlService.formatDate(this.dateTo, 'g', 'en-US');
    }

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

  onSelect(state) {
    this.stateSelected = state;
    this.loadDataFromApi();
  }

  OnDateFilterChange() {
    console.log(new Date( this.dateFrom));
    console.log(this.dateTo);
    if (this.dateFrom && this.dateTo) {
      this.loadDataFromApi();
    }
  }

  GetStateFilter() {
    switch (this.stateSelected) {
      case 'draft':
        return 'bản nháp';
      case 'process':
        return 'đang xử lý';
      case 'done':
        return 'hoàn thành';
      default:
        return 'Tất cả trạng thái';
    }
  }

}
