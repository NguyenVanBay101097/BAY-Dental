import { Component, EventEmitter, Input, Output } from '@angular/core';

import { SidebarNavHelper } from '../app-sidebar-nav.service';
import { INavData } from '../app-sidebar-nav';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar-nav-dropdown, cui-sidebar-nav-dropdown',
  template: `
    <a class="nav-link nav-dropdown-toggle" (click)="itemClick.emit()">
      <i *ngIf="helper.hasIcon(item)" [ngClass]="item | appSidebarNavIcon"></i>
      <ng-container>{{item.name}}</ng-container>
      <span *ngIf="helper.hasBadge(item)" [ngClass]="item | appSidebarNavBadge">{{ item.badge.text }}</span>
    </a>
    <app-sidebar-nav-items
      class="nav-dropdown-items nav-sub"
      [items]="item.children">
    </app-sidebar-nav-items>
  `,
  styles: [
    '.nav-dropdown-toggle { cursor: pointer; }',
    '.nav-dropdown-items { display: block; }'
  ],
  providers: [SidebarNavHelper]
})
export class AppSidebarNavDropdownComponent {
  @Input() item: INavData;
  @Output() itemClick = new EventEmitter<any>();

  constructor(
    public helper: SidebarNavHelper,
    public router: Router
  ) { }

  get isActive() {
    return this.item.children.some(x => this.helper.isActive(this.router, x));
  }
}
