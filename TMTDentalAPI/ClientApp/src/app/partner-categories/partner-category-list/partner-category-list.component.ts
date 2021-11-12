import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PartnerCategoryCuDialogComponent } from '../partner-category-cu-dialog/partner-category-cu-dialog.component';
import { PartnerCategoryImportComponent } from '../partner-category-import/partner-category-import.component';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from '../partner-category.service';

@Component({
  selector: 'app-partner-category-list',
  templateUrl: './partner-category-list.component.html',
  styleUrls: ['./partner-category-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class PartnerCategoryListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();

  title = 'Nhãn khách hàng';

  constructor(
    private partnerCategoryService: PartnerCategoryService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

  }

  importFromExcel() {
    const modalRef = this.modalService.open(PartnerCategoryImportComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PartnerCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.partnerCategoryService.getPaged(val).pipe(
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(PartnerCategoryCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhãn khách hàng';

    modalRef.result.then(() => {
      this.notifyService.notify("success","Lưu thành công");
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: PartnerCategoryBasic) {
    let modalRef = this.modalService.open(PartnerCategoryCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa nhãn khách hàng';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify("success","Lưu thành công");
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: PartnerCategoryBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa nhãn khách hàng';
    modalRef.componentInstance.body = 'Bạn có chắc chắn xóa nhãn khách hàng';
    modalRef.result.then(() => {
      this.partnerCategoryService.delete(item.id).subscribe(() => {
      this.notifyService.notify("success","Xóa thành công");
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
