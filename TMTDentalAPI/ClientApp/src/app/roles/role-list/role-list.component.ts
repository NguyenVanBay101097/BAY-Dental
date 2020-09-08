import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { RoleService, ApplicationRolePaged, ApplicationRoleBasic } from '../role.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { CheckableSettings, TreeItemLookup, CheckedState } from '@progress/kendo-angular-treeview';
import { WebService } from 'src/app/core/services/web.service';

const indexChecked = (keys, index) => keys.filter(k => k === index).length > 0;

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})
export class RoleListComponent implements OnInit {

  public checkedKeys: any[] = [];
  formGroup: FormGroup;
  featuresTreeData: any = [];
  features: any;
  roles: any = [];
  selectedRole: any;
  displayRole: any;
  mode = 'view';
  functions: any[] = [];

  constructor(private roleService: RoleService, private router: Router, private fb: FormBuilder,
    private webService: WebService) { }

  ngOnInit() {
    this.loadFeatures();
    this.loadRoles();
  }

  loadFeatures() {
    this.webService.getFeatures().subscribe((data: any) => {
      this.features = data.map(function (x) {
        var item = {
          id: x.id,
          text: x.text,
          value: x.id,
          items: []
        };

        if (x.children && x.children.length > 0) {
          item.items = x.children.map(function (o) {
            return {
              id: o.id,
              text: o.text,
              value: o.id,
              name: o.name,
              items: o.objects && o.objects.length > 0 ? o.objects.map(function (o) {
                return {
                  id: o.id,
                  text: o.text,
                  value: o.id,
                  name: o.name,
                  type: 'object',
                  items: o.functions.map(function (f) {
                    return {
                      id: f.id,
                      text: f.text,
                      value: f.id,
                      type: 'function',
                      object: o.name,
                      requireds: f.requireds
                    };
                  })
                };
              }) : o.functions.map(function (f) {
                return {
                  id: f.id,
                  text: f.text,
                  value: f.id,
                  type: 'function',
                  object: o.name,
                  requireds: f.requireds
                };
              })
            };
          });
        } else {
          item.items = x.objects.map(function (o) {
            return {
              id: o.id,
              text: o.text,
              value: o.id,
              name: o.name,
              type: 'object',
              items: o.functions.map(function (f) {
                return {
                  id: f.id,
                  text: f.text,
                  value: f.id,
                  type: 'function',
                  object: o.name,
                  requireds: f.requireds
                };
              })
            };
          });
        }

        return item;
      });
    });
  }

  handleChecking(node: TreeItemLookup) {
    this.checkNode(node);
    this.checkParents(node.parent);
  }

  private checkNode(node: TreeItemLookup, check?: boolean): void {
    const key = node.item.dataItem.id;
    const idx = this.functions.indexOf(key);

    const isChecked = idx > -1;
    const shouldCheck = check === undefined ? !isChecked : check;
    const isKeyPresent = key !== undefined && key !== null;

    if (!isKeyPresent || (isChecked && check)) { return; }

    if (isChecked) {
      this.functions.splice(idx, 1);
    } else {
      this.functions.push(key);
    }

    node.children.map(n => this.checkNode(n, shouldCheck));
  }

  private checkParents(parent: any): void {
    let currentParent = parent;

    while (currentParent) {
      const parentKey = currentParent.item.dataItem.id;
      const parentIndex = this.functions.indexOf(parentKey);

      if (this.allChildrenSelected(currentParent.children)) {
        if (parentIndex === -1) {
          this.functions.push(parentKey);
        }
      } else if (parentIndex > -1) {
        this.functions.splice(parentIndex, 1);
      }

      currentParent = currentParent.parent;
    }
  }

  private allChildrenSelected(children: any[]): boolean {
    const isCheckedReducer = (checked, item) => (
      checked && this.isItemChecked(item.dataItem, item.dataItem.id) === 'checked'
    );

    return children.reduce(isCheckedReducer, true);
  }

  protected isItemChecked(_: any, index: string): CheckedState {
    const checkedKeys = this.functions.filter((k) => k.indexOf(index) === 0);

    if (indexChecked(checkedKeys, index)) {
      return 'checked';
    }

    if (checkedKeys.length) {
      return 'indeterminate';
    }

    return 'none';
  }

  loadRoles() {
    var val = new ApplicationRolePaged();
    this.roleService.getPaged(val).subscribe((data: any) => {
      this.roles = data.items;
    });
  }

  selectRole(role) {
    this.selectedRole = role;

    this.roleService.get(role.id).subscribe((result: any) => {
      this.displayRole = result;
    });
  }

  createRole() {
    this.mode = 'create';
    this.displayRole = {
      functions: []
    };
  }

  public fetchChildren(node: any): Observable<any[]> {
    //Return the items collection of the parent node as children.
    return of(node.items);
  }

  public hasChildren(node: any): boolean {
    //Check if the parent node has children.
    return node.items && node.items.length > 0;
  }

  public get checkableSettings(): CheckableSettings {
    return {
      checkChildren: true,
      checkParents: true,
      enabled: true,
      mode: 'multiple',
      checkOnClick: false
    };
  }

  onSave() {
    if (this.mode == 'create') {
      this.roleService.create(this.displayRole).subscribe((result: any) => {
        this.roles.unshift(result);
      });
    } else {

    }


  }
}

