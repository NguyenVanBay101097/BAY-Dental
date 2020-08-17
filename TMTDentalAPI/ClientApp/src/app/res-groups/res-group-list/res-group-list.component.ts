import { Component, ViewChild, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ResGroupService, ResGroupPaged, ResGroupBasic } from '../res-group.service';
import { Router } from '@angular/router';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-res-group-list',
  templateUrl: './res-group-list.component.html',
  styleUrls: ['./res-group-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class ResGroupListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private resGroupService: ResGroupService,
    private router: Router, private modalService: NgbModal, private notificationService: NotificationService) {
  }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

  }

  resetData() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Reset dữ liệu phân quyền';
    modalRef.componentInstance.body = 'Những thiết lập nhóm quyền trên người dùng sẽ mất, bạn có chắc chắn?';
    modalRef.result.then(() => {
      this.resGroupService.resetSecurityData().subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  updateModels() {
    this.resGroupService.updateModels().subscribe(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ResGroupPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.resGroupService.getPaged(val).pipe(
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
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['res-groups/create']);
  }

  editItem(item: ResGroupBasic) {
    this.router.navigate(['res-groups/edit/', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa nhóm quyền';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.resGroupService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    });
  }
}

