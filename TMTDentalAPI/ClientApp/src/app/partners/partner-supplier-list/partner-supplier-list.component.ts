import { NotificationService } from '@progress/kendo-angular-notification';
import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerPaged, PartnerBasic } from '../partner-simple';
import { PartnerActivePatch, PartnerService } from '../partner.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { PartnerSupplierCuDialogComponent } from 'src/app/shared/partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';

@Component({
  selector: 'app-partner-supplier-list',
  templateUrl: './partner-supplier-list.component.html',
  styleUrls: ['./partner-supplier-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerSupplierListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();
  title = 'Nhà cung cấp';
  filterActive = [
    { name: 'Hoạt động', value: true },
    { name: 'Ngưng hoạt động', value: false }
  ];
  active: boolean = true;
  defaultFilterActive: any = this.filterActive[0];
  constructor(private partnerService: PartnerService, private modalService: NgbModal, private notificationService: NotificationService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.supplier = true;
    val.active = (this.active || this.active == false) ? this.active : '';
    val.search = this.search || '';
    val.computeCreditDebit = true;

    this.loading = true;
    this.partnerService.getPaged(val).pipe(
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

  importFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.type = 'supplier';
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Import thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    }, () => {
    });
  }

  onPaymentChange() {
    this.loadDataFromApi();
  }


  createItem() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.title;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: PartnerBasic) {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: ' + this.title;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: PartnerBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;

    modalRef.result.then(() => {
      this.partnerService.delete(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  onChangeFilterActive(e) {
    this.active = e ? e.value : null;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onClickActive(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = (!item.active ? 'Hiện nhà cung cấp ' : 'Ẩn nhà cung cấp ') + item.name;
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn ' + ((!item.active ? 'hiện nhà cung cấp ' : 'ẩn nhà cung cấp ') + item.name);
    modalRef.result.then(() => {
      var res = new PartnerActivePatch();
      res.active = item.active ? false : true;
      this.partnerService.patchActive(item.id, res).subscribe(() => {
        this.notificationService.show({
          content:  !item.active ? 'Hiện nhà cung cấp thành công ' : 'Ẩn nhà cung cấp thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      });
    }, () => {
    });


  }

}
