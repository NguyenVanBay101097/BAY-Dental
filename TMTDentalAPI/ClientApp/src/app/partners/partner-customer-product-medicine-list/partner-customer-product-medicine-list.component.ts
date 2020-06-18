import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductMedicineCuDialogComponent } from 'src/app/products/product-medicine-cu-dialog/product-medicine-cu-dialog.component';
import { Product } from 'src/app/products/product';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-partner-customer-product-medicine-list',
  templateUrl: './partner-customer-product-medicine-list.component.html',
  styleUrls: ['./partner-customer-product-medicine-list.component.css']
})
export class PartnerCustomerProductMedicineListComponent implements OnInit {

  id: string;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchCateg: ProductCategoryBasic;

  constructor(
    private activeRoute: ActivatedRoute, 
    private productService: ProductService, 
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.snapshot['_routerState']._root.children[0].value.params.id; 
    this.loadData(); 
  }

  loadData() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.categId = this.searchCateg ? this.searchCateg.id : '';
    val.type2 = 'medicine';

    this.productService.getPaged(val).pipe(
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

  createProductMedicine() {
    let modalRef = this.modalService.open(ProductMedicineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Thuốc';
    modalRef.result.then(() => {
      this.loadData();
    }, () => {
    });
  }

  editProductMedicine(item: Product) {
    let modalRef = this.modalService.open(ProductMedicineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa Thuốc';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadData();
    }, () => {
    });
  }

  deleteProductMedicine(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa Thuốc';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Thuốc này?';

    modalRef.result.then(() => {
      this.productService.delete(item.id).subscribe(() => {
        this.loadData();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
  
}
