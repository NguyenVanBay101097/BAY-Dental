import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleService } from '../role.service';
import { CheckableSettings } from '@progress/kendo-angular-treeview';
import { of, Observable } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { UserService } from 'src/app/users/user.service';

@Component({
  selector: 'app-role-create-update',
  templateUrl: './role-create-update.component.html',
  styleUrls: ['./role-create-update.component.css']
})
export class RoleCreateUpdateComponent implements OnInit {
  id: string;
  roleForm: FormGroup;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  permissionData: any[];
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  filteredUsers: UserSimple[] = [];
  selectUser: UserSimple;

  public checkedKeys: any[] = [];
  public enableCheck = true;
  public checkChildren = true;
  public checkParents = true;
  public checkOnClick = true;
  public checkMode: any = 'multiple';
  public selectionMode: any = 'single';

  public get checkableSettings(): CheckableSettings {
    return {
      checkChildren: this.checkChildren,
      checkParents: this.checkParents,
      enabled: this.enableCheck,
      mode: this.checkMode,
      checkOnClick: this.checkOnClick
    };
  }

  public children = (dataItem: any): Observable<any[]> => {
    return of(dataItem.Items);
  }

  public hasChildren = (dataItem: any): boolean => !!dataItem.Items;

  constructor(private fb: FormBuilder, private roleService: RoleService, private route: ActivatedRoute,
    private notificationService: NotificationService, private router: Router,
    private userService: UserService) {
  }

  ngOnInit() {
    this.roleForm = this.fb.group({
      name: ['', Validators.required],
      users: this.fb.array([])
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });

    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.loadData();
    }

    this.loadPermissionTree();
  }

  onChangeUser(value: any) {
    if (value) {
      var flag = true;
      this.users.controls.forEach(control => {
        if (control.get('id').value == value.id) {
          flag = false;
          return;
        }
      });

      if (flag) {
        this.users.push(this.fb.group(value));
      }
    }
  }

  deleteLine(index: number) {
    this.users.removeAt(index);
  }

  addUser() {
    if (this.selectUser) {
      var flag = true;
      this.users.controls.forEach(control => {
        if (control.get('id').value == this.selectUser.id) {
          flag = false;
          return;
        }
      });

      if (flag) {
        this.users.push(this.fb.group(this.selectUser));
      }
    }
  }

  searchUsers(filter?: string) {
    return this.userService.autocomplete(filter);
  }

  loadPermissionTree() {
    this.roleService.permissionTree().subscribe((result: any[]) => {
      this.permissionData = result;
    });
  }

  loadData() {
    this.roleService.get(this.id).subscribe(result => {
      this.roleForm.patchValue(result);
      this.checkedKeys = result.functions;

      let control = this.roleForm.get('users') as FormArray;
      control.clear(); //reset form array
      result.users.forEach(line => {
        var g = this.fb.group(line);
        control.push(g);
      });
    });
  }

  get users() {
    return this.roleForm.get('users') as FormArray;
  }

  onNew() {
    this.router.navigate(['roles/create']);
  }

  onSave() {
    if (!this.roleForm.valid) {
      return;
    }

    this.saveOrUpdate().subscribe((result: any) => {
      if (this.id) {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadData();
      } else {
        this.router.navigate(['roles/edit/', result.id])
      }
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var data = this.getBodyData();
    if (this.id) {
      return this.roleService.update(this.id, data);
    } else {
      return this.roleService.create(data);
    }
  }

  getBodyData() {
    var data = this.roleForm.value;
    data.functions = this.checkedKeys;
    return data;
  }
}

