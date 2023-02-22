import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { DataResult } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators';
import { LaboWarrantyPaged, LaboWarrantyService } from 'src/app/labo-orders/labo-warranty.service';
import { CheckPermissionService } from '../check-permission.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { NotifyService } from '../services/notify.service';
import { WarrantyCuDidalogComponent } from '../warranty-cu-didalog/warranty-cu-didalog.component';

@Component({
  selector: 'app-labo-warranty-detail-list',
  templateUrl: './labo-warranty-detail-list.component.html',
  styleUrls: ['./labo-warranty-detail-list.component.css']
})
export class LaboWarrantyDetailListComponent implements OnInit {
  @Input() item: any;
  @Input() dateReceiptFrom: any;
  @Input() dateReceiptTo: any;
  limit = 20;
  skip = 0;
  search: string = '';
  state: string = '';
  loading = true;
  gridData: GridDataResult;
  canUpdate = false;
  canAdd = false;
  canDelete = false;

  constructor(
    private laboWarrantyService: LaboWarrantyService,
    private modalService: NgbModal,
    private intlService: IntlService,
    private notifyService: NotifyService,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.checkRole();
  }

  loadDataFromApi() {
    let val = new LaboWarrantyPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.supplierId = '';
    val.states = this.state || '';
    val.laboOrderId = this.item.id || '';
    val.dateReceiptFrom = this.dateReceiptFrom ? this.intlService.formatDate(this.dateReceiptFrom, "yyyy-MM-dd") : '';
    val.dateReceiptTo = this.dateReceiptTo ? this.intlService.formatDate(this.dateReceiptTo, "yyyy-MM-dd") : '';
    this.laboWarrantyService.getPaged(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => { this.loading = false; });
  }

  createNewWarranty() {
    const date = new Date(this.item.warrantyPeriod);
    const today = new Date(new Date().toDateString());
    if (this.item.warrantyPeriod && date < today) {
        this.notifyService.notify("error", "Labo đã hết hạn bảo hành");
    } else if (!this.item.dateExport) {
      this.notifyService.notify("error", "Không thể tạo bảo hành khi chưa xuất Labo cho khách");
    } else {
      const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.laboOrderId = this.item.id;
      modalRef.componentInstance.laboTeeth = this.item.teeth;

      modalRef.result.then((res) => {
        this.loadDataFromApi()
      }, (err) => { console.log(err) });
    }
  }

  editItem(item) {
    const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.laboOrderId = this.item.id;
    modalRef.componentInstance.laboWarrantyId = item.id;

    modalRef.result.then((res) => {
      this.loadDataFromApi()
    }, (err) => { console.log(err) });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xác nhận xóa phiếu bảo hành';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa phiếu bảo hành?';
    modalRef.result.then(() => {
      this.laboWarrantyService.delete(item.id).subscribe((res) => {
        this.notifyService.notify("success", "Xóa thành công");
        this.loadDataFromApi()
      }, err => console.log(err))
    });
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  checkRole() {
    this.canAdd = this.checkPermissionService.check(['Labo.LaboWarranty.Create']);
    this.canUpdate = this.checkPermissionService.check(['Labo.LaboWarranty.Update']);
    this.canDelete = this.checkPermissionService.check(['Labo.LaboWarranty.Delete']);
  }

  get showOnlyBeveragesCreate() {
    var isShow = this.checkPermissionService.check(['Labo.LaboWarranty.Create']);
    return isShow === true;
  }
}
