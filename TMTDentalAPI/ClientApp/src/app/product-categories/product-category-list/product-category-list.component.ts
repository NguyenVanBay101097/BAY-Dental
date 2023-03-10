import { Component, ViewChild, OnInit, Inject } from '@angular/core';
import { ProductCategoryService, ProductCategoryPaged, ProductCategoryDisplay } from '../product-category.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProductCategory } from '../product-category';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductCategoryImportExcelDialogComponent } from '../product-category-import-excel-dialog/product-category-import-excel-dialog.component';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-product-category-list',
  templateUrl: './product-category-list.component.html',
  providers: [ProductCategoryService],
  styleUrls: ['./product-category-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ProductCategoryListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();
  type: string;

  constructor(private productCategoryService: ProductCategoryService,
    private modalService: NgbModal,
    private route: ActivatedRoute,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.type = params.get('type');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type;

    this.productCategoryService.getPaged(val).pipe(
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
    let modalRef = this.modalService.open(ProductCategoryImportExcelDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = this.type;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  getTypeTitle() {
    switch (this.type) {
      case 'service':
        return 'Nh??m d???ch v???';
      case 'product':
        return 'Nh??m v???t t??';
      case 'medicine':
        return 'Nh??m thu???c';
        case 'labo':
        return 'Nh??m Labo';
      default:
        return 'Nh??m s???n ph???m';
    }
  }

  createItem() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m: ' + this.getTypeTitle();
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(result => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: ProductCategory) {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a: ' + this.getTypeTitle();
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a: ' + this.getTypeTitle();
    modalRef.result.then(() => {
      this.productCategoryService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
