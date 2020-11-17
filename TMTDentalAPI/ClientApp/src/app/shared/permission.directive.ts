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
    let hasPermission = true;

    if (this.allPermissions) {
      for (const checkPermission of this.permissions) {
        const permissionFound = this.allPermissions.find(x => x.toUpperCase() === checkPermission.toUpperCase());
        if (!permissionFound) {
          hasPermission = false;
          break;
        }
      }
    }
    return hasPermission;
  }
}
