import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';

@Component({
  selector: 'app-layout-header',
  templateUrl: './layout-header.component.html',
  styleUrls: ['./layout-header.component.css']
})
export class LayoutHeaderComponent implements OnInit {

  constructor(private sidebarService: NavSidebarService) { }

  ngOnInit() {
  }

  toggleSidebar() {
    this.sidebarService.toggle();
  }
}
