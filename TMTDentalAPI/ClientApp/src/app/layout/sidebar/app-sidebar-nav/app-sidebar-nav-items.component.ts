import {Component, Inject, Input, OnInit, Renderer2} from '@angular/core';
import {Router} from '@angular/router';
import {DOCUMENT} from '@angular/common';

import {SidebarNavHelper} from '../app-sidebar-nav.service';
import {INavData} from '../app-sidebar-nav';

@Component({
  selector: 'app-sidebar-nav-items, cui-sidebar-nav-items',
  template: `
    <ng-container *ngFor="let item of items">
      <ng-container [ngSwitch]="helper.itemType(item)">
        <app-sidebar-nav-dropdown
          *ngSwitchCase="'dropdown'"
          [item]="item"
          (itemClick)="itemClick(item)"
          [class.active]="item === itemSelected"
          [ngClass]="item | appSidebarNavItemClass">
        </app-sidebar-nav-dropdown>
        <app-sidebar-nav-divider
          *ngSwitchCase="'divider'"
          [item]="item"
          [ngClass]="item | appSidebarNavItemClass"
          [appHtmlAttr]="item.attributes">
        </app-sidebar-nav-divider>
        <app-sidebar-nav-title
          *ngSwitchCase="'title'"
          [item]="item"
          [ngClass]="item | appSidebarNavItemClass"
          [appHtmlAttr]="item.attributes">
        </app-sidebar-nav-title>
        <app-sidebar-nav-label
          *ngSwitchCase="'label'"
          [item]="item"
          class="nav-item"
          [ngClass]="item | appSidebarNavItemClass">
        </app-sidebar-nav-label>
        <ng-container
          *ngSwitchCase="'empty'">
        </ng-container>
        <app-sidebar-nav-link
          *ngSwitchDefault
          [item]="item"
          class="nav-item"
          [class.active]="helper.isActive(router, item)"
          [ngClass]="item | appSidebarNavItemClass"
          (linkClick)="hideMobile()"
          (itemClick)="itemLinkClick($event, item)"
        >
        </app-sidebar-nav-link>
      </ng-container>
    </ng-container>
  `
})
export class AppSidebarNavItemsComponent implements OnInit {

  protected _items: INavData[];

  @Input()
  set items(items: INavData[]) {
    this._items = [...items];
  }
  get items(): INavData[] {
    return this._items;
  }

  itemSelected: INavData;

  constructor(
    @Inject(DOCUMENT) private document: any,
    private renderer: Renderer2,
    public router: Router,
    public helper: SidebarNavHelper
  ) {}

  ngOnInit(): void {
    this.itemSelected = this.items.find(x => this.helper.isActive(this.router, x)); 
  }

  public hideMobile() {
    if (this.document.body.classList.contains('sidebar-show')) {
      this.renderer.removeClass(this.document.body, 'sidebar-show');
    }
  }

  itemClick(item) {
    if (item === this.itemSelected) {
      this.itemSelected = null;
    } else {
      this.itemSelected = item;
    }
  }

  itemLinkClick(data, item) {
    if (data.url == item.url) {
      this.itemSelected = item;
    }
  }
}
