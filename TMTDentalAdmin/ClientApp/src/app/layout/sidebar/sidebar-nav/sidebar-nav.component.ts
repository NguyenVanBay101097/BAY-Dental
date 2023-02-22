import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-sidebar-nav',
  templateUrl: './sidebar-nav.component.html',
  styleUrls: ['./sidebar-nav.component.css']
})
export class SidebarNavComponent implements OnInit {
  @Input() navItems: any[] = [];
  @HostBinding('class.sidebar-nav') sidebarNavClass = true;

  constructor() { }

  ngOnInit() {
  }

}
