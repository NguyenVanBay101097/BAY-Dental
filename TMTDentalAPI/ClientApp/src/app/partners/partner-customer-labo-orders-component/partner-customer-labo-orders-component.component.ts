import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SaleOrderLinesLaboPaged } from 'src/app/core/services/sale-order-line.service';
import { LaboOrderPaged, LaboOrderService } from 'src/app/labo-orders/labo-order.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';

@Component({
  selector: 'app-partner-customer-labo-orders-component',
  templateUrl: './partner-customer-labo-orders-component.component.html',
  styleUrls: ['./partner-customer-labo-orders-component.component.css']
})
export class PartnerCustomerLaboOrdersComponentComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchUpdate = new Subject<string>();
  state: string;
  filterStatus = [
    { name: 'Nháp', value: 'draft' },
    { name: 'Đơn hàng', value: 'confirmed' },
  ];
  customerId: string;

  constructor(private laboOrderService: LaboOrderService, 
    private route: ActivatedRoute, private modalService: NgbModal) { }

  ngOnInit() {
    this.customerId = this.route.parent.snapshot.paramMap.get('id');

    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new LaboOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.state = this.state == undefined ? '' : this.state;
    val.customerId = this.customerId || '';
    val.search = this.search || '';
    console.log(val);
    this.laboOrderService.getLaboForSaleOrderLine(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(res);
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onStateChange(e) {
    var value = e ? e.value : null;
    if (value) {
      this.state = value;
    } else {
      this.state = null;
    }
    this.skip = 0;
    this.loadDataFromApi();
  }

  GetTeeth(val) {
    var list = [];
    if (val.teeth.length) {
      list.push(val.teeth.map(x => x.name).join(','));
    }
    return list;
  }

  stateGet(state) {
    switch (state) {
      case 'confirmed':
        return 'Đơn hàng';
      default:
        return 'Nháp';
    }
  }

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;

    modalRef.result.then(res => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu Labo';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}
