import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { UomCrUpComponent } from '../uom-cr-up/uom-cr-up.component';
import { UomImportExcelDialogComponent } from '../uom-import-excel-dialog/uom-import-excel-dialog.component';
import { UoMPaged, UomService } from '../uom.service';

@Component({
  selector: 'app-uom-list',
  templateUrl: './uom-list.component.html',
  styleUrls: ['./uom-list.component.css']
})
export class UomListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  title = 'Đơn vị tính';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private uoMService: UomService,
    private modalService: NgbModal,
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

  loadDataFromApi() {
    this.loading = true;
    var val = new UoMPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.uoMService.getPaged(val).pipe(
      map((response: any) =>
        (<GridDataResult>{
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
    let modalRef = this.modalService.open(UomImportExcelDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
      scrollable: true
    });

    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => {}
    );
  }

  createItem() {
    let modalRef = this.modalService.open(UomCrUpComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm đơn vị tính';
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  editItem(item) {
    let modalRef = this.modalService.open(UomCrUpComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa đơn vị tính';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa đơn vị tính';

    modalRef.result.then(() => {
      this.uoMService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success', 'Xóa thành công');
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
