import { LaboOrderBasic, LaboOrderPaged, LaboOrderService } from './../labo-order.service';
import { Component, Input, OnInit, Output } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { Subject } from 'rxjs';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-labo-order-detail-list',
  templateUrl: './labo-order-detail-list.component.html',
  styleUrls: ['./labo-order-detail-list.component.css']
})
export class LaboOrderDetailListComponent implements OnInit {
  @Input() public item: any;
  @Input() public state: string;
  @Output() reload : Subject<boolean> = new Subject<boolean>();
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: LaboOrderBasic[];
  loading = false;
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];
  // check permissions
  canAdd: boolean= false;
  canUpdate: boolean = false;
  canDelete: boolean = false;

  constructor(private laboOrderService: LaboOrderService, private modalService: NgbModal,
    private printService: PrintService, 
    private checkPermissionService: CheckPermissionService,
    private notifyService: NotifyService,
    ) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.checkRole();
  }

  loadDataFromApi() {
    this.loading = true;
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
      console.log(this.gridData);
      
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

  getTeeth(val) {
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
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo phiếu Labo';
    modalRef.componentInstance.saleOrderLineId = this.item.id;
    // modalRef.componentInstance.saleOrderLineLabo = this.item;
    modalRef.result.then(res => {
      this.loadDataFromApi();
      this.reload.next(true);
    }, () => {
    });
  }

  editItem(item) {    
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;
    // modalRef.componentInstance.saleOrderLineLabo = this.item;

    modalRef.result.then(res => {
      this.loadDataFromApi();
      this.reload.next(true);
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu Labo';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink([item.id]).subscribe(() => {
        this.notifyService.notify("success", "Xóa phiếu Labo thành công");
        this.loadDataFromApi();
        this.reload.next(true);
      });
    });
  }

  printLabo(item: any) {
    this.laboOrderService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result);
    });
  }

  checkRole(){
    this.canAdd = this.checkPermissionService.check(['Labo.LaboOrder.Create']);
    this.canUpdate = this.checkPermissionService.check(['Labo.LaboOrder.Update']);
    this.canDelete = this.checkPermissionService.check(['Labo.LaboOrder.Delete']);
  }

  getToothType(key) {
    var type = this.toothTypeDict.find(x=> x.value == key);
    return type;
  }
}
