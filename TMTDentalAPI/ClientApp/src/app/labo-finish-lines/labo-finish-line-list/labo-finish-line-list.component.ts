import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboFinishLineCuDialogComponent } from '../labo-finish-line-cu-dialog/labo-finish-line-cu-dialog.component';
import { LaboFinishLineBasic, LaboFinishLinePaged, LaboFinishLineService } from '../labo-finish-line.service';
import { LaboFinnishLineImportComponent } from '../labo-finnish-line-import/labo-finnish-line-import.component';

@Component({
  selector: 'app-labo-finish-line-list',
  templateUrl: './labo-finish-line-list.component.html',
  styleUrls: ['./labo-finish-line-list.component.css']
})
export class LaboFinishLineListComponent implements OnInit {

  constructor(
    private laboFinishLineService: LaboFinishLineService, private notificationService: NotificationService,
    private modalService: NgbModal
  ) { }

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
    var val = new LaboFinishLinePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.laboFinishLineService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(this.gridData);
      
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
    let modalRef = this.modalService.open(LaboFinishLineCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm đường hoàn tất Labo';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: LaboFinishLineBasic) {
    let modalRef = this.modalService.open(LaboFinishLineCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa đường hoàn tất Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  deleteItem(item: LaboFinishLineBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa đường hoàn tất Labo';
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa đường hoàn tất Labo ${item.name}?`;
    modalRef.result.then(() => {
      this.laboFinishLineService.delete(item.id).subscribe(() => {
        this.notify('success', 'Xóa thành công');
        this.loadDataFromApi();
      });
    }, () => {
    });
  }

  onImport() {
    let modalRef = this.modalService.open(LaboFinnishLineImportComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable:true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'finish_line';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

}
