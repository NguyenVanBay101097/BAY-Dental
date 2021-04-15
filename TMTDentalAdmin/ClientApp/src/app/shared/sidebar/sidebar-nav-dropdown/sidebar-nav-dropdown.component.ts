import { Component, Input, OnInit } from '@angular/core';
import { INavData } from '../sidebar-nav';

@Component({
  selector: 'app-sidebar-nav-dropdown',
  templateUrl: './sidebar-nav-dropdown.component.html',
  styleUrls: ['./sidebar-nav-dropdown.component.css']
})
export class SidebarNavDropdownComponent implements OnInit {
  @Input() item: INavData;
  constructor() { }

  ngOnInit() {
  }

}
