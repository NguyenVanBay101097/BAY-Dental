import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';

@Component({
  selector: 'app-product-category-list',
  templateUrl: './product-category-list.component.html',
  styleUrls: ['./product-category-list.component.css']
})
export class ProductCategoryListComponent implements OnInit {
@Input() type: string;
@Output() onSelect =new EventEmitter<any>();
@Output() onDelete =new EventEmitter<any>();
searchCate: string;
categories: any[];
sourceCategories: any[];
searchCateUpdate = new Subject<string>();
category: any;

  constructor(private productCategoryService: ProductCategoryService,
    private modalService: NgbModal
    ) { }

  ngOnInit() {
    this.searchCateUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.searchCategories(value);
      });
    this.loadCategories();
  }

  searchCategories(val) {
    val = val.trim().toLowerCase();
    if (val === '') {
      this.categories = this.sourceCategories;
      return;
    }
    this.categories = this.sourceCategories.filter(x => x.name.toLowerCase().includes(val));
  }

  loadCategories() {
    if(!this.type) {
      return;
    }
    var val = new ProductCategoryPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = '';
    val.type = this.type;

    this.productCategoryService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.sourceCategories = res.data;
      if(!this.searchCate) {
        this.categories = this.sourceCategories;
      }
    }, err => {
      console.log(err);
    })
  }

  createCate() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: nhóm dịch vụ';
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(result => {
      debugger;
      this.loadCategories();
    }, () => {
    });
  }

  editCate(item: ProductCategory) {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: nhóm dịch vụ';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(() => {
      this.loadCategories();
    }, () => {
    });
  }

  deleteCate(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: nhóm dịch vụ';
    modalRef.result.then(() => {
      this.productCategoryService.delete(item.id).subscribe(() => {
        this.loadCategories();
        if(this.category && this.category.id == item.id) {
          this.category = null;
          this.onDelete.emit();
        }
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  onSelectCate(cate: any) {
    this.category = cate;
    this.onSelect.emit(cate);
  }

}
