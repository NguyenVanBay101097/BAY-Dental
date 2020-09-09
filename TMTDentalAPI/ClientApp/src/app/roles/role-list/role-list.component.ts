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
  roles: any = [];
  selectedRole: any;
  displayRole: any;
  mode = 'view';
  functions: any[] = [];
  id: string;

  constructor(private roleService: RoleService, private router: Router, private fb: FormBuilder,
    private webService: WebService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.loadFeatures();
    this.loadRoles();
  }

  loadFeatures() {
    this.webService.getFeatures().subscribe((data: any) => {
      this.featureGroups = data;
    });
  }

  // Custom logic handling Indeterminate state when custom data item property is persisted
  public isChecked = (dataItem: any, index: string): CheckedState => {
    if (this.containsItem(dataItem)) { return 'checked'; }

    if (this.isIndeterminate(dataItem.ops)) { return 'indeterminate'; }

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

  loadRoles() {
    var val = new ApplicationRolePaged();
    this.roleService.getPaged(val).subscribe((data: any) => {
      this.roles = data.items;

      if (this.roles.length) {
        this.selectRole(this.roles[0]);
      }
    });
  }

  selectRole(role) {
    this.selectedRole = role;
    this.id = role.id;

    this.roleService.get(role.id).subscribe((result: any) => {
      this.displayRole = result;
      this.checkedKeys = result.functions;
    });
  }

  createRole() {
    this.id = null;
    this.checkedKeys = [];
    this.selectedRole = null;
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
    return of(node.ops);
  }

  public hasChildren(node: any): boolean {
    //Check if the parent node has children.
    return node.ops && node.ops.length > 0;
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
    if (!this.id) {
      this.displayRole.functions = this.checkedKeys;
      this.roleService.create(this.displayRole).subscribe((result: any) => {
        this.roles.unshift(result);
        this.selectRole(result);
      });
    } else {
      this.displayRole.functions = this.checkedKeys;
      this.roleService.update(this.id, this.displayRole).subscribe((result: any) => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }
}

