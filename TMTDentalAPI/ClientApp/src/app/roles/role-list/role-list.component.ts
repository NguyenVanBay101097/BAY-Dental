import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { RoleService, ApplicationRolePaged, ApplicationRoleBasic } from '../role.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, of, Subject } from 'rxjs';
import { CheckableSettings, TreeItemLookup, CheckedState } from '@progress/kendo-angular-treeview';
import { WebService } from 'src/app/core/services/web.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { AuthResource } from 'src/app/auth/auth.resource';

const indexChecked = (keys, index) => keys.filter(k => k === index).length > 0;

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css'],
})
export class RoleListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private roleService: RoleService,
    private notificationService: NotificationService,
    private authResource: AuthResource,
    private modalService: NgbModal,
    private route: Router) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    const val = new ApplicationRolePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.roleService.getPaged(val).pipe(
      map(response => ({
        data: response.items,
        total: response.totalItems
      } as GridDataResult))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.route.navigate(['/roles/form']);
  }

  editItem(item) {
    this.route.navigate(['/roles/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa nhóm quyền';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa nhóm quyền này?';
    modalRef.result.then(() => {
      this.roleService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}

