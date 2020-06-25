import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Product } from 'src/app/products/product';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToaThuocPaged, ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { ToaThuocCuDialogSaveComponent } from 'src/app/toa-thuocs/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';

@Component({
  selector: 'app-partner-customer-product-toa-thuoc-list',
  templateUrl: './partner-customer-product-toa-thuoc-list.component.html',
  styleUrls: ['./partner-customer-product-toa-thuoc-list.component.css']
})
export class PartnerCustomerProductToaThuocListComponent implements OnInit {

  id: string;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchCateg: ProductCategoryBasic;

  constructor(
    private activeRoute: ActivatedRoute, 
    private toaThuocService: ToaThuocService, 
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.snapshot['_routerState']._root.children[0].value.params.id; 
    this.loadData(); 
  }

  loadData() {
    this.loading = true;
    var val = new ToaThuocPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.partnerId = this.id;

    this.toaThuocService.getPaged(val).pipe(
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

  createProductToaThuoc() {
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Toa Thuốc';
    modalRef.componentInstance.partnerId = this.id;
    modalRef.result.then(() => {
      this.loadData();
    }, () => {
    });
  }

  editProductToaThuoc(item: Product) {
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa Toa Thuốc';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.partnerId = this.id;

    modalRef.result.then(() => {
      this.loadData();
    }, () => {
    });
  }

  deleteProductToaThuoc(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa Toa Thuốc';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Toa Thuốc này?';

    modalRef.result.then(() => {
      this.toaThuocService.delete(item.id).subscribe(() => {
        this.loadData();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
  
}
