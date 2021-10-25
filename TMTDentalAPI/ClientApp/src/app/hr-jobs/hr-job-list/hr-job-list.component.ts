import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { HrJobCuDialogComponent } from '../hr-job-cu-dialog/hr-job-cu-dialog.component';
import { HrJobService, HrJobsPaged } from '../hr-job.service';

@Component({
  selector: 'app-hr-job-list',
  templateUrl: './hr-job-list.component.html',
  styleUrls: ['./hr-job-list.component.css']
})
export class HrJobListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string = '';
  searchUpdate = new Subject<string>();
  constructor(
    private hrJobService: HrJobService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    let val = new HrJobsPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search;

    this.loading = true;
    this.hrJobService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res: any) => {
      this.gridData = res;

      this.loading = false;
    }, error => {
      console.log(error);
      this.loading = false;
    })
  }

  createItem() {
    const modalRef = this.modalService.open(HrJobCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chức vụ nhân viên';
    modalRef.result.then((res: any) => {
      this.loadDataFromApi();
    })
  }

  editItem(item: any) {
    const modalRef = this.modalService.open(HrJobCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chức vụ nhân viên';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then((res: any) => {
      this.loadDataFromApi();
    })
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa chức vụ nhân viên';
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa?`;
    modalRef.result.then(() => {
      this.hrJobService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success','Xóa thành công');
        this.loadDataFromApi();
      },(error) => {
        console.log(error);
      });
    }, (error) => {
      console.log(error);
    });
  }

  pageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }
}
