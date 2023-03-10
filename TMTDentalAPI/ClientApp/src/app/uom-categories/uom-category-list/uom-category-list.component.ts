import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { UomCategoryService, UoMCategoryPaged } from '../uom-category.service';
import { UomCategoryCrUpComponent } from '../uom-category-cr-up/uom-category-cr-up.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-uom-category-list',
  templateUrl: './uom-category-list.component.html',
  styleUrls: ['./uom-category-list.component.css']
})
export class UomCategoryListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  title = 'Nhóm đơn vị tính';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private modalService: NgbModal,
    private UoMCategoryService: UomCategoryService,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new UoMCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.UoMCategoryService.getPaged(val).pipe(
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

  createItem() {
    let modalRef = this.modalService.open(UomCategoryCrUpComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhóm đơn vị tính';
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item) {
    let modalRef = this.modalService.open(UomCategoryCrUpComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa nhóm đơn vị tính';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa nhóm đơn vị tính';

    modalRef.result.then(() => {
      this.UoMCategoryService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success', 'Xóa thành công');
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
