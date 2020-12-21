import { LaboOrderBasic, LaboOrderPaged, LaboOrderService } from './../labo-order.service';
import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-labo-order-detail-list',
  templateUrl: './labo-order-detail-list.component.html',
  styleUrls: ['./labo-order-detail-list.component.css']
})
export class LaboOrderDetailListComponent implements OnInit {
  @Input() public item: any;
  @Input() public state: string;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: LaboOrderBasic[];
  loading = false;
  constructor(private laboOrderService: LaboOrderService,private modalService: NgbModal) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    debugger
    var val = new LaboOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.saleOrderLineId = this.item.id;
    val.state = this.state == undefined ? '' : this.state;
    this.laboOrderService.getLaboForSaleOrderLine(val).pipe(
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

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  GetTeeth(val){
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

  createItem() {
   // this.router.navigate(['/labo-orders/form']);
  }

  editItem(item: LaboOrderBasic) {
    //this.router.navigate(['/labo-orders/form'], { queryParams: { id: item.id } });
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
