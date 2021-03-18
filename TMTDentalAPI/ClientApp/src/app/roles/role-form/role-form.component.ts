import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CheckableSettings, CheckedState } from '@progress/kendo-angular-treeview';
import { Observable, of } from 'rxjs';
import { AuthResource } from 'src/app/auth/auth.resource';
import { WebService } from 'src/app/core/services/web.service';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { RoleService } from '../role.service';

@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html',
  styleUrls: ['./role-form.component.css']
})
export class RoleFormComponent implements OnInit {
  roleForm: FormGroup;
  role: any;
  id: string;
  submitted = false;
  selectedUsers: any[] = [];
  userList: any[] = [];
  featureGroups: any;
  public checkedKeys: any[] = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private roleService: RoleService,
    private userService: UserService,
    private webService: WebService,
    private notificationService: NotificationService,
    private router: Router,
    private authResource: AuthResource
  ) { }

  ngOnInit() {
    this.roleForm = this.fb.group({
      id: null,
      name: [null, Validators.required],
    });
    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
    });

    this.loadRole();
    this.loadFeatures();
    this.loadUserList();
  }

  get f() {
    return this.roleForm.controls;
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

  loadRole() {
    if (!this.id) {
      return;
    }
    this.roleService.get(this.id).subscribe((res: any) => {
      this.role = res;
      this.roleForm.patchValue(res);
      this.selectedUsers = res.users;
      this.checkedKeys = res.functions;
    });
  }

  loadUserList() {
    const val = new UserPaged();
    val.limit = 100;
    this.userService.getPaged(val).subscribe((result: any) => {
      this.userList = result.items;
    });
  }

  loadFeatures() {
    this.webService.getFeatures().subscribe((data: any) => {
      this.featureGroups = data;
    });
  }

  isUserSelected(user) {
    return this.selectedUsers.filter(x => x.id == user.id).length > 0;
  }

  selectUser(user) {
    const exists = this.selectedUsers.filter(x => x.id == user.id);
    if (!exists.length) {
      this.selectedUsers.push(user);
    } else {
      this.selectedUsers = this.selectedUsers.filter(x => x.id != user.id);
    }
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

  // Custom logic handling Indeterminate state when custom data item property is persisted
  private containsItem(item: any): boolean {
    return this.checkedKeys.indexOf(item['permission']) > -1;
  }

  private isIndeterminate(items: any[] = []): boolean {
    let idx = 0;
    let item;

    while (item = items[idx]) {
      if (this.isIndeterminate(item.children) || this.containsItem(item)) {
        return true;
      }

      idx += 1;
    }

    return false;
  }

  public isChecked = (dataItem: any, index: string): CheckedState => {
    if (this.containsItem(dataItem)) { return 'checked'; }

    if (this.isIndeterminate(dataItem.children)) { return 'indeterminate'; }

    return 'none';
  }

  public fetchChildren(node: any): Observable<any[]> {
    //Return the items collection of the parent node as children.
    return of(node.children);
  }

  public hasChildren(node: any): boolean {
    //Check if the parent node has children.
    return node.children && node.children.length > 0;
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

  loadPermission() {
    this.authResource.getPermission().subscribe((res: any) => {
      localStorage.setItem('user_permission', JSON.stringify(res));
    });
  }

  onSave() {
    this.submitted = true;
    if (this.roleForm.invalid) {
      return;
    }

    const val = this.roleForm.value;
    val.functions = this.checkedKeys;
    val.userIds = this.selectedUsers.map(x => x.id);

    if (!this.id) {
      this.roleService.create(val).subscribe((result: any) => {
        this.notify('success', 'Lưu thành công');
        this.router.navigate(['/roles/form'], { queryParams: { id: result.id } });
        this.loadFeatures();
      });
    } else {
      this.roleService.update(this.id, val).subscribe((result: any) => {
        this.notify('success', 'Lưu thành công');
        this.loadPermission();
        this.loadRole();
        this.loadFeatures();
      });
    }
  }
}
