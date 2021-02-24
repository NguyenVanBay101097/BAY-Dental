import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appPermission]'
})
export class PermissionDirective implements OnInit {
  private permissions = [];
  private allPermissions;
  private user_permission;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
  ) { }

  ngOnInit() {
    const pm = localStorage.getItem('user_permission');
    this.user_permission = JSON.parse(pm);
    if (this.user_permission) {
      if (this.user_permission.isUserRoot) {
        this.viewContainer.createEmbeddedView(this.templateRef);
        return true;
      }
      if (this.user_permission.permission) {
        this.allPermissions = this.user_permission.permission;
      }
    } 
    this.updateView();
  }

  @Input()
  set appPermission(val) {
    this.permissions = val;
    this.updateView();
  }

  private updateView() {
    if (this.checkPermission()) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

  private checkPermission() {
    let hasPermission = false;

    if (this.allPermissions) {
      for (let checkPermission of this.permissions) {
        checkPermission = checkPermission.toUpperCase(); 
        // tách check permission thành mảng
        let checkPermissionArr = [];
        let index = 0;
        while(index >= 0) {
          index = checkPermission.indexOf('.', index + 1);
          if(index <= 0) {
            checkPermissionArr.push(checkPermission);
            break;
          }
          checkPermissionArr.push(checkPermission.substring(0, index));
        }
        // check 
        const permissionFound = this.allPermissions.find(x => checkPermissionArr.indexOf(x.toUpperCase()) != -1);
        if (!permissionFound) {
          hasPermission = false;
          break;
        } else {
          hasPermission = true;
        }
      }
    }
    return hasPermission;
  }
}
