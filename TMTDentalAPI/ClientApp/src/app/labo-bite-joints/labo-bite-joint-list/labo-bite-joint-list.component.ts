import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { LaboFinnishLineImportComponent } from 'src/app/labo-finish-lines/labo-finnish-line-import/labo-finnish-line-import.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboBiteJointCuDialogComponent } from '../labo-bite-joint-cu-dialog/labo-bite-joint-cu-dialog.component';
import { LaboBiteJointBasic, LaboBiteJointPaged, LaboBiteJointService } from '../labo-bite-joint.service';

@Component({
  selector: 'app-labo-bite-joint-list',
  templateUrl: './labo-bite-joint-list.component.html',
  styleUrls: ['./labo-bite-joint-list.component.css']
})
export class LaboBiteJointListComponent implements OnInit {
  constructor(
    private laboBiteJointService: LaboBiteJointService, private notificationService: NotificationService,
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
    var val = new LaboBiteJointPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.laboBiteJointService.getPaged(val).pipe(
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
    let modalRef = this.modalService.open(LaboBiteJointCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khớp cắn Labo';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: LaboBiteJointBasic) {
    let modalRef = this.modalService.open(LaboBiteJointCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khớp cắn Labo';
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

  deleteItem(item: LaboBiteJointBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa kiểu nhịp Labo';
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa kiểu nhịp Labo ${item.name}?`;
    modalRef.result.then(() => {
      this.laboBiteJointService.delete(item.id).subscribe(() => {
        this.notify('success','Xóa thành công');
        this.loadDataFromApi();
      });
    }, () => {
    });
  }

  onImport() {
    let modalRef = this.modalService.open(LaboFinnishLineImportComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static',scrollable:true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'labo_bitejoint';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }
}
