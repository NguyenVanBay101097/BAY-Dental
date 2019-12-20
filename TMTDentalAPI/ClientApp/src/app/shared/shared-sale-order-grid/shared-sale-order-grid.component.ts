import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

@Component({
  selector: 'app-shared-sale-order-grid',
  templateUrl: './shared-sale-order-grid.component.html',
  styleUrls: ['./shared-sale-order-grid.component.css']
})
export class SharedSaleOrderGridComponent implements OnInit {
  @Input() gridData: GridDataResult;
  @Input() limit;
  @Input() skip;
  @Input() loading;
  selectedIds: string[] = [];
  @Input() hiddenSelectAll = false;
  @Input() hiddenColumnPartner = false;

  @Output() pageChange = new EventEmitter<PageChangeEvent>();
  @Output() editBtnClick = new EventEmitter<SaleOrderBasic>();
  @Output() deleteBtnClick = new EventEmitter<SaleOrderBasic>();

  constructor() { }
  ngOnInit() {
  }

  onPageChange(event: PageChangeEvent): void {
    this.pageChange.emit(event);
  }

  stateGet(state) {
    switch (state) {
      case 'sale':
        return 'Đơn hàng';
      case 'done':
        return 'Đã khóa';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  editItem(item: SaleOrderBasic) {
    this.editBtnClick.emit(item);
  }

  deleteItem(item: SaleOrderBasic) {
    this.deleteBtnClick.emit(item);
  }
}
