import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PartnerTitleCuDialogComponent } from 'src/app/shared/partner-title-cu-dialog/partner-title-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PartnerTitle, PartnerTitlePaged, PartnerTitleService } from '../partner-title.service';

@Component({
  selector: 'app-partner-title-list',
  templateUrl: './partner-title-list.component.html',
  styleUrls: ['./partner-title-list.component.css']
})
export class PartnerTitleListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  
  search: string;
  searchUpdate = new Subject<string>();
  
  constructor(
    private modalService: NgbModal, 
    private partnerTitleService: PartnerTitleService,
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

  loadDataFromApi() {
    this.loading = true;
    var val = new PartnerTitlePaged();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search || '';

    this.partnerTitleService.getPaged(val).pipe(
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
    const modalRef = this.modalService.open(PartnerTitleCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm danh xưng';
    modalRef.result.then(() => {
      this.notifyService.notify("success","Lưu thành công");
       this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item: PartnerTitle) {
    const modalRef = this.modalService.open(PartnerTitleCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa danh xưng';
    modalRef.componentInstance.itemId = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify("success","Lưu thành công");
       this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item: PartnerTitle) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa danh xưng';
    modalRef.componentInstance.body = 'Bạn có chắc chắn xóa danh xưng';

    modalRef.result.then(() => {
      this.partnerTitleService.delete(item.id).subscribe(() => {
      this.notifyService.notify("success","Xóa thành công");
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }
}
