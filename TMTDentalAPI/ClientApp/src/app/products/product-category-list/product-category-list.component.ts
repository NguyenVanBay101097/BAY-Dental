import { Component, DoCheck, EventEmitter, Input, IterableDiffer, IterableDiffers, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-product-category-list',
  templateUrl: './product-category-list.component.html',
  styleUrls: ['./product-category-list.component.css']
})
export class ProductCategoryListComponent implements OnInit, DoCheck {
  @Input() type: string;
  @Output() onSelect = new EventEmitter<any>();
  @Output() onDelete = new EventEmitter<any>();
  @Output() createBtnEvent = new EventEmitter<any>();
  @Output() updateBtnEvent = new EventEmitter<any>();
  searchCate: string;
  @Input() categories: any[];
  sourceCategories: any[];
  searchCateUpdate = new Subject<string>();
  category: any;
  canAdd = false;
  iterableDiffer: IterableDiffer<any>;
  constructor(private productCategoryService: ProductCategoryService,
    private modalService: NgbModal,
    private checkPermissionService: CheckPermissionService,
    private iterableDiffers: IterableDiffers,
    private notifyService: NotifyService
  ) {
    this.iterableDiffer = this.iterableDiffers.find([]).create(null);
   }

  ngOnInit() {
    this.sourceCategories = this.categories.slice();
    this.checkPermission();
  }

  ngDoCheck() {
    var changes = this.iterableDiffer.diff(this.categories);
    if (changes) {
      this.sourceCategories = this.categories.slice();
    }
  }

  onSearchChange(val: string) {
    this.sourceCategories = this.categories.filter(x => x.name.toLowerCase().includes(val.toLowerCase()));
  }

  loadCategories() {
    if (!this.type) {
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
      if (!this.searchCate) {
        this.categories = this.sourceCategories;
      }
    }, err => {
      console.log(err);
    })
  }

  createCate() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m: ' + this.getTitle();
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(result => {
      this.notifyService.notify('success', 'L??u th??nh c??ng');
      this.createBtnEvent.emit(result);
    }, () => {
    });
  }

  editCate(item: ProductCategory, index) {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a: ' + this.getTitle();
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'L??u th??nh c??ng');
      this.updateBtnEvent.emit(item.id);
    }, () => {
    });
  }

  deleteCate(item, index) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a: ' + this.getTitle();
    modalRef.result.then(() => {
      this.productCategoryService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success', 'X??a th??nh c??ng');
        //emit v??? cha ????? cha remove categ
        this.onDelete.emit(index);
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  onSelectCate(cate: any) {
    if (this.category === cate) {
      this.category = null;
    } else {
      this.category = cate;
    }

    this.onSelect.emit(this.category);
  }

  getTitle() {
    switch (this.type) {
      case 'service':
        return 'Nh??m d???ch v???'
      case 'product':
        return 'Nh??m v???t t??';
      case 'medicine':
        return 'Nh??m thu???c';
      default:
        return '';
    }
  }

  checkPermission(){
    this.canAdd = this.checkPermissionService.check(['Catalog.ProductCategory.Create']);
  }
}
