import { Component, OnInit } from '@angular/core';
import { ProductService, ProductPaged, ProductBasic2, ProductLaboBasic } from '../product.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ProductDialogComponent } from '../product-dialog/product-dialog.component';
import { ProductServiceCuDialogComponent } from '../product-service-cu-dialog/product-service-cu-dialog.component';
import { Product } from '../product';
import { ProductAdvanceFilter } from '../product-advance-filter/product-advance-filter.component';
import { ProductMedicineCuDialogComponent } from '../product-medicine-cu-dialog/product-medicine-cu-dialog.component';
import { ProductLaboCuDialogComponent } from '../product-labo-cu-dialog/product-labo-cu-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
@Component({
  selector: 'app-product-labo-list',
  templateUrl: './product-labo-list.component.html',
  styleUrls: ['./product-labo-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ProductLaboListComponent implements OnInit {
  constructor(private productService: ProductService, private windowService: WindowService, private dialogService: DialogService,
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

    this.productService.getLaboPaged(val).pipe(
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
    let modalRef = this.modalService.open(ProductLaboCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm Labo';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: ProductLaboBasic) {
    let modalRef = this.modalService.open(ProductLaboCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: ProductLaboBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa Labo';
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa labo ${item.name}?`;
    modalRef.result.then(() => {
      this.productService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    }, () => {
    });
  }
}


