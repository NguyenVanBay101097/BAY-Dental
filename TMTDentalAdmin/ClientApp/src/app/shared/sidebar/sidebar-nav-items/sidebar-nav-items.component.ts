import { DOCUMENT } from '@angular/common';
import { Component, Inject, Input, OnInit, Renderer2 } from '@angular/core';
import { Router } from '@angular/router';
import { INavData } from '../sidebar-nav';
import { SidebarNavHelper } from '../sidebar-nav.service';

@Component({
  selector: 'app-sidebar-nav-items',
  templateUrl: './sidebar-nav-items.component.html',
  styleUrls: ['./sidebar-nav-items.component.css']
})
export class SidebarNavItemsComponent {

  protected _items: INavData[];

  @Input()
  set items(items: INavData[]) {
    this._items = [...items];
  }
  get items(): INavData[] {
    return this._items;
  }

  constructor(
    @Inject(DOCUMENT) private document: any,
    private renderer: Renderer2,
    public router: Router,
    public helper: SidebarNavHelper
  ) {}

  public hideMobile() {
    if (this.document.body.classList.contains('sidebar-show')) {
      this.renderer.removeClass(this.document.body, 'sidebar-show');
    }
  }

}
