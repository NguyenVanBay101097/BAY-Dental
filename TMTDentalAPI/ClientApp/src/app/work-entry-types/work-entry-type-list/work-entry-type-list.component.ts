
import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { WorkEntryTypeService, WorkEntryTypePage } from '../work-entry-type.service';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { WorkEntryTypeCreateUpdateDialogComponent } from '../work-entry-type-create-update-dialog/work-entry-type-create-update-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-work-entry-type-list',
  templateUrl: './work-entry-type-list.component.html',
  styleUrls: ['./work-entry-type-list.component.css']
})
export class WorkEntryTypeListComponent implements OnInit {
  title: string = "Loại chấm công";
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;

  constructor(
    private workEntryTypeService: WorkEntryTypeService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new WorkEntryTypePage();
    val.limit = this.limit;
    val.offset = this.skip;
    val.filter = this.search || '';
    this.workEntryTypeService.getPaged(val).pipe(
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

  createItem() {
    let modalRef = this.modalService.open(WorkEntryTypeCreateUpdateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.title;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    let modalRef = this.modalService.open(WorkEntryTypeCreateUpdateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: ' + this.title;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;

    modalRef.result.then(() => {
      this.workEntryTypeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
        this.notificationService.show({
          content: 'Xóa thành công!',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

}
