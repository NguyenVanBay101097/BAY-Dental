import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { RoleService, ApplicationRolePaged, ApplicationRoleBasic } from '../role.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
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
  host: {
    class: 'o_action o_view_controller'
  }
})
export class RoleListComponent implements OnInit {

  public checkedKeys: any[] = [];
  formGroup: FormGroup;
  featuresTreeData: any = [];
  featureGroups: any;
  selectedRole: any;
  displayRole: any;
  mode = 'view';
  functions: any[] = [];
  id: string;
  rolesPaged: any;
  page: number = 1;
  pageSize: number = 10;
  title = 'Nhóm quyền';
  userList: any[] = [];
  selectedUsers: any[] = [];

  constructor(private roleService: RoleService, private router: Router, private fb: FormBuilder,
    private webService: WebService, private notificationService: NotificationService, private modalService: NgbModal,
    private authResource: AuthResource,
    private userService: UserService) { }

  ngOnInit() {
    this.loadFeatures();
    this.loadRoles();
  }

  loadFeatures() {
    this.webService.getFeatures().subscribe((data: any) => {
      this.featureGroups = data;
    });
  }

  selectUser(user) {
    var exists = this.selectedUsers.filter(x => x.id == user.id);
    if (!exists.length) {
      this.selectedUsers.push(user);
    } else {
      this.selectedUsers = this.selectedUsers.filter(x => x.id != user.id);
    }
  }

  isUserSelected(user) {
    return this.selectedUsers.filter(x => x.id == user.id).length > 0;
  }

  loadUserList() {
    var val = new UserPaged();
    val.limit = 100;
    this.userService.getPaged(val).subscribe((result: any) => {
      this.userList = result.items;
    });
  }

  // Custom logic handling Indeterminate state when custom data item property is persisted
  public isChecked = (dataItem: any, index: string): CheckedState => {
    if (this.containsItem(dataItem)) { return 'checked'; }

    if (this.isIndeterminate(dataItem.children)) { return 'indeterminate'; }

    return 'none';
  }

  private containsItem(item: any): boolean {
    return this.checkedKeys.indexOf(item['permission']) > -1;
  }

  private isIndeterminate(items: any[] = []): boolean {
    let idx = 0;
    let item;

    while (item = items[idx]) {
      if (this.isIndeterminate(item.items) || this.containsItem(item)) {
        return true;
      }

      idx += 1;
    }

    return false;
  }

  deleteRole(role, e) {
    e.stopPropagation();

    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa nhóm quyền ${role.name} ?`;

    modalRef.result.then(() => {
      this.roleService.delete(role.id).subscribe(() => {
        this.loadRoles();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  loadRoles(roleId?: any) {
    var val = new ApplicationRolePaged();
    val.limit = this.pageSize;
    val.offset = (this.page - 1) * this.pageSize;
    this.roleService.getPaged(val).subscribe((data: any) => {
      this.rolesPaged = data;

      if (roleId) {
        var roles = this.rolesPaged.items.filter(x => x.id == roleId);
        if (roles.length) {
          this.selectRole(roles[0]);
        }
      } else if (this.rolesPaged.items.length) {
        this.selectRole(this.rolesPaged.items[0]);
      }
    });
  }

  onPageChange(page) {
    this.page = page;
    this.selectedRole = null;
    this.loadRoles();
  }

  selectRole(role) {
    this.selectedRole = role;
    this.id = role.id;

    this.roleService.get(role.id).subscribe((result: any) => {
      this.displayRole = result;
      this.checkedKeys = result.functions;
      this.selectedUsers = result.users;

      this.loadFeatures();
      this.loadUserList();
    });
  }

  createRole() {
    this.id = null;
    this.checkedKeys = [];
    this.selectedRole = null;
    this.selectedUsers = [];
    this.displayRole = {
      functions: []
    };
  }

  onChildrenLoaded(args: any): void {
    if (this.checkedKeys.indexOf(args.item.dataItem.permission) === -1) {
      return;
    }

    const keys = args.children.reduce((acc, item) => {
      const existingKey = this.checkedKeys.find(key => item.dataItem.permission === key);

      if (!existingKey) {
        acc.push(item.dataItem.permission);
      }

      return acc;
    }, []);

    if (keys.length) {
      this.checkedKeys = this.checkedKeys.concat(keys);
    }
  }

  public fetchChildren(node: any): Observable<any[]> {
    //Return the items collection of the parent node as children.
    return of(node.children);
  }

  public hasChildren(node: any): boolean {
    //Check if the parent node has children.
    return node.children && node.children.length > 0;
  }

  public get checkableSettings(): CheckableSettings {
    return {
      checkChildren: true,
      checkParents: true,
      enabled: true,
      mode: 'multiple',
      checkOnClick: true
    };
  }

  onSave() {
    if (!this.displayRole.name) {
      this.notificationService.show({
        content: 'Vui lòng nhập tên nhóm quyền',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    this.displayRole.functions = this.checkedKeys;
    this.displayRole.userIds = this.selectedUsers.map(x => x.id);

    if (!this.id) {
      this.roleService.create(this.displayRole).subscribe((result: any) => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadPermission();
        this.loadRoles(result.id);
      });
    } else {
      this.roleService.update(this.id, this.displayRole).subscribe((result: any) => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadPermission();
        this.loadRoles(this.id);
      });
    }
  }

  loadPermission() {
    this.authResource.getPermission().subscribe((res: any) => {
      localStorage.setItem('user_permission', JSON.stringify(res));
    });
  }
}

