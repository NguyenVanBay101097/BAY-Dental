import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DialogService, WindowService } from '@progress/kendo-angular-dialog';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductImportExcelDialogComponent } from '../product-import-excel-dialog/product-import-excel-dialog.component';
import { ProductLaboAttachCuDialogComponent } from '../product-labo-attach-cu-dialog/product-labo-attach-cu-dialog.component';
import { ProductLaboBasic, ProductPaged, ProductService } from '../product.service';

@Component({
  selector: 'app-product-labo-attach-list',
  templateUrl: './product-labo-attach-list.component.html',
  styleUrls: ['./product-labo-attach-list.component.css']
})

export class ProductLaboAttachListComponent implements OnInit {
  constructor(private productService: ProductService, private notificationService: NotificationService,
    private modalService: NgbModal) { }
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchCategId: string;
  searchUpdate = new Subject<string>();

  opened = false;

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

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type2 = 'labo_attach';

    this.productService.getPaged(val).pipe(
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

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(ProductLaboAttachCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm gửi kèm Labo';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: ProductLaboBasic) {
    let modalRef = this.modalService.open(ProductLaboAttachCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa gửi kèm Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  deleteItem(item: ProductLaboBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa gửi kèm Labo';
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa gửi kèm Labo ${item.name}?`;
    modalRef.result.then(() => {
      this.productService.delete(item.id).subscribe(() => {
        this.notify('success','Xóa thành công');
        this.loadDataFromApi();
      });
    }, () => {
    });
  }

  onImport() {
    let modalRef = this.modalService.open(ProductImportExcelDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static',scrollable: true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'labo_attach';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }
}
