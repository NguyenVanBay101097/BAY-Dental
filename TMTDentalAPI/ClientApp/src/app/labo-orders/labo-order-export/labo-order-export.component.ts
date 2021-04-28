import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { LaboOrderExportDialogComponent } from '../labo-order-export-dialog/labo-order-export-dialog.component';
import { ExportLaboPaged, LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-export',
  templateUrl: './labo-order-export.component.html',
  styleUrls: ['./labo-order-export.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class LaboOrderExportComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  stateFilter: string;
  dateExportFrom: Date;
  dateExportTo: Date;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Đã xuất', value: 'daxuat' },
    { text: 'Chưa xuất', value: 'chuaxuat' }
  ];

  
  filterLaboStatus = [
    { name: 'Đã xuất', value: 'daxuat' },
    { name: 'Chưa xuất', value: 'chuaxuat' }
  ];
  stateFilterOptionSelected = this.stateFilterOptions[0];

  canUpdate = false;
  canUpdateSaleOrder = false;

  constructor(
    private laboOrderService: LaboOrderService, 
    private intlService: IntlService, 
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.checkRole();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  onStateSelectChange(e) {
    this.stateFilter = e? e.value : null;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onDateSearchChange(data) {
    this.dateExportFrom = data.dateFrom;
    this.dateExportTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  getState(dataItem: any) {
    if (dataItem.dateExport) {
      return 'Đã xuất';
    }

    return 'Chưa xuất';
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ExportLaboPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.stateFilter || '';
    if (this.dateExportFrom) {
      val.dateExportFrom = this.intlService.formatDate(this.dateExportFrom, 'd', 'en-US');
    }
    if (this.dateExportTo) {
      val.dateExportTo = this.intlService.formatDate(this.dateExportTo, 'd', 'en-US');
    }

    this.laboOrderService.getExportLabo(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      console.log(res);
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderExportDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.labo = item;
    modalRef.result.then(rs => {
      if(rs == 'update'){
        this.notificationService.show({
          content: 'Cập nhật thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }else{
        this.notificationService.show({
          content: 'Hủy nhận đơn hàng thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }   
      this.loadDataFromApi();
    }, er => { });
  }

  checkRole(){
    this.canUpdate = this.checkPermissionService.check(['Labo.ExportLabo.Update']);
    this.canUpdateSaleOrder = this.checkPermissionService.check(['Basic.SaleOrder.Update']);
  }
}
