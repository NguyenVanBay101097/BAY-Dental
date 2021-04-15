import { Component, HostBinding, Input, OnInit } from '@angular/core';
import { SidebarNavHelper } from '@shared/sidebar/sidebar-nav.service';

@Component({
  selector: 'app-sidebar-nav-item',
  templateUrl: './sidebar-nav-item.component.html',
  styleUrls: ['./sidebar-nav-item.component.css']
})
export class SidebarNavItemComponent implements OnInit {
  @HostBinding('class.nav-item') navItemClass = true;
  @Input() item: any;

  private iconClasses = {};
  constructor(
    public helper: SidebarNavHelper
  ) { }

  ngOnInit() {
    this.iconClasses = this.helper.getIconClass(this.item);
  }

  getLabelIconClass() {
    // const variant = `text-${this.item.label.variant}`;
    // this.iconClasses[variant] = !!this.item.label.variant;
    // const labelClass = this.item.label.class;
    // this.iconClasses[labelClass] = !!labelClass;
    return this.iconClasses;
  }

}
