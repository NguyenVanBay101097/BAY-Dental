import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { WarrantyCuDidalogComponent } from 'src/app/shared/warranty-cu-didalog/warranty-cu-didalog.component';
import { LaboOrderService } from '../../labo-order.service';
import { LaboWarrantyPaged, LaboWarrantyService } from '../../labo-warranty.service';

@Component({
  selector: 'app-labo-order-warranty-list',
  templateUrl: './labo-order-warranty-list.component.html',
  styleUrls: ['./labo-order-warranty-list.component.css']
})
export class LaboOrderWarrantyListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string = '';
  state: string = '';
  searchUpdate = new Subject<string>();
  laboOrder: any;
  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Mới', value: 'new' },
    { text: 'Đã gửi', value: 'sent' },
    { text: 'Đã nhận', value: 'received' },
    { text: 'Đã Lắp', value: 'assembled' }
  ];
  constructor(
    private laboWarrantyService: LaboWarrantyService,
    private modalService: NgbModal,
    private laboOrderService: LaboOrderService,

  ) { }

  ngOnInit() {
    this.loadDataFromApi()
  }

  loadLaboOrder(laboOrderId) {
    this.laboOrderService.get(laboOrderId).subscribe(result => {
      this.laboOrder = result;
      console.log(this.laboOrder);
      
      // this.patchValue(result);
      // this.processTeeth(result.saleOrderLine.teeth);
    });
  }

  loadDataFromApi() {
    let val = new LaboWarrantyPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.supplierId = '';
    val.state = this.state || '';
    val.laboOrderId = '';
    val.dateReceiptFrom = '';
    val.dateReceiptTo = '';
    this.laboWarrantyService.getPaged(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      console.log(this.gridData);

      this.loading = false;
    }, err => { this.loading = false; });
  }

  editItem() {

  }

  editWarranty(item) {
    const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    // modalRef.componentInstance.laboId = item.id;
    modalRef.componentInstance.laboWarrantyId = item.id;
    // modalRef.componentInstance.laboTeeth = item.teeth;

    modalRef.result.then((res) => {
      this.loadDataFromApi()
    }, (err) => { console.log(err) });
  }

  editLabo(item) {
    console.log(item);
    this.loadLaboOrder(item.laboOrderId)
    // const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    // modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    // modalRef.componentInstance.id = item.laboOrderId;
    // // modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;

    // modalRef.result.then(res => {
    //   this.loadDataFromApi();
    // }, () => {
    // });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  exportExcelFile() {

  }

  showState(state) {
    switch (state) {
      case 'new':
        return 'Mới'
      case 'sent':
        return 'Đã gửi'
      case 'received':
        return 'Đã nhận'
      case 'assembled':
        return 'Đã lắp'
      default:
        return 'Nháp'
    }
  }
}
