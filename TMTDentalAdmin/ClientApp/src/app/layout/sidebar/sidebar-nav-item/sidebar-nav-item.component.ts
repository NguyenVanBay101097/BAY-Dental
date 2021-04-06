import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-sidebar-nav-item',
  templateUrl: './sidebar-nav-item.component.html',
  styleUrls: ['./sidebar-nav-item.component.css']
})
export class SidebarNavItemComponent implements OnInit {
  @HostBinding('class.nav-item') navItemClass = true;
  @Input() item: any;
  constructor() { }

  ngOnInit() {
  }

}
